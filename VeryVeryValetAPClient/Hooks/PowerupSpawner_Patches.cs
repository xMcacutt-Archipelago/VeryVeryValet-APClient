using HarmonyLib;

namespace VeryVeryValetAPClient.Hooks
{
    [HarmonyPatch(typeof(PowerupSpawner))]
    public class PowerupSpawner_Patches
    {
        [HarmonyPatch(nameof(PowerupSpawner.OnLevelStart))]
        [HarmonyPostfix]
        public static void OnOnLevelStart(PowerupSpawner __instance)
        {
            if (CustomSaveDataHandler.Data.StoredPowerups <= 0) return;
            CustomSaveDataHandler.Data.StoredPowerups--;
            __instance.Spawn();
        } 
    }
}