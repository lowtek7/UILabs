using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI.Animation;
using UI.Animation.Enum;
using UnityEngine;

namespace UI.Base
{
	/// <summary>
	/// 슬라이드 애니메이션이 적용된 UI 뷰
	/// </summary>
	public class SlideAnimatedUIView : UIView
	{
		[SerializeField] protected SlideAnimationSettings slideSettings = new();
		private RectTransform rectTransform;
		private Vector2 originalPosition;
		private Canvas rootCanvas;

		public override async UniTask Initialize()
		{
			await base.Initialize();
			rectTransform = transform as RectTransform;
			rootCanvas = GetComponentInParent<Canvas>();
			originalPosition = rectTransform.anchoredPosition;
		}

		protected Vector2 GetSlidePosition()
		{
			// 현재 캔버스의 크기를 기준으로 상대적인 위치 계산
			Vector2 canvasSize = rootCanvas.GetComponent<RectTransform>().rect.size;

			return slideSettings.slideDirection switch
			{
				SlideDirection.Right => originalPosition + new Vector2(canvasSize.x * slideSettings.slideOffset, 0),
				SlideDirection.Left => originalPosition + new Vector2(-canvasSize.x * slideSettings.slideOffset, 0),
				SlideDirection.Top => originalPosition + new Vector2(0, canvasSize.y * slideSettings.slideOffset),
				SlideDirection.Bottom => originalPosition + new Vector2(0, -canvasSize.y * slideSettings.slideOffset),
				_ => originalPosition
			};
		}

		public override async UniTask Show()
		{
			gameObject.SetActive(true);

			// 시작 위치 설정
			Vector2 startPosition = GetSlidePosition();
			rectTransform.anchoredPosition = startPosition;

			// 원래 위치로 슬라이드
			await rectTransform.DOAnchorPos(originalPosition, slideSettings.Duration)
				.SetEase(slideSettings.EaseType)
				.AsyncWaitForCompletion();
		}

		public override async UniTask Hide()
		{
			// 슬라이드 아웃
			Vector2 endPosition = GetSlidePosition();
			await rectTransform.DOAnchorPos(endPosition, slideSettings.Duration)
				.SetEase(slideSettings.EaseType)
				.AsyncWaitForCompletion();

			gameObject.SetActive(false);
		}
	}
}
