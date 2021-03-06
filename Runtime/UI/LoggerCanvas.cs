using System.Text;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace LucasIndustries.Runtime {
	public class LoggerCanvas : MonoBehaviour {
#if ENABLE_LI_LOGGER
		#region Public/Private Variables
		public static LoggerCanvas Instance;
		public static Action<string> OnCommandSubmit;
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
			Instance = this;
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
				ToggleMenu();
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
			if (string.IsNullOrEmpty(commandInputField.text)) { return; }
			sb.Append($"[{System.DateTime.Now.ToString("G")}] => {commandInputField.text}\n");
			logText.text = sb.ToString();
			OnCommandSubmit?.Invoke(commandInputField.text);
			commandInputField.text = string.Empty;
		}

		private void ClearLogButtonClick() {
			ClearLogger();
		}
		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		public void ToggleMenu() {
			loggerMenu.SetActive(!loggerMenu.activeSelf);
		}

		public void OpenMenu() {
			loggerMenu.SetActive(true);
		}

		public void CloseMenu() {
			loggerMenu.SetActive(false);
		}

		public void ClearLogger() {
			sb.Clear();
			logText.text = sb.ToString();
		}
		#endregion

		#region Private Methods

		#endregion
#endif
	}
}