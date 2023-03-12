using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Preferences;
using KitchenLib.Utils;
using System.Reflection;

namespace KitchenCountUp
{
    public class Mod : BaseMod
    {
        public const string MOD_GUID = "Swift.PlateUp.CountUp";
        public const string MOD_NAME = "CountUp!";
        public const string MOD_VERSION = "2.0.0";
        public const string MOD_AUTHOR = "Swift & Nova";
        public const string MOD_GAMEVERSION = ">=1.1.1";

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            PreferenceManager = new PreferenceManager(MOD_GUID);
            UpdateIconsPreference = PreferenceManager.RegisterPreference(new PreferenceBool("UpdateHighItemIcons", true));
            ItemSplitPreference = PreferenceManager.RegisterPreference(new PreferenceBool("CountItemSplits", true));
            BinPreference = PreferenceManager.RegisterPreference(new PreferenceBool("CountBinSpace", true));
            LimitedProviderPreference = PreferenceManager.RegisterPreference(new PreferenceBool("CountProviderRemaining", true));
            PreferenceManager.Load();

            ModsPreferencesMenu<MainMenuAction>.RegisterMenu(MOD_NAME, typeof(PreferencesMenu<MainMenuAction>), typeof(MainMenuAction));
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(PreferencesMenu<PauseMenuAction>), typeof(PauseMenuAction));
            Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(PreferencesMenu<MainMenuAction>), new PreferencesMenu<MainMenuAction>(args.Container, args.Module_list));
            };
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(PreferencesMenu<PauseMenuAction>), new PreferencesMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
        }

        // Preferences
        public static PreferenceManager PreferenceManager;

        public static PreferenceBool UpdateIconsPreference;

        public static PreferenceBool ItemSplitPreference;
        public static PreferenceBool BinPreference;
        public static PreferenceBool LimitedProviderPreference;
    }
}
