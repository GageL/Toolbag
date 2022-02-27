using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

namespace LucasIndustries.Runtime {
	[DefaultExecutionOrder(-2)]
	[RequireComponent(typeof(CanvasGroup))]
	public class UIFadeControl : MonoBehaviour {
		#region Public/Private Variables
		public Action OnShowStarted;
		public Action OnShowCompleted;
		public Action OnHideStarted;
		public Action OnHideCompleted;

		
		[FoldoutGroup("UIFadeControl")]
		[SerializeField] private CanvasGroup canvasGroup;
		[FoldoutGroup("UIFadeControl")]
		[SerializeField] private bool startShown = false;
		[FoldoutGroup("UIFadeControl")]
		[SerializeField] private float fadeInSpeed = 0.5f;
		[FoldoutGroup("UIFadeControl")]
		[SerializeField] private Ease fadeInEase = Ease.InOutQuad;
		[FoldoutGroup("UIFadeControl")]
		[SerializeField] private float fadeOutSpeed = 0.5f;
		[FoldoutGroup("UIFadeControl")]
		[SerializeField] private Ease fadeOutEase = Ease.InOutQuad;

		public float FadeOutSpeed { get { return fadeOutSpeed; } }
		public float FadeInSpeed { get { return fadeInSpeed; } }
		#endregion

		#region Runtime Variables
		[HideInEditorMode]
		[GUIColor(.82f, .40f, .34f)]
		[FoldoutGroup("UIFadeControl Runtime")]
		[SerializeField] protected bool isControlShown = false;
		#endregion

		#region Native Methods
		private void OnValidate() {
			TryGetRef();
			//InstantControl(startShown);
		}

		private void Awake() {
			if (canvasGroup == null) { return; }
			InstantControl(startShown);
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		public void InstantControl(bool show) {
			if (canvasGroup == null) { return; }
			canvasGroup.alpha = show ? 1 : 0;
			isControlShown = show;
			SetValues(show);
		}

		public void Show(Action OnStart = null, Action OnComplete = null) {
			if (canvasGroup == null) { return; }
			OnShow();
			canvasGroup.DOFade(1, fadeInSpeed).SetEase(fadeInEase).OnStart(() => { OnStart?.Invoke(); OnShowStarted?.Invoke(); }).OnComplete(() => { SetValues(true); isControlShown = true; OnComplete?.Invoke(); OnShowCompleted?.Invoke(); });
		}
		protected virtual void OnShow() { }

		public void Hide(Action OnStart = null, Action OnComplete = null) {
			if (canvasGroup == null) { return; }
			OnHide();
			canvasGroup.DOFade(0, fadeOutSpeed).SetEase(fadeOutEase).OnStart(() => { SetValues(false); OnStart?.Invoke(); OnHideStarted?.Invoke(); }).OnComplete(() => { isControlShown = false; OnComplete?.Invoke(); OnHideCompleted?.Invoke(); });
		}
		protected virtual void OnHide() { }

		public bool IsShown() {
			return isControlShown;
		}
		#endregion

		#region Private Methods
		private void TryGetRef() {
			if (canvasGroup != null) { return; }
			canvasGroup = this.GetComponent<CanvasGroup>();
		}

		private void SetValues(bool show) {
			if (canvasGroup == null) { return; }
			canvasGroup.interactable = show ? true : false;
			canvasGroup.blocksRaycasts = show ? true : false;
		}
		#endregion
	}
}