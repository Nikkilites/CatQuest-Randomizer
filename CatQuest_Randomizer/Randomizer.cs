using BepInEx;
using BepInEx.Logging;
using CatQuest_Randomizer.Archipelago;
using System.Collections;
using UnityEngine;

namespace CatQuest_Randomizer
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Randomizer : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        public static ConnectionHandler ConnectionHandler { get; private set; }
        public static ItemHandler ItemHandler { get; private set; }
        public static LocationHandler LocationHandler { get; private set; }
        public static GoalHandler GoalHandler { get; private set; }

        private void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("Cat Quest Logger");

            ConnectionHandler = new ConnectionHandler();
            LocationHandler = new();
            ItemHandler = new();
            GoalHandler = new();

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            ConnectionHandler.Connect("localhost", "Nikki", null);
            ConnectionHandler.CheckForNewItems();
        }

        private IEnumerator TriggerCollectEventCoroutine()
        {
            yield return new WaitForSeconds(20f); // Wait for 5 seconds before triggering
        }
    }
}