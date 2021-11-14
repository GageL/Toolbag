using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.Editor {
	public static class EditorInputCommands {

		#region Static Variables

		#endregion

		#region Public Variables

		#endregion

		#region Private Variables

		#endregion

		#region Unity Methods

		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		[MenuItem("LucasIndustries/Editor Commands/Clear Console _F3")]
		static void ClearConsole() {
			Debug.ClearDeveloperConsole();
			var assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));
			var type = assembly.GetType("UnityEditor.LogEntries");
			var method = type.GetMethod("Clear");
			method.Invoke(new object(), null);
			Debug.Log("EditorInputCommands | ClearConsole");
		}
		#endregion

		#region Public Methods

		#endregion

		#region Local Methods

		#endregion
	}

	#region Associated Classes

	#endregion

	#region Associated Enums

	#endregion
}