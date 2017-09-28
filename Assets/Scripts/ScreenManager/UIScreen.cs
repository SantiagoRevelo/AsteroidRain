using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class UIScreen : MonoBehaviour {
	public ScreenDefinitions screenDefinition;

	private CanvasGroup canvasGroup;
	protected Animator animator;

	GameObject ScreenWrapper;

	public delegate void Callback();
	Callback callbackOnOpen;

	bool ScreenOpened;
	bool ScreenClosed;

	public virtual void Awake()
	{
		animator = GetComponent<Animator>();
		canvasGroup = gameObject.GetComponentInChildren<CanvasGroup> ();
		
		RectTransform rect = GetComponent<RectTransform>();
		rect.offsetMax = rect.offsetMin = new Vector2(0, 0);
		ScreenWrapper = transform.Find ("Content Wrapper").gameObject;
	}
		
	public virtual void OpenWindow(Callback openCallback = null) {
		callbackOnOpen = openCallback;

		ScreenWrapper.SetActive (true);
		
		IsOpen = true;
	}

	public virtual void CloseWindow(Callback closeCallback = null) {
		IsOpen = false;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
	}

	public bool IsOpen
	{
		get { return Animator.GetBool("IsOpen"); }
		set {
			if (Animator != null) {
				Animator.SetBool("IsOpen", value);
				ScreenOpened = false;
				ScreenClosed = false;
			}
		}
	}

	public Animator Animator {
		get {
			if (animator == null) {
				animator = GetComponent<Animator>();
			}
			return animator;
		}
	}

	public bool InOpenState {
		get {
			return ScreenOpened;
		}
	}

	void OnEndAnimationOpen_Handle() {
		if (callbackOnOpen != null) 
			callbackOnOpen();

		ScreenOpened = true;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.interactable = true;
	}

	public bool InCloseState {
		get {
			return ScreenClosed;
		}
	}

	void OnEndAnimationClose_Handle() {
		ScreenClosed = true;
	}
}
