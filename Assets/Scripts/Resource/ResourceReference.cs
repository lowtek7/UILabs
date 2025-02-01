using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Resource
{
	/// <summary>
	/// 리소스 참조 관리를 위한 클래스
	/// </summary>
	public class ResourceReference
	{
		public UnityEngine.Object Asset { get; }
		public AsyncOperationHandle Handle { get; }
		private int refCount;
		private float lastAccessTime;

		public ResourceReference(AsyncOperationHandle handle, UnityEngine.Object asset)
		{
			Handle = handle;
			Asset = asset;
			refCount = 1;
			UpdateAccessTime();
		}

		public void AddRef()
		{
			refCount++;
			UpdateAccessTime();
		}

		public bool Release()
		{
			refCount--;
			return refCount <= 0;
		}

		public bool IsUnused(float unusedThreshold)
			=> refCount <= 0 && (Time.realtimeSinceStartup - lastAccessTime) > unusedThreshold;

		private void UpdateAccessTime()
			=> lastAccessTime = Time.realtimeSinceStartup;

		public int RefCount => refCount;
	}
}
