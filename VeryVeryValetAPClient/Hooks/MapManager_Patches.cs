using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace VeryVeryValetAPClient.Hooks
{
    [HarmonyPatch(typeof(MapManager))]
    public class MapManager_Patches
    {
        [HarmonyPatch(nameof(MapManager.Init))]
        [HarmonyPrefix]
        static void OnInit(MapManager __instance)
        {
            if (!PluginMain.ArchipelagoHandler?.IsConnected ?? true)
                return;

            var map = __instance._data;

            var originalLevels = map._sections
                .Skip(1)
                .SelectMany(s => s.levels)
                .ToList();

            var normalizedIndices = BuildNormalizedIndices();

            var vanillaToNormalized = normalizedIndices
                .Select((vanillaIndex, normalizedIndex) => new { vanillaIndex, normalizedIndex })
                .ToDictionary(x => x.vanillaIndex, x => x.normalizedIndex);

            var normalizedLevels = new LevelMapSection.Entry[originalLevels.Count];

            for (var vanillaIndex = 0; vanillaIndex < originalLevels.Count; vanillaIndex++)
            {
                var normalizedIndex = vanillaToNormalized[vanillaIndex];
                normalizedLevels[normalizedIndex] = originalLevels[vanillaIndex];
            }

            var normalizedNameToIndex = normalizedIndices
                .Select((vanillaIndex, normalizedIndex) => new
                {
                    name = LevelOrderHandler.LevelNumberToName[vanillaIndex].Name,
                    normalizedIndex
                })
                .ToDictionary(x => x.name, x => x.normalizedIndex);

            var shuffled = new LevelMapSection.Entry[normalizedLevels.Length];

            for (int vanillaIndex = 0; vanillaIndex < originalLevels.Count; vanillaIndex++)
            {
                var normalizedIndex = vanillaToNormalized[vanillaIndex];
                var originLevel = LevelOrderHandler.LevelNumberToName[vanillaIndex];

                var newLevelName = originLevel.Type switch
                {
                    APLevelType.Level => PluginMain.SlotData?.LevelMap[originLevel.Name],
                    APLevelType.Bonus => PluginMain.SlotData?.BonusMap[originLevel.Name],
                    APLevelType.Final => PluginMain.SlotData?.FinalMap[originLevel.Name],
                    _ => null
                };

                if (newLevelName == null)
                    continue;
                
                var destinationIndex = normalizedNameToIndex[newLevelName];

                shuffled[normalizedIndex] = normalizedLevels[destinationIndex];
            }

            var index = 0;
            
            for (var sectionIndex = 1; sectionIndex < map._sections.Length; sectionIndex++)
            {
                var section = map._sections[sectionIndex];

                for (var levelIndex = 0; levelIndex < section.levels.Length; levelIndex++)
                {
                    section.levels[levelIndex] = shuffled[index++];

                }
            }

            index = 0;
            LevelData? previousLevel = null;
            
            for (var sectionIndex = 1; sectionIndex < map._sections.Length; sectionIndex++)
            {
                var section = map._sections[sectionIndex];

                for (var levelIndex = 0; levelIndex < section.levels.Length; levelIndex++)
                {
                    var terminalEntry = __instance._terminals[sectionIndex]._entries[levelIndex];

                    int starCount = 2 * index;

                    if (starCount > 0)
                    {
                        var gateFace = terminalEntry._spindleTitle._faces[4];
                        gateFace.text.SetKey("locked");
                        gateFace.text.enabled = true;
                        gateFace.text.m_Text.enabled = true;

                        var gateStars = terminalEntry._spindleStars[0];
                        gateStars.SetFace(MapTerminalSpindle.Face.Gate, true, StarType.Default);

                        var tens = terminalEntry._spindleStars[1]._faces[4];
                        tens.text.SetKey("generic_key");
                        tens.text.m_Text.text = starCount < 10
                            ? "0"
                            : starCount.ToString()[0].ToString();
                        tens.text.m_Text.enabled = true;

                        var units = terminalEntry._spindleStars[2]._faces[4];
                        units.text.SetKey("generic_key");
                        units.text.m_Text.text = starCount < 10
                            ? starCount.ToString()
                            : starCount.ToString()[1].ToString();
                        units.text.m_Text.enabled = true;

                        section.levels[levelIndex].starCount = starCount;
                    }

                    if (previousLevel != null)
                        section.levels[levelIndex].prereqs = new[] { previousLevel };
                    else
                        section.levels[levelIndex].prereqs = new LevelData[] { };
                    previousLevel = section.levels[levelIndex].data;
                    index++;
                }
            }
        }

        private static List<int> BuildNormalizedIndices()
        {
            var result = new List<int>();

            for (var sectionIndex = 0; sectionIndex < 4; sectionIndex++)
            {
                var start = sectionIndex * 6;

                var normals = new List<int>();
                int bonus = -1;
                int final = -1;

                for (var i = 0; i < 6; i++)
                {
                    var globalIndex = start + i;
                    var type = LevelOrderHandler.LevelNumberToName[globalIndex].Type;

                    switch (type)
                    {
                        case APLevelType.Level:
                            normals.Add(globalIndex);
                            break;

                        case APLevelType.Bonus:
                            bonus = globalIndex;
                            break;

                        case APLevelType.Final:
                            final = globalIndex;
                            break;
                    }
                }

                result.AddRange(normals);
                result.Add(bonus);
                result.Add(final);
            }

            return result;
        }

        [HarmonyPatch(nameof(MapManager.GetDisplayStarCount))]
        [HarmonyPrefix]
        public static bool GetDisplayStarCount(ref int __result)
        {
            __result = CustomSaveDataHandler.Data.StarCount;
            return false;
        }
        
    }
}