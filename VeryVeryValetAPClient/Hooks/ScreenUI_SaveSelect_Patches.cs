using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace VeryVeryValetAPClient.Hooks
{
    public class ConnectInputHandler : MonoBehaviour
    {
        public static bool IsInputMode = false;
        private static bool inputModeRequested = false;
        private static TextMeshProUGUI? currentText = null;
        private static Stack<ControllerMap> savedMaps = new Stack<ControllerMap>();
        
        private static void DisableInput()
        {
            foreach (var player in ReInput.players.Players)
            {
                foreach (var controllerMap in player.controllers.maps.GetAllMaps().Where(x => x.enabled))
                {
                    savedMaps.Push(controllerMap);
                    controllerMap.enabled = false;
                }
            }
        }

        private static void EnableInput()
        {
            while (savedMaps.Count > 0)
            {
                var map = savedMaps.Pop();
                map.enabled = true;
            }
        }

        public static void InitializeInputMode(TextMeshProUGUI text)
        {
            inputModeRequested = true;
            currentText = text;
            DisableInput();
        }

        private static void CompleteInput()
        {
            IsInputMode = false;
            currentText = null;
            EnableInput();
        }

        private static void ProcessInput(char c)
        {
            if (!IsInputMode)
                return;
            if (c == '\b' && currentText!.text.Length > 0)
                currentText.text = currentText.text.Substring(0, currentText.text.Length - 1);
            if (char.IsControl(c))
                return;
            currentText!.text += c;
        }
        
        void Update()
        {
            if (IsInputMode)
            {
                foreach (var c in Input.inputString)
                    ProcessInput(c);
                if (Input.GetKeyDown(KeyCode.Return))
                    CompleteInput();
            }

            if (!IsInputMode && inputModeRequested)
            {
                IsInputMode = true;
                inputModeRequested = false;
            }
        }
    }
    
    
    [HarmonyPatch(typeof(ScreenUI_SaveSelect))]
    public class ScreenUI_SaveSelect_Patches
    {
        private static TMP_FontAsset? font = null;

        [HarmonyPatch(nameof(ScreenUI_SaveSelect.Init))]
        [HarmonyPrefix]
        static void OnInit(ScreenUI_SaveSelect __instance)
        {
            var serverObj = __instance._rootGroup.gameObject.transform.GetChild(0);
            if (__instance._rootGroup.transform.childCount != 4)
            {
                APConsole.Instance.Log("Welcome to Very Very Valet Archipelago!");
                font = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(x => x.name == "Text-regular");
                Object.Instantiate(serverObj, __instance._rootGroup.transform);
            }
        }
        
        [HarmonyPatch(nameof(ScreenUI_SaveSelect.ScreenShow))]
        [HarmonyPostfix]
        static void OnScreenShow(ScreenUI_SaveSelect __instance, bool doShow)
        {
            if (!doShow) return;
            
            var serverObj = __instance._rootGroup.gameObject.transform.GetChild(0);
            var serverButton = serverObj.GetComponent<Button>();
            var locs = serverObj.GetComponentsInChildren<LocalizedText>(true);
            var texts = serverObj.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var loc in locs)
                Object.Destroy(loc);
            foreach (var text in texts)
                text.text = "Server"; 
            
            var slotObj = __instance._rootGroup.gameObject.transform.GetChild(1);
            var slotButton = slotObj.GetComponent<Button>();
            locs = slotObj.GetComponentsInChildren<LocalizedText>(true);
            texts = slotObj.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var loc in locs)
                Object.Destroy(loc);
            foreach (var text in texts)
                text.text = "Slot"; 
            
            var passwordObj = __instance._rootGroup.gameObject.transform.GetChild(2);
            var passwordButton = passwordObj.GetComponent<Button>();
            locs = passwordObj.GetComponentsInChildren<LocalizedText>(true);
            texts = passwordObj.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var loc in locs)
                Object.Destroy(loc);
            foreach (var text in texts)
                text.text = "Password";

            var connectObj = __instance._rootGroup.transform.GetChild(3).gameObject;
            texts = connectObj.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var loc in locs)
                Object.Destroy(loc);
            foreach (var text in texts)
                text.text = "Connect!"; 
            var connectButton = connectObj.GetComponent<Button>();
            
            ConnectionInfoHandler.Load();
            
            var serverInputObj = __instance._windowDatas[0].window._enableHandle;
            locs = serverInputObj.GetComponentsInChildren<LocalizedText>(true);
            var serverTmp = serverInputObj.GetComponentInChildren<TextMeshProUGUI>(true);
            foreach (var loc in locs)
                Object.Destroy(loc);
            serverTmp.text = ConnectionInfoHandler.SavedServer;
            serverTmp.font = font;
            
            var slotInputObj = __instance._windowDatas[1].window._enableHandle;
            locs = slotInputObj.GetComponentsInChildren<LocalizedText>(true);
            var slotTmp = slotInputObj.GetComponentInChildren<TextMeshProUGUI>(true);
            foreach (var loc in locs)
                Object.Destroy(loc);
            slotTmp.text = ConnectionInfoHandler.SavedSlot; 
            slotTmp.font = font;
            
            var passwordInputObj = __instance._windowDatas[2].window._enableHandle;
            locs = passwordInputObj.GetComponentsInChildren<LocalizedText>(true);
            var passTmp = passwordInputObj.GetComponentInChildren<TextMeshProUGUI>(true);
            foreach (var loc in locs)
                Object.Destroy(loc);
            passTmp.text = ConnectionInfoHandler.SavedPassword; 
            passTmp.font = font;

            
            serverButton.onClick = new Button.ButtonClickedEvent();
            serverButton.onClick.AddListener(() =>
            {
                ConnectInputHandler.InitializeInputMode(serverTmp);
            });

            slotButton.onClick = new Button.ButtonClickedEvent();
            slotButton.onClick.AddListener(() =>
            {
                ConnectInputHandler.InitializeInputMode(slotTmp);
            });
            
            passwordButton.onClick = new Button.ButtonClickedEvent();
            passwordButton.onClick.AddListener(() =>
            {
                ConnectInputHandler.InitializeInputMode(passTmp);
            });
            
            connectButton.onClick = new Button.ButtonClickedEvent();
            connectButton.onClick.AddListener(() =>
            {
                PluginMain.ArchipelagoHandler!.CreateSession(serverTmp.text, slotTmp.text, passTmp.text);
                PluginMain.ArchipelagoHandler.OnConnected += () =>
                {
                    ConnectionInfoHandler.Save(serverTmp.text, slotTmp.text, passTmp.text);
                    PlayerSave.SetActiveSlot(1, () => __instance._finishSelect(0));
                    var locServerObj = __instance._rootGroup.gameObject.transform.GetChild(0);
                    var locTexts = locServerObj.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (var text in locTexts)
                        text.text = "Let's Valet!"; 
                };
                PluginMain.ArchipelagoHandler.OnConnectionFailed += (e) =>
                {
                    APConsole.Instance.Log($"Connection failed: {e}");
                };
                PluginMain.ArchipelagoHandler.Connect();
            });
        }
    }
}