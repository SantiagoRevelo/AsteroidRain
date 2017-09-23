using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ScreenDefinitions
{
	NONE,
	MAIN_MENU,
	GAMEPLAY,
	GAME_OVER
}

public class ScreenManager : MonoBehaviour {

	public static ScreenManager Instance { get; private set;}

	public List<UIScreen> screens;
	public UIScreen currentGUIScreen;

	private UIScreen _newScreen;

	private GameObject ProfilePlayerInstance;

	public UIScreen lastGUIScreen { get; set; }

	public Action<ScreenDefinitions> OnScreenChange;

	void Awake() {
		
		if(Instance != null && Instance != this) {
			Destroy(gameObject);
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
		screens = new List<UIScreen> ();
		FindScreens ();
	}

	void Start() {			
	}

	void FindScreens() {
		GameObject[] screensFound = GameObject.FindGameObjectsWithTag ("uiscreen");
		foreach (GameObject s in screensFound) {
			screens.Add (s.GetComponent<UIScreen> ());
		}
	}
	
    public void ShowLastScreen()
    {
		ShowScreen(lastGUIScreen.screenDefinition);
    }

    /// <summary>
    /// Shows the screen.
    /// </summary>
    /// <param name="guiScreen">GUI screen.</param>
	public void ShowScreen(ScreenDefinitions definition, UIScreen.Callback theCallback= null) {

		//resultCallback = theCallback;
		UIScreen uiScreen = screens.Find (s => s.screenDefinition == definition);

		if (currentGUIScreen != null && uiScreen != currentGUIScreen) {
            currentGUIScreen.CloseWindow();
		}

        lastGUIScreen = currentGUIScreen;
		currentGUIScreen = uiScreen;
		
		if (currentGUIScreen != null) {
			currentGUIScreen.OpenWindow(theCallback);
		}
	}

	public void HideScreen() {
		if (currentGUIScreen != null) {
			currentGUIScreen.CloseWindow();
		}
	}
}
