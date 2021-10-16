using CellMenu;
using GameData;
using Globals;
using MTFO.Utilities;
using UnityEngine;
using Tier = Il2CppSystem.Collections.Generic.List<CellMenu.CM_ExpeditionIcon_New>;

namespace MTFO.HotReload
{
    class HotRundownManager : IHotManager
    {
        public HotRundownManager()
        {
            Rundown = MainMenuGuiLayer.Current.PageRundownNew;
        }

        public void Reload(int id)
        {
            if (HasValidRundown)
            {
                Rundown.m_dataIsSetup = false;
                CleanIconsOfTier();
                TryPlaceRundown();
            }
            else
            {
                Log.Warn($"Failed to place the rundown due to missing Rundown id {Global.RundownIdToLoad}");
            }

            Log.Message("Reloaded Rundown");
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
                GameObject.Destroy(icon.gameObject);
            }
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
            else
            {
                Log.Warn("Unable to place rundown due to null data during reload");
            }
        }

        private bool HasValidRundown => GameDataBlockBase<RundownDataBlock>.s_blockByID.ContainsKey(Global.RundownIdToLoad);
        private RundownDataBlock m_currentRundownData => GameDataBlockBase<RundownDataBlock>.GetBlock(Global.RundownIdToLoad);
        private CM_PageRundown_New Rundown;
    }
}
