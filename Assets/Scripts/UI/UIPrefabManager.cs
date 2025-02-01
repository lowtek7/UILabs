using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Resource;
using UI.Data;
using UnityEngine;

namespace UI
{
	/// <summary>
	/// UI 프리팹 매니저
	/// </summary>
	public class UIPrefabManager
	{
		private readonly ResourceManager resourceManager;
		private readonly UIPrefabData prefabData;
		private readonly Dictionary<string, GameObject> cachedPrefabs = new();

		public UIPrefabManager(ResourceManager resourceManager, UIPrefabData prefabData)
		{
			this.resourceManager = resourceManager;
			this.prefabData = prefabData;
			this.prefabData.Initialize();
		}

		public async UniTask PreloadCachedPrefabs()
		{
			var tasks = prefabData.GetAllPrefabInfos()
				.Where(info => info.IsCached)
				.Select(async info =>
				{
					var prefab = await info.PrefabReference.LoadAssetAsync().Task;
					cachedPrefabs[info.Key] = prefab;
				});

			await UniTask.WhenAll(tasks);
		}

		public async UniTask<GameObject> GetPrefab(string key)
		{
			var info = prefabData.GetPrefabInfo(key);
			if (info == null)
				throw new Exception($"UI Prefab not found for key: {key}");

			if (cachedPrefabs.TryGetValue(key, out var cachedPrefab))
				return cachedPrefab;

			return await info.PrefabReference.LoadAssetAsync().Task;
		}

		public void ReleasePrefab(string key)
		{
			var info = prefabData.GetPrefabInfo(key);
			if (info == null || info.IsCached) return;

			info.PrefabReference.ReleaseAsset();
		}
	}
}
