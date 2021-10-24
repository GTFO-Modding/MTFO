using CellMenu;
using MTFO.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MTFO.HotReload
{
    public class HotReloaderBehaviour : MonoBehaviour
    {
        public HotReloaderBehaviour(IntPtr value) : base(value) { }

        /// <summary>
        /// Instantiates a CM_Item button and adds the returned component to it
        /// </summary>
        public static HotReloaderBehaviour Instantiate(CM_PageRundown_New pageRundownNew)
        {
            if (pageRundownNew == null) return null;
            GameObject button = Instantiate(
                original: pageRundownNew.m_discordButton.gameObject,
                parent: pageRundownNew.m_discordButton.transform.parent,
                worldPositionStays: false);
            return button.AddComponent<HotReloaderBehaviour>();
        }


        /// <summary>
        /// Adds callback to a button and manager to a dictionary if it doesn't exist already
        /// </summary>
        public void AddManager(IHotManager manager)
        {
            if (!Managers.Contains(manager))
            {
                Button.add_OnBtnPressCallback((Action<int>)manager.OnHotReload);
                Managers.Add(manager);
            }
        }

        /// <summary>
        /// Removes callback from a button and manager from a dictionary if it does exist already
        /// </summary>
        public void RemoveManager(IHotManager manager)
        {
            if (Managers.Contains(manager))
            {
                Button.remove_OnBtnPressCallback((Action<int>)manager.OnHotReload);
                Managers.Remove(manager);
            }
        }

        /// <summary>
        /// Unity Monobehaviour method that is called when the component is created
        /// </summary>
        void Awake()
        {
            gameObject.name = objName;
            gameObject.SetActive(true);
            gameObject.transform.localPosition = buttonPosition;
            Button = gameObject.GetComponent<CM_Item>();
            Button.SetText(buttonLabel);
            Log.Verbose("Created hot reload button");
        }

        public CM_Item Button;
        public readonly List<IHotManager> Managers = new();
        private readonly string buttonLabel = "Reload Game Data";
        private readonly Vector3 buttonPosition = new(0, 77, 0);
        private readonly string objName = "Button HotReload";
    }
}
