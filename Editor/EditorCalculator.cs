using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.Editor {
	public class EditorCalculator : EditorWindow {
		#region Public/Private Variables
		[Serializable]
		private enum OperationMode { None, Divide, Multiply, Add, Subtract };
		[Serializable]
		public class CachedOperation {

		}
		#endregion

		#region Runtime Variables
		[SerializeField] private Vector2 appendingValueScroll;
		[SerializeField] private string operationLine;
		[SerializeField] private string appendingValue;
		[SerializeField] private string previousAppendedValue;
		[SerializeField] private OperationMode currentOperation;
		[SerializeField] private List<string> previousOperations;
		[SerializeField] private bool didCalculate;
		#endregion

		#region Native Methods
		private void OnEnable() {
			operationLine = string.Empty;
			appendingValue = "0";
			previousAppendedValue = string.Empty;
			currentOperation = OperationMode.None;
			previousOperations = new List<string>();
			didCalculate = false;
		}

		private void OnGUI() {
			Event _event = Event.current;
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
				{
					if (GUILayout.Button(EditorGUIUtility.IconContent($"{(EditorGUIUtility.isProSkin ? "d_" : "")}_Menu"), EditorStyles.toolbarButton)) {
						var menu = new GenericMenu();
						menu.AddItem(new GUIContent("Help/Debug/operationLine"), false, delegate { Debug.Log(operationLine); });
						menu.AddItem(new GUIContent("Help/Debug/appendingValue"), false, delegate { Debug.Log(appendingValue); });
						menu.AddItem(new GUIContent("Help/Debug/previousAppendedValue"), false, delegate { Debug.Log(previousAppendedValue); });
						menu.AddItem(new GUIContent("Help/Debug/currentOperation"), false, delegate { Debug.Log(currentOperation); });
						menu.AddItem(new GUIContent("Help/Debug/didCalculate"), false, delegate { Debug.Log(didCalculate); });
						menu.AddItem(new GUIContent("About"), false, delegate { EditorUtility.DisplayDialog("About", "This is a simple calculator that was created to do in-editor calculations quickly without the need for an external calculator.", "Close"); });
						menu.AddItem(new GUIContent(""), false, null);
						menu.AddItem(new GUIContent("Reload"), false, delegate { Close(); ShowWindow(); });
						menu.AddItem(new GUIContent("Exit"), false, delegate { Close(); });
						menu.ShowAsContext();
					}
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(EditorGUIUtility.IconContent($"{(EditorGUIUtility.isProSkin ? "d_" : "")}UnityEditor.ProfilerWindow"), EditorStyles.toolbarButton)) {
						if (EditorUtility.DisplayDialog("Operations", "Would you like to post all cached operations to the log?", "Yes", "Cancel")) {
							Debug.Log("Pushing all cached operations this session:");
							foreach (string operation in previousOperations) {
								Debug.Log($"\t{operation}");
							}
						}
					}
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(102));
				{
					EditorGUILayout.LabelField(operationLine, TextGUIStyle(EditorStyles.label, FontStyle.Normal, 12, TextAnchor.MiddleRight, true, new Color(1, 1, 1, .3f)), GUILayout.ExpandWidth(true), GUILayout.Height(26));
					EditorGUILayout.SelectableLabel(appendingValue, TextGUIStyle(EditorStyles.label, FontStyle.Bold, 28, TextAnchor.LowerRight, true, Color.white), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(42));
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("C", GUILayout.Width(56), GUILayout.Height(42))) {
						Clear();
					}
					if (GUILayout.Button("Del", GUILayout.Width(56), GUILayout.Height(42))) {
						Delete();
					}
					if (GUILayout.Button("", GUILayout.Width(56), GUILayout.Height(42))) {
						GUI.FocusControl("");
					}
					if (GUILayout.Button("/", GUILayout.Width(56), GUILayout.Height(42))) {
						DoOperation(OperationMode.Divide);
					}
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(42));
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("7", GUILayout.Width(56), GUILayout.Height(42))) {
						AppendValue(7, false);
					}
					if (GUILayout.Button("8", GUILayout.Width(56), GUILayout.Height(42))) {
						AppendValue(8, false);
					}
					if (GUILayout.Button("9", GUILayout.Width(56), GUILayout.Height(42))) {
						AppendValue(9, false);
					}
					if (GUILayout.Button("*", GUILayout.Width(56), GUILayout.Height(42))) {
						DoOperation(OperationMode.Multiply);
					}
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(42));
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("4", GUILayout.Width(56), GUILayout.Height(42))) {
						AppendValue(4, false);
					}
					if (GUILayout.Button("5", GUILayout.Width(56), GUILayout.Height(42))) {
						AppendValue(5, false);
					}
					if (GUILayout.Button("6", GUILayout.Width(56), GUILayout.Height(42))) {
						AppendValue(6, false);
					}
					if (GUILayout.Button("-", GUILayout.Width(56), GUILayout.Height(42))) {
						DoOperation(OperationMode.Subtract);
					}
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(42));
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("1", GUILayout.Width(56), GUILayout.Height(42))) {
						AppendValue(1, false);
					}
					if (GUILayout.Button("2", GUILayout.Width(56), GUILayout.Height(42))) {
						AppendValue(2, false);
					}
					if (GUILayout.Button("3", GUILayout.Width(56), GUILayout.Height(42))) {
						AppendValue(3, false);
					}
					if (GUILayout.Button("+", GUILayout.Width(56), GUILayout.Height(42))) {
						DoOperation(OperationMode.Add);
					}
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(42));
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("+/-", GUILayout.Width(56), GUILayout.Height(42))) {
						InverseValue();
					}
					if (GUILayout.Button("0", GUILayout.Width(56), GUILayout.Height(42))) {
						AppendValue(0, true);
					}
					if (GUILayout.Button(".", GUILayout.Width(56), GUILayout.Height(42))) {
						AddDecimalPoint();
					}
					if (GUILayout.Button("=", GUILayout.Width(56), GUILayout.Height(42))) {
						Calculate();
					}
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndVertical();
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		private static GUIStyle TextGUIStyle(GUIStyle rootStyle, FontStyle fontStyle, int fontSize, TextAnchor alignment, bool wordWrap, Color color) {
			GUIStyle _style = new GUIStyle(rootStyle);
			_style.fontStyle = FontStyle.Bold;
			_style.fontSize = fontSize;
			_style.alignment = alignment;
			_style.wordWrap = wordWrap;
			_style.richText = true;
			_style.normal.textColor = color;
			return _style;
		}

		[MenuItem("Lucas Industries/Calculator")]
		private static void ShowWindow() {
			EditorCalculator _window = (EditorCalculator)GetWindow(typeof(EditorCalculator), true, "Calculator");
			_window.Show();
			_window.minSize = new Vector2(240, 350);
			_window.maxSize = new Vector2(240, 350);
		}
		#endregion

		#region Public Methods

		#endregion

		#region Private Methods
		private void Clear() {
			operationLine = string.Empty;
			appendingValue = "0";
			previousAppendedValue = string.Empty;
			currentOperation = OperationMode.None;
			didCalculate = false;
			GUI.FocusControl("");
		}

		private void Delete() {
			if (didCalculate) { return; }
			if (appendingValue.Length == 1) {
				appendingValue = "0";
			} else {
				appendingValue = appendingValue.Remove(appendingValue.Length - 1, 1);
			}
			GUI.FocusControl("");
		}

		private void AppendValue(int input, bool isZero) {
			if (didCalculate) {
				operationLine = string.Empty;
				didCalculate = false;
				appendingValue = "0";
			}
			if (isZero) {
				if (appendingValue[0] == '0') {
					return;
				}
			}
			if (appendingValue[0] == '0' && (appendingValue.Length == 1 || appendingValue[1] != '.')) {
				appendingValue = string.Empty;
			}
			appendingValue += input;
			GUI.FocusControl("");
		}

		private void DoOperation(OperationMode operationMode) {
			RemoveEndingDecimal();
			if (currentOperation != operationMode) {
				Calculate();
			}
			currentOperation = operationMode;
			switch (currentOperation) {
				case OperationMode.Add:
					operationLine = appendingValue + " + ";
					break;
				case OperationMode.Subtract:
					operationLine = appendingValue + " - ";
					break;
				case OperationMode.Divide:
					operationLine = appendingValue + " / ";
					break;
				case OperationMode.Multiply:
					operationLine = appendingValue + " * ";
					break;
			}
			if (!string.IsNullOrEmpty(previousAppendedValue)) {
				Calculate();
			}
			previousAppendedValue = appendingValue;
			if (didCalculate) {
				switch (currentOperation) {
					case OperationMode.Add:
						operationLine = previousAppendedValue + " + ";
						break;
					case OperationMode.Subtract:
						operationLine = previousAppendedValue + " - ";
						break;
					case OperationMode.Divide:
						operationLine = previousAppendedValue + " / ";
						break;
					case OperationMode.Multiply:
						operationLine = previousAppendedValue + " * ";
						break;
				}
			}
			appendingValue = "0";
			didCalculate = false;
			GUI.FocusControl("");
		}

		private void InverseValue() {
			if (appendingValue[0] == '-') {
				appendingValue = appendingValue.Remove(0, 1);
			} else {
				appendingValue = '-' + appendingValue;
			}
			GUI.FocusControl("");
		}

		private void AddDecimalPoint() {
			if (appendingValue.Contains(".")) { return; }
			appendingValue += ".";
			GUI.FocusControl("");
		}

		private void RemoveEndingDecimal() {
			if (appendingValue.Length >= 2 && appendingValue[appendingValue.Length - 1] == '.') {
				appendingValue = appendingValue.Remove(appendingValue.Length - 1, 1);
			}
		}

		private void Calculate() {
			if (string.IsNullOrEmpty(previousAppendedValue)) { return; }
			RemoveEndingDecimal();
			double _raw = 0;
			switch (currentOperation) {
				case OperationMode.Add:
					_raw = double.Parse(previousAppendedValue) + double.Parse(appendingValue);
					break;
				case OperationMode.Subtract:
					_raw = double.Parse(previousAppendedValue) - double.Parse(appendingValue);
					break;
				case OperationMode.Divide:
					_raw = double.Parse(previousAppendedValue) / double.Parse(appendingValue);
					break;
				case OperationMode.Multiply:
					_raw = double.Parse(previousAppendedValue) * double.Parse(appendingValue);
					break;
			}
			operationLine += appendingValue + " =";
			if (IsInt(_raw)) {
				appendingValue = _raw.ToString();
			} else {
				appendingValue = _raw.ToString("0.0000");
			}
			previousAppendedValue = string.Empty;
			didCalculate = true;
			previousOperations.Add(operationLine + " " + appendingValue);
			GUI.FocusControl("");
		}

		private bool IsInt(double N) {
			int X = (int)N;
			double temp2 = N - X;
			if (temp2 > 0) {
				return false;
			}
			return true;
		}
		#endregion
	}
}