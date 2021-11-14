using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace LucasIndustries.Runtime {
	public class UIButton : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
		#region Public/Private Variables
		public SelectionStateType CurrentSelectionState { get => currentSelectionState; }
		public bool IsSelectable { get => isSelectable; }

		[SerializeField] private bool startDisabled = false;
		[SerializeField] private bool isSelectable = false;
		[SerializeField] private bool isAnimation = false;

		public enum TransitionType {
			None,
			Color,
			Sprite
		}
		public enum TargetGraphicType {
			Image,
			Text
		}
		public enum SelectionStateType {
			Idle,
			Highlighted,
			Pressed,
			Selected,
			Disabled
		}

		[Serializable]
		public class Element {
			public Graphic TargetGraphic { get => targetGraphic; }
			public TargetGraphicType GraphicType { get => graphicType; }
			public TransitionType Transition { get => transition; }
			public ColorBlock ColorTransitions { get => colorTransitions; }
			public SpriteState SpriteTransitions { get => spriteTransitions; }
			[HideInInspector] public Sprite CachedNormalSprite;
			
			[Required]
			[HideIf("@transition == TransitionType.None")]
			[SerializeField] private Graphic targetGraphic;
			[InfoBox("You must have an Image taget in order to use a sprite transition", InfoMessageType.Error, "@transition == TransitionType.Sprite && graphicType == TargetGraphicType.Text")]
			[HideIf("@transition == TransitionType.None")]
			[SerializeField] private TargetGraphicType graphicType = TargetGraphicType.Image;
			[SerializeField] private TransitionType transition = TransitionType.None;
			[ShowIf("@transition == TransitionType.Color")]
			[SerializeField] private ColorBlock colorTransitions = new ColorBlock() {
				normalColor = new Color(1f,1f,1f),
				highlightedColor = new Color(.8f, .8f, .8f),
				pressedColor = new Color(.65f, .65f, .65f),
				selectedColor = new Color(1f,1f,1f),
				disabledColor = new Color(1f,1f,1f, 0.3f),
				colorMultiplier = 1f,
				fadeDuration = .1f
			};
			[ShowIf("@transition == TransitionType.Sprite && graphicType == TargetGraphicType.Image")]
			[SerializeField] private SpriteState spriteTransitions;
		}
		[ShowIf("@isAnimation == false")]
		[SerializeField] private Element[] elements;

		public Animator Animator { get => animator; }
		[ShowIf("@isAnimation == true")]
		[Required]
		[SerializeField] private Animator animator;
		[ShowIf("@isAnimation == true")]
		[SerializeField] private AnimationTriggers animationTransitions;

		[FoldoutGroup("Events")]
		public UnityEvent PointerEnterEvent;
		[FoldoutGroup("Events")]
		public UnityEvent PointerExitEvent;
		[FoldoutGroup("Events")]
		public UnityEvent PointerDownEvent;
		[FoldoutGroup("Events")]
		public UnityEvent PointerUpEvent;
		[FoldoutGroup("Events")]
		public UnityEvent PointerClickEvent;
		#endregion

		#region Runtime Variables
		public static UIButton CurrentHoveredButton;
		[GUIColor(1, .4f, .4f)]
		[Title("Runtime Debug", "These values are set at runtime during play mode")]
		[SerializeField] private SelectionStateType currentSelectionState;
		#endregion

		#region Native Methods
		protected override void Awake() {
			InitializeElements();
		}
		#endregion

		#region Callback Methods
		public virtual void OnPointerEnter(PointerEventData eventData) {
			if (currentSelectionState == SelectionStateType.Selected) { return; }
			if (currentSelectionState == SelectionStateType.Disabled) { return; }
			CurrentHoveredButton = this;
			SetButtonState(SelectionStateType.Highlighted);
			PointerEnterEvent?.Invoke();
		}

		public virtual void OnPointerExit(PointerEventData eventData) {
			if (currentSelectionState == SelectionStateType.Selected) { return; }
			if (currentSelectionState == SelectionStateType.Disabled) { return; }
			CurrentHoveredButton = null;
			SetButtonState(SelectionStateType.Idle);
			PointerExitEvent?.Invoke();
		}

		public virtual void OnPointerDown(PointerEventData eventData) {
			if (currentSelectionState == SelectionStateType.Selected) { return; }
			if (currentSelectionState == SelectionStateType.Disabled) { return; }
			SetButtonState(SelectionStateType.Pressed);
			PointerDownEvent?.Invoke();
		}

		public virtual void OnPointerUp(PointerEventData eventData) {
			if (currentSelectionState == SelectionStateType.Selected) { return; }
			if (currentSelectionState == SelectionStateType.Disabled) { return; }
			SetButtonState(SelectionStateType.Idle);
			PointerUpEvent?.Invoke();
		}

		public virtual void OnPointerClick(PointerEventData eventData) {
			if (currentSelectionState == SelectionStateType.Selected) { return; }
			if (currentSelectionState == SelectionStateType.Disabled) { return; }
			PointerClickEvent?.Invoke();
		}
		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		public void SetButtonState(SelectionStateType selectionState, bool ignoreDisabled = true) {
			if (!ignoreDisabled && currentSelectionState == SelectionStateType.Disabled) { return; }
			switch (selectionState) {
				case SelectionStateType.Idle:
					currentSelectionState = SelectionStateType.Idle;
					for (int i = 0; i < elements.Length; i++) {
						if (!elements[i].TargetGraphic) { return; }
						switch (elements[i].Transition) {
							case TransitionType.Color:
								if (CurrentHoveredButton == this) {
									currentSelectionState = SelectionStateType.Highlighted;
									elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.highlightedColor, elements[i].ColorTransitions.fadeDuration);
								} else {
									elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.normalColor, elements[i].ColorTransitions.fadeDuration);
								}
								break;
							case TransitionType.Sprite:
								if (elements[i].GraphicType != TargetGraphicType.Image) { return; }
								if (CurrentHoveredButton == this) {
									if (!elements[i].SpriteTransitions.highlightedSprite) { return; }
									currentSelectionState = SelectionStateType.Highlighted;
									((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.highlightedSprite;
								} else {
									if (!elements[i].CachedNormalSprite) { return; }
									((Image)elements[i].TargetGraphic).sprite = elements[i].CachedNormalSprite;
								}
								break;
							case TransitionType.None:

								break;
						}
					}
					break;
				case SelectionStateType.Highlighted:
					currentSelectionState = SelectionStateType.Highlighted;
					for (int i = 0; i < elements.Length; i++) {
						if (!elements[i].TargetGraphic) { return; }
						switch (elements[i].Transition) {
							case TransitionType.Color:
								elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.highlightedColor, elements[i].ColorTransitions.fadeDuration);
								break;
							case TransitionType.Sprite:
								if (elements[i].GraphicType != TargetGraphicType.Image) { return; }
								if (!elements[i].SpriteTransitions.highlightedSprite) { return; }
								((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.highlightedSprite;
								break;
						}
					}
					break;
				case SelectionStateType.Pressed:
					currentSelectionState = SelectionStateType.Pressed;
					for (int i = 0; i < elements.Length; i++) {
						if (!elements[i].TargetGraphic) { return; }
						switch (elements[i].Transition) {
							case TransitionType.Color:
								elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.pressedColor, elements[i].ColorTransitions.fadeDuration);
								break;
							case TransitionType.Sprite:
								if (elements[i].GraphicType != TargetGraphicType.Image) { return; }
								if (!elements[i].SpriteTransitions.pressedSprite) { return; }
								((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.pressedSprite;
								break;
						}
					}
					break;
				case SelectionStateType.Selected:
					if (isSelectable) {
						currentSelectionState = SelectionStateType.Selected;
						for (int i = 0; i < elements.Length; i++) {
							if (!elements[i].TargetGraphic) { return; }
							switch (elements[i].Transition) {
								case TransitionType.Color:
									elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.selectedColor, elements[i].ColorTransitions.fadeDuration);
									break;
								case TransitionType.Sprite:
									if (elements[i].GraphicType != TargetGraphicType.Image) { return; }
									if (!elements[i].SpriteTransitions.selectedSprite) { return; }
									((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.selectedSprite;
									break;
							}
						}
					}
					break;
				case SelectionStateType.Disabled:
					currentSelectionState = SelectionStateType.Disabled;
					for (int i = 0; i < elements.Length; i++) {
						if (!elements[i].TargetGraphic) { return; }
						switch (elements[i].Transition) {
							case TransitionType.Color:
								elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.disabledColor, elements[i].ColorTransitions.fadeDuration);
								break;
							case TransitionType.Sprite:
								if (elements[i].GraphicType != TargetGraphicType.Image) { return; }
								if (!elements[i].SpriteTransitions.pressedSprite) { return; }
								((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.disabledSprite;
								break;
							case TransitionType.None:

								break;
						}
					}
					break;
			}
		}
		#endregion

		#region Private Methods
		private void InitializeElements() {
			for (int i = 0; i < elements.Length; i++) {
				if (!elements[i].TargetGraphic) { return; }
				if (elements[i].Transition == TransitionType.Sprite) {
					if (elements[i].GraphicType != TargetGraphicType.Image) { return; }
					elements[i].CachedNormalSprite = ((Image)elements[i].TargetGraphic).sprite;
				}
			}
			if (startDisabled) {
				SetButtonState(SelectionStateType.Disabled);
			} else {
				SetButtonState(SelectionStateType.Idle);
			}
		}

		private Graphic TryGetGraphic() {
			return this.GetComponent<Graphic>();
		}
		#endregion
	}
}