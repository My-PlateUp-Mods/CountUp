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
    public class ItemCountSyncSystem : GameSystemBase, IModSystem
    {
        private EntityQuery query;

        protected override void Initialise()
        {
            base.Initialise();
            query = GetEntityQuery(new QueryHelper().All(typeof(CItem), typeof(CLinkedView)).Any(typeof(CSplittableItem), typeof(CItemUndergoingProcess), typeof(CCountUpItem)));
        }

        protected override void OnUpdate()
        {
            if (!NetworkingUtils.IsHost()) return;
            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities)
            {
                Require<CItem>(entity, out var itemComp);
                bool hasSplittable = Require<CSplittableItem>(entity, out var splittable);
                bool undergoingProcess = Has<CItemUndergoingProcess>(entity);

                if (!hasSplittable && !undergoingProcess)
                {
                    if (Require<CCountUpItem>(entity, out var countUp) && countUp.ItemID != 0)
                    {
                        Set(entity, new CCountUpItem
                        {
                            ItemID = 0,
                            Count = 0,
                            UndergoingProcess = false,
                            IsPartial = false
                        });
                    }
                    continue;
                }
                
                if (!GameData.Main.TryGet<Item>(itemComp.ID, out var item))
                    continue;

                int splitAdditive = item.IsSplittable && item.SplitDepletedItems.Contains(item.SplitSubItem) ? 1 : 0;
                int count = hasSplittable ? splittable.RemainingCount + splitAdditive : 0;

                Set(entity, new CCountUpItem
                {
                    ItemID = itemComp.ID,
                    Count = count,
                    UndergoingProcess = undergoingProcess,
                    IsPartial = itemComp.IsPartial
                });
            }
        }
    }
}
