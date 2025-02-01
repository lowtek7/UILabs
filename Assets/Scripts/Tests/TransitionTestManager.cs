using UI;

namespace Tests
{
	using UnityEngine;
	using UnityEngine.UI;
	using Cysharp.Threading.Tasks;
	using System;
	using System.Threading;

	public class TransitionTestManager : MonoBehaviour
	{
		[Header("Transition Settings")] [SerializeField]
		private UITransitionEffect transitionEffect;

		[Header("Test UI Panels")] [SerializeField]
		private CanvasGroup panel1;

		[SerializeField]
		private CanvasGroup panel2;

		[Header("Test Controls")] [SerializeField]
		private Button transitionButton;

		[SerializeField]
		private Button resetButton;

		private bool isSecondPanelActive = false;
		private CancellationTokenSource _cts = new CancellationTokenSource();

		void Awake()
		{
			// 초기 상태 설정
			if (panel1 != null) panel1.gameObject.SetActive(true);
			if (panel2 != null) panel2.gameObject.SetActive(false);

			// 버튼 이벤트 설정
			if (transitionButton != null)
			{
				transitionButton.onClick.AddListener(() => TriggerTransition().Forget());
			}

			if (resetButton != null)
			{
				resetButton.onClick.AddListener(ResetUI);
			}
		}

		private void OnDestroy()
		{
			_cts.Cancel();
			_cts.Dispose();
		}

		private async UniTaskVoid TriggerTransition()
		{
			try
			{
				// 버튼 비활성화
				transitionButton.interactable = false;
				resetButton.interactable = false;

				if (isSecondPanelActive)
				{
					await transitionEffect.TransitionToNewUIAsync(panel2, panel1);
				}
				else
				{
					await transitionEffect.TransitionToNewUIAsync(panel1, panel2);
				}

				// 상태 업데이트
				isSecondPanelActive = !isSecondPanelActive;

				// 버튼 다시 활성화
				transitionButton.interactable = true;
				resetButton.interactable = true;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Transition failed: {ex.Message}");
				ResetUI();
			}
		}

		private void ResetUI()
		{
			// UI 초기 상태로 리셋
			panel1.gameObject.SetActive(true);
			panel2.gameObject.SetActive(false);
			isSecondPanelActive = false;

			// 버튼 상태 복구
			transitionButton.interactable = true;
			resetButton.interactable = true;
		}
	}
}
