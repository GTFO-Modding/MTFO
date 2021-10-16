using CellMenu;
using GameData;
using MTFO.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MTFO.HotReload
{
    class HotReloader : MonoBehaviour
    {
        public HotReloader(IntPtr intPtr) : base(intPtr) { }

        void Awake()
        {
            gameObject.SetActive(true);
            gameObject.transform.localPosition = m_position;
            m_button = gameObject.GetComponent<CM_Item>();
            m_button.SetText(m_text);
            this.m_button.add_OnBtnPressCallback((Action<int>)this.ReloadData);
            this.AddOnReloadListener(new HotRundownManager());
            this.AddOnReloadListener(new HotGearManager());
        }

        /// <summary>
        /// Adds callback to a button and manager to a dictionary if it doesn't exist already
        /// </summary>
        /// <param name="manager"></param>
        public void AddOnReloadListener(HotManagerBase manager)
        {
            if (!this.m_Managers.Contains(manager))
            {
                this.m_button.add_OnBtnPressCallback((Action<int>)manager.Reload);
                this.m_Managers.Add(manager);
            }
        }

        /// <summary>
        /// Removes callback from a button and manager from a dictionary if it doesn't exist already
        /// </summary>
        /// <param name="manager"></param>
        public void RemoveOnReloadListener(HotManagerBase manager)
        {
            if (this.m_Managers.Contains(manager))
            {
                this.m_button.remove_OnBtnPressCallback((Action<int>)manager.Reload);
                this.m_Managers.Remove(manager);
            }
                
        }

        /// <summary>
        /// Re-initializes game data
        /// </summary>
        public void ReloadData(int id)
        {
            GameDataInit.ReInitialize(); // refresh game data
            Log.Message("Reinitialized GameData");
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

        public static HotReloader Current;
        private CM_Item m_button;
        private readonly List<HotManagerBase> m_Managers = new();
        private readonly string m_text = "Reload Game Data";
        private readonly Vector3 m_position = new(0, 77, 0);
    }
}
