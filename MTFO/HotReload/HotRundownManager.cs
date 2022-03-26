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
            rundownPage = MainMenuGuiLayer.Current.PageRundownNew;
        }

        public void OnHotReload(int id)
        {
            if (hasValidRundown)
            {
                rundownPage.m_dataIsSetup = false;
                CleanIconsOfTier();
                TryPlaceRundown();
            }
            else
            {
                Log.Warn($"Failed to place the rundown due to missing Rundown id {Global.RundownIdToLoad}");
            }

            Log.Verbose("Reloaded Rundown");
        }

        /// <summary>
        /// Destroy all expedition buttons
        /// </summary>
        private void CleanIconsOfTier()
        {
            CleanIconsOfTier(rundownPage.m_expIconsTier1);
            CleanIconsOfTier(rundownPage.m_expIconsTier2);
            CleanIconsOfTier(rundownPage.m_expIconsTier3);
            CleanIconsOfTier(rundownPage.m_expIconsTier4);
            CleanIconsOfTier(rundownPage.m_expIconsTier5);
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
            rundownPage.m_currentRundownData = this.rundownDataCurrent;
            if (rundownPage.m_currentRundownData != null)
            {
                rundownPage.PlaceRundown(rundownPage.m_currentRundownData);
                rundownPage.m_dataIsSetup = true;
            }
            else
            {
                Log.Warn("Unable to place rundown due to null data during reload");
            }
        }

        private bool hasValidRundown => GameDataBlockBase<RundownDataBlock>.s_blockByID.ContainsKey(Global.RundownIdToLoad);
        private RundownDataBlock rundownDataCurrent => GameDataBlockBase<RundownDataBlock>.GetBlock(Global.RundownIdToLoad);
        private CM_PageRundown_New rundownPage;
    }
}
