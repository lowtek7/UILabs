using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	/// <summary>
	/// CanvasScaler 설정을 위한 헬퍼 클래스
	/// </summary>
	public static class CanvasScalerHelper
	{
		public static void SetupCanvasScaler(CanvasScaler scaler, Vector2 referenceResolution)
		{
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = referenceResolution;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

			// 16:9 해상도에 최적화된 matchWidthOrHeight 값 설정
			float matchValue = GetOptimalMatchValue(Screen.width, Screen.height);
			scaler.matchWidthOrHeight = matchValue;
		}

		private static float GetOptimalMatchValue(float width, float height)
		{
			float ratio = width / height;

			// 16:9를 기준으로 보정값 계산
			if (ratio > 16f/9f)
			{
				return 1f; // 더 넓은 화면은 높이에 맞춤
			}
			else if (ratio < 16f/9f)
			{
				return 0f; // 더 좁은 화면은 너비에 맞춤
			}

			return 0.5f; // 16:9 화면은 중간값 사용
		}
	}
}
