using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlobalUIPresenter : MonoBehaviour {

    // General UI Elements
    public GameObject Timer;
    public GameObject ReturnToMenuButton;

    // UI dynamc elements
    public Image ProgressBar;

    // Use this for initialization
    public void Initialize () {
        // Initial ui configuration
	    this.Timer.SetActive(true);
        this.ReturnToMenuButton.SetActive(false);
	}

    public void DisplayEndGameLayout()
    {
        // Final ui configuration
        this.Timer.SetActive(false);
        this.ReturnToMenuButton.SetActive(true);
    }

    void Update()
    {
        // Error check
        if (GamePresenter.Instance == null)
            return;

        // Update core game timer
        this.ProgressBar.fillAmount = GamePresenter.Instance.CurrentMatchDuration / GamePresenter.Instance.MatchDuration;
    }
	
}
