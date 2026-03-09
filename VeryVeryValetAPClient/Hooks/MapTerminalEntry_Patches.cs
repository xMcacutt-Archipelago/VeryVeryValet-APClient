using System;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VeryVeryValetAPClient.Hooks
{
    [HarmonyPatch(typeof(MapTerminalEntry))]
    public class MapTerminalEntry_Patches
    {
        [HarmonyPatch(nameof(MapTerminalEntry.Refresh))]
        [HarmonyPrefix]
        static bool Refresh(MapTerminalEntry __instance, bool instant = true, bool debugUnlock = false, bool updateVisuals = true)
        {
            var mapManager = Object.FindObjectOfType<MapManager>();
            if (mapManager == null || (PluginMain.SlotData.RequireLevelCompletions && __instance.data != mapManager.data._sections[1].levels[0].data))
                return true;
            __instance._isGated = __instance._entry.starCount > 0;
            __instance._animState = MapTerminalEntry.AnimState.None;
            __instance._isRevealed = true;
            __instance._startStarList = null;
            __instance._finalStarList = null;
            __instance._isCompleted = PlayerSave.GetLevelHasBeenCompleted(__instance.data.saveKey);
            var levelCompleteData = PlayerSave.GetLevelCompleteData(__instance.data.saveKey);
            if (levelCompleteData != null)
                __instance._finalStarList = levelCompleteData.GetStarListCombined();
            var mapEntry = PlayerSave.GetMapEntry(__instance.data.saveKey);
            if (mapEntry != null)
            {
                __instance._isRevealed = mapEntry.isRevealed;
                __instance._isGated = mapEntry.isGated;
                __instance._startStarList = mapEntry.lastStarList;
            }
            __instance.SetDisplay(__instance._entry.entryType, __instance._startStarList);
            var face = __instance._entry.entryType;
            if (__instance.isGated)
                face = MapTerminalSpindle.Face.Gate;
            __instance.ShowFace(face, instant);
            return false;
        }
    }
}