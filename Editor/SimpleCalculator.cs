using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.Editor {
	public class SimpleCalculator : EditorWindow {
		private string currentInput1 = "0";
		private int currentOperation = 0;
		private string currentInput2 = "0";
		private float currentOutput = 0;

		private static readonly string[] operations = new string[] { "+", "-", @"\", "*" };

		private void OnGUI() {
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.LabelField("Input");
				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal();
				{
					currentInput1 = EditorGUILayout.TextField(currentInput1);
					currentOperation = EditorGUILayout.Popup(currentOperation, operations, GUILayout.Width(48));
					if (GUILayout.Button("|", GUILayout.Width(18))) {
						Swap();
					}
					currentInput2 = EditorGUILayout.TextField(currentInput2);
					if (GUILayout.Button("=", GUILayout.Width(18))) {
						DoMath(currentInput1, currentInput2);
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
				GUILayout.Space(5);
				EditorGUILayout.LabelField("Output");
				EditorGUI.indentLevel++;
				EditorGUILayout.SelectableLabel(currentOutput.ToString());
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndVertical();
		}

		[MenuItem("LucasIndustries/SimpleCalculator")]
		private static void ShowWindow() {
			GetWindow(typeof(SimpleCalculator), false, "Simple Calculator").Show();
		}

		private void Swap() {
			string _v1 = currentInput2;
			string _v2 = currentInput1;
			currentInput1 = _v1;
			currentInput2 = _v2;
			GUI.FocusControl("");
		}

		private void DoMath(string input1, string input2) {
			switch (operations[currentOperation]) {
				case "+":
					currentOutput = float.Parse(input1) + float.Parse(input2);
					break;
				case "-":
					currentOutput = float.Parse(input1) - float.Parse(input2);
					break;
				case @"\":
					currentOutput = float.Parse(input1) / float.Parse(input2);
					break;
				case "*":
					currentOutput = float.Parse(input1) * float.Parse(input2);
					break;
			}
			GUI.FocusControl("");
		}
	}
}