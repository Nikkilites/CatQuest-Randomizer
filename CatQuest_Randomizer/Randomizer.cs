using BepInEx;
using BepInEx.Logging;

namespace CatQuest_Randomizer
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Randomizer : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
            
        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
