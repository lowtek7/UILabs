using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Resource
{
	public class ResourceManager : MonoBehaviour
	{
		private readonly Dictionary<string, ResourceReference> resourceCache = new();
		private readonly Dictionary<string, UniTaskCompletionSource<ResourceReference>> loadingOperations = new();

		// 메모리 관리 설정
		private const float MemoryCheckInterval = 30f; // 메모리 체크 주기 (초)
		private const float UnusedThreshold = 180f; // 미사용 리소스 제거 임계값 (초)
		private const float MemoryThresholdMB = 512f; // 메모리 사용량 임계값 (MB)

		public async UniTask Initialize()
		{
			// Addressables 초기화
			var initHandle = Addressables.InitializeAsync();
			await initHandle;
			Addressables.Release(initHandle);

			// 메모리 모니터링 시작
			MonitorMemoryUsage().Forget();
		}

		/// <summary>
		/// 리소스 로드
		/// </summary>
		public async UniTask<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
		{
			try
			{
				var reference = await LoadResourceReferenceAsync<T>(key);
				return reference.Asset as T;
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to load asset {key}: {e.Message}");
				throw;
			}
		}

		/// <summary>
		/// 리소스 참조 로드
		/// </summary>
		private async UniTask<ResourceReference> LoadResourceReferenceAsync<T>(string key) where T : UnityEngine.Object
		{
			// 1. 캐시 체크
			if (resourceCache.TryGetValue(key, out var cachedReference))
			{
				cachedReference.AddRef();
				return cachedReference;
			}

			// 2. 동시 로딩 체크
			if (loadingOperations.TryGetValue(key, out var loadingTask))
			{
				var reference = await loadingTask.Task;
				reference.AddRef();
				return reference;
			}

			// 3. 새 로딩 작업 시작
			var tcs = new UniTaskCompletionSource<ResourceReference>();
			loadingOperations[key] = tcs;

			try
			{
				await CheckMemoryStatus();

				var operation = Addressables.LoadAssetAsync<T>(key);
				var asset = await operation;

				var reference = new ResourceReference(operation, asset);
				resourceCache[key] = reference;

				tcs.TrySetResult(reference);
				loadingOperations.Remove(key);

				return reference;
			}
			catch (Exception e)
			{
				loadingOperations.Remove(key);
				tcs.TrySetException(e);
				throw;
			}
		}

		/// <summary>
		/// 리소스 해제
		/// </summary>
		public void ReleaseAsset(string key)
		{
			if (resourceCache.TryGetValue(key, out var reference) && reference.Release())
			{
				Addressables.Release(reference.Handle);
				resourceCache.Remove(key);
			}
		}

		/// <summary>
		/// 메모리 상태 모니터링
		/// </summary>
		private async UniTaskVoid MonitorMemoryUsage()
		{
			while (true)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(MemoryCheckInterval));
				await CheckMemoryStatus();
			}
		}

		/// <summary>
		/// 메모리 상태 체크 및 관리
		/// </summary>
		private async UniTask CheckMemoryStatus()
		{
			var currentMemoryMB = GetCurrentMemoryUsage();

			// 메모리 임계값 체크
			if (currentMemoryMB > MemoryThresholdMB)
			{
				await CleanupUnusedResources();

				// 여전히 메모리가 높다면 강제 정리
				if (GetCurrentMemoryUsage() > MemoryThresholdMB)
				{
					await ForceCleanupResources();
				}
			}
		}

		/// <summary>
		/// 현재 메모리 사용량 확인
		/// </summary>
		private float GetCurrentMemoryUsage()
			=> UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);

		/// <summary>
		/// 미사용 리소스 정리
		/// </summary>
		private async UniTask CleanupUnusedResources()
		{
			var keysToRemove = resourceCache
				.Where(kvp => kvp.Value.IsUnused(UnusedThreshold))
				.Select(kvp => kvp.Key)
				.ToList();

			foreach (var key in keysToRemove)
			{
				ReleaseAsset(key);
			}

			await UniTask.Yield();
		}

		/// <summary>
		/// 강제 리소스 정리
		/// </summary>
		private async UniTask ForceCleanupResources()
		{
			// 참조 카운트가 가장 낮은 순으로 정렬
			var sortedResources = resourceCache
				.OrderBy(kvp => kvp.Value.RefCount)
				.ToList();

			// 하위 50% 리소스 제거
			int cleanupCount = sortedResources.Count / 2;
			for (int i = 0; i < cleanupCount; i++)
			{
				ReleaseAsset(sortedResources[i].Key);
			}

			await UniTask.Yield();
		}

		/// <summary>
		/// 디버그 정보 출력
		/// </summary>
		public void LogResourceStatus()
		{
			Debug.Log($"=== Resource Manager Status ===");
			Debug.Log($"Total Cached Resources: {resourceCache.Count}");
			Debug.Log($"Current Memory Usage: {GetCurrentMemoryUsage():F2}MB");

			foreach (var kvp in resourceCache)
			{
				Debug.Log($"Resource: {kvp.Key}, RefCount: {kvp.Value.RefCount}");
			}
		}

		private void OnDestroy()
		{
			// 모든 리소스 정리
			foreach (var key in resourceCache.Keys.ToList())
			{
				ReleaseAsset(key);
			}

			resourceCache.Clear();
			loadingOperations.Clear();
		}
	}
}
