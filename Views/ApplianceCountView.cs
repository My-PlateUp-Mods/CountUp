using Kitchen;
using KitchenData;
using KitchenLib.References;
using KitchenMods;
using MessagePack;
using TMPro;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Kitchen.NetworkSupport;
using System.Linq;

namespace KitchenCountUp.Views
{
    public class ApplianceCountView : UpdatableObjectView<ApplianceCountView.ViewData>
    {
        public TextMeshPro CountText;

        protected override void UpdateData(ViewData data)
        {
            CountText.gameObject.SetActive(data.UseCount);
            CountText.text = data.Count.ToString();
        }

        [UpdateInGroup(typeof(ViewSystemsGroup))]
        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            private EntityQuery query;

            protected override void Initialise()
            {
                query = GetEntityQuery(new QueryHelper().All(typeof(CCountUpAppliance), typeof(CLinkedView)));
            }

            protected override void OnUpdate()
            {
                var entities = query.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    Require<CCountUpAppliance>(entity, out var countUp);
                    Require<CLinkedView>(entity, out var view);
                    SendUpdate(view, new ViewData()
                    {
                        Count = countUp.Count,
                        UseCount = countUp.UseCount
                    }, MessageType.SpecificViewUpdate);
                }
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
