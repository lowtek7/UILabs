using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
	/// <summary>
	/// 사용 예시: 확인 팝업
	/// </summary>
	public class GameConfirmPopup : GamePopupBase
	{
		[SerializeField]
		private Text messageText;
		[SerializeField]
		private Button confirmButton;
		[SerializeField]
		private Button cancelButton;

		private TaskCompletionSource<bool> resultTask;

		public override async UniTask Initialize()
		{
			await base.Initialize();

			if (confirmButton != null)
				confirmButton.onClick.AddListener(() => OnConfirm());

			if (cancelButton != null)
				cancelButton.onClick.AddListener(() => OnCancel());
		}

		public async UniTask<bool> ShowConfirm(string title, string message)
		{
			SetTitle(title);

			if (messageText != null)
				messageText.text = message;

			resultTask = new TaskCompletionSource<bool>();

			await Show();
			return await resultTask.Task;
		}

		private void OnConfirm()
		{
			resultTask?.TrySetResult(true);
			Hide().Forget();
		}

		private void OnCancel()
		{
			resultTask?.TrySetResult(false);
			Hide().Forget();
		}
	}
}
