using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using CatQuest_Randomizer.Model;
using System;
using System.Collections.Generic;

namespace CatQuest_Randomizer.Archipelago
{
    public class ConnectionHandler
    {
        private ArchipelagoSession session;
        private const string gameName = "Cat Quest";
        public Dictionary<string, object> SlotData { get; private set; }
        public bool Connected { get; private set; }

        public ConnectionHandler() { }

        public bool Connect(string server, string player, string pass)
        {
            Randomizer.Logger.LogInfo($"Will try to connect to server with {server}, {player}, {pass}.");

            LoginResult result;

            try
            {
                session = ArchipelagoSessionFactory.CreateSession(server);
                session.Items.ItemReceived += Randomizer.ItemHandler.OnItemReceived;
                session.Socket.SocketClosed += OnDisconnect;
                session.Socket.ErrorReceived += OnError;
                result = session.TryConnectAndLogin(gameName, player, ItemsHandlingFlags.AllItems, password: pass, requestSlotData: true);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {server} as {player}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                Randomizer.Logger.LogInfo(errorMessage);
                return false;
            }

            Connected = true;
            LoginSuccessful loginSuccess = (LoginSuccessful)result;

            SlotData = loginSuccess.SlotData;

            Randomizer.Logger.LogInfo($"Successfully connected to {server}.");

            return true;
        }

        public void OnDisconnect(string reason)
        {
            if (Connected)
            {
                Connected = false;
                session = null;
                HelperMethods.SaveCatQuestGame();
                Randomizer.Logger.LogInfo($"Disconnected {reason}");
                throw new Exception($"You have been disconnected from Archipelago. Please reload the game and login again. Your game has been saved. You can find more information in the mod log");
            }
        }

        public void OnError(Exception e, string message)
        {
            message += $"\n    Called from OnError";
            HelperMethods.SaveCatQuestGame();
            Randomizer.Logger.LogInfo($"Disconnected {message}");
            throw new Exception($"An error has occured in your connection to Archipelago. Please reload the game and login again. Your game has been saved. You can find more information in the mod log");
        }

        public void SendLocation(Location location)
        {
            if (!Connected)
            {
                HelperMethods.SaveCatQuestGame();
                Randomizer.Logger.LogInfo($"Location could not be sent. No longer connected to archipelago");
                throw new Exception($"Location could not be sent. No longer connected to archipelago. Your game has been saved. You can find more information in the mod log");
            }

            long apId = session.Locations.GetLocationIdFromName(gameName, location.Name);

            Randomizer.Logger.LogInfo($"Sending {location.Name} location to server");

            session.Locations.CompleteLocationChecksAsync(OnLocationSent, apId);
        }

        void OnLocationSent(bool successful)
        {
            Randomizer.Logger.LogInfo($"Check sent successfully: {successful}");
        }

        public void SendGoal()
        {
            var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;

            Randomizer.Logger.LogInfo($"Sending goal to server");

            session.Socket.SendPacket(statusUpdatePacket);
        }

        public int GetThisSlotId()
        {
            return session.ConnectionInfo.Slot;
        }

        public string GetPlayerNameFromSlot(int slot)
        {
            return session.Players.GetPlayerName(slot) ?? "Server";
        }

        public string GetPlayerAliasFromSlot(int slot)
        {
            return session.Players.GetPlayerAlias(slot) ?? "Server";
        }

        public string GetItemNameFromId(long id)
        {
            return session.Items.GetItemName(id) ?? $"Item[{id}]";
        }

        public string GetLocationNameFromId(long id)
        {
            return session.Locations.GetLocationNameFromId(id) ?? $"Location[{id}]";
        }

        public int GetServerDataStorage(string key)
        {
            return session.DataStorage[key];
        }

        public void UpdateServerDataStorage(string key, int value)
        {
            Randomizer.Logger.LogInfo($"Update Server Data Storage {key} to {value}");
            session.DataStorage[key] = value;
        }
    }
}
