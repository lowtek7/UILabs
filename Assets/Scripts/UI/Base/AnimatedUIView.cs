using Cysharp.Threading.Tasks;
using UI.Animation;
using UnityEngine;

namespace UI.Base
{
	/// <summary>
	/// 애니메이션이 적용된 UI 기본 클래스
	/// </summary>
	public abstract class AnimatedUIView : UIView
	{
		[SerializeField]
		protected UIAnimationSettings animationSettings = new();

		public override async UniTask Show()
		{
			gameObject.SetActive(true);
			await UIAnimationUtil.PlayShowAnimation(
				rectTransform,
				animationSettings.ShowAnimation,
				animationSettings.Duration,
				animationSettings.EaseType);
		}

		public override async UniTask Hide()
		{
			await UIAnimationUtil.PlayHideAnimation(
				rectTransform,
				animationSettings.HideAnimation,
				animationSettings.Duration,
				animationSettings.EaseType);
		}
	}
}
