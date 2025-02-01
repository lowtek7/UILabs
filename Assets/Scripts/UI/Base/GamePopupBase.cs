using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Base
{
	/// <summary>
	/// 기본 팝업 윈도우 템플릿
	/// </summary>
	public abstract class GamePopupBase : AnimatedUIView
	{
		[Header("Common Popup Elements")]
		[SerializeField]
		protected Button closeButton;
		[SerializeField]
		protected Image backgroundDim;
		[SerializeField]
		protected Text titleText;
		[SerializeField]
		protected RectTransform contentContainer;

		public override async UniTask Initialize()
		{
			await base.Initialize();

			if (closeButton != null)
				closeButton.onClick.AddListener(() => Hide().Forget());

			if (backgroundDim != null)
				backgroundDim.raycastTarget = true;  // 뒤의 UI 터치 방지
		}

		public void SetTitle(string title)
		{
			if (titleText != null)
				titleText.text = title;
		}
	}
}
