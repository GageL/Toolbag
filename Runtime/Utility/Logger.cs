using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LucasIndustries.Runtime {
	public static class Logger {
#if ENABLE_LI_LOGGER
		#region Public/Private Variables
		public enum LogType { Log, Warning, Error };

		public delegate void OnLogPrintEvent(object message, LogType logType, StackTrace stack);
		public static event OnLogPrintEvent OnLogPrint;
		#endregion

		#region Runtime Variables

		#endregion

		#region Native Methods

		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CreateCanvas() {
			LoggerCanvas _loggerCanvas = Object.Instantiate(Resources.Load<LoggerCanvas>(typeof(LoggerCanvas).Name));
			_loggerCanvas.name = typeof(LoggerCanvas).Name;
		}

		public static void Print(object message = null, LogType logType = LogType.Log) {
			StackTrace _st = new StackTrace();
			string _output = $"{_st.GetFrame(1).GetMethod().DeclaringType} | {_st.GetFrame(1).GetMethod().Name} | {(message != null ? message.ToString() : string.Empty)}";
			OnLogPrint?.Invoke(_output, logType, _st);
			switch (logType) {
				case LogType.Log:
					UnityEngine.Debug.Log(_output);
					break;
				case LogType.Warning:
					UnityEngine.Debug.LogWarning(_output);
					break;
				case LogType.Error:
					UnityEngine.Debug.LogError(_output);
					break;
			}
		}
		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
#endif
	}

	#region Enums

	#endregion
}