using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace LucasIndustries.Runtime {
	public class InitSiblingIndex : MonoBehaviour {
		#region Static/Const Variables

		#endregion

		#region Public Variables

		#endregion

		#region Private Variables
		[SerializeField] private bool runOnAwake = false;
		[SerializeField] private int siblingIndex = 0;
		#endregion

		#region MonoBehavior Native
		private void OnValidate() {
			if (runOnAwake) { return; }
			DoSibling();
		}

		private void Awake() {
			if (!runOnAwake) { return; }
			DoSibling();
		}

		private void Update() {
			if (runOnAwake) { return; }
			DoSibling();
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods

		#endregion

		#region Private Methods
		private void DoSibling() {
			if (siblingIndex == 0) {
				this.transform.SetAsFirstSibling();
			} else if (siblingIndex == -1) {
				this.transform.SetAsLastSibling();
			} else {
				this.transform.SetSiblingIndex(siblingIndex);
			}
		}
		#endregion
	}

	#region Classes

	#endregion

	#region Enums

	#endregion
}