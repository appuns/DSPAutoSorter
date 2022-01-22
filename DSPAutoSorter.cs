using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using static UnityEngine.GUILayout;


namespace DSPAutoSorter
{
    [BepInPlugin("Appun.DSP.plugin.AutoSorter", "DSPAutoSorter", "1.2.5")]

    public class DSPAutoSorter : BaseUnityPlugin
    {
        static public DSPAutoSorter self;

        private static ConfigEntry<bool> enableForcedSort;
        private static ConfigEntry<bool> enableSortInInventry;
        private static ConfigEntry<bool> enableSortInStorage;
        private static ConfigEntry<bool> enableSortInFuelChamber;
        private static ConfigEntry<bool> enableSortInMiner;
        private static ConfigEntry<bool> enableSortInAssembler;

        //public static GameObject configButton;

        public static Sprite settingIconSprite;
        public static Sprite squareIconSprite;

        public void Awake()
        {
            LogManager.Logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            enableSortInInventry = Config.Bind("General", "enableSortInInventry", true, "enable sort in normal inventry.");
            enableSortInStorage = Config.Bind("General", "enableSortInStorage", true, "enable sort in storages.");
            enableSortInFuelChamber = Config.Bind("General", "enableSortInFuelChamber", true, "enable sort in Mecha fuelchamber.");
            enableSortInMiner = Config.Bind("General", "enableSortInMiner", true, "enable sort in inventry of miner.");
            enableSortInAssembler = Config.Bind("General", "enableSortInAssembler", true, "enable sort in inventry of Assemblers or smelter.");
            enableForcedSort = Config.Bind("ForcedSort", "enableForcedSort", true, "enable forced sort.");

            // Sprite[] sprites = (Sprite[])Resources.LoadAll<Sprite>("");
            //      //LogManager.Logger.LogInfo("----------------------------------------icon load");
            //foreach(Sprite sprite in sprites)
            // {
            //     if (sprite.name == "settings-icon")
            //     {
            //         settingIconSprite = sprite;
            //         //LogManager.Logger.LogInfo("-------------------------------------settingIconSprite name : " + settingIconSprite.name);
            //     } else if (sprite.name == "solid-bg")
            //     {
            //         squareIconSprite = sprite;
            //         //LogManager.Logger.LogInfo("-------------------------------------squareIconSprite name : " + squareIconSprite.name);
            //     }
            // }
            settingIconSprite = Resources.Load<Sprite>("ui/textures/sprites/icons/settings-icon");
            if (settingIconSprite == null )
            {
                LogManager.Logger.LogInfo("-------------------------------------settingIconSprite is null");

            }

            squareIconSprite = Resources.Load<Sprite>("ui/textures/sprites/icons/solid-bg");
            if (squareIconSprite == null)
            {
                LogManager.Logger.LogInfo("-------------------------------------squareIconSprite is null");

            }

        }




        //インベントリ
        [HarmonyPatch(typeof(UIGame), "OpenPlayerInventory")]
        public static class UIGame_OpenPlayerInventory_Postfix
        {
            [HarmonyPostfix]

            public static void Postfix(UIGame __instance)
            {

                if (enableSortInInventry.Value)
                {
                    __instance.inventory.OnSort();
                }
            }
        }

        //インベントリ強制ソート
        [HarmonyPatch(typeof(UIStorageGrid), "_OnUpdate")]
        public static class UIStorageGrid__OnUpdate_Postfix
        {
            [HarmonyPostfix]

            public static void Postfix(UIStorageGrid __instance)
            {
                if (enableSortInInventry.Value && enableForcedSort.Value && __instance.name == "Player Inventory") 
                {
                    __instance.OnSort();
                    //LogManager.Logger.LogInfo("+++++++++++++++++++++++++++++++++" + __instance.name);
                }
            }
        }


        //メカ燃焼室
        [HarmonyPatch(typeof(UIMechaWindow), "_OnOpen")]
        public static class UIMechaWindow_OnOpen_Postfix
        {
            [HarmonyPostfix]

            public static void Postfix(UIMechaWindow __instance)
            {


                if (enableSortInFuelChamber.Value)
                {
                    GameMain.mainPlayer.mecha.reactorStorage.Sort(true);
                }

            }
        }

        //メカ燃焼室強制
        [HarmonyPatch(typeof(UIMechaWindow), "_OnUpdate")]
        public static class UIMechaWindow_OnUpdate_Postfix
        {
            [HarmonyPostfix]

            public static void Postfix(UIMechaWindow __instance)
            {


                if (enableSortInFuelChamber.Value && enableForcedSort.Value)
                {
                    GameMain.mainPlayer.mecha.reactorStorage.Sort(true);
                }

            }
        }

        //ストレージのオプションボタン作成
        //[HarmonyPatch(typeof(UIStorageWindow), "_OnInit")]
        //public static class UIStorageWindow_OnInit_Postfix
        //{
        //    [HarmonyPrefix]

        //    public static void Prefix()
        //    {
        //        LogManager.Logger.LogInfo("+++++++++++++++++++++++++++++++++ストレージのオプションボタン作成");
        //        GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Player Inventory/panel-bg/btn-box").GetComponent<RectTransform>().sizeDelta = new Vector2(85, GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Player Inventory/panel-bg/btn-box").GetComponent<RectTransform>().sizeDelta.y);　// 60，24
        //        GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Player Inventory/panel-bg/btn-box").transform.localPosition = new Vector3(219, GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Player Inventory/panel-bg/btn-box").transform.localPosition.y, 0);　// 234, 325, 0
        //        GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Player Inventory/panel-bg/btn-box/sort-btn").transform.localPosition = new Vector3(-27, 0, 0); // -15, 0, 0
        //        GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Player Inventory/panel-bg/btn-box/close-btn").transform.localPosition = new Vector3(30, 0, 0); // 15, 0. 0

