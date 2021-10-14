using CellMenu;
using GameData;
using Gear;
using Globals;
using MTFO.Utilities;
using System;
using UnityEngine;
using Tier = Il2CppSystem.Collections.Generic.List<CellMenu.CM_ExpeditionIcon_New>;

namespace MTFO.HotReload
{
    public class HotReloadInjector
    {
        /// <summary>
        /// Gets called on CM_PageRundown_New.OnEnable if the BepInEx config for HotReloading is true.
        /// </summary>
        [HarmonyLib.HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(CM_PageRundown_New), nameof(CM_PageRundown_New.OnEnable))]
        public static void OnEnable() => HotReloader.Setup();
    }


    class HotReloader : MonoBehaviour
    {
        public HotReloader(IntPtr intPtr) : base(intPtr) { }

        void Awake()
        {
            Rundown = MainMenuGuiLayer.Current.PageRundownNew;
            gameObject.SetActive(true);
            gameObject.transform.localPosition = m_position;
            CM_Item Button = gameObject.GetComponent<CM_Item>();
            Button.SetText(m_text);
            Button.add_OnBtnPressCallback((Action<int>)ReloadData);
        }

        /// <summary>
        /// Clears all RenderTextures for every persistentID of the gear. Note: does not destroy the texture objects!
        /// </summary>
        private void CleanGearIcons()
        {
            GearManager.m_allGearWithPostedIconJobs.Clear();
            foreach (var instance in GearManager.Current.m_allGearIconTexturesPerInstanceKey)
                instance.Clear();
        }

        /// <summary>
        /// Destroy all selectable gear in the lobby, leaving nothing selectable
        /// </summary>
        [Obsolete]
        private void CleanGearLobbySlots()
        {
            if (CM_PageLoadout.Current != null)
            {
                var inventoryItems = CM_PageLoadout.Current.m_popupAlign.gameObject
                                   .GetComponentsInChildren<CM_InventorySlotItem>(true);
                foreach (var item in inventoryItems) Destroy(item.gameObject);
            }
        }

        /// <summary>
        /// Clears the GearIDRange data from all inventory slots
        /// </summary>
        private void CleanGearSlots()
        {
            for (int i = 0; i < m_gearSlots; ++i)
            {
                GearManager.Current.m_gearPerSlot[i].Clear();
            }
        }

        /// <summary>
        /// Destroy all expedition buttons
        /// </summary>
        private void CleanIconsOfTier()
        {
            CleanIconsOfTier(Rundown.m_expIconsTier1);
            CleanIconsOfTier(Rundown.m_expIconsTier2);
            CleanIconsOfTier(Rundown.m_expIconsTier3);
            CleanIconsOfTier(Rundown.m_expIconsTier4);
            CleanIconsOfTier(Rundown.m_expIconsTier5);
        }

        /// <summary>
        /// Destroy expedition buttons of a tier
        /// </summary>
        private void CleanIconsOfTier(Tier tier)
        {
            foreach (var icon in tier)
            {

                Destroy(icon.gameObject);
            }
        }

        /// <summary>
        /// Load and set up all the gear from the offline data
        /// </summary>
        private void LoadOfflineGearDatas()
        {
            var blocks = GameDataBlockBase<PlayerOfflineGearDataBlock>.GetAllBlocks();
            Log.Debug($"Loading {blocks.Length} gear");
            for (int index = 0; index < blocks.Length; ++index)
            {
                if (blocks[index].Type == eOfflineGearType.StandardInventory
                || blocks[index].Type == eOfflineGearType.RundownSpecificInventory)
                {
                    if (!string.IsNullOrEmpty(blocks[index].GearJSON))
                    {
                        ParseAndStashGear(
                            itemId: blocks[index].name,
                            itemInstanceId: $"OfflineGear_ID_{blocks[index].persistentID}",
                            gearJSON: blocks[index].GearJSON,
                            offlineType: blocks[index].Type
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Sets up gear referencing an item base and assigns it a inventory slot
        /// </summary>
        private void ParseAndStashGear(
            string itemId,
            string itemInstanceId,
            string gearJSON,
            eOfflineGearType offlineType)
        {
            var gearIdRange = new GearIDRange(gearJSON);
            if (gearIdRange == null) return;
            gearIdRange.PlayfabItemId = itemId;
            gearIdRange.PlayfabItemInstanceId = itemInstanceId;
            gearIdRange.OfflineGearType = offlineType;
            var itemDataBlock = gearIdRange.GetCompID(eGearComponent.BaseItem) > 0U
                              ? GameDataBlockBase<ItemDataBlock>.GetBlock(gearIdRange.GetCompID(eGearComponent.BaseItem))
                              : null;
            if (itemDataBlock != null)
                GearManager.Current.m_gearPerSlot[(int)itemDataBlock.inventorySlot].Add(gearIdRange);
        }

        /// <summary>
        /// Re-initializes the Gear and Rundown
        /// </summary>
        public void ReloadData(int id)
        {
            GameDataInit.ReInitialize(); // refresh game data
            Log.Message("Reinitialized GameData");
            ReloadGear();
            Log.Message("Reloaded Gear");
            ReloadRundown();
            Log.Message("Reloaded Rundown");
        }

        private void ReloadGear()
        {
            GearManager.Current.m_offlineSetupDone = false;
            this.CleanGearIcons();
            this.CleanGearSlots();
            GearManager.Current.SetupGearContainers(); //Re-initialize most data parameters
            this.LoadOfflineGearDatas();
            GearManager.GenerateAllGearIcons();
            GearManager.Current.m_offlineSetupDone = true;
        }

        private void ReloadRundown()
        {
            if (HasValidRundown)
            {
                Rundown.m_dataIsSetup = false;
                this.CleanIconsOfTier();
                this.TryPlaceRundown();
            }
            else
            {
                Log.Error($"Failed to place the rundown due to missing Rundown id {Global.RundownIdToLoad}");
            }
        }

        /// <summary>
        /// Create a HotReloader instance if it doesn't exist and assigns it to a singleton
        /// </summary>
        public static void Setup()
        {
            if (Current != null || MainMenuGuiLayer.Current.PageRundownNew == null) return;

            GameObject button = Instantiate(
                original: MainMenuGuiLayer.Current.PageRundownNew.m_discordButton.gameObject,
                parent: MainMenuGuiLayer.Current.PageRundownNew.m_discordButton.transform.parent,
                worldPositionStays: false);
            button.name = "Button HotReload";
            Current = button.AddComponent<HotReloader>();

            Log.Debug("Created hot reload button");
        }

        /// <summary>
        /// Replace the rundown data
        /// </summary>
        private void TryPlaceRundown()
        {
            Rundown.m_currentRundownData = this.m_currentRundownData;
            if (Rundown.m_currentRundownData != null)
            {
                Rundown.PlaceRundown(Rundown.m_currentRundownData);
                Rundown.m_dataIsSetup = true;
            }
        }

        private bool HasValidRundown => GameDataBlockBase<RundownDataBlock>.s_blockByID.ContainsKey(Global.RundownIdToLoad);
        private RundownDataBlock m_currentRundownData => GameDataBlockBase<RundownDataBlock>.GetBlock(Global.RundownIdToLoad);
        public static HotReloader Current;
        private int m_gearSlots = 3;
        private string m_text = "Reload Game Data";
        private Vector3 m_position = new(0, 77, 0);
        private CM_PageRundown_New Rundown;
    }
}
