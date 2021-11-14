using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.Editor {
	public class ScriptTemplateFormatter : UnityEditor.AssetModificationProcessor {
		#region Static/Const Variables

		#endregion

		#region Public Variables

		#endregion

		#region Private Variables

		#endregion

		#region MonoBehavior Native

		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		public static void OnWillCreateAsset(string path) {
			path = path.Replace(".meta", "");
			int index = path.LastIndexOf(".");
			if (index < 0) { return; }
			string file = path.Substring(index);
			if (file != ".cs" && file != ".js" && file != ".boo") {
				return;
			}
			index = Application.dataPath.LastIndexOf("Assets");
			path = Application.dataPath.Substring(0, index) + path;
			file = System.IO.File.ReadAllText(path);
			file = file.Replace("#COMPANYNAME#", PlayerSettings.companyName.Replace(" ", ""));
			file = file.Replace("#PRODUCTNAME#", PlayerSettings.productName.Replace(" ", ""));
			file = file.Replace("#CREATIONDATE#", System.DateTime.Now + "");
			System.IO.File.WriteAllText(path, file);
			AssetDatabase.Refresh();
		}
		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
	}
}