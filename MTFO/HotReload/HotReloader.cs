using CellMenu;
using GameData;
using MTFO.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MTFO.HotReload
{
    public class HotReloader : MonoBehaviour
    {
        public HotReloader(IntPtr intPtr) : base(intPtr) { }

        void Awake()
        {
            gameObject.SetActive(true);
            gameObject.transform.localPosition = buttonPosition;
            button = gameObject.GetComponent<CM_Item>();
            button.SetText(buttonLabel);
            AddOnReloadListener(new HotGameDataManager());
            AddOnReloadListener(new HotRundownManager());
            AddOnReloadListener(new HotGearManager());
        }

        /// <summary>
        /// Adds callback to a button and manager to a dictionary if it doesn't exist already
        /// </summary>
        public void AddOnReloadListener(IHotManager manager)
        {
            if (!managers.Contains(manager))
            {
                button.add_OnBtnPressCallback((Action<int>)manager.OnHotReload);
                managers.Add(manager);
            }
        }

        /// <summary>
        /// Removes callback from a button and manager from a dictionary if it does exist already
        /// </summary>
        public void RemoveOnReloadListener(IHotManager manager)
        {
            if (managers.Contains(manager))
            {
                button.remove_OnBtnPressCallback((Action<int>)manager.OnHotReload);
                managers.Remove(manager);
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

        public static HotReloader Current;
        private CM_Item button;
        private readonly string buttonLabel = "Reload Game Data";
        private readonly Vector3 buttonPosition = new(0, 77, 0);
        private readonly List<IHotManager> managers = new();
    }
}
