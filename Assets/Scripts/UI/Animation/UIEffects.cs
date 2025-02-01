using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Animation
{
	/// <summary>
	/// 자주 사용되는 UI 효과 모음
	/// </summary>
	public static class UIEffects
	{
		/// <summary>
		/// 버튼 클릭 효과
		/// </summary>
		public static void AddButtonClickEffect(this Button button, float scaleDuration = 0.1f)
		{
			button.onClick.AddListener(() =>
			{
				var sequence = DOTween.Sequence();
				sequence.Append(button.transform.DOScale(0.9f, scaleDuration))
					.Append(button.transform.DOScale(1f, scaleDuration));
			});
		}

		/// <summary>
		/// 텍스트 타이핑 효과
		/// </summary>
		public static async UniTask PlayTypingEffect(this Text text, string message, float typingSpeed = 0.05f)
		{
			text.text = "";
			foreach (char c in message)
			{
				text.text += c;
				await UniTask.Delay(TimeSpan.FromSeconds(typingSpeed));
			}
		}

		/// <summary>
		/// 이미지 깜박임 효과
		/// </summary>
		public static void PlayBlinkEffect(this Image image, float duration = 0.5f, float minAlpha = 0.3f)
		{
			image.DOFade(minAlpha, duration).SetLoops(-1, LoopType.Yoyo);
		}

		/// <summary>
		/// 숫자 변경 애니메이션
		/// </summary>
		public static async UniTask PlayNumberChangeAnimation(this Text text, int fromValue, int toValue, float duration = 0.5f)
		{
			float elapsedTime = 0;
			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float progress = elapsedTime / duration;
				int currentValue = (int)Mathf.Lerp(fromValue, toValue, progress);
				text.text = currentValue.ToString();
				await UniTask.Yield();
			}
			text.text = toValue.ToString();
		}
	}
}
