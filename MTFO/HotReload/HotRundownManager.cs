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
            m_rundown = MainMenuGuiLayer.Current.PageRundownNew;
        }

        public void Reload(int id)
        {
            if (m_hasValidRundown)
            {
                m_rundown.m_dataIsSetup = false;
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
            CleanIconsOfTier(m_rundown.m_expIconsTier1);
            CleanIconsOfTier(m_rundown.m_expIconsTier2);
            CleanIconsOfTier(m_rundown.m_expIconsTier3);
            CleanIconsOfTier(m_rundown.m_expIconsTier4);
            CleanIconsOfTier(m_rundown.m_expIconsTier5);
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
            m_rundown.m_currentRundownData = this.m_currentRundownData;
            if (m_rundown.m_currentRundownData != null)
            {
                m_rundown.PlaceRundown(m_rundown.m_currentRundownData);
                m_rundown.m_dataIsSetup = true;
            }
            else
            {
                Log.Warn("Unable to place rundown due to null data during reload");
            }
        }

        private bool m_hasValidRundown => GameDataBlockBase<RundownDataBlock>.s_blockByID.ContainsKey(Global.RundownIdToLoad);
        private RundownDataBlock m_currentRundownData => GameDataBlockBase<RundownDataBlock>.GetBlock(Global.RundownIdToLoad);
        private CM_PageRundown_New m_rundown;
    }
}
