# CountUp!

Displays a number above certain foods to indicate how many portions are left for serving. Works for Base game foods & modded ones.

### Requirements

- **KitchenLib**: Required for mod loading and base functionality.
- **PreferenceSystem**: Required for the options menu.
- **PlatePatch**: **Highly Recommended / Required for Multiplayer**. Fixes native PlateUp networking to allow modded components (like counters) to sync to clients.

---

### Technical Details (For Modders)

As of PlateUp! 1.3.0, the game shifted to strict AOT (Ahead-of-Time) compilation and native Networking to support cross-platform play.

This mod has been updated to use the new native ECS multiplayer architecture:

- Custom data is synchronized via `IModComponent` structs (`CCountUpAppliance` and `CCountUpItem`).
- These components are placed in the `Kitchen` namespace and use `[MessagePackObject]` attributes so the networking engine recognizes and synchronizes them to all clients.
- Host Identity logic is managed by `NetworkingUtils.IsHost()` which utilizes snapshots of the network peer list. This ensures that only the host populates shared components.
- View systems are unrestricted to ensure local UI updates on all machines, while the underlying data is host-authoritative.
- Menu registration now uses `PreferenceSystem`
