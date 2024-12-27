using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CatQuest_Randomizer.Patches
{
	[BepInPlugin("enable.logger", "EnableLogger", "1.0.0")]
	public class EnableLogger : BaseUnityPlugin
	{
		private static ManualLogSource Logger;

		private void Awake()
		{
			Logger = base.Logger;
			Harmony harmony = new Harmony("enable.logger");
			harmony.PatchAll();
		}
	}
}
