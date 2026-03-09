using System.Numerics;
using HarmonyLib;

namespace VeryVeryValetAPClient.Hooks
{
    [HarmonyPatch(typeof(LevelController))]
    public class LevelController_Patches
    {
        [HarmonyPatch(nameof(LevelController.OnTractorPenalty))]
        [HarmonyPrefix]
        public static void OnTractorPenalty(LevelController __instance, Vector3 position)
        {
            if (PluginMain.SlotData.DeathLink)
                PluginMain.ArchipelagoHandler.SendDeath();
        }
        
        [HarmonyPatch(nameof(LevelController.CraneEject))]
        [HarmonyPrefix]
        [HarmonyPatch(new [] {typeof(Car)})]
        public static void OnCraneEject(Car car)
        {
            if (PluginMain.SlotData.DeathLink)
                PluginMain.ArchipelagoHandler.SendDeath();
        }
    }
}