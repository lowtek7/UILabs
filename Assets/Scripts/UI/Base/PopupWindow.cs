using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Base
{
	/// <summary>
	/// 팝업 윈도우 시스템
	/// </summary>
	public abstract class PopupWindow : UIView
	{
		[SerializeField]
		protected Button closeButton;

		public override async UniTask Initialize()
		{
			await base.Initialize();

			if (closeButton != null)
			{
				closeButton.onClick.AddListener(() => Hide());
			}
		}

		public override async UniTask Show()
		{
			await base.Show();
			PlayShowAnimation();
		}

		public override async UniTask Hide()
		{
			PlayHideAnimation();
			await base.Hide();
		}

		protected virtual void PlayShowAnimation()
		{
			transform.localScale = Vector3.zero;
			transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
		}

		protected virtual void PlayHideAnimation()
		{
			transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
		}
	}
}
