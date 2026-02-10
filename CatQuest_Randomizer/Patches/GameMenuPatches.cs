using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using static CatQuest_Randomizer.SaveDataHandler;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using CatQuest_Randomizer.Extentions;

namespace CatQuest_Randomizer.Patches
{
    [HarmonyPatch(typeof(Title))]
    [HarmonyPatch("Menu_OnNormalContinueSelect", MethodType.Normal)]
    public class ContinueGamePatch
    {
        static void Prefix()
        {
            RoomInfoData room = SaveDataHandler.RoomInfo;
            Randomizer.ConnectionHandler.Connect(room.Server, room.Playername, room.Password);
            SlotDataHandler slotData = Randomizer.SlotDataHandler.CollectSlotData(Randomizer.ConnectionHandler.SlotData);

            Randomizer.LocationHandler.Init();
            Randomizer.GoalHandler.Init();

            Randomizer.Logger.LogInfo("Game was continued");
        }
    }

    [HarmonyPatch(typeof(NewGamePanel), "Start")]
    public class EditMenuOptionPatch
    {
        private static NewGamePanelOption menuItem;

        static void Prefix(NewGamePanel __instance)
        {
            Randomizer.Logger.LogInfo("Will edit menu item for archipelago.");

            FieldInfo listField = typeof(NewGamePanel).GetField("listItems", BindingFlags.NonPublic | BindingFlags.Instance);
            if (listField == null)
            {

                Randomizer.Logger.LogError("Could not find the 'listItems' field in NewGamePanel.");
                return;
            }

            if (listField.GetValue(__instance) is not List<NewGamePanelOption> listItems || listItems.Count == 0)
            {
                Randomizer.Logger.LogError("listItems is null or empty.");
                return;
            }

            menuItem = listItems[listItems.Count - 1];

            menuItem.SetName("New Archipelago Game");

            FieldInfo descField = typeof(NewGamePanelOption).GetField("optionDescription", BindingFlags.NonPublic | BindingFlags.Instance);
            if (descField != null)
            {
                var textComponent = descField.GetValue(menuItem) as TextMeshProUGUI;
                if (textComponent != null)
                {
                    textComponent.text = "Connect to server and start game";
                }
            }
            else
            {
                Randomizer.Logger.LogError("Could not find 'optionDescription' field in NewGamePanelOption.");
            }
        }
    }

    [HarmonyPatch(typeof(Title), "Start")]
    public class ArchipelagoVersionPatch
    {
        static void Prefix(Title __instance)
        {
            FieldInfo copyrightField = typeof(Title).GetField("copyright", BindingFlags.NonPublic | BindingFlags.Instance);
            if (copyrightField != null)
            {
                var copyrightImg = copyrightField.GetValue(__instance) as TextMeshProUGUI;
                if (copyrightImg != null && Randomizer.DataStorageHandler.apTitleSprite != null)
                {
                    copyrightImg.text = copyrightImg.text + " " + Randomizer.DataStorageHandler.modInfo.ModVersion + ".";
                }
            }
        }
    }

