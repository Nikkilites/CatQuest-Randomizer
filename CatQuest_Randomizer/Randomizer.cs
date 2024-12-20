using BepInEx;
using BepInEx.Logging;

namespace CatQuest_Randomizer
{
    [BepInPlugin(ModInfo.GUID, ModInfo.NAME, ModInfo.VERSION)]
    public class Randomizer : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
            
        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {ModInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
