using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.Editor {
	public class SimpleCalculatorEditor : EditorWindow {
		private float currentInput1 = 0;
		private int currentOperation = 0;
		private float currentInput2 = 0;
		private float currentOutput = 0;

		private static readonly string[] operations = new string[] { "+", "-", @"\", "*" };

		private void OnGUI() {
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.LabelField("Input:", TextGUIStyle(EditorStyles.label, FontStyle.Bold, 0, TextAnchor.MiddleLeft, false), GUILayout.Height(18));
				//EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal();
				{
					currentInput1 = EditorGUILayout.FloatField(currentInput1, GUILayout.Height(18));
					currentOperation = EditorGUILayout.Popup(currentOperation, operations, GUILayout.Width(32), GUILayout.Height(18));
					if (GUILayout.Button("↔", GUILayout.Width(26), GUILayout.Height(18))) {
						Swap();
					}
					currentInput2 = EditorGUILayout.FloatField(currentInput2, GUILayout.Height(18));
					if (GUILayout.Button("=", GUILayout.Width(26), GUILayout.Height(18))) {
						Calculate();
					}
				}
				EditorGUILayout.EndHorizontal();
				//EditorGUI.indentLevel--;
				EditorGUILayout.LabelField("Result:", TextGUIStyle(EditorStyles.label, FontStyle.Bold, 0, TextAnchor.MiddleLeft, false), GUILayout.Height(18));
				EditorGUI.indentLevel++;
				EditorGUILayout.SelectableLabel($"<color=#8eff5d>{currentOutput}</color>", TextGUIStyle(EditorStyles.label, FontStyle.BoldAndItalic, 0, TextAnchor.MiddleLeft, false), GUILayout.Height(18));
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndVertical();
		}

		private static GUIStyle TextGUIStyle(GUIStyle rootStyle, FontStyle fontStyle, int fontSize, TextAnchor alignment, bool wordWrap) {
			GUIStyle _style = new GUIStyle(rootStyle);
			_style.fontStyle = FontStyle.Bold;
			_style.fontSize = fontSize;
			_style.alignment = alignment;
			_style.wordWrap = wordWrap;
			_style.richText = true;
			return _style;
		}

		[MenuItem("Lucas Industries/Simple Calculator")]
		private static void ShowWindow() {
			SimpleCalculatorEditor _window = (SimpleCalculatorEditor)GetWindow(typeof(SimpleCalculatorEditor), true, "Simple Calculator");
			_window.Show();
			_window.minSize = new Vector2(244, 85);
			_window.maxSize = new Vector2(244, 85);
			Debug.Log(_window.position.size);
		}

		private void Swap() {
			float _v1 = currentInput2;
			float _v2 = currentInput1;
			currentInput1 = _v1;
			currentInput2 = _v2;
			GUI.FocusControl("");
		}

		private void Calculate() {
			switch (operations[currentOperation]) {
				case "+":
					currentOutput = currentInput1 + currentInput2;
					break;
				case "-":
					currentOutput = currentInput1 - currentInput2;
					break;
				case @"\":
					currentOutput = currentInput1 / currentInput2;
					break;
				case "*":
					currentOutput = currentInput1 * currentInput2;
					break;
			}
			GUI.FocusControl("");
		}
	}
}