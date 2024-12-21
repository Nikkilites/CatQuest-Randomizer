using BepInEx;
using BepInEx.Logging;
using System.Collections;
using UnityEngine;
using static PlayerEvent;
using System.Text;
using UniRx;

namespace CatQuest_Randomizer
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Randomizer : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            StartCoroutine(TriggerCollectEventCoroutine());
        }

        private IEnumerator TriggerCollectEventCoroutine()
        {
            yield return new WaitForSeconds(20f); // Wait for 5 seconds before triggering

        }
    }
}