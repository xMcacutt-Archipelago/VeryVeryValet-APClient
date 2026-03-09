using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VeryVeryValetAPClient
{
    public class ItemWrapper
    {
        public int Index;
        public ItemInfo Info;

        public ItemWrapper(int index, ItemInfo info)
        {
            Index = index;
            Info = info;
        }
    }
    
    public class ItemHandler : MonoBehaviour
    {
        private Queue<ItemWrapper> cachedItems = new Queue<ItemWrapper>();

        private bool IsGameReady()
        {
            return PluginMain.ArchipelagoHandler.IsConnected;
        }

        public void HandleItem(int index, ItemInfo item, bool save = true)
        {
            try
            {
                if (!IsGameReady())
                {
                    APConsole.Instance.DebugLog($"Game not ready, caching item: {item.ItemName} (index {index})");
                    cachedItems.Enqueue(new ItemWrapper(index, item));
                    return;
                }

                if (cachedItems.Count > 0)
                {
                    APConsole.Instance.DebugLog($"Processing {cachedItems.Count} cached items...");
                    FlushQueue();
                }

                ProcessItem(index, item);
            }
            catch (Exception ex)
            {
                APConsole.Instance.DebugLog($"HandleItem Error: {ex}");
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
                FlushQueue();
            if (Input.GetKeyDown(KeyCode.F4))
                PluginMain.ArchipelagoHandler.Kill();
        }
        
        public void FlushQueue()
        {
            if (!IsGameReady())
            {
                APConsole.Instance.DebugLog("Attempted to flush queue but game is not ready");
                return;
            }

            int processedCount = 0;
            while (cachedItems.Count > 0)
            {
                var itemWrapper = cachedItems.Dequeue();
                ProcessItem(itemWrapper.Index, itemWrapper.Info);
                processedCount++;
            }

            APConsole.Instance.DebugLog($"Flushed {processedCount} cached items");
            if (processedCount > 0)
                CustomSaveDataHandler.Save();
        }

        private void ProcessItem(int index, ItemInfo item)
        {
            if (index < CustomSaveDataHandler.Data.ItemIndex)
            {
                APConsole.Instance.DebugLog($"Item {index} already processed (current: {CustomSaveDataHandler.Data.ItemIndex})");
                return;
            }

            CustomSaveDataHandler.Data.ItemIndex++;

            switch (item.ItemId)
            {
                case 0x100: // Star
                    CustomSaveDataHandler.Data.StarCount++;
                    var levelSelect = FindObjectOfType<ScreenUI_LevelSelect>();
                    if (levelSelect != null && levelSelect.enabled)
                        levelSelect.RefreshStarCount(true);
                    break;
                case 0x101: // Random Power-Up
                    CustomSaveDataHandler.Data.StoredPowerups++;
                    break;
                default:
                    PluginMain.logger.LogWarning($"Unknown item: {item.ItemId} ({item.ItemName})");
                    break;
            }

            CustomSaveDataHandler.Save();
        }
    }
}