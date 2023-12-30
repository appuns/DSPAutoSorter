using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]



namespace DSPAutoSorter
{
    [BepInPlugin("Appun.DSP.plugin.AutoSorter", "DSPAutoSorter", "1.2.9")]
    [HarmonyPatch]
    public class DSPAutoSorter : BaseUnityPlugin
    {
        static public DSPAutoSorter self;

        private static ConfigEntry<bool> enableForcedSort;
        private static ConfigEntry<bool> enableSortInInventry;
        private static ConfigEntry<bool> enableSortInStorage;
        private static ConfigEntry<bool> enableSortInFuelChamber;
        private static ConfigEntry<bool> enableSortInMiner;
        private static ConfigEntry<bool> enableSortInAssembler;
        private static ConfigEntry<bool> enableSortInBattleBase;

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
            enableSortInBattleBase = Config.Bind("General", "enableSortInBattleBase", true, "enable sort in inventry of Battlefield Analysis Base.");
            enableForcedSort = Config.Bind("ForcedSort", "enableForcedSort", true, "enable forced sort.");
        }


        //変化があったときは強制ソート 
        [HarmonyPrefix,HarmonyPatch(typeof(UIStorageGrid), "OnStorageContentChanged")]
        public static void UIStorageGrid_OnStorageContentChanged_Prefix(UIStorageGrid __instance)
        {

            if (__instance.storage == null || !enableForcedSort.Value)
            {
                return;
            }
            var parentObj = __instance.transform.parent.gameObject;
            //LogManager.Logger.LogInfo("------------------------------------------------------------------parentObj.name : " + parentObj.name);

            if (enableSortInInventry.Value && parentObj.name == "Player Inventory")
            {
                __instance.OnSort();
            }
            else if (enableSortInFuelChamber.Value && parentObj.name == "fuel-group")
            {
                __instance.OnSort();
            }
            else if (enableSortInStorage.Value && parentObj.name == "Storage Window")
            {
                __instance.OnSort();
            }
            else if (enableSortInMiner.Value && parentObj.name == "Miner Window")
            {
                __instance.OnSort();
            }
            else if (enableSortInAssembler.Value && parentObj.name == "Assembler Window")
            {
                __instance.OnSort();
            }
            else if (enableSortInBattleBase.Value && parentObj.name == "storage-group")
            {
                __instance.OnSort();
            }
        }

        //インベントリを開いたらソート
        [HarmonyPostfix, HarmonyPatch(typeof(UIInventoryWindow), "_OnOpen")]

        public static void UIGame_OpenPlayerInventory_Postfix(UIInventoryWindow __instance)
        {
            if (enableForcedSort.Value)
            {
                __instance.inventory.OnSort();
            }
        }


        //メカ燃焼室を開いたらソート
        [HarmonyPostfix, HarmonyPatch(typeof(UIMechaWindow), "_OnOpen")]
        public static void UIMechaWindow_OnOpen_Postfix(UIMechaWindow __instance)
        {
            if (enableSortInFuelChamber.Value)
            {
                GameMain.mainPlayer.mecha.reactorStorage.Sort(true);
            }

        }


        //ストレージを開いたらソート
        [HarmonyPostfix, HarmonyPatch(typeof(UIStorageWindow), "_OnOpen")]
        public static void UIStorageWindow_OnOpen_Postfix(UIStorageWindow __instance)
        {
            if (enableSortInStorage.Value)
            {
                __instance.OnSortClick();
            }
        }


        //採掘機のインベントリを開いたらソート
        [HarmonyPostfix, HarmonyPatch(typeof(UIMinerWindow), "_OnOpen")]
        public static void UIMinerWindow_OnOpen_Postfix(UIMinerWindow __instance)
        {
            if (enableSortInMiner.Value)
            {
                if (__instance.playerInventory.active)
                {
                    ref StorageComponent Storage = ref AccessTools.FieldRefAccess<UIStorageGrid, StorageComponent>(__instance.playerInventory, "storage");
                    Storage.Sort();
                }
            }
        }


        //組立機のインベントリを開いたらソート
        [HarmonyPostfix, HarmonyPatch(typeof(UIAssemblerWindow), "_OnOpen")]
        public static void UIAssemblerWindow_OnOpen_Postfix(UIAssemblerWindow __instance)
        {
            if (enableSortInAssembler.Value)
            {
                if (__instance.playerInventory.active)
                {
                    ref StorageComponent Storage = ref AccessTools.FieldRefAccess<UIStorageGrid, StorageComponent>(__instance.playerInventory, "storage");
                    Storage.Sort();
                }
            }
        }

        //バトルベースのインベントリを開いたらソート
        [HarmonyPostfix, HarmonyPatch(typeof(UIBattleBaseWindow), "_OnOpen")]
        public static void UIBattleBaseWindow_OnOpen_Postfix(UIBattleBaseWindow __instance)
        {
            if (enableSortInBattleBase.Value)
            {
                __instance.storage.Sort();
            }
        }
    }


    public class LogManager
    {
        public static ManualLogSource Logger;
    }
}