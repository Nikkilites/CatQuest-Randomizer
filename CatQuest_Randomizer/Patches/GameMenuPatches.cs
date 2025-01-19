using HarmonyLib;
using static CatQuest_Randomizer.SaveDataHandler;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(Title), nameof(Title.StartNormalGame))]
    public class NewGamePatch
    {
        static void Prefix()
        {
            Randomizer.Logger.LogInfo("New game was started");
            //When implemented, gets implemented as an extension that takes the info written in the ui to login.
            //SaveDataHandler.SaveRoomInfo(server, player, password);
            ItemIndex = 0;
        }
    }

    [HarmonyPatch(typeof(Title))]
    [HarmonyPatch("Menu_OnNormalContinueSelect", MethodType.Normal)]
    public class ContinueGamePatch
    {
        static void Prefix()
        {
            Randomizer.Logger.LogInfo("Game was continued");
        }
    }

    [HarmonyPatch(typeof(Title), nameof(Title.StartGame))]
    public class GameOpenPatch
    {
        static void Postfix()
        {
            RoomInfoData room = SaveDataHandler.RoomInfo;
            Randomizer.ConnectionHandler.Connect(room.Server, room.Playername, room.Password);
        }
    }
}
