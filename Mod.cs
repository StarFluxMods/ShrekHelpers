using System.Collections.Generic;
using KitchenLib;
using KitchenLib.Logging;
using KitchenLib.Logging.Exceptions;
using KitchenMods;
using System.Linq;
using System.Reflection;
using Kitchen;
using KitchenData;
using KitchenLib.Event;
using KitchenLib.References;
using KitchenLib.Utils;
using UnityEngine;

namespace ShrekHelpers
{
    public class Mod : BaseMod, IModSystem
    {
        public const string MOD_GUID = "com.starfluxgames.shrekhelpers";
        public const string MOD_NAME = "Shrek Helpers";
        public const string MOD_VERSION = "0.1.0";
        public const string MOD_AUTHOR = "StarFluxGames";
        public const string MOD_GAMEVERSION = ">=1.1.4";

        public static AssetBundle Bundle;
        public static KitchenLogger Logger;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            Logger.LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        protected override void OnUpdate()
        {
        }
        
        Dictionary<int, string> ids = new Dictionary<int, string>
        {
            {ApplianceReferences.GhostKnife, "Shrek_Knife"},
            {ApplianceReferences.GhostClipboard, "Shrek_Clip"},
            {ApplianceReferences.GhostScrubber, "Shrek_Scrub"},
            {ApplianceReferences.GhostRollingPin, "Shrek_Roll"},
        };

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).FirstOrDefault() ?? throw new MissingAssetBundleException(MOD_GUID);
            Logger = InitLogger();

            Events.BuildGameDataEvent += (sender, args) =>
            {
                foreach (int id in ids.Keys)
                {
                    Appliance appliance = args.gamedata.Get<Appliance>(id);
                    appliance.Prefab = MaterialUtils.AssignMaterialsByNames(Bundle.LoadAsset<GameObject>(ids[id]));
                    ApplianceInteractorView view = GameObjectUtils.GetChildObject(appliance.Prefab, "Shrek").AddComponent<ApplianceInteractorView>();
                    FieldInfo info = ReflectionUtils.GetField<ApplianceInteractorView>("Animator");
                    info.SetValue(view, GameObjectUtils.GetChildObject(appliance.Prefab, "Shrek").GetComponent<Animator>());
                }
            };
        }
    }
}
