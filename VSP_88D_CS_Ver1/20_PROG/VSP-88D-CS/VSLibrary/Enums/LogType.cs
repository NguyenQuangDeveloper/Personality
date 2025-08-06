
namespace VSLibrary.Enums
{
	/// <summary>
	/// 로그 유형을 정의하는 Enum
	/// </summary>
	public enum LogType
	{
		/// <summary>일반 정보 로그</summary>
		Info,

		/// <summary>경고 로그</summary>
		Warning,

		/// <summary>오류 로그</summary>
		Error,

		/// <summary>예외(Exception) 발생 로그</summary>
		Exception,

		/// <summary>디버깅 용도의 로그</summary>
		Debug,

		/// <summary>치명적인 오류 (Fatal Error)</summary>
		Fatal,

		/// <summary>UI 관련 로그</summary>
		UI,

		/// <summary>쓰레드(Thread) 관련 로그</summary>
		Thread,

		/// <summary>네트워크(Network) 관련 로그</summary>
		Network,

		/// <summary>시스템(System) 내부 이벤트 관련 로그</summary>
		System,

		/// <summary>데이터베이스(Database) 관련 로그</summary>
		Database,

		/// <summary>보안(Security) 관련 로그</summary>
		Security
	}
}

