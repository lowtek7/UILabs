using Cysharp.Threading.Tasks;
using UI.Interfaces;
using UnityEngine;

namespace UI.Base
{
	public abstract class UIView : MonoBehaviour, IView
	{
		protected CanvasGroup canvasGroup;
		protected RectTransform rectTransform;

		public virtual async UniTask Initialize()
		{
			canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
			rectTransform = GetComponent<RectTransform>();
			await UniTask.CompletedTask;
		}

		public virtual async UniTask Show()
		{
			gameObject.SetActive(true);
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			await UniTask.CompletedTask;
		}

		public virtual async UniTask Hide()
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			gameObject.SetActive(false);
			await UniTask.CompletedTask;
		}
	}
}
