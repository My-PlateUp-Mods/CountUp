# CountUp! Multiplayer Testing Guide

This guide is for testers verifying the multiplayer synchronization of the CountUp! v2.1.3 update.

## Test Setup

1. **Host** installs the new `CountUp.dll`.
2. **Host & Client** should have **PlatePatch** installed to ensure multiplayer data synchronization.
3. **Client** connects to the lobby.
4. Verify that the Host has enabled the features in the **Options > Mod Preferences** (or Pause Menu) via the new `PreferenceSystem` UI.
   - *Count Bin Space*
   - *Count Provider Remaining*
   - *Count Item Splits*
   - *Show Counts For Colorblind Providers* (tweak)
   - *Hide Portion Icons For Infinitely Portionable Items* (tweak)

## Scenario 1: Bins & Providers (ApplianceCountView)

1. Load into the **Lobby/Headquarters**.
2. Have the **Host** pull a **Plate** from the plate stack in the lobby. The counter over the remaining plates should hover.
3. Have the **Client** verify they can see the exact same plate count hovering over the provider.
4. Have the **Client** pull a plate. Verify the number decreases simultaneously for both players.
5. **During a 'Run' (e.g. Day 1)**: Throw any available food item into a standard **Garbage Bin**. Both players should see the capacity counter update.

*Note: While some tests (like plates) work in the lobby, Day 1 of a new run is the best environment for a full verification of all counters.*

## Scenario 2: Splittables (ItemIconView)

1. Create a **Pizza** or **Bread** (any splittable item).
2. Cook the item and place it on a counter.
3. Have the **Host** slice the item.
4. Verify the hovering counter correctly displays the number of slices remaining.
5. Have the **Client** grab a slice. Verify that both the physical slices and the hovering counter decrease accurately.
6. If the splittable leaves an empty container (such as a pot), verify that the counter does not display.

## Scenario 3: Colorblind & Infinite Tweaks

1. Place a **Ketchup**, **Mustard**, or **Chocolate Syrup** stand in the kitchen.
2. Verify that **by default**:
   - There are no hovering numbers above these providers.
   - The portion icons (the circular split sprite) are visible when holding a condiment bottle in your hand.
3. Tweak 1 Test (**Hide Portion Icons For Infinitely Portionable Items**):
   - Open **Mod Preferences** and toggle this option **On**.
   - Grab a Ketchup bottle. Verify that the circular portion icon is now hidden, showing only the colorblind letter `K`.
   - Toggle the option back **Off**. Verify that the portion icon immediately reappears.
4. Tweak 2 Test (**Show Counts For Colorblind Providers**):
   - Open **Mod Preferences** and toggle this option **On**.
   - Verify that a numerical counter (e.g. `3`) immediately appears above Ketchup and Chocolate Syrup.
   - Verify that this count is **offset to the right** (hovering over the rightmost bottle slot) and does not overlap the colorblind labels (`K K` or `Cs`).
   - Grab a bottle. Verify the count decreases to `2` in real-time.

## Expected Behavior

- **Success:**
  - The Client sees the exact numbers the Host sees at all times, and they update in real-time.
  - No `MissingMethodException` is thrown in the client's log files.
  - Client frame rates remain stable without spikes or performance degradation while counters are visible.
- **Failure:**
  - The Client sees numbers that do not match the Host's values or seeing no numbers when the Host sees them.
  - The client's PlateUp! logs (usually located at `%appdata%\..\LocalLow\It's Happening\PlateUp\Player.log`) contain `MissingMethodException` relating to `Kitchen.IObjectView.GetSubView<T>()` or view updates.

*Note: The mod now uses a centralized identity-based host check (`NetworkingUtils.IsHost()`) and the `Kitchen` namespace for custom components. This ensures that host-sent data is natively synchronized to all clients via PlateUp's engine.*

## File Manifest (v2.1.3)

### Modified

- `Mod.cs`
- `CountUp.csproj`
- `Components/CCountUpData.cs`
- `Systems/ApplianceCountSyncSystem.cs`
- `Views/ItemIconView.cs`
- `Views/ApplianceCountView.cs`

---

## Developer & Tester Reference: Why Providers Behave Differently

When testing, you may observe visual discrepancies between different item providers. For example:
- **Ketchup/Mustard** shows two colorblind labels (`K K` or `M M`) on the stand.
- **Chocolate Syrup** only shows a single `Cs` label on the stand and never displays a portion counter.
- **Soy Sauce** shows a portion counter `3` but no colorblind labels.

Here is the technical explanation for these behaviors, and the **Tweak Solution** we provide:

### 1. Structural GDO differences (`CItemHolder` vs. Visuals)

The game handles item rendering on counters differently depending on the appliance GDO properties:

* **Ketchup (`SourceKetchup`, ID: -965827229):**
  - Contains the `CItemHolder` component.
  - Spawns actual, individual `CItem` squeeze bottle entities in its slots at runtime. 
  - The game hides the active grab slot's label (Right), while the preview bottles in the queue slots (Left, Middle) display their colorblind labels normally, resulting in **`K K`**.
* **Chocolate Syrup (`SourceChocolateSyrup`, ID: 825737084):**
  - Contains `CItemProvider` but **no `CItemHolder`** component.
  - Does **not** spawn real item entities in the slots while docked on the counter.
  - Instead, the 3 visual bottles are static models pre-baked directly inside the appliance prefab. The developer configured the colorblind GameObject to be active *only* on the middle bottle (`Chocolate Syrup (1)`):
    ```yaml
    HoldPoint/
      - Chocolate Syrup (Colour Blind: Active: False)
      - Chocolate Syrup (1) (Colour Blind: Active: True)  <--- Centered "Cs"
      - Chocolate Syrup (2) (Colour Blind: Active: False)
    ```
  - Since these are static visual clones (not ECS entities), they never run `ItemIconView`. This is why there is only a single **`Cs`** label and no portion counter/icon visible on the counter itself. 
  - Once a player interacts with the stand, a real `CItem` Chocolate Syrup bottle entity is instantiated directly into the player's hands, causing its colorblind label and portion icons to immediately appear.

### 2. Programmatic Provider Count Hiding Logic
In `ApplianceCountSyncSystem.cs`, we determine whether to hide or show counts on providers programmatically to prevent UI overlap:

```csharp
bool isInfinite = provider.Maximum > 50;
bool hasColorblindLabel = item.Prefab != null && item.Prefab.transform.Find("Colour Blind") != null;

// Only hide for colorblind items if the player hasn't enabled the tweak
bool hideForColorblind = hasColorblindLabel && !Mod.ShowCountsForColorblindProvidersPreference.Get();

isInfiniteOrOverlapping = isInfinite || hideForColorblind;
```

* **Soy Sauce:** Prefab has no `"Colour Blind"` transform, and maximum capacity is `3`. It evaluates to `false`, rendering the count `3` cleanly.
* **Ketchup:** Prefab contains a `"Colour Blind"` child. By default, it evaluates to `true` (count hidden).
* **Chocolate Syrup:** Base item prefab (`SundaeSyrupChocolate`, ID: 2105393112) contains a `"Colour Blind"` child. By default, it evaluates to `true` (count hidden).

### 3. The "Show Counts For Colorblind Providers" Tweak (Off by default)
To allow players to see counts for Ketchup and Chocolate Syrup, we added a tweak option under **Mod Preferences > Counters**:
- **When Off (Default):** Counts for colorblind providers are hidden, maintaining a clean default layout.
- **When On:** The system forces the count to display on Ketchup, Mustard, and Chocolate Syrup. To prevent overlapping the colorblind text (`K K` or `Cs`), the mod dynamically shifts the count's local position to the right (`X = 0.4f`), rendering it neatly above the third bottle slot:
  ```csharp
  if (data.HasColorblindConflict)
  {
      CountText.transform.localPosition = new UnityEngine.Vector3(0.4f, 1.25f, 0f);
  }
  else
  {
      CountText.transform.localPosition = new UnityEngine.Vector3(0f, 1.25f, 0f);
  }
  ```