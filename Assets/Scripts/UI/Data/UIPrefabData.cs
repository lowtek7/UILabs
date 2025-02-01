using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace UI.Data
{
	[CreateAssetMenu(fileName = "UIPrefabData", menuName = "UILabs/UI/PrefabData")]
	public class UIPrefabData : ScriptableObject
	{
		[System.Serializable]
		public class PrefabInfo
		{
			public string Key;
			public AssetReferenceGameObject PrefabReference;
			public UIType Type;
			public bool IsCached;
		}

		public enum UIType
		{
			Window,
			Popup,
			Panel,
			HUD
		}

		[SerializeField] private List<PrefabInfo> prefabInfos = new();
		private Dictionary<string, PrefabInfo> prefabMap;

		public void Initialize()
		{
			prefabMap = prefabInfos.ToDictionary(info => info.Key, info => info);
		}

		public PrefabInfo GetPrefabInfo(string key)
		{
			if (prefabMap == null) Initialize();
			return prefabMap.GetValueOrDefault(key);
		}

		public IEnumerable<PrefabInfo> GetAllPrefabInfos() => prefabInfos;
	}
}
