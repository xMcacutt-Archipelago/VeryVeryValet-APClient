using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace VeryVeryValetAPClient.Hooks
{
    [HarmonyPatch(typeof(PlayerSave))]
    public class PlayerSave_Patches
    {
        public static readonly char[] APHeader = { 'A', 'P', 'C', 'M' };
        
        [HarmonyPatch(nameof(PlayerSave.GetSaveFolder))]
        [HarmonyPrefix]
        public static bool GetSaveFolder(ref string __result)
        {
            __result = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ArchipelagoSaves");
            return false;
        }
        
        [HarmonyPatch(nameof(PlayerSave._getFilenameForSlot))]
        [HarmonyPrefix]
        public static bool _getFilenameForSlot(int slot, ref string __result)
        {
            __result = $"Save_{PluginMain.ArchipelagoHandler.Slot}_{PluginMain.ArchipelagoHandler.seed}.sav";
            return false;
        }

        [HarmonyPatch(nameof(PlayerSave.GetTotalStarsEarned))]
        [HarmonyPatch(new [] {typeof(StarFilter)})]
        [HarmonyPrefix]
        public static bool OnGetTotalStarsEarned(StarFilter filter, ref int __result)
        { 
            __result = CustomSaveDataHandler.Data.StarCount;
            return false;
        }

        [HarmonyPatch(nameof(PlayerSave.SaveLevelComplete))]
        [HarmonyPostfix]
        public static void OnSaveLevelComplete(string shortLevelName, PlayerSave.SaveLevelResult __result)
        { 
            if (!LevelOrderHandler.SaveKeyToLevelName.TryGetValue(shortLevelName, out var saveKey))
            {
                Console.WriteLine(shortLevelName);
                return;
            }
            var stars = __result.starCount;
            var levelIndex = LevelOrderHandler.LevelNameToNormalizedNumber[saveKey];
            if (levelIndex == 23)
            {
                APConsole.Instance.Log("GOAL!!!");
                PluginMain.ArchipelagoHandler.Release();
                return;
            }
            var baseLocId = 0x100 + levelIndex * 3;
            var locList = new List<long>();
            for (var i = 0; i < stars; i++)
                locList.Add(baseLocId + i);
            PluginMain.ArchipelagoHandler.CheckLocations(locList.ToArray());
        }

        [HarmonyPatch(nameof(PlayerSave._save))]
        [HarmonyPostfix]
        public static void _save(string filename)
        {
            CustomSaveDataHandler.Save();
        }

        [HarmonyPatch(nameof(PlayerSave._actuallyLoad))]
        [HarmonyPostfix]
        public static void _actuallyLoad(string filename)
        {
            CustomSaveDataHandler.Load();
        }
    }
}