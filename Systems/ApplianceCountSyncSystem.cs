using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using Kitchen.NetworkSupport;
using KitchenCountUp;
using System.Linq;

namespace KitchenCountUp.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ApplianceCountSyncSystem : GameSystemBase, IModSystem
    {
        private EntityQuery query;

        protected override void Initialise()
        {
            base.Initialise();
            query = GetEntityQuery(new QueryHelper().All(typeof(CLinkedView)).Any(typeof(CItemProvider), typeof(CApplianceBin)));
        }

        protected override void OnUpdate()
        {
            if (!NetworkingUtils.IsHost()) return;
            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities)
            {
                bool hasProvider = Require<CItemProvider>(entity, out var provider);
                bool hasBin = Require<CApplianceBin>(entity, out var bin);

                bool isInfiniteOrOverlapping = false;
                bool hasColorblindConflict = false;
                if (hasProvider && GameData.Main.TryGet<Item>(provider.ProvidedItem, out var item))
                {
                    bool hasColorblindLabel = item.Prefab != null && item.Prefab.transform.Find("Colour Blind") != null;
                    bool isInfinite = provider.Maximum > 50;
                    hasColorblindConflict = hasColorblindLabel;

                    bool hideForColorblind = hasColorblindLabel && !Mod.ShowCountsForColorblindProvidersPreference.Get();
                    isInfiniteOrOverlapping = isInfinite || hideForColorblind;
                }

                var useCount = (hasProvider && !isInfiniteOrOverlapping && Mod.LimitedProviderPreference.Get() && provider.Maximum > 1) || (hasBin && Mod.BinPreference.Get() && bin.Capacity < 300);

                int count = 0;
                if (useCount)
                {
                    if (hasProvider)
                    {
                        int heldItemOffset = (Require<CItemHolder>(entity, out var holder) && holder.HeldItem != Entity.Null) ? 1 : 0;
                        count = provider.Available + heldItemOffset;
                    }
                    else if (hasBin)
                    {
                        count = bin.Capacity - bin.CurrentAmount;
                    }
                }

                Set(entity, new CCountUpAppliance { Count = count, UseCount = useCount });
                if (hasColorblindConflict)
                {
                    Set(entity, new CCountUpColorblindConflict { HasColorblindConflict = true });
                }
                else if (Has<CCountUpColorblindConflict>(entity))
                {
                    EntityManager.RemoveComponent<CCountUpColorblindConflict>(entity);
                }
            }
        }
    }
}
