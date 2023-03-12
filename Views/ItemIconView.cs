using Kitchen;
using Kitchen.Modules;
using KitchenData;
using KitchenMods;
using MessagePack;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenCountUp.Views
{
    internal class ItemIconView : UpdatableObjectView<ItemIconView.ViewData>
    {
        public TextMeshPro ProcessIcons;

        protected override void UpdateData(ViewData Data)
        {
            if (ProcessIcons == null || !GameData.Main.TryGet<Item>(Data.ItemID, out var item))
                return;

            if (Data.UndergoingProcess)
            {
                UpdateForProcess(Data, item);
            }
            else
            {
                UpdateForDefault(Data, item);
            }
        }

        private void UpdateForDefault(ViewData Data, Item item)
        {
            ProcessIcons.gameObject.transform.localPosition = Vector3.up * 0.8f;
            string iconSet = Mod.ItemSplitPreference.Get() && Data.Count > 0 && Data.Count < 300 ? $"{Item.GetIconSet(item)}<cspace=-35>{Data.Count}</cspace>" : Item.GetIconSet(item);
            ProcessIcons.text = !Data.IsPartial ? iconSet : "";
        }

        private void UpdateForProcess(ViewData Data, Item item)
        {
            ProcessIcons.gameObject.transform.localPosition = Vector3.up * 1.25f;
            ProcessIcons.text = Mod.ItemSplitPreference.Get() && Data.Count > 0 && Data.Count < 300 ? Data.Count.ToString() : "";
        }

        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            private EntityQuery query;
            protected override void Initialise()
            {
                query = GetEntityQuery(new QueryHelper().All(typeof(CItem), typeof(CLinkedView)));
            }

            protected override void OnUpdate()
            {
                var entities = query.ToEntityArray(Allocator.Temp);

                foreach (var entity in entities)
                {
                    Require<CItem>(entity, out var itemComp);
                    Require<CLinkedView>(entity, out var view);

                    if (!GameData.Main.TryGet<Item>(itemComp.ID, out var item))
                        continue;

                    int splitAdditive = item.IsSplittable && item.SplitDepletedItems.Contains(item.SplitSubItem) ? 1 : 0;

                    SendUpdate(view, new ViewData()
                    {
                        ItemID = itemComp.ID,
                        Count = Require<CSplittableItem>(entity, out var splittable) ? splittable.RemainingCount + splitAdditive : 0,
                        UndergoingProcess = Has<CItemUndergoingProcess>(entity),
                        IsPartial = itemComp.IsPartial,
                    }, MessageType.SpecificViewUpdate);

                }

            }
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public int ItemID;

            [Key(1)] public int Count;

            [Key(2)] public bool UndergoingProcess;

            [Key(3)] public bool IsPartial;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<ItemIconView>();

            public bool IsChangedFrom(ViewData check) => ItemID != check.ItemID || Count != check.Count || 
                IsPartial != check.IsPartial || UndergoingProcess != check.UndergoingProcess;
        }
    }
}
