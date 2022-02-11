using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace LucasIndustries.Runtime {
	[RequireComponent(typeof(AudioSource))]
	[ScriptExecutionOrder(-1)]
	public class AudioSourceInterpolater : MonoBehaviour {
		#region Static/Const Variables

		#endregion

		#region Public Variables
		[HideInInspector]
		public AudioSource audioSource;
		#endregion

		#region Private Variables
		[SerializeField] private float startVolume;
		[SerializeField] private bool playOnStart;
		[SerializeField] private bool loopTrack;

		private Tween audioTween;
		#endregion

		#region MonoBehavior Native
		private void Awake() {
			audioSource = this.GetComponent<AudioSource>();
			audioSource.loop = false;
			audioSource.playOnAwake = false;
			audioSource.enabled = false;

			audioSource.volume = startVolume;
			audioSource.loop = loopTrack;
			audioSource.enabled = true;
		}

		private void Start() {
			if (playOnStart) {
				audioSource.Play();
			}
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		public void InterpolateVolume(float destination, float duration, Ease ease, bool doPlay = false, bool doStop = false) {
			if (audioTween != null) {
				audioTween.Kill();
			}
			if (doPlay) {
				audioSource.Play();
			}
			audioTween = audioSource.DOFade(destination, duration).SetEase(ease).OnComplete(() => {
				if (doStop) {
					audioSource.Stop();
				}
			});
		}
		#endregion

		#region Private Methods

		#endregion
	}

	#region Classes

	#endregion

	#region Enums

	#endregion
}