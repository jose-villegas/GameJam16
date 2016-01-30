using UnityEngine;
using System.Collections;

public class GamePresenter : MonoBehaviour {

    // Core presenters
    private PlayerPresenter playerPresenter;

    /// <summary>
    /// Only this player component must use the start and update methods
    /// </summary>
    void Start () {
	    // Get core presenters
        this.playerPresenter = this.GetComponentInChildren<PlayerPresenter>();

        // Initialize core presenters
        this.playerPresenter.Initialize();

        // Display connected gamepad
        this.PrintGamepad();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    #region Debug
    void PrintGamepad()
    {
        if (Input.GetJoystickNames().Length <= 0) return;

        int i = 0;
        while (i < Input.GetJoystickNames().Length)
        {
            Debug.Log(Input.GetJoystickNames()[i]);
            i++;
        }
    }
    #endregion
}