    [HarmonyPatch(typeof(Title), "Start")]
    public class ArchipelagoLogoPatch
    {
        static void Postfix(Title __instance)
        {
            FieldInfo imgField = typeof(Title).GetField("title", BindingFlags.NonPublic | BindingFlags.Instance);
            if (imgField != null)
            {
                var titleImg = imgField.GetValue(__instance) as Image;
                if (titleImg != null && Randomizer.DataStorageHandler.apTitleSprite != null)
                {
                    titleImg.sprite = Randomizer.DataStorageHandler.apTitleSprite;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Title), "ShowConfirmationPanel")]
    public class ConfirmationPanelPatch
    {
        static void Prefix(Title __instance, ref string title, ref string confirmation, ref System.Action onYes, ref System.Action onNo)
        {
            if (title != "New Game")
                return;

            Randomizer.Logger.LogInfo("Will make custom pop up for archipelago.");

            title = "Archipelago Setup";
            confirmation = "Input your room info below:";

            ConfirmationPanel confirmationPanel = AccessTools.Field(typeof(Title), "confirmationPanel").GetValue(__instance) as ConfirmationPanel;
            TextMeshProUGUI confirmationText = AccessTools.Field(typeof(ConfirmationPanel), "confirmation").GetValue(confirmationPanel) as TextMeshProUGUI;
            Transform confirmationParent = confirmationText.transform.parent;

            RoomInfoData room = SaveDataHandler.RoomInfo;

            List<TextMeshProUGUI> InputTexts =
            [
                CreateInputField(confirmationParent, room.Server, confirmationText, 1),
                CreateInputField(confirmationParent, room.Playername, confirmationText, 2),
                CreateInputField(confirmationParent, "", confirmationText, 3)
            ];

            ServerInputHandler.Setup(InputTexts, new List<string> { "Server: ", "Name: ", "Password: " }, confirmationPanel.gameObject);

            var oldNo = onNo;
            onNo = () =>
            {
                Randomizer.Logger.LogInfo("Destroy Custom Popup.");
                confirmationPanel.gameObject.GetComponent<ServerInputHandler>().CleanUp();

                for (int i = 0; i < InputTexts.Count; i++)
                {
                    UnityEngine.Object.Destroy(InputTexts[i].gameObject);
                }

                oldNo?.Invoke();
            };

            var oldYes = onYes;
            onYes = () =>
            {
                string server = GetValue(InputTexts[0].text, "Server: ");
                string player = GetValue(InputTexts[1].text, "Name: ");
                string password = GetValue(InputTexts[2].text, "Password: ");

                ItemIndex = 0;
                SaveRoomInfo(server, player, password);

                if (!Randomizer.ConnectionHandler.Connect(server, player, password))
                {
                    Randomizer.Logger.LogInfo("Could not connect.");
                    Randomizer.Logger.LogInfo("Destroy Custom Popup.");
                    confirmationPanel.gameObject.GetComponent<ServerInputHandler>().CleanUp();

                    for (int i = 0; i < InputTexts.Count; i++)
                    {
                        UnityEngine.Object.Destroy(InputTexts[i].gameObject);
                    }

                    oldNo?.Invoke();
                }
                else
                {
                    Randomizer.SlotDataHandler.ResetSlotData();

                    SlotDataHandler slotData = Randomizer.SlotDataHandler.CollectSlotData(Randomizer.ConnectionHandler.SlotData);
                    Randomizer.LocationHandler.Init();
                    Randomizer.GoalHandler.Init();

                    Randomizer.Logger.LogInfo("New game was started");
                    oldYes?.Invoke();
                }
            };
        }

        private static TextMeshProUGUI CreateInputField(Transform parent, string placeholderText, TextMeshProUGUI confirmationText, int order)
        {
            GameObject InputObject = GameObject.Instantiate(confirmationText.gameObject, parent);
            TextMeshProUGUI inputText = InputObject.GetComponent<TextMeshProUGUI>();
            inputText.name = "ServerInput";
            inputText.text = placeholderText;

            inputText.alignment = TextAlignmentOptions.MidlineLeft;
            InputObject.transform.SetSiblingIndex(confirmationText.transform.GetSiblingIndex() + order);

            return inputText;
        }

        public static string GetValue(string str, string strToRemove)
        {
            string newValue = str.Replace("<", string.Empty);
            newValue = newValue.Replace(strToRemove, string.Empty);
            return newValue;
        }
    }
}

public class ServerInputHandler : MonoBehaviour
{
    private static List<TextMeshProUGUI> inputFields = [];
    private static List<string> prefixes = [];
    private static List<string> currentTexts = [];
    private static int activeField = 0;

    public static void Setup(List<TextMeshProUGUI> fields, List<string> fieldPrefixes, GameObject parentPanel)
    {
        inputFields = fields;
        prefixes = fieldPrefixes;
        currentTexts = [.. fields.Select(f => f != null ? f.text ?? "" : "")];
        activeField = 0;

        if (parentPanel.GetComponent<ServerInputHandler>() == null)
            parentPanel.AddComponent<ServerInputHandler>();
    }

    void Update()
    {
        if (inputFields == null || inputFields.Count == 0) return;

        // --- Handle navigation keys (Tab, Shift+Tab, Up/Down arrows)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (shift)
                activeField = (activeField - 1 + inputFields.Count) % inputFields.Count;
            else
                activeField = (activeField + 1) % inputFields.Count;

            // stop here this frame so Tab isn't also processed as text input (usually not present, but safe)
            UpdateVisuals();
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            activeField = (activeField - 1 + inputFields.Count) % inputFields.Count;
            UpdateVisuals();
            return;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            activeField = (activeField + 1) % inputFields.Count;
            UpdateVisuals();
            return;
        }
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // backspace
            {
                if (currentTexts[activeField].Length > 0)
                    currentTexts[activeField] = currentTexts[activeField].Substring(0, currentTexts[activeField].Length - 1);
            }
            else
            {
                currentTexts[activeField] += c;
            }
        }

        UpdateVisuals();
    }
    public void CleanUp()
    {
        inputFields = [];
        prefixes = [];
        currentTexts = [];
        activeField = 0;
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < inputFields.Count; i++)
        {
            if (inputFields[i] != null)
            {
                inputFields[i].text = prefixes[i] + currentTexts[i] + (i == activeField ? "<" : "");
            }
        }
    }
}
