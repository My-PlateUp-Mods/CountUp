using Kitchen;
using KitchenData;
using KitchenLib.References;
using KitchenMods;
using MessagePack;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenCountUp.Views
{
    public class ApplianceCountView : UpdatableObjectView<ApplianceCountView.ViewData>
    {
        public TextMeshPro CountText;

        protected override void UpdateData(ViewData data)
        {
            if (CountText == null)
                return;
            CountText.gameObject.SetActive(data.UseCount);
            CountText.text = data.Count.ToString();
        }

        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            private EntityQuery query;
            protected override void Initialise()
            {
                query = GetEntityQuery(new QueryHelper().All(typeof(CAppliance), typeof(CLinkedView)).Any(typeof(CItemProvider), typeof(CApplianceBin)));
            }

            protected override void OnUpdate()
            {
                var entities = query.ToEntityArray(Allocator.Temp);

                foreach (var entity in entities)
                {
                    Require<CLinkedView>(entity, out var view);

                    bool hasProvider = Require<CItemProvider>(entity, out var provider);
                    bool hasBin = Require<CApplianceBin>(entity, out var bin);
                    var useCount = (hasProvider && Mod.LimitedProviderPreference.Get() && provider.Maximum > 1) || (hasBin && Mod.BinPreference.Get() && bin.Capacity < 300);
                    SendUpdate(view, new ViewData()
                    {
                        Count = useCount ? (hasProvider && provider.Maximum < 100 ? provider.Available : hasBin ? bin.Capacity - bin.CurrentAmount : 0) : 0,
                        UseCount = useCount
                    }, MessageType.SpecificViewUpdate);
                }

                entities.Dispose();
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public int Count;

            [Key(1)] public bool UseCount;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<ApplianceCountView>();

            public bool IsChangedFrom(ViewData check) => UseCount != check.UseCount || Count != check.Count;
        }
    }
}
