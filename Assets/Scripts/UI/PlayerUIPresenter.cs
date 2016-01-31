using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerUIPresenter : MonoBehaviour
{
    // Player references
    private PlayerController playerController;

    // UI elements
    public Image ProgressBar;

    // UI Portraits
    public Image HumanPortrait;
    public Image WolfPortrait;

    // End game messages
    public GameObject VictorMessage;
    public GameObject DrawMessage;
    public GameObject DefeatMessage;

	// Use this for initialization
	public void Initialize (PlayerController playerController) {
	    // Store references
	    this.playerController = playerController;
	}

    // Display End Game Messages
    public void DisplayEndGameMessage(GamePresenter.EndGameMessage message)
    {
        switch (message)
        {
            case GamePresenter.EndGameMessage.Winner:
                this.VictorMessage.SetActive(true);
                break;
            case GamePresenter.EndGameMessage.Defeated:
                this.DefeatMessage.SetActive(true);
                break;
            case GamePresenter.EndGameMessage.Draw:
                this.DrawMessage.SetActive(true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Error check
        if (this.playerController == null || GamePresenter.Instance == null)
            return;

        // Update scroring
        this.ProgressBar.fillAmount = GamePresenter.Instance.PlayerPresenter.PlayerScores[(int) this.playerController.PlayerId]/GamePresenter.Instance.MatchDuration;

        // Set proper player portrait
        this.HumanPortrait.gameObject.SetActive(false);
        this.WolfPortrait.gameObject.SetActive(false);
        switch (this.playerController.Type)
        {
            case Entity.PlayerType.Human:
                this.HumanPortrait.gameObject.SetActive(true);
                break;
            case Entity.PlayerType.Monster:
                this.WolfPortrait.gameObject.SetActive(true);
                break;

        }
    }
}
