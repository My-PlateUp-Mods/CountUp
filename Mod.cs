using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Preferences;
using KitchenLib.Utils;
using PreferenceSystem;
using System.Reflection;

namespace KitchenCountUp
{
    public class Mod : BaseMod
    {
        public const string MOD_GUID = "Swift.PlateUp.CountUp";
        public const string MOD_NAME = "CountUp!";
        public const string MOD_VERSION = "2.1.1";
        public const string MOD_AUTHOR = "Swift, Nova & Penthilus";
        public const string MOD_GAMEVERSION = ">=1.3.0";

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            PreferenceManager = new PreferenceManager(MOD_GUID);
            UpdateIconsPreference = PreferenceManager.RegisterPreference(new PreferenceBool("UpdateHighItemIcons", true));
            ItemSplitPreference = PreferenceManager.RegisterPreference(new PreferenceBool("CountItemSplits", true));
            BinPreference = PreferenceManager.RegisterPreference(new PreferenceBool("CountBinSpace", true));
            LimitedProviderPreference = PreferenceManager.RegisterPreference(new PreferenceBool("CountProviderRemaining", true));
            PreferenceManager.Load();
            
            PreferenceSystemManager PreferenceSystemManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            PreferenceSystemManager
                .AddLabel("Tweaks")
                .AddOption<bool>(
                    Mod.UpdateIconsPreference.Key,
                    Mod.UpdateIconsPreference.Get(),
                    new bool[] { false, true },
                    new string[] { "Off", "On" },
                    (value) => { Mod.UpdateIconsPreference.Set(value); Mod.PreferenceManager.Save(); }
                )
                .AddInfo("Updates item icons that can not be directly split.")
                .AddLabel("Counters")
                .AddOption<bool>(
                    Mod.ItemSplitPreference.Key,
                    Mod.ItemSplitPreference.Get(),
                    new bool[] { false, true },
                    new string[] { "Off", "On" },
                    (value) => { Mod.ItemSplitPreference.Set(value); Mod.PreferenceManager.Save(); }
                )
                .AddInfo("Counts remaining item splits.")
                .AddOption<bool>(
                    Mod.LimitedProviderPreference.Key,
                    Mod.LimitedProviderPreference.Get(),
                    new bool[] { false, true },
                    new string[] { "Off", "On" },
                    (value) => { Mod.LimitedProviderPreference.Set(value); Mod.PreferenceManager.Save(); }
                )
                .AddInfo("Counts remaining items in providers.")
                .AddOption<bool>(
                    Mod.BinPreference.Key,
                    Mod.BinPreference.Get(),
                    new bool[] { false, true },
                    new string[] { "Off", "On" },
                    (value) => { Mod.BinPreference.Set(value); Mod.PreferenceManager.Save(); }
                )
                .AddInfo("Counts remaining space in bins.")
                .AddSpacer()
                .AddSpacer();
            PreferenceSystemManager.RegisterMenu(PreferenceSystemManager.MenuType.MainMenu);
            PreferenceSystemManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }

        // Preferences
        public static PreferenceManager PreferenceManager;

        public static PreferenceBool UpdateIconsPreference;

        public static PreferenceBool ItemSplitPreference;
        public static PreferenceBool BinPreference;
        public static PreferenceBool LimitedProviderPreference;
    }
}
