using System;
using System.Linq;
using HarmonyLib;

namespace VeryVeryValetAPClient.Hooks
{
    [HarmonyPatch(typeof(ScreenUI_Title))]
    public class ScreenUI_Title_Patches
    {
        [HarmonyPatch(nameof(ScreenUI_Title.Init))]
        [HarmonyPostfix]
        static void OnInit(ScreenUI_Title __instance)
        {
            var texts = __instance._playButtonUi._button
                .GetComponentsInChildren<LocalizedText>(true);
            foreach (var loc in texts)
                UnityEngine.Object.Destroy(loc);
            var tmps = __instance._playButtonUi._button
                .GetComponentsInChildren<TMPro.TMP_Text>(true);
            foreach (var tmp in tmps.Where(tmp => tmp.text == "Play!"))
                tmp.text = "Connect!";
        }

        [HarmonyPatch(nameof(ScreenUI_Title.OnPlay))]
        [HarmonyPrefix]
        public static bool OnPlay(ScreenUI_Title __instance, DefaultButtonUi button, string name)
        {
            if (!PluginMain.ArchipelagoHandler.IsConnected)
                return true;
            if (!ScreenManager.instance.isSafeToAdjust)
                return false;
            PlayerMgr.ActiveMaxPlayerCount = GameConstants.PlayCountValet;
            ActiveSceneManager.ValetGameMode = ActiveSceneManager.ValetGameModeState.Campaign;
            __instance.AdvanceVia(button, (Action) (() =>
            {
                ActiveSceneManager.instance.GoToMenu(MenuSetManager.CameraState.ValetSelect);
            }));
            return false;
        }

        [HarmonyPatch(nameof(ScreenUI_Title.RefreshScreen))]
        [HarmonyPostfix]
        static void OnRefreshScreen(ScreenUI_Title __instance)
        {
            __instance._valetGamesButtonUi.gameObject.SetActive(false);
            __instance._achievementsButtonUi.gameObject.SetActive(false);
        }
    }
}