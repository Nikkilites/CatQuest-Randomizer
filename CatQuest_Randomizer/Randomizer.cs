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

            string archipelagoPlayerName = "Asara";
            PlayerData playerData = Game.instance.gameData.player;
            string format = "+{0} recieved from {1}";
            string text = string.Format(format, 50, archipelagoPlayerName);

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.EXP, text, Game.instance.player.GetPosition(), 1f);
            playerData.progression.xp.Value += 50;

            Logger.LogInfo($"Collected EXP");

            yield return new WaitForSeconds(10f); // Wait for 5 seconds before triggering

            CombatTextSystem.current.ShowText(CombatTextSystem.TextType.GOLD, text, Game.instance.player.GetPosition(), 1f);
            playerData.currency.gold.Value += 50;

            Logger.LogInfo($"Collected GOLD");
        }
    }
}