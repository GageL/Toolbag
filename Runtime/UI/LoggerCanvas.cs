using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace LucasIndustries.Runtime {
	public class LoggerCanvas : MonoBehaviour {
		#region Public/Private Variables
		[SerializeField] private GameObject loggerMenu;
		[SerializeField] private TMP_Text logText;
		[SerializeField] private Button clearLogButton;
		#endregion

		#region Runtime Variables
		private StringBuilder sb;
		#endregion

		#region Native Methods
		private void Awake() {
			DontDestroyOnLoad(this.gameObject);
			loggerMenu.SetActive(false);
			sb = new StringBuilder();
		}

		private void OnEnable() {
			Application.logMessageReceived += LogMessageReceived;
			clearLogButton.onClick.AddListener(() => ClearLogButtonClick());
		}

		private void OnDisable() {
			Application.logMessageReceived -= LogMessageReceived;
			clearLogButton.onClick.RemoveListener(() => ClearLogButtonClick());
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.BackQuote)) {
				loggerMenu.SetActive(!loggerMenu.activeSelf);
			}
		}
		#endregion

		#region Callback Methods
		private void LogMessageReceived(string condition, string stackTrace, UnityEngine.LogType type) {
			sb.Append($"[{System.DateTime.Now.ToString("G")}] => {condition}");
			if (type == UnityEngine.LogType.Error) {
				sb.Append($"\n\t{stackTrace}");
			}
			logText.text = sb.ToString();
		}

		private void ClearLogButtonClick() {
			sb.Clear();
			logText.text = sb.ToString();
		}
		#endregion

		#region Static Methods

		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
	}
}