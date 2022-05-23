using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.Editor {
	[Serializable]
	public class PlayerPrefPair {
		public string Key { get; set; }
		public object Value { get; set; }
		public bool IsEditing { get; set; }
		public Type ValueType { get; set; }
	}

	public class EditorPlayerPrefsManager : EditorWindow {
		#region Public/Private Variables
		private static Vector2 tableScroll;
		private static PlayerPrefPair[] cachedPrefs = new PlayerPrefPair[0];
		private static int fieldWidth = 200;
		private static int fieldButtonWidth = 22;
		private static int fieldHeight = 26;
		private static readonly string[] prefTypes = new string[] { typeof(int).Name, typeof(float).Name, typeof(string).Name };
		private static int currentNewPrefType = 0;
		private static string newPrefKey;
		private static int newPrefIntValue;
		private static float newPrefFloatValue;
		private static string newPrefStringValue;
		#endregion

		#region Runtime Variables

		#endregion

		#region Native Methods
		private void OnEnable() {
			cachedPrefs = GetPrefs();
		}

		private void OnGUI() {
			EditorGUILayout.BeginVertical(GUILayout.Width(WindowWidth()));
			{
				DrawToolbar();
				GUILayout.Space(6);
				DrawNewPrefFields();
				GUILayout.Space(12);
				HorizontalLine(new Color(.5f, .5f, .5f, .5f));
				GUILayout.Space(12);
				DrawTable();
			}
			EditorGUILayout.EndVertical();
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		private static void DrawToolbar() {
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				EditorGUILayout.LabelField($"Player Prefs Found: <color=#8eff5d>{cachedPrefs.Length}</color>", TextGUIStyle(EditorStyles.toolbar, FontStyle.Bold, 0, TextAnchor.MiddleLeft, false));
				GUILayout.FlexibleSpace();
				GUI.color = new Color(1, 1, .4f);
				if (GUILayout.Button("Refresh", EditorStyles.toolbarButton)) {
					cachedPrefs = GetPrefs();
				}
				GUI.color = new Color(.4f, 1, 1);
				if (GUILayout.Button("Apply", EditorStyles.toolbarButton)) {
					foreach (PlayerPrefPair playerPrefPair in cachedPrefs.ToArray()) {
						if (playerPrefPair.ValueType == typeof(int)) {
							PlayerPrefs.SetInt(playerPrefPair.Key, (int)playerPrefPair.Value);
						}
						if (playerPrefPair.ValueType == typeof(float)) {
							PlayerPrefs.SetFloat(playerPrefPair.Key, (float)playerPrefPair.Value);
						}
						if (playerPrefPair.ValueType == typeof(string)) {
							PlayerPrefs.SetString(playerPrefPair.Key, (string)playerPrefPair.Value);
						}
					}
				}
				GUI.color = new Color(1, .4f, .4f);
				if (GUILayout.Button("Delete All", EditorStyles.toolbarButton)) {
					DeleteAllPrefs();
					cachedPrefs = GetPrefs();
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();
		}

		private static void DrawNewPrefFields() {
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.LabelField($"Add New Pref", TextGUIStyle(EditorStyles.label, FontStyle.Bold, 14, TextAnchor.MiddleLeft, false), GUILayout.ExpandWidth(true));
				currentNewPrefType = EditorGUILayout.Popup(currentNewPrefType, prefTypes);
				HorizontalLine();
				EditorGUILayout.BeginHorizontal(GUILayout.Height(fieldHeight));
				{
					VerticalLine();
					newPrefKey = EditorGUILayout.TextField(newPrefKey, TextGUIStyle(EditorStyles.textField, FontStyle.Normal, 10, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth), GUILayout.Height(fieldHeight));
					VerticalLine();
					object _value = null;
					Type _type = null;
					if (prefTypes[currentNewPrefType] == typeof(int).Name) {
						newPrefIntValue = EditorGUILayout.IntField(newPrefIntValue, TextGUIStyle(EditorStyles.textField, FontStyle.Normal, 10, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth), GUILayout.Height(fieldHeight));
						_value = newPrefIntValue;
						_type = typeof(int);
					}
					if (prefTypes[currentNewPrefType] == typeof(float).Name) {
						newPrefFloatValue = EditorGUILayout.FloatField(newPrefFloatValue, TextGUIStyle(EditorStyles.textField, FontStyle.Normal, 10, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth), GUILayout.Height(fieldHeight));
						_value = newPrefFloatValue;
						_type = typeof(float);
					}
					if (prefTypes[currentNewPrefType] == typeof(string).Name) {
						newPrefStringValue = EditorGUILayout.TextField(newPrefStringValue, TextGUIStyle(EditorStyles.textField, FontStyle.Normal, 10, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth), GUILayout.Height(fieldHeight));
						_value = newPrefStringValue;
						_type = typeof(string);
					}
					VerticalLine();
					GUI.color = Color.green;
					if (GUILayout.Button("+", GUILayout.Width(22), GUILayout.Height(fieldHeight))) {
						AddPref(newPrefKey, _value, _type);
					}
					VerticalLine();
					GUI.color = Color.red;
					if (GUILayout.Button("x", GUILayout.Width(22), GUILayout.Height(fieldHeight))) {
						ClearNewPrefFields();
					}
					GUI.color = Color.white;
					VerticalLine();
				}
				EditorGUILayout.EndHorizontal();
				HorizontalLine();
			}
			EditorGUILayout.EndVertical();
		}

		private static void ClearNewPrefFields() {
			newPrefKey = string.Empty;
			newPrefIntValue = 0;
			newPrefFloatValue = 0;
			newPrefStringValue = string.Empty;
		}

		private static void DrawTable() {
			tableScroll = EditorGUILayout.BeginScrollView(tableScroll);
			{
				HorizontalLine();
				EditorGUILayout.BeginHorizontal(GUILayout.Height(fieldHeight));
				{
					VerticalLine();
					EditorGUILayout.LabelField($"Key", TextGUIStyle(EditorStyles.label, FontStyle.Bold, 14, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth));
					VerticalLine();
					EditorGUILayout.LabelField($"Value", TextGUIStyle(EditorStyles.label, FontStyle.Bold, 14, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth));
					VerticalLine();
					EditorGUILayout.LabelField($"E", TextGUIStyle(EditorStyles.label, FontStyle.Bold, 14, TextAnchor.MiddleCenter, false), GUILayout.Width(fieldButtonWidth));
					VerticalLine();
					EditorGUILayout.LabelField($"D", TextGUIStyle(EditorStyles.label, FontStyle.Bold, 14, TextAnchor.MiddleCenter, false), GUILayout.Width(fieldButtonWidth));
					VerticalLine();
				}
				EditorGUILayout.EndHorizontal();
				HorizontalLine();
				foreach (PlayerPrefPair playerPrefPair in cachedPrefs.ToArray()) {
					EditorGUILayout.BeginHorizontal(GUILayout.Height(fieldHeight));
					{
						VerticalLine();
						if (playerPrefPair.IsEditing) {
							playerPrefPair.Key = EditorGUILayout.TextField($"{playerPrefPair.Key}", TextGUIStyle(EditorStyles.textField, FontStyle.Normal, 10, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth), GUILayout.Height(fieldHeight));
						} else {
							EditorGUILayout.SelectableLabel($"{playerPrefPair.Key}", TextGUIStyle(EditorStyles.label, FontStyle.Normal, 10, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth), GUILayout.Height(fieldHeight));
						}
						VerticalLine();
						if (playerPrefPair.IsEditing) {
							if (playerPrefPair.ValueType == typeof(int)) {
								playerPrefPair.Value = EditorGUILayout.IntField((int)playerPrefPair.Value, TextGUIStyle(EditorStyles.textField, FontStyle.Normal, 10, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth), GUILayout.Height(fieldHeight));
							}
							if (playerPrefPair.ValueType == typeof(float)) {
								playerPrefPair.Value = EditorGUILayout.FloatField((float)playerPrefPair.Value, TextGUIStyle(EditorStyles.textField, FontStyle.Normal, 10, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth), GUILayout.Height(fieldHeight));
							}
							if (playerPrefPair.ValueType == typeof(string)) {
								playerPrefPair.Value = EditorGUILayout.TextField((string)playerPrefPair.Value, TextGUIStyle(EditorStyles.textField, FontStyle.Normal, 10, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth), GUILayout.Height(fieldHeight));
							}
						} else {
							EditorGUILayout.SelectableLabel($"{playerPrefPair.Value.ToString()}", TextGUIStyle(EditorStyles.label, FontStyle.Normal, 10, TextAnchor.MiddleLeft, false), GUILayout.Width(fieldWidth), GUILayout.Height(fieldHeight));
						}
						VerticalLine();
						GUI.color = Color.green;
						if (GUILayout.Button("E", GUILayout.Width(22), GUILayout.Height(fieldHeight))) {
							playerPrefPair.IsEditing = !playerPrefPair.IsEditing;
						}
						GUI.color = Color.white;
						VerticalLine();
						GUI.color = Color.red;
						if (GUILayout.Button("D", GUILayout.Width(22), GUILayout.Height(fieldHeight))) {
							DeletePref(playerPrefPair.Key);
						}
						GUI.color = Color.white;
						VerticalLine();
					}
					EditorGUILayout.EndHorizontal();
					HorizontalLine();
				}
			}
			EditorGUILayout.EndScrollView();
		}

		private static float WindowWidth() {
			return (fieldWidth * 2) + 34 + (fieldButtonWidth * 2);
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

		[MenuItem("Lucas Industries/Player Prefs Manager")]
		private static void ShowWindow() {
			EditorPlayerPrefsManager _window = (EditorPlayerPrefsManager)GetWindow(typeof(EditorPlayerPrefsManager), true, "Player Prefs Manager");
			_window.minSize = new Vector2(WindowWidth(), 268);
			_window.maxSize = new Vector2(WindowWidth(), 268);
			_window.Show();
		}

		private static void HorizontalLine(Color color = default(Color)) {
			if (color == default(Color)) {
				color = Color.gray;
			}
			GUIStyle _style = new GUIStyle();
			_style.stretchWidth = true;
			_style.normal.background = EditorGUIUtility.whiteTexture;
			_style.margin = new RectOffset(0, 0, 0, 0);
			_style.fixedHeight = 2;
			var c = GUI.color;
			GUI.color = color;
			GUILayout.Box(GUIContent.none, _style);
			GUI.color = c;
		}

		private static void VerticalLine(Color color = default(Color)) {
			if (color == default(Color)) {
				color = Color.gray;
			}
			GUIStyle _style = new GUIStyle();
			_style.stretchHeight = true;
			_style.normal.background = EditorGUIUtility.whiteTexture;
			_style.margin = new RectOffset(0, 0, 0, 0);
			_style.fixedWidth = 2;
			var c = GUI.color;
			GUI.color = color;
			GUILayout.Box(GUIContent.none, _style);
			GUI.color = c;
		}

		private static PlayerPrefPair[] GetPrefs() {
			if (Application.platform == RuntimePlatform.WindowsEditor) {
#if UNITY_5_5_OR_NEWER
				Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Unity\\UnityEditor\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);
#else
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);
#endif
				if (registryKey != null) {
					string[] valueNames = registryKey.GetValueNames();
					PlayerPrefPair[] tempPlayerPrefs = new PlayerPrefPair[valueNames.Length];
					int i = 0;
					foreach (string valueName in valueNames) {
						string key = valueName;
						int index = key.LastIndexOf("_");
						key = key.Remove(index, key.Length - index);
						object ambiguousValue = registryKey.GetValue(valueName);
						if (ambiguousValue.GetType() == typeof(int)) {
							if (PlayerPrefs.GetInt(key, -1) == -1 && PlayerPrefs.GetInt(key, 0) == 0) {
								ambiguousValue = PlayerPrefs.GetFloat(key);
							}
						} else if (ambiguousValue.GetType() == typeof(byte[])) {
							ambiguousValue = System.Text.Encoding.Default.GetString((byte[])ambiguousValue);
						}
						//Debug.Log(ambiguousValue.GetType());
						tempPlayerPrefs[i] = new PlayerPrefPair() { Key = key, Value = ambiguousValue, ValueType = ambiguousValue.GetType() };
						i++;
					}
					return tempPlayerPrefs;
				} else {
					return new PlayerPrefPair[0];
				}
			} else {
				throw new NotSupportedException("EditorPlayerPrefsManager doesn't support this Unity Editor platform");
			}
		}

		private static void AddPref(string key, object value, Type type) {
			if (string.IsNullOrEmpty(key)) { Debug.Log($"Error: Cannot create a pref with a null key"); return; }
			if (value == null) { Debug.Log($"Error: Cannot create a pref with a null value"); return; }
			if (EditorUtility.DisplayDialog("Add", $"Are you sure you want to add '{key}/{value.ToString()}' to prefs?", "Yes", "Cancel")) {
				if (!PlayerPrefs.HasKey(key)) {
					if (type == typeof(int)) {
						PlayerPrefs.SetInt(key, (int)value);
					}
					if (type == typeof(float)) {
						PlayerPrefs.SetFloat(key, (float)value);
					}
					if (type == typeof(string)) {
						PlayerPrefs.SetString(key, (string)value);
					}
					cachedPrefs = GetPrefs();
					ClearNewPrefFields();
					Selection.activeObject = null;
				}
			}
		}

		private static void DeletePref(string key) {
			if (EditorUtility.DisplayDialog("Delete", $"Are you sure you want to delete '{key}' from prefs?", "Yes", "Cancel")) {
				if (PlayerPrefs.HasKey(key)) {
					PlayerPrefs.DeleteKey(key);
					cachedPrefs = GetPrefs();
					Selection.activeObject = null;
				}
			}
		}

		private static void DeleteAllPrefs() {
			if (EditorUtility.DisplayDialog("Delete All", $"Are you sure you want to delete all prefs?", "Yes", "Cancel")) {
				PlayerPrefs.DeleteAll();
				cachedPrefs = GetPrefs();
				Selection.activeObject = null;
			}
		}
		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
	}
}