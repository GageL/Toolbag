using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.Editor {
	public class PlayerPrefsManager : EditorWindow {
		private Vector2 tableScroll;
		private PlayerPrefPair[] cachedPrefs = new PlayerPrefPair[0];

		private void OnEnable() {
			cachedPrefs = GetPrefs();
		}

		private void OnGUI() {
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				{
					EditorGUILayout.LabelField($"Player Prefs Found: {cachedPrefs.Length}");
					GUILayout.FlexibleSpace();
					GUI.color = new Color(.4f, 1, .4f);
					if (GUILayout.Button("Refresh", EditorStyles.toolbarButton)) {
						cachedPrefs = GetPrefs();
					}
					GUI.color = new Color(1, .4f, .4f);
					if (GUILayout.Button("Delete All", EditorStyles.toolbarButton)) {
						DeleteAllPrefs();
						cachedPrefs = GetPrefs();
					}
					GUI.color = Color.white;
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(10);
				tableScroll = EditorGUILayout.BeginScrollView(tableScroll);
				{
					EditorGUILayout.BeginVertical(EditorStyles.helpBox);
					{
						if (cachedPrefs.Length > 0) {
							foreach (PlayerPrefPair playerPrefPair in cachedPrefs.ToArray()) {
								EditorGUILayout.BeginVertical(EditorStyles.helpBox);
								{
									EditorGUILayout.SelectableLabel($"Key: {playerPrefPair.Key}", GUILayout.Height(18));
									EditorGUILayout.SelectableLabel($"Value: {playerPrefPair.Value.ToString()}", GUILayout.Height(18));
									GUI.color = new Color(1, .4f, .4f);
									if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false), GUILayout.Height(18))) {
										DeletePref(playerPrefPair.Key);
									}
									GUI.color = Color.white;
								}
								EditorGUILayout.EndVertical();
								GUILayout.Space(4);
							}
						} else {
							EditorGUILayout.LabelField($"No player prefs exist");
						}
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.EndVertical();
		}

		[MenuItem("LucasIndustries/PlayerPrefsManager")]
		private static void ShowWindow() {
			GetWindow(typeof(PlayerPrefsManager), false, "Player Prefs Manager").Show();
		}

		private PlayerPrefPair[] GetPrefs() {
			if (Application.platform == RuntimePlatform.WindowsEditor) {
#if UNITY_5_5_OR_NEWER
				Microsoft.Win32.RegistryKey registryKey =
					Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Unity\\UnityEditor\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);
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
						tempPlayerPrefs[i] = new PlayerPrefPair() { Key = key, Value = ambiguousValue };
						i++;
					}
					return tempPlayerPrefs;
				} else {
					return new PlayerPrefPair[0];
				}
			} else {
				throw new NotSupportedException("PlayerPrefsEditor doesn't support this Unity Editor platform");
			}
		}

		private void AddPref(string key, string value) {
			if (!PlayerPrefs.HasKey(key)) {
				PlayerPrefs.SetString(key, value);
			}
		}

		private void DeletePref(string key) {
			if (EditorUtility.DisplayDialog("Confirm", $"Are you sure you want to add '{key}' to prefs?", "Yes", "Cancel")) {
				if (PlayerPrefs.HasKey(key)) {
					PlayerPrefs.DeleteKey(key);
					cachedPrefs = GetPrefs();
				}
			}
		}

		private void DeleteAllPrefs() {
			if (EditorUtility.DisplayDialog("Confirm", $"Are you sure you want to delete all prefs?", "Yes", "Cancel")) {
				PlayerPrefs.DeleteAll();
				cachedPrefs = GetPrefs();
			}
		}
	}

	[Serializable]
	public struct PlayerPrefPair {
		public string Key { get; set; }
		public object Value { get; set; }
		public object TableScroll { get; set; }
	}
}