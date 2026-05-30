# CountUp! Multiplayer Testing Guide

This guide is for testers verifying the multiplayer synchronization of the CountUp! v2.1.1 update.

## Test Setup

1. **Host** installs the new `CountUp.dll`.
2. **Host & Client** should have **PlatePatch** installed to ensure multiplayer data synchronization.
3. **Client** connects to the lobby.
4. Verify that the Host has enabled the features in the **Options > Mod Preferences** (or Pause Menu) via the new `PreferenceSystem` UI.
   - *Count Bin Space*
   - *Count Provider Remaining*
   - *Count Item Splits*

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

## Expected Behavior

- **Success:** The Client sees the exact numbers the Host sees at all times, and they update in real-time.
- **Failure:** The Client sees numbers that do not match the Host's values or seeing no numbers when the Host sees them.

*Note: The mod now uses a centralized identity-based host check (`NetworkingUtils.IsHost()`) and the `Kitchen` namespace for custom components. This ensures that host-sent data is natively synchronized to all clients via PlateUp's engine.*

## File Manifest (v2.1.1)

### Modified

- `Mod.cs`
- `CountUp.csproj`
- `Views/ApplianceCountView.cs`
- `Views/ItemIconView.cs`

### Added

- `Components/CCountUpData.cs`
- `Systems/ApplianceCountSyncSystem.cs`
- `Systems/ItemCountSyncSystem.cs`
- `NetworkingUtils.cs`
- `docs/testing_guide.md`

### Removed

- `PreferencesMenu.cs`
