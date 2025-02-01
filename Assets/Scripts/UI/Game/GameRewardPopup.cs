using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
	/// <summary>
	/// 사용 예시: 보상 팝업
	/// </summary>
	public class GameRewardPopup : GamePopupBase
	{
		[SerializeField]
		private Image rewardIcon;
		[SerializeField]
		private Text rewardAmountText;
		[SerializeField]
		private Transform rewardEffect;

		public async UniTask ShowReward(Sprite icon, int amount)
		{
			SetTitle("보상 획득!");

			if (rewardIcon != null)
				rewardIcon.sprite = icon;

			if (rewardAmountText != null)
				rewardAmountText.text = $"x{amount:N0}";

			await Show();

			// 보상 효과 재생
			if (rewardEffect != null)
			{
				await rewardEffect.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack)
					.AsyncWaitForCompletion();
			}
		}
	}
}
