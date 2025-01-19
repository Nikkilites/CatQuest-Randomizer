using HarmonyLib;
using UnityEngine;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.Land))]
    public class DisableLandingOnWaterPatch
    {
        static bool Prefix()
        {
            AnimationEventHandler animationEventHandler = Object.FindObjectOfType<AnimationEventHandler>();
            if (animationEventHandler == null)
            {
                Randomizer.Logger.LogError("Could not find AnimationEventHandler instance.");
                return true; // Allow the method to run if the handler isn't found
            }

            int mWaterLayerMask = (int)AccessTools.Field(typeof(AnimationEventHandler), "mWaterLayerMask").GetValue(animationEventHandler);
            Vector2 playerPosition = Game.instance.gameData.player.worldmap.position.Value;

            bool PlayerIsInWater = Physics2D.OverlapPoint(playerPosition, mWaterLayerMask);

            Randomizer.Logger.LogInfo($"Player is in water: {PlayerIsInWater}");

            if (!Game.instance.IsUnlocked(Unlockables.WaterWalking) & PlayerIsInWater)
            {
                Randomizer.Logger.LogInfo("Landing on water was disabled");
                return false;
            }
            return true;
        }
    }
}
