using VSLibrary.Enums;

namespace VSLibrary.Common.MVVM.Interfaces
{
	/// <summary>
	/// 로그 기록을 위한 기본 인터페이스
	/// </summary>
	public interface IVSLogger
	{
		void LogInfo(string message);
		void LogError(string message, Exception ex = null);
		void LogDebug(string message);
		void LogWarning(string message);
		/// <summary>
		/// 로그를 기록합니다.
		/// </summary>
		/// <param name="message">로그 메시지</param>
		/// <param name="logType">로그 유형</param>
		void WriteLine(string message, LogType logType = LogType.Info, Exception ex = null);

		/// <summary>
		/// 예외(Exception) 발생 로그
		/// </summary>
		/// <param name="ex">Exception</param>
		void WriteLine( Exception ex = null);
		void Dispose();

    }
}
