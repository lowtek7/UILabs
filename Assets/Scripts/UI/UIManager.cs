using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Resource;
using Service;
using UI.Base;
using UI.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UIManager : MonoBehaviour
	{
		private readonly Dictionary<Type, IPresenter> presenters = new();
		private readonly Dictionary<Type, IView> views = new();
		private ResourceManager resourceManager;
		private Canvas mainCanvas;
		private CanvasScaler canvasScaler;
		private readonly Vector2 referenceResolution = new(1920, 1080);

		public async UniTask Initialize()
		{
			var go = new GameObject("MainCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			mainCanvas = go.GetComponent<Canvas>();
			mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

			canvasScaler = go.GetComponent<CanvasScaler>();
			CanvasScalerHelper.SetupCanvasScaler(canvasScaler, referenceResolution);

			DontDestroyOnLoad(go);
		}

		/// <summary>
		/// UI 프리팹 생성 및 초기화
		/// </summary>
		public async UniTask<T> CreateUIAsync<T>(string prefabPath) where T : UIView
		{
			try
			{
				var prefab = await resourceManager.LoadAssetAsync<GameObject>(prefabPath);
				var instance = Instantiate(prefab, mainCanvas.transform).GetComponent<T>();
				await instance.Initialize();
				return instance;
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to create UI {typeof(T).Name}: {e.Message}");
				throw;
			}
		}

		/// <summary>
		/// MVP 컴포넌트 등록
		/// </summary>
		public void RegisterMVP<TView, TPresenter, TModel>(TView view, TPresenter presenter)
			where TView : IView
			where TPresenter : IPresenter
			where TModel : IModel
		{
			views[typeof(TView)] = view;
			presenters[typeof(TPresenter)] = presenter;
		}

		/// <summary>
		/// 프레젠터 가져오기
		/// </summary>
		public TPresenter GetPresenter<TPresenter>() where TPresenter : class, IPresenter
		{
			if (presenters.TryGetValue(typeof(TPresenter), out var presenter))
			{
				return presenter as TPresenter;
			}

			return null;
		}
	}
}
