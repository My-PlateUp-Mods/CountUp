using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenCountUp
{
    public class PreferencesMenu<T> : KLMenu<T>
    {
        public PreferencesMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            AddLabel("Tweaks");
            Add(new Option<bool>(new List<bool> { false, true }, Mod.UpdateIconsPreference.Get(), new List<string> { "Off", "On" }
            )).OnChanged += delegate (object _, bool newVal)
            {
                Mod.UpdateIconsPreference.Set(newVal);
            };
            AddInfo("Updates item icons that can not be directly split.");

            AddLabel("Counters");
            Add(new Option<bool>(new List<bool> { false, true }, Mod.ItemSplitPreference.Get(), new List<string> { "Off", "On" }
            )).OnChanged += delegate (object _, bool newVal)
            {
                Mod.ItemSplitPreference.Set(newVal);
            };
            AddInfo("Counts remaining item splits.");

            Add(new Option<bool>(new List<bool> { false, true }, Mod.LimitedProviderPreference.Get(), new List<string> { "Off", "On" }
            )).OnChanged += delegate (object _, bool newVal)
            {
                Mod.LimitedProviderPreference.Set(newVal);
            };
            AddInfo("Counts remaining items in providers.");

            Add(new Option<bool>(new List<bool> { false, true }, Mod.BinPreference.Get(), new List<string> { "Off", "On" }
            )).OnChanged += delegate (object _, bool newVal)
            {
                Mod.BinPreference.Set(newVal);
            };
            AddInfo("Counts remaining space in bins.");

            AddButton("Apply", delegate
            {
                Mod.PreferenceManager.Save();
                RequestPreviousMenu();
            });

            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate
            {
                RequestPreviousMenu();
            });
        }
    }
}