        //        //ボタンの作成
        //        configButton = Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Player Inventory/panel-bg/btn-box/sort-btn"), GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Player Inventory/panel-bg/btn-box").transform) as GameObject;
        //        configButton.name = "configButton";
        //        configButton.transform.localPosition = new Vector3(1.5f, 0, 0);
        //        configButton.transform.Find("x").localPosition = new Vector3(0, 0, 0);
        //        configButton.GetComponent<RectTransform>().sizeDelta = new Vector2(27, 18);
        //        //var sprite = Resources.Load<Sprite>("settings-icon");
        //        //LogManager.Logger.LogInfo("sprite.name" + sprite.name);

        //        //configButton.transform.Find("x").GetComponent<Image>().sprite = settingIconSprite;
        //        //LogManager.Logger.LogInfo("sprite.name 1 = " + configButton.GetComponent<Image>().sprite.name);
        //        //configButton.GetComponent<Image>().sprite = squareIconSprite;
        //        //LogManager.Logger.LogInfo("sprite.name 2 = " + configButton.GetComponent<Image>().sprite.name);
        //        //ボタンイベントの作成
        //        //configButton.GetComponent<UIButton>().button.onClick.AddListener(new UnityAction(OnSignButtonClick));

        //    }
        //}


        //ストレージ
        [HarmonyPatch(typeof(UIStorageWindow), "_OnOpen")]
        public static class UIStorageWindow_OnOpen_Postfix
        {
            [HarmonyPostfix]

            public static void Postfix(UIStorageWindow __instance)
            {
                if (enableSortInStorage.Value)
                {
                    __instance.OnSortClick();
                }
            }
        }

        //ストレージ強制
        [HarmonyPatch(typeof(UIStorageWindow), "_OnUpdate")]
        public static class UIStorageWindow_OnUpdate_Postfix
        {
            [HarmonyPostfix]

            public static void Postfix(UIStorageWindow __instance)
            {
                if (enableSortInStorage.Value && enableForcedSort.Value)
                {
                    __instance.OnSortClick();
                }
            }
        }



        //採掘機のインベントリ
        [HarmonyPatch(typeof(UIMinerWindow), "_OnOpen")]
        public static class UIMinerWindow_OnOpen_Postfix
        {
            [HarmonyPostfix]

            public static void Postfix(UIMinerWindow __instance)
            {
                if (enableSortInMiner.Value)
                {
                    //ref UIStorageGrid playerInventory = ref AccessTools.FieldRefAccess<UIMinerWindow, UIStorageGrid>(UIRoot.instance.uiGame.minerWindow, "playerInventory");
                    if (__instance.playerInventory.active)
                    {
                        ref StorageComponent Storage = ref AccessTools.FieldRefAccess<UIStorageGrid, StorageComponent>(__instance.playerInventory, "storage");
                        Storage.Sort();
                    }
                }
            }
        }

        //採掘機のインベントリ強制
        [HarmonyPatch(typeof(UIMinerWindow), "_OnUpdate")]
        public static class UIMinerWindow__OnUpdate_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(UIMinerWindow __instance)
            {

                if (enableSortInMiner.Value && enableForcedSort.Value)
                {
                    if (__instance.playerInventory.active)
                    {
                        ref StorageComponent Storage = ref AccessTools.FieldRefAccess<UIStorageGrid, StorageComponent>(__instance.playerInventory, "storage");
                        Storage.Sort();
                    }
                }
            }
        }



        //組立機のインベントリ
        [HarmonyPatch(typeof(UIAssemblerWindow), "_OnOpen")]
        public static class UIAssemblerWindow_OnOpen_Postfix
        {
            [HarmonyPostfix]

            public static void Postfix(UIAssemblerWindow __instance)
            {
                if (enableSortInAssembler.Value)
                {
                    //ref UIStorageGrid playerInventory = ref AccessTools.FieldRefAccess<UIAssemblerWindow, UIStorageGrid>(UIRoot.instance.uiGame.assemblerWindow, "playerInventory");
                    if (__instance.playerInventory.active)
                    {
                        ref StorageComponent Storage = ref AccessTools.FieldRefAccess<UIStorageGrid, StorageComponent>(__instance.playerInventory, "storage");
                        Storage.Sort();
                    }

                 }
            }
        }

        //組立機のインベントリ強制
        [HarmonyPatch(typeof(UIAssemblerWindow), "_OnUpdate")]
        public static class UIAssemblerWindow_OnUpdate_Postfix
        {
            [HarmonyPostfix]

            public static void Postfix(UIAssemblerWindow __instance)
            {
                if (enableSortInAssembler.Value && enableForcedSort.Value)
                {
                    //ref UIStorageGrid playerInventory = ref AccessTools.FieldRefAccess<UIAssemblerWindow, UIStorageGrid>(UIRoot.instance.uiGame.assemblerWindow, "playerInventory");
                    if (__instance.playerInventory.active)
                    {
                        ref StorageComponent Storage = ref AccessTools.FieldRefAccess<UIStorageGrid, StorageComponent>(__instance.playerInventory, "storage");
                        Storage.Sort();
                    }

                }
            }
        }

    }


    public class LogManager
    {
        public static ManualLogSource Logger;
    }
}