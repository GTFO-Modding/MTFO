using CellMenu;
using MTFO.Utilities;
using GameData;
using System;
using UnityEngine;
using Gear;

namespace MTFO.HotReload
{
	public class HotReloadInjector
	{
		public static bool Created = false;
		public static void PostFix()
		{
			if (Created) return;
			Created = true;
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<HotReloader>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}


	class HotReloader : MonoBehaviour
	{
		private CM_PageRundown_New Rundown;
		private GameObject ReloadButton = null;
		public HotReloader(IntPtr intPtr) : base(intPtr){}

		void Awake()
        {
			Rundown = FindObjectOfType<CM_PageRundown_New>();
			ReloadButton = Instantiate(Rundown.m_discordButton.gameObject);
			ReloadButton = Instantiate(Rundown.m_discordButton.gameObject);
			ReloadButton.gameObject.SetActive(true);


			ReloadButton.gameObject.transform.position = Rundown.m_discordButton.gameObject.transform.position;
			ReloadButton.gameObject.transform.parent = Rundown.m_discordButton.gameObject.transform;
			RectTransform rect = ReloadButton.gameObject.GetComponent<RectTransform>();
			rect.transform.position += new Vector3(0, 100, 0);
			CM_Item Button = ReloadButton.GetComponent<CM_Item>();

			Button.SetText("Reload Game Data");

			Button.add_OnBtnPressCallback((Action<int>)((_) =>
			{
				ReloadData();
			}));

			RectTransform transform = ReloadButton.GetComponent<RectTransform>();
			var aPos = transform.position;
			aPos.y += 10;
			transform.position = aPos;
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

			// refresh player offline
			var gearManager = GearManager.Current;

			GearManager.m_allGearWithPostedIconJobs.Clear();

			gearManager.Setup();
			gearManager.LoadOfflineGearDatas();
			gearManager.OnGearLoadingDone();

			GearManager.GenerateAllGearIcons();

			// refresh game data
			GameDataInit.ReInitialize();
			Rundown.gameObject.SetActive(true);
			Log.Message("Reloaded!");
		}
	}
}
