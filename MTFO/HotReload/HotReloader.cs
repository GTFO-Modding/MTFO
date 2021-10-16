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
            this.AddOnReloadListener(this.ReloadData);
            this.AddOnReloadListener(new HotRundownManager());
            this.AddOnReloadListener(new HotGearManager());
        }

        public void AddOnReloadListener(HotManagerBase manager)
        {
            this.AddOnReloadListener(manager.Reload);
            if (!this.m_Managers.Contains(manager))
                this.m_Managers.Add(manager);
        }

        public void AddOnReloadListener(Action<int> value)
        {
            this.m_button.add_OnBtnPressCallback(value);
        }

        public void RemoveOnReloadListener(HotManagerBase manager)
        {
            this.RemoveOnReloadListener(manager.Reload);
            if (this.m_Managers.Contains(manager))
                this.m_Managers.Remove(manager);
        }

        public void RemoveOnReloadListener(Action<int> value)
        {
            this.m_button.remove_OnBtnPressCallback(value);
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
        private List<HotManagerBase> m_Managers = new();
        private string m_text = "Reload Game Data";
        private Vector3 m_position = new(0, 77, 0);
    }
}
