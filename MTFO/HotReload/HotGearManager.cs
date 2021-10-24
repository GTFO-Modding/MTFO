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
            for (int i = 0; i < gearSlotsTotal; ++i)
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
            if (blocks == null)
            {
                Log.Warn("Unable to get Player Offline Gear blocks");
                return;
            }
            Log.Verbose($"Loading {blocks.Length} gear");
            foreach (var block in blocks) OfflineGear.Load(block);
        }

        private readonly int gearSlotsTotal = 3;
    }
}
