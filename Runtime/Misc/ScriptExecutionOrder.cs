using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LucasIndustries.Runtime {
	public class ScriptExecutionOrder : Attribute {
		#region Public/Private Variables
		public int order;
		public ScriptExecutionOrder(int order) { this.order = order; }
		#endregion

		#region Runtime Variables

		#endregion

		#region Native Methods

		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	public class ScriptExecutionOrderManager {
		static ScriptExecutionOrderManager() {
			foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts()) {
				if (monoScript.GetClass() != null) {
					foreach (var attr in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptExecutionOrder))) {
						var newOrder = ((ScriptExecutionOrder)attr).order;
						if (MonoImporter.GetExecutionOrder(monoScript) != newOrder) {
							MonoImporter.SetExecutionOrder(monoScript, newOrder);
						}
					}
				}
			}
		}
	}
#endif
}