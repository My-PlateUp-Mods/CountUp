using Kitchen;
using KitchenLib;
using KitchenMods;
using System.Reflection;
using UnityEngine;

namespace KitchenCountUp
{
    public class Mod : BaseMod
    {
        public const string MOD_GUID = "Swift.PlateUp.CountUp";
        public const string MOD_NAME = "CountUp!";
        public const string MOD_VERSION = "1.1.0";
        public const string MOD_AUTHOR = "Swift";
        public const string MOD_GAMEVERSION = ">=1.1.1";

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            Log($"{MOD_GUID} v{MOD_VERSION} in use!"); 
        }
    }
}
