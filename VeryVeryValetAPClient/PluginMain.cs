using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.SceneManagement;
using VeryVeryValetAPClient.Hooks;

namespace VeryVeryValetAPClient
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class PluginMain : BaseUnityPlugin
    {        
        public static ConfigEntry<bool>? EnableDebugLogging;
        public static ConfigEntry<bool>? FilterLog;
        public static ConfigEntry<float>? MessageInTime;
        public static ConfigEntry<float>? MessageHoldTime;
        public static ConfigEntry<float>? MessageOutTime;
        public const string GameName = "Very Very Valet";
        private const string PluginName = "VeryVeryValetAPClient";
        private const string GUID = "very_very_valet_ap_client";
        private const string Version = "1.0.0";

        private readonly Harmony _harmony = new Harmony(GUID);
        public static ManualLogSource? logger;
        
        public static ArchipelagoHandler ArchipelagoHandler;
        public static SlotData SlotData;
        public static ConnectInputHandler ConnectInputHandler;
        public static ItemHandler ItemHandler;
        
        void Awake()
        {
            logger = Logger;
            _harmony.PatchAll();
            DontDestroyOnLoad(gameObject);
            
            SceneManager.sceneLoaded += (scene, mode) =>
            {
            };
            
            ArchipelagoHandler = gameObject.AddComponent<ArchipelagoHandler>();
            ConnectInputHandler = gameObject.AddComponent<ConnectInputHandler>();
            ItemHandler = gameObject.AddComponent<ItemHandler>();
            
            EnableDebugLogging = Config.Bind(
                "Logging",
                "EnableDebugLogging",
                false,
                "Enables or disables debug logging in the Archipelago Console."
            );
            
            FilterLog = Config.Bind(
                "Logging",
                "FilterLog",
                false,
                "Filter the archipelago log to only show messages relevant to you."
            );
            
            MessageInTime = Config.Bind(
                "Logging",
                "MessageInTime",
                0.25f,
                "How long messages take to animate in."
            );
            
            MessageHoldTime = Config.Bind(
                "Logging",
                "MessageHoldTime",
                3f,
                "How long messages stay in the log before animating out."
            );
            
            MessageOutTime = Config.Bind(
                "Logging",
                "MessageOutTime",
                0.5f,
                "How long messages stay in the log before animating out."
            );
        }
    }
}