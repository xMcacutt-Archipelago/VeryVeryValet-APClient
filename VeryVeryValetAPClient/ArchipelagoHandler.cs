using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Colors;
using Archipelago.MultiClient.Net.Converters;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.MessageLog.Parts;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = System.Random;

namespace VeryVeryValetAPClient
{
    public class ArchipelagoHandler : MonoBehaviour
    {
        private ArchipelagoSession? Session { get; set; }

        private string? Server { get; set; }
        public string? Slot { get; set; }
        private string? Password { get; set; }
        public string? seed;
        public bool IsConnected => Session?.Socket.Connected ?? false;

        public event System.Action? OnConnected;
        public event Action<string>? OnConnectionFailed;
        public event System.Action? OnDisconnected;

        private ConcurrentQueue<long> _locationsToCheck = new ConcurrentQueue<long>();
        private readonly Random _random = new Random();

        public volatile bool connectionFinished;
        public volatile bool connectionSucceeded;
        private readonly bool _queueBreak = false;
        
        private string? _lastDeath;
        private DateTime _lastDeathLinkTime = DateTime.Now;
        
        private readonly string[] _deathMessages =
        {
            "had a skill issue (died)",
            "drove off a cliff (died)",
            "wasn't very valet (died)",
            "never got their license (died)",
            "was abducted by aliens (died)",
            "caused a pile-up (died)",
            "went the wrong way down a one way street (died)",
            "took too long (died)",
            "only knows how to drive automatic (died)",
            "didn't check their mirrors (died)",
            "forgot to look both ways (died)",
        };

        public void CreateSession(string server, string slot, string password)
        {
            Server = server;
            Slot = slot;
            Password = password;
            _locationsToCheck = new ConcurrentQueue<long>();
            Session = ArchipelagoSessionFactory.CreateSession(Server);
            Session.MessageLog.OnMessageReceived += OnMessageReceived;
            Session.Socket.ErrorReceived += OnError;
            Session.Socket.SocketClosed += OnSocketClosed;
            Session.Items.ItemReceived += ItemReceived;
        }

        public IEnumerator ConnectRoutine()
        {
            APConsole.Instance.Log($"Logging in to {Server} as {Slot}...");
            var connectTask = Session!.ConnectAsync();

            yield return new WaitUntil(() => connectTask.IsCompleted);

            if (connectTask.Exception != null)
            {
                APConsole.Instance.Log(connectTask.Exception.ToString());
                yield break;
            }
            
            seed = connectTask.Result.SeedName;
            
            var loginTask = Session?.LoginAsync(
                PluginMain.GameName,
                Slot,
                ItemsHandlingFlags.AllItems,
                new System.Version(0, 6, 5), 
                new string[0],
                password: Password
            );

            yield return new WaitUntil(() => loginTask.IsCompleted);
            if (loginTask.Exception != null)
            {
                APConsole.Instance.Log(loginTask.Exception.ToString());
                yield break;
            }

            
            if (loginTask.Result.Successful)
            {
                APConsole.Instance.Log($"Success! Connected to {Server}");
                var successResult = (LoginSuccessful)loginTask.Result;
                PluginMain.SlotData = new SlotData(successResult.SlotData);

                PluginMain.ArchipelagoHandler.StartCoroutine(RunCheckQueue());
                connectionSucceeded = true;
                connectionFinished = true;
                OnConnected?.Invoke();
                yield break;
            }

            connectionSucceeded = false;
            connectionFinished = true;
            if (loginTask.Result != null)
            {
                var failure = (LoginFailure)loginTask.Result;
                var errorMessage = $"Failed to Connect to {Server} as {Slot}:";
                errorMessage = failure.Errors.Aggregate(errorMessage, (current, error) => current + $"\n    {error}");
                errorMessage =
                    failure.ErrorCodes.Aggregate(errorMessage, (current, error) => current + $"\n    {error}");
                OnConnectionFailed?.Invoke(errorMessage);
                APConsole.Instance.Log(errorMessage);
            }

            APConsole.Instance.Log("Attempting reconnect...");
        }

        public void Connect()
        {
            StartCoroutine(ConnectRoutine());
        }

        public void Disconnect()
        {
            if (Session == null)
                return;
            StopAllCoroutines();
            Session.Socket.DisconnectAsync();
            Session = null;
            APConsole.Instance.Log("Disconnected from Archipelago");
        }

        private void OnError(Exception ex, string message)
        {
            APConsole.Instance.Log($"Socket error: {message} - {ex.Message}");
        }

        private void OnSocketClosed(string reason)
        {
            StopAllCoroutines();
            APConsole.Instance.Log($"Socket closed: {reason}");
            OnDisconnected?.Invoke();
        }

        private void ItemReceived(ReceivedItemsHelper helper)
        {
            try
            {
                while (helper.Any())
                {
                    var itemIndex = helper.Index;
                    var item = helper.DequeueItem();
                    PluginMain.ItemHandler.HandleItem(itemIndex, item);
                }
            }
            catch (Exception ex)
            {
                APConsole.Instance.Log($"ItemReceived Error: {ex}");
                throw;
            }
        }
        
        public void Release()
        {
            Session?.SetGoalAchieved();
            Session?.SetClientState(ArchipelagoClientState.ClientGoal);
        }

        public void CheckLocations(long[] ids)
        {
            ids.ToList().ForEach(CheckLocation);
        }

        public void CheckLocation(long id)
        {
            if (!IsLocationChecked(id))
                _locationsToCheck.Enqueue(id);
        }

        private IEnumerator RunCheckQueue()
        {
            while (true)
            {
                if (_locationsToCheck.TryDequeue(out var locationId))
                {
                    Session?.Locations.CompleteLocationChecks(locationId);
                    APConsole.Instance.DebugLog($"Sent location check: {locationId}");
                }

                if (_queueBreak)
                    yield break;
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        public void ResyncItems()
        {
            if (!IsConnected)
            {
                APConsole.Instance.DebugLog("Cannot resync items: Not connected to Archipelago");
                return;
            }

            APConsole.Instance.DebugLog("Resyncing items from server...");
            var items = Session?.Items.AllItemsReceived;
            if (items != null)
                for (var i = 0; i < items.Count; i++)
                    PluginMain.ItemHandler.HandleItem(i, items[i], false);

            CustomSaveDataHandler.Save();
            if (items != null)
                APConsole.Instance.DebugLog($"Resync complete. Processed up to item {items.Count}");
        }
        
        private void PacketReceived(ArchipelagoPacketBase packet)
        {
            switch (packet)
            {
                case BouncePacket bouncePacket:
                    BouncePacketReceived(bouncePacket);
                    break;
            }
        }
        
        public void SendDeath()
        {
            APConsole.Instance.DebugLog("SendDeath called");
            if (!PluginMain.SlotData.DeathLink)
                return;

            var packet = new BouncePacket();
            var now = DateTime.Now;

            if (now - _lastDeathLinkTime < TimeSpan.FromSeconds(2))
                return;

            packet.Tags = new List<string> { "DeathLink" };
            packet.Data = new Dictionary<string, JToken>
            {
                { "time", now.ToUnixTimeStamp() },
                { "source", Slot },
                { "cause", $"{Slot} {_deathMessages[_random.Next(_deathMessages.Length)]}" }
            };

            _lastDeathLinkTime = now;
            Session?.Socket.SendPacket(packet);
        }

        private void BouncePacketReceived(BouncePacket packet)
        {
            if (PluginMain.SlotData.DeathLink)
                ProcessBouncePacket(packet, "DeathLink", ref _lastDeath, (source, data) =>
                    HandleDeathLink(source, data["cause"]?.ToString() ?? "Unknown"));
        }

        private static void ProcessBouncePacket(BouncePacket packet, string tag, ref string? lastTime,
            Action<string, Dictionary<string, JToken>> handler)
        {
            if (!packet.Tags.Contains(tag)) return;
            if (!packet.Data.TryGetValue("time", out var timeObj))
                return;
            if (lastTime == timeObj.ToString())
                return;
            lastTime = timeObj.ToString();
            if (!packet.Data.TryGetValue("source", out var sourceObj))
                return;
            var source = sourceObj?.ToString() ?? "Unknown";
            if (packet.Data.TryGetValue("cause", out var causeObj))
            {
                var cause = causeObj?.ToString() ?? "Unknown";
                APConsole.Instance.DebugLog($"Received Bounce Packet with Tag: {tag} :: {cause}");
            }

            handler(source, packet.Data);
        }

        private void HandleDeathLink(string source, string cause)
        {
            if (!PluginMain.SlotData.DeathLink)
                return;
            APConsole.Instance.Log(cause);
            if (source == Slot)
                return;
            Kill();
        }

        public void Kill()
        {
            var playerManager = FindObjectOfType<PlayerMgr>();
            if (playerManager == null) 
                return;
            playerManager.SessionStart(false);
        }
        
        public bool IsLocationChecked(long id)
        {
            return Session != null && Session.Locations.AllLocationsChecked.Contains(id);
        }

        public int CountLocationsCheckedInRange(long start, long end)
        {
            return Session != null ? Session.Locations.AllLocationsChecked.Count(loc => loc >= start && loc < end) : 0;
        }

        public int CountLocationsCheckedInRange(long start, long end, long delta)
        {
            return Session != null
                ? Session.Locations.AllLocationsChecked.Count(loc =>
                    loc >= start && loc < end && loc % delta == start % delta)
                : 0;
        }

        public void UpdateTags(List<string> tags)
        {
            var packet = new ConnectUpdatePacket
            {
                Tags = tags.ToArray(),
                ItemsHandling = ItemsHandlingFlags.AllItems
            };
            Session?.Socket.SendPacket(packet);
        }

        private void OnMessageReceived(LogMessage message)
        {
            string messageStr;
            if (message.Parts.Any(x => x.Type == MessagePartType.Player) &&
                PluginMain.FilterLog != null &&
                PluginMain.FilterLog.Value &&
                !message.Parts.Any(x => x.Text.Contains(Session!.Players.GetPlayerName(Session.ConnectionInfo.Slot))))
                return;
            if (message.Parts.Length == 1)
            {
                messageStr = message.Parts[0].Text;
            }
            else
            {
                var builder = new StringBuilder();
                foreach (var part in message.Parts)
                {
                    builder.Append($"{part.Text}");
                }

                messageStr = builder.ToString();
            }

            APConsole.Instance.Log(messageStr);
        }

        public ScoutedItemInfo? TryScoutLocation(long locationId)
        {
            return Session?.Locations.ScoutLocationsAsync(locationId)?.Result?.Values.First();
        }
    }
}