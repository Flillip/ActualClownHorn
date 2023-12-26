using ActualClownHorn.Patches;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ActualClownHorn
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class ActualClownHornBase : BaseUnityPlugin
    {
        internal const string ModGuid = "ActualClownHorn";
        internal const string ModName = "Actual Clown Horn";
        internal const string ModVersion = "1.0.1";
        internal const string ModAuthor = "flillip";

        private readonly Harmony _harmony = new Harmony(ModGuid);
        public static ActualClownHornBase Instance;

        internal ManualLogSource logger;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            logger = BepInEx.Logging.Logger.CreateLogSource(ModGuid);
            logger.LogInfo(ModName + " Has loaded");

            _harmony.PatchAll(typeof(GrabbableObjectPatch));
        }
    }
}
