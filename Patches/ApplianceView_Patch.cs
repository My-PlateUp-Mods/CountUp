using HarmonyLib;
using Kitchen;
using KitchenCountUp.Views;
using KitchenData;
using KitchenLib.References;
using System.Runtime.Remoting.Contexts;
using TMPro;
using UnityEngine;

namespace KitchenCountUp.Patches
{
    [HarmonyPatch(typeof(ApplianceView))]
    internal class ApplianceView_Patch
    {
        [HarmonyPatch(nameof(ApplianceView.Initialise))]
        [HarmonyPostfix]
        internal static void Initialise_Postfix(ApplianceView __instance)
        {
            GameObject colourBlind = Object.Instantiate(GameData.Main.Get<Item>(ItemReferences.PieMeatCooked).Prefab.transform.Find("Colour Blind").gameObject);
            colourBlind.gameObject.transform.SetParent(__instance.gameObject.transform, false);
            colourBlind.name = "Counter";
            Transform Title = colourBlind.transform.Find("Title");
            Title.localPosition = Vector3.up * 1.25f;
            Object.Destroy(colourBlind.gameObject.GetComponent<ColourBlindMode>());
            __instance.gameObject.AddComponent<ApplianceCountView>().CountText = Title.gameObject.GetComponent<TextMeshPro>();
        }
    }
}
