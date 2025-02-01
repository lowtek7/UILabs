using Cysharp.Threading.Tasks;

namespace UI.Interfaces
{
	/// <summary>
	/// UI 기본 인터페이스들
	/// </summary>
	public interface IView
	{
		UniTask Initialize();
		UniTask Show();
		UniTask Hide();
	}

	public interface IPresenter
	{
		UniTask Initialize();
		void Dispose();
	}

	public interface IModel
	{
		// 모델 공통 인터페이스
	}
}
