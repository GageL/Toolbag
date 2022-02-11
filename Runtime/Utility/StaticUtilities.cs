using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace LucasIndustries.Runtime {
	public static class StaticUtilities {
		#region String
		public static bool ContainsSpecialCharacter(this string input) {
			return input.Any(ch => !char.IsLetterOrDigit(ch));
		}

		public static bool IsNullEmptyOrWhitespace(this string input) {
			return string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input);
		}

		public static bool ContainsSpaces(this string input) {
			return input.Any(ch => char.IsWhiteSpace(ch));
		}

		public static bool IsStructuredEmail(this string input) {
			try {
				var test = new System.Net.Mail.MailAddress(input);
				return true;
			} catch (FormatException ex) {
				return false;
			}
			//Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
			//Match match = regex.Match(input);
			//if (match.Success) {
			//	return true;
			//} else {
			//	return false;
			//}
		}

		public static bool HasUpperAndLowerChar(this string input) {
			return input.Any(char.IsUpper) && input.Any(char.IsLower);
		}

		public static bool HasDigit(this string input) {
			return input.Any(char.IsDigit);
		}
		#endregion

		#region Float
		public static float Truncate(this float value, int digits) {
			double mult = Math.Pow(10.0, digits);
			double result = Math.Truncate(mult * value) / mult;
			return (float)result;
		}

		public static float Round(this float value, int digits) {
			return (float)(Math.Round((double)value, digits));
		}

		public static float LerpNoFrame(this float currentValue, float targetValue, float lerpRate) {
			currentValue = Mathf.Lerp(currentValue, targetValue, 1.0f - Mathf.Exp(-lerpRate * Time.deltaTime));
			return currentValue;
		}
		#endregion

		#region Int
		public static bool IsEvenNumber(this int value) {
			bool result = false;
			if (value % 2 == 0) {
				result = true;
			} else {
				result = false;
			}
			return result;
		}
		#endregion

		#region Vector3
		public static Vector3 LerpNoFrame(this Vector3 currentValue, Vector3 targetValue, float lerpRate) {
			currentValue = Vector3.Lerp(currentValue, targetValue, 1.0f - Mathf.Exp(-lerpRate * Time.deltaTime));
			return currentValue;
		}

		public static bool DistanceMatched(this Vector3 currentPosition, Vector3 targetPosition, float dist) {
			if ((currentPosition - targetPosition).sqrMagnitude <= dist * dist) {
				return true;
			} else {
				return false;
			}
		}
		#endregion

		#region Vector2
		public static Vector2 LerpNoFrame(this Vector2 currentValue, Vector2 targetValue, float lerpRate) {
			currentValue = Vector2.Lerp(currentValue, targetValue, 1.0f - Mathf.Exp(-lerpRate * Time.deltaTime));
			return currentValue;
		}

		public static bool DistanceMatched(this Vector2 currentPosition, Vector2 targetPosition, float dist) {
			if ((currentPosition - targetPosition).sqrMagnitude <= dist * dist) {
				return true;
			} else {
				return false;
			}
		}
		#endregion

		#region Enum
		public static T EnumFromString<T>(string value) {
			return (T)Enum.Parse(typeof(T), value, true);
		}
		#endregion

		#region System
		public static bool IsHeadlessEnvironment() {
			return SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
		}
		#endregion
	}
}