using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenLib.References;
using KitchenLib.Utils;
using TMPro;
using UnityEngine;

namespace PortionCounter
{
    [HarmonyPatch(typeof(ObjectsSplittableView))]
    class ObjectsSplittableView_Patches
    {
        [HarmonyPatch("UpdateData")]
        [HarmonyPostfix]
        static void UpdateData_Post(ObjectsSplittableView __instance, SplittableItemView.ViewData data)
        {
            var TMP = Patch_Util.checkAddTMP(__instance);
            TMP.text = $"{data.Remaining + 1}";
        }
    }

    [HarmonyPatch(typeof(PositionSplittableView))]
    class PositionSplittableView_Patches
    {
        [HarmonyPatch("UpdateData")]
        [HarmonyPostfix]
        static void UpdateData_Post(PositionSplittableView __instance, SplittableItemView.ViewData data)
        {
            var TMP = Patch_Util.checkAddTMP(__instance);
            TMP.text = data.Remaining.ToString();
        }
    }

    public class Patch_Util
    {
        public static TextMeshPro checkAddTMP(SplittableItemView view)
        {
            GameObject text = GameObjectUtils.GetChildObject(view.gameObject, "Amount");
            if (text == null)
            {
                text = Object.Instantiate<GameObject>(GameData.Main.Get<Item>(ItemReferences.PieMeatCooked).Prefab.transform.Find("Colour Blind").gameObject);
                Object.Destroy(text.GetComponent<ColourBlindMode>());
                text.name = "Amount";
                text.transform.parent = view.transform;
                text.SetActive(true);
                text.transform.position = view.transform.position;
                var title = text.transform.Find("Title");
                title.gameObject.SetActive(true);
                title.localPosition = new Vector3(0, 1, 0);
            }
            return text.transform.Find("Title").gameObject.GetComponent<TextMeshPro>();
        }
    }
}
