using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.Editor {
	public class FindScriptUtilityWindow : EditorWindow {

		#region Public Variables

		#endregion

		#region Private Variables

		#endregion

		#region Local Variables
		public static EditorWindow window;
		public static FindScriptUtilityData findScriptUtilityData;
		#endregion

		#region Main Functions
		private void OnEnable() {
			GetEditorData();
		}

		private void OnDisable() {
			SaveEditorData();
		}

		private void OnGUI() {
			EditorGUILayout.BeginVertical();
			{
				if (findScriptUtilityData == null) {
					GetEditorData();
					return;
				}
				EditorGUILayout.BeginHorizontal("toolbar");
				{
					EditorGUILayout.LabelField("Script Finder", GUI.skin.GetStyle("BoldLabel"));
				}
				EditorGUILayout.EndHorizontal();
				findScriptUtilityData.findNullComponents = EditorGUILayout.Toggle(new GUIContent("Find Null Components"), findScriptUtilityData.findNullComponents);
				if (findScriptUtilityData.findNullComponents) {
					findScriptUtilityData.script = null;
					findScriptUtilityData.scriptName = null;
				} else {
					findScriptUtilityData.manualScriptName = EditorGUILayout.Toggle(new GUIContent("Manual Script Name"), findScriptUtilityData.manualScriptName);
					if (findScriptUtilityData.manualScriptName) {
						findScriptUtilityData.scriptName = EditorGUILayout.TextField(findScriptUtilityData.scriptName);
					} else {
						findScriptUtilityData.script = (MonoScript)EditorGUILayout.ObjectField(findScriptUtilityData.script, typeof(MonoScript), false);
					}
				}
				findScriptUtilityData.findScriptMode = (FindScriptMode)EditorGUILayout.EnumPopup(new GUIContent("Search Mode"), findScriptUtilityData.findScriptMode);
				if (GUILayout.Button("Find", GUILayout.Height(25))) {
					if ((findScriptUtilityData.manualScriptName ? string.IsNullOrEmpty(findScriptUtilityData.scriptName) : findScriptUtilityData.script == null) && !findScriptUtilityData.findNullComponents) {
						window.ShowNotification(new GUIContent("Failed: No script info"));
						findScriptUtilityData.foundGameObjects.Clear();
					} else {
						switch (findScriptUtilityData.findScriptMode) {
							case FindScriptMode.Scene:
								FindObjectsInScene();
								break;
							case FindScriptMode.Project:
								FindObjectsInProject();
								break;
						}
					}
				}
				if (findScriptUtilityData.findNullComponents) {
					if (findScriptUtilityData.foundGameObjects.Count > 0) {
						GUI.color = new Color(1f, 0.4f, 0.4f);
						if (GUILayout.Button("Remove Null Components", GUILayout.Height(25))) {
							DeleteNullComponents();
						}
						GUI.color = Color.white;
					}
				}
				GUILayout.Space(10);
				EditorGUILayout.BeginHorizontal("toolbar");
				{
					EditorGUILayout.LabelField("Found Objects", GUI.skin.GetStyle("BoldLabel"));
					EditorGUILayout.LabelField(" " + findScriptUtilityData.foundGameObjects.Count);
				}
				EditorGUILayout.EndHorizontal();
				if (findScriptUtilityData.foundGameObjects.Count > 0) {
					findScriptUtilityData.showfoundGameObjects = EditorGUILayout.Foldout(findScriptUtilityData.showfoundGameObjects, (findScriptUtilityData.showfoundGameObjects ? "Hide" : "Show"));
					if (findScriptUtilityData.showfoundGameObjects) {
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.Space(20);
							findScriptUtilityData.scrollValue = EditorGUILayout.BeginScrollView(findScriptUtilityData.scrollValue);
							{
								EditorGUILayout.BeginVertical();
								{
									foreach (GameObject go in findScriptUtilityData.foundGameObjects) {
										if (go == null) {
											window.ShowNotification(new GUIContent("An Object has been deleted from found list"));
											findScriptUtilityData.foundGameObjects.Clear();
											return;
										}
										EditorGUILayout.BeginHorizontal();
										{
											EditorGUILayout.LabelField(go.name, GUI.skin.GetStyle("BoldLabel"));
											if (GUILayout.Button("Sel", GUILayout.Width(28), GUILayout.Height(18))) {
												SelectObject(go);
											}
										}
										EditorGUILayout.EndHorizontal();
									}
								}
								EditorGUILayout.EndVertical();
							}
							EditorGUILayout.EndScrollView();
						}
						EditorGUILayout.EndHorizontal();
					}
				}
			}
			EditorGUILayout.EndVertical();
		}
		#endregion

		#region Callbacks

		#endregion

		#region Static Functions
		[MenuItem("LucasIndustries/Find Script")]
		public static void InitializeWindow() {
			window = GetWindow<FindScriptUtilityWindow>(false);
			window.titleContent = new GUIContent("Find Script");
			window.Show();
			SetEditorDirty();
			GetEditorData();
		}

		public static void SetEditorDirty() {
			EditorUtility.SetDirty(window);
		}

		public static void GetEditorData() {
			if (!EditorPrefs.HasKey("findScriptUtilityData")) {
				EditorPrefs.SetString("findScriptUtilityData", JsonUtility.ToJson(new FindScriptUtilityData()));
			}
			findScriptUtilityData = JsonUtility.FromJson<FindScriptUtilityData>(EditorPrefs.GetString("findScriptUtilityData"));
		}

		public static void SaveEditorData() {
			EditorPrefs.SetString("findScriptUtilityData", JsonUtility.ToJson(findScriptUtilityData));
		}
		#endregion

		#region Public Functions

		#endregion

		#region Private Functions
		private void FindObjectsInScene() {
			findScriptUtilityData.foundGameObjects.Clear();
			GameObject[] gos = FindObjectsOfType<GameObject>();
			foreach (GameObject g in gos) {
				foreach (Component c in g.GetComponents<Component>()) {
					if (findScriptUtilityData.findNullComponents) {
						if (c == null) {
							findScriptUtilityData.foundGameObjects.Add(g);
						}
					} else {
						if (c.GetType().Name == (findScriptUtilityData.manualScriptName ? findScriptUtilityData.scriptName : findScriptUtilityData.script.GetClass().Name)) {
							findScriptUtilityData.foundGameObjects.Add(g);
						}
					}
				}
			}
		}

		private void FindObjectsInProject() {
			findScriptUtilityData.foundGameObjects.Clear();
			string[] temp = AssetDatabase.GetAllAssetPaths();
			List<string> paths = new List<string>();
			foreach (string s in temp) {
				if (s.Contains(".prefab"))
					paths.Add(s);
			}
			bool finished = false;
			foreach (string path in paths) {
				if (finished) {
					break;
				}
				UnityEngine.Object o = AssetDatabase.LoadMainAssetAtPath(path);
				GameObject go;
				try {
					go = (GameObject)o;
					Component[] components = go.GetComponentsInChildren<Component>(true);
					foreach (Component c in components) {
						if (c.GetType().Name == (findScriptUtilityData.manualScriptName ? findScriptUtilityData.scriptName : findScriptUtilityData.script.GetClass().Name)) {
							finished = true;
							findScriptUtilityData.foundGameObjects.Add(go);
							break;
						}
					}
				} catch {

				}
			}
		}

		private void SelectObject(GameObject selection) {
			Selection.activeGameObject = selection;
		}

		private void DeleteNullComponents() {
			foreach (GameObject go in findScriptUtilityData.foundGameObjects) {
				Component[] components = go.GetComponents<Component>();
				var r = 0;
				for (int i = 0; i < components.Length; i++) {
					if (components[i] == null) {
						EditorUtility.SetDirty(go);

						var serializedObject = new SerializedObject(go);
						var prop = serializedObject.FindProperty("m_Component");
						prop.DeleteArrayElementAtIndex(i - r);
						r++;
						serializedObject.ApplyModifiedProperties();
					}
				}
			}
			findScriptUtilityData.foundGameObjects.Clear();
		}
		#endregion
	}

	#region Base Classes
	[Serializable]
	public class FindScriptUtilityData {
		public Vector2 scrollValue;
		public bool manualScriptName;
		public MonoScript script;
		public string scriptName;
		public bool findNullComponents;
		public FindScriptMode findScriptMode;
		public List<GameObject> foundGameObjects = new List<GameObject>();
		public bool showfoundGameObjects;
	}
	#endregion

	#region Base Enums
	[Serializable]
	public enum FindScriptMode { Scene, Project };
	#endregion
}