using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI.Animation.Enum;
using UnityEngine;

namespace UI.Animation
{
	/// <summary>
	/// UI 애니메이션 유틸리티
	/// </summary>
	public static class UIAnimationUtil
	{
		private static readonly Vector2 RightOutPosition = new(1920f, 0);
		private static readonly Vector2 LeftOutPosition = new(-1920f, 0);
		private static readonly Vector2 TopOutPosition = new(0, 1080f);
		private static readonly Vector2 BottomOutPosition = new(0, -1080f);

		public static Vector2 GetSafeAreaAdjustedPosition(RectTransform target, Vector2 position)
		{
			// SafeArea 고려
			Rect safeArea = Screen.safeArea;
			Vector2 screenSize = new(Screen.width, Screen.height);

			// SafeArea 기준으로 위치 조정
			float xRatio = safeArea.width / screenSize.x;
			float yRatio = safeArea.height / screenSize.y;

			return new Vector2(
				position.x * xRatio,
				position.y * yRatio
			);
		}

		/// <summary>
		/// UI 요소 표시 애니메이션
		/// </summary>
		public static async UniTask PlayShowAnimation(RectTransform target, UIAnimationType type, float duration = 0.3f,
			Ease ease = Ease.OutQuad)
		{
			if (target == null) return;

			// 시작 전 초기화
			target.gameObject.SetActive(true);
			var canvasGroup = target.GetComponent<CanvasGroup>();
			if (canvasGroup == null) canvasGroup = target.gameObject.AddComponent<CanvasGroup>();

			var sequence = DOTween.Sequence();

			switch (type)
			{
				case UIAnimationType.Fade:
					sequence.Append(PlayFadeIn(canvasGroup, duration, ease));
					break;

				case UIAnimationType.Scale:
					sequence.Append(PlayScaleIn(target, duration, ease));
					break;

				case UIAnimationType.ScaleWithFade:
					sequence.Append(PlayScaleIn(target, duration, ease))
						.Join(PlayFadeIn(canvasGroup, duration, ease));
					break;

				case UIAnimationType.SlideFromRight:
					sequence.Append(PlaySlideIn(target, RightOutPosition, duration, ease));
					break;

				case UIAnimationType.SlideFromLeft:
					sequence.Append(PlaySlideIn(target, LeftOutPosition, duration, ease));
					break;

				case UIAnimationType.SlideFromTop:
					sequence.Append(PlaySlideIn(target, TopOutPosition, duration, ease));
					break;

				case UIAnimationType.SlideFromBottom:
					sequence.Append(PlaySlideIn(target, BottomOutPosition, duration, ease));
					break;

				case UIAnimationType.PopUp:
					sequence.Append(PlayPopUpIn(target, duration, ease));
					break;

				case UIAnimationType.Bounce:
					sequence.Append(PlayBounceIn(target, duration, ease));
					break;
			}

			await sequence.Play().AsyncWaitForCompletion();
		}

		/// <summary>
		/// UI 요소 숨기기 애니메이션
		/// </summary>
		public static async UniTask PlayHideAnimation(RectTransform target, UIAnimationType type, float duration = 0.3f,
			Ease ease = Ease.InQuad)
		{
			if (target == null) return;

			var canvasGroup = target.GetComponent<CanvasGroup>();
			if (canvasGroup == null) canvasGroup = target.gameObject.AddComponent<CanvasGroup>();

			var sequence = DOTween.Sequence();

			switch (type)
			{
				case UIAnimationType.Fade:
					sequence.Append(PlayFadeOut(canvasGroup, duration, ease));
					break;

				case UIAnimationType.Scale:
					sequence.Append(PlayScaleOut(target, duration, ease));
					break;

				case UIAnimationType.ScaleWithFade:
					sequence.Append(PlayScaleOut(target, duration, ease))
						.Join(PlayFadeOut(canvasGroup, duration, ease));
					break;

				case UIAnimationType.SlideFromRight:
					sequence.Append(PlaySlideOut(target, RightOutPosition, duration, ease));
					break;

				case UIAnimationType.SlideFromLeft:
					sequence.Append(PlaySlideOut(target, LeftOutPosition, duration, ease));
					break;

				case UIAnimationType.SlideFromTop:
					sequence.Append(PlaySlideOut(target, TopOutPosition, duration, ease));
					break;

				case UIAnimationType.SlideFromBottom:
					sequence.Append(PlaySlideOut(target, BottomOutPosition, duration, ease));
					break;

				case UIAnimationType.PopUp:
					sequence.Append(PlayPopUpOut(target, duration, ease));
					break;

				case UIAnimationType.Bounce:
					sequence.Append(PlayBounceOut(target, duration, ease));
					break;
			}

			await sequence.Play().AsyncWaitForCompletion();
			target.gameObject.SetActive(false);
		}

		// 개별 애니메이션 구현
		private static Tween PlayFadeIn(CanvasGroup target, float duration, Ease ease) =>
			target.DOFade(1f, duration).SetEase(ease).From(0f);

		private static Tween PlayFadeOut(CanvasGroup target, float duration, Ease ease) =>
			target.DOFade(0f, duration).SetEase(ease);

		private static Tween PlayScaleIn(RectTransform target, float duration, Ease ease) =>
			target.DOScale(Vector3.one, duration).SetEase(ease).From(Vector3.zero);

		private static Tween PlayScaleOut(RectTransform target, float duration, Ease ease) =>
			target.DOScale(Vector3.zero, duration).SetEase(ease);

		private static Tween PlaySlideIn(RectTransform target, Vector2 fromPosition, float duration, Ease ease)
		{
			var originalPosition = target.anchoredPosition;
			return target.DOAnchorPos(originalPosition, duration).SetEase(ease).From(fromPosition);
		}

		private static Tween PlaySlideOut(RectTransform target, Vector2 toPosition, float duration, Ease ease) =>
			target.DOAnchorPos(toPosition, duration).SetEase(ease);

		private static Sequence PlayPopUpIn(RectTransform target, float duration, Ease ease)
		{
			var sequence = DOTween.Sequence();
			sequence.Append(target.DOScale(1.1f, duration * 0.6f).SetEase(Ease.OutQuad).From(0f))
				.Append(target.DOScale(1f, duration * 0.4f).SetEase(Ease.InQuad));
			return sequence;
		}

		private static Sequence PlayPopUpOut(RectTransform target, float duration, Ease ease)
		{
			var sequence = DOTween.Sequence();
			sequence.Append(target.DOScale(1.1f, duration * 0.4f).SetEase(Ease.OutQuad))
				.Append(target.DOScale(0f, duration * 0.6f).SetEase(Ease.InQuad));
			return sequence;
		}

		private static Sequence PlayBounceIn(RectTransform target, float duration, Ease ease)
		{
			var sequence = DOTween.Sequence();
			sequence.Append(target.DOScale(1.2f, duration * 0.4f).SetEase(Ease.OutQuad).From(0f))
				.Append(target.DOScale(0.8f, duration * 0.3f).SetEase(Ease.InQuad))
				.Append(target.DOScale(1f, duration * 0.3f).SetEase(Ease.OutQuad));
			return sequence;
		}

		private static Sequence PlayBounceOut(RectTransform target, float duration, Ease ease)
		{
			var sequence = DOTween.Sequence();
			sequence.Append(target.DOScale(1.2f, duration * 0.3f).SetEase(Ease.OutQuad))
				.Append(target.DOScale(0f, duration * 0.7f).SetEase(Ease.InQuad));
			return sequence;
		}
	}
}
