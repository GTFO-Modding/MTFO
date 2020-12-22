using CellMenu;
using DataDumper.Utilities;
using GameData;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DataDumper.HotReload
{
	public class HotReloadInjector
	{
		public static void PostFix()
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<HotReloader>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}


	class HotReloader : MonoBehaviour
	{
		private CM_PageRundown_New Rundown;
		private GameObject ReloadButton = null;
		public HotReloader(IntPtr intPtr) : base(intPtr)
		{
			Rundown = FindObjectOfType<CM_PageRundown_New>();
		}

		public void Update()
		{
			//this is the worst code ive ever written but it kept breaking when i did it otherways, please someone fix this
			if (Rundown == null)
			{
				Rundown = FindObjectOfType<CM_PageRundown_New>();
				return;
			}
			else
			{
				if (ReloadButton == null)
				{
					ReloadButton = Instantiate(Rundown.m_discordButton.gameObject);
					ReloadButton.gameObject.SetActive(true);


					ReloadButton.gameObject.transform.position = Rundown.m_discordButton.gameObject.transform.position;
					ReloadButton.gameObject.transform.parent = Rundown.m_discordButton.gameObject.transform;
					RectTransform rect = ReloadButton.gameObject.GetComponent<RectTransform>();
					rect.transform.position += new Vector3(0, 100, 0);
					CM_Item Button = ReloadButton.GetComponent<CM_Item>();


					Button.SetText("Reload Game Data");

					Button.add_OnBtnPressCallback((Action<int>)((number) =>
					{
						ReloadData();
					}));

					RectTransform transform = ReloadButton.GetComponent<RectTransform>();
					var aPos = transform.position;
					aPos.y += 10;
					transform.position = aPos;
					return;
				}
			}
		}

		private void CleanIconsOfTier(Il2CppSystem.Collections.Generic.List<CM_ExpeditionIcon_New> tier)
		{
			foreach (var icon in tier)
			{
				Destroy(icon.gameObject);
			}
		}

		public void ReloadData()
		{
			Rundown.m_dataIsSetup = false;
			Rundown.gameObject.SetActive(false);
			CleanIconsOfTier(Rundown.m_expIconsTier1);
			CleanIconsOfTier(Rundown.m_expIconsTier2);
			CleanIconsOfTier(Rundown.m_expIconsTier3);
			CleanIconsOfTier(Rundown.m_expIconsTier4);
			CleanIconsOfTier(Rundown.m_expIconsTier5);

			GameDataInit.ReInitialize();
			Rundown.gameObject.SetActive(true);
			Log.Message("Reloaded!");
		}
	}
}
