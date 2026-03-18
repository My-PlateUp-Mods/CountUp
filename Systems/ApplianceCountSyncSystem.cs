using Kitchen;
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
                var useCount = (hasProvider && Mod.LimitedProviderPreference.Get() && provider.Maximum > 1) || (hasBin && Mod.BinPreference.Get() && bin.Capacity < 300);

                int count = useCount ? (hasProvider ? provider.Available : hasBin ? bin.Capacity - bin.CurrentAmount : 0) : 0;

                Set(entity, new CCountUpAppliance { Count = count, UseCount = useCount });
            }
        }
    }
}
