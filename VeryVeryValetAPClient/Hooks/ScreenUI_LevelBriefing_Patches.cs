using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace VeryVeryValetAPClient.Hooks
{
    [HarmonyPatch(typeof(ScreenUI_LevelBriefing))]
    public class ScreenUI_LevelBriefing_Patches
    {
        [HarmonyPatch(nameof(ScreenUI_LevelBriefing.SetDisplayText))]
        [HarmonyPostfix]
        [HarmonyPatch(new [] {typeof(LevelData)})]
        public static void SetDisplayText(ScreenUI_LevelBriefing __instance, LevelData data)
        {
            __instance._allowRuleSets = false;
            ActiveSceneManager.instance.gameRuleDictionary.GetCurrentPreset();
            var optionsMain = Object.FindObjectOfType<ScreenUI_OptionsMain>();
            var displayKey = PluginMain.SlotData.RequireRedStars ? "difficulty_hard" : "difficulty_normal";
            if (optionsMain != null)
            {
                var preset = optionsMain._ruleDictionary._presets.FirstOrDefault(x => x._displayKey == displayKey);
                optionsMain.SetPreset(preset);
                PlayerSave.SavePreset(data.objective, preset);
                if (!optionsMain.RefreshCheckmarks())
                    return;
                optionsMain._toasterUi.Toast(optionsMain._toastDifficultyChangedKey, 0.0f, preset.displayTranslated);
            }
        }
    }
}