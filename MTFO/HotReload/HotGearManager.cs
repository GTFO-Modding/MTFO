using CellMenu;
using GameData;
using Gear;
using MTFO.Utilities;
using System;
using UnityEngine;

namespace MTFO.HotReload
{
    class HotGearManager : IHotManager
    {
        public void OnHotReload(int id)
        {
            GearManager.Current.m_offlineSetupDone = false;
            this.CleanGearIcons();
            this.CleanGearSlots();
            GearManager.Current.SetupGearContainers(); //Re-initialize most data parameters
            this.LoadOfflineGearDatas();
            GearManager.GenerateAllGearIcons();
            GearManager.Current.m_offlineSetupDone = true;
            Log.Message("Reloaded Gear");
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
                foreach (var item in inventoryItems) GameObject.Destroy(item.gameObject);
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
        /// Load and set up all the gear from the offline data
        /// </summary>
        private void LoadOfflineGearDatas()
        {
            var blocks = GameDataBlockBase<PlayerOfflineGearDataBlock>.GetAllBlocks();
            Log.Verbose($"Loading {blocks.Length} gear");
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
            if (gearIdRange == null)
            {
                Log.Warn($"Unable to stash gear {itemId} due to null GearIDRange");
                return;
            }
            gearIdRange.PlayfabItemId = itemId;
            gearIdRange.PlayfabItemInstanceId = itemInstanceId;
            gearIdRange.OfflineGearType = offlineType;
            uint compID = gearIdRange.GetCompID(eGearComponent.BaseItem);
            var itemDataBlock = compID > 0U
                              ? GameDataBlockBase<ItemDataBlock>.GetBlock(compID)
                              : null;
            if (itemDataBlock == null)
            {
                Log.Warn($"Unable to stash gear {itemId} due to null ItemDataBlock");
                return;
            }
            GearManager.Current.m_gearPerSlot[(int)itemDataBlock.inventorySlot].Add(gearIdRange);
        }

        private readonly int m_gearSlots = 3;
    }
}
