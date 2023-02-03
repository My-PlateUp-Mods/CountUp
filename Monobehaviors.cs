using Kitchen;
using KitchenData;
using TMPro;
using UnityEngine;

namespace KitchenCountUp
{
    public class SplitCounterView : SplittableItemView
    {
        protected override void UpdateData(ViewData data)
        {
            var container = gameObject.GetComponent<CounterContainer>();
            if (container == null)
                return;

            // Update container
            container.Remaining = data.Remaining;
            container.UpdateText();
        }
    }

    public class CounterContainer : MonoBehaviour
    {
        public int Remaining = 0;
        public int Additional = 0;
        public string IconString = "";
        public TextMeshPro TMP;

        public void UpdateText()
        {
            TMP.text = IconString + (Remaining + Additional).ToString();
        }
    }
}
