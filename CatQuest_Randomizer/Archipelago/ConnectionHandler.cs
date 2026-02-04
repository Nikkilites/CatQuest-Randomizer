using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using CatQuest_Randomizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CatQuest_Randomizer.Archipelago
{
    public class ConnectionHandler
    {
        private ArchipelagoSession session;
        private const string gameName = "Cat Quest";
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
                result = session.TryConnectAndLogin(gameName, player, ItemsHandlingFlags.AllItems, password: pass);
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

            Randomizer.Logger.LogInfo($"Successfully connected to {server}.");

            return true;
        }

        public void OnDisconnect(string reason)
        {
            if (Connected)
            {
                session.Socket.Disconnect();
                Connected = false;
                session = null;
            }
        }

        public void OnError(Exception e, string message)
        {
            message += $"\n    Called from OnError";
            throw new Exception(message);
        }

        public void SendLocation(Location location)
        {
            if (!Connected)
            {
                return;
            }

            long apId = session.Locations.GetLocationIdFromName(gameName, location.Name);

            Randomizer.Logger.LogInfo($"Sending {location.Name} location to server");

            session.Locations.CompleteLocationChecksAsync(OnLocationSent, apId);
        }

        public void SendLocations(IEnumerable<Location> locations)
        {
            if (!Connected)
            {
                return;
            }

            long[] apIds = locations.Select(x => session.Locations.GetLocationIdFromName(gameName, x.Name)).ToArray();

            Randomizer.Logger.LogInfo("Sending multiple locations to server.");

            session.Locations.CompleteLocationChecksAsync(OnLocationSent, apIds);
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

        public string GetPlayerNameFromSlot(int slot)
        {
            return session.Players.GetPlayerName(slot) ?? "Server";
        }

        public string GetItemNameFromId(long id)
        {
            return session.Items.GetItemName(id) ?? $"Item[{id}]";
        }

        public string GetLocationNameFromId(long id)
        {
            return session.Locations.GetLocationNameFromId(id) ?? $"Location[{id}]";
        }
    }
}
