using UI.Animation.Enum;

namespace UI.Animation
{
	/// <summary>
	/// 슬라이드 애니메이션 설정
	/// </summary>
	[System.Serializable]
	public class SlideAnimationSettings : UIAnimationSettings
	{
		public SlideDirection slideDirection = SlideDirection.Right;
		public float slideOffset = 1f; // 화면 크기에 대한 비율 (1 = 100%)
	}
}
