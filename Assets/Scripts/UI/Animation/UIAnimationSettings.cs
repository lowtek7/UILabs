using DG.Tweening;
using UI.Animation.Enum;
using UnityEngine;

namespace UI.Animation
{
	/// <summary>
	/// UI 애니메이션 설정
	/// </summary>
	[System.Serializable]
	public class UIAnimationSettings
	{
		public UIAnimationType ShowAnimation = UIAnimationType.Fade;
		public UIAnimationType HideAnimation = UIAnimationType.Fade;
		public float Duration = 0.3f;
		public Ease EaseType = Ease.OutQuad;
	}
}
