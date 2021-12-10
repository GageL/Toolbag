using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace LucasIndustries.Runtime {
	public class LoggerCanvas : MonoBehaviour {
		#region Public/Private Variables
		[SerializeField] private GameObject loggerMenu;
		[SerializeField] private TMP_Text logText;
		[SerializeField] private TMP_InputField commandInputField;
		[SerializeField] private Button submitLogButton;
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
			if (FindObjectOfType<EventSystem>() == null) {
				Instantiate(Resources.Load("LegacyEventSystem")).name = "LegacyEventSystem";
            }
		}

		private void OnEnable() {
			Application.logMessageReceived += LogMessageReceived;
			submitLogButton.onClick.AddListener(() => SubmitLogButtonClick());
			clearLogButton.onClick.AddListener(() => ClearLogButtonClick());
		}

		private void OnDisable() {
			Application.logMessageReceived -= LogMessageReceived;
			submitLogButton.onClick.RemoveListener(() => SubmitLogButtonClick());
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
			sb.Append($"[{System.DateTime.Now.ToString("G")}] => {condition}\n");
			if (type == UnityEngine.LogType.Error) {
				sb.Append($"\t{stackTrace}\n");
			}
			logText.text = sb.ToString();
		}

		private void SubmitLogButtonClick() {
			if (string.IsNullOrEmpty(commandInputField.text)) { commandInputField.text = string.Empty; return; }
			sb.Append($"[{System.DateTime.Now.ToString("G")}] => {commandInputField.text}\n");
			logText.text = sb.ToString();
			commandInputField.text = string.Empty;
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