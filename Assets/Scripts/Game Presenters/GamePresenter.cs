using UnityEngine;
using System.Collections;

public class GamePresenter : MonoBehaviour {
    // Singleton
    private static GamePresenter _instance = null;
    public static GamePresenter Instance
    {
        get { return GamePresenter._instance; }
    }

    // Core presenters
    public PlayerPresenter PlayerPresenter { get; private set; }
    public EnvironmentPresenter EnvironmentPresenter { get; private set; }
    public GlobalUIPresenter UiPresenter { get; private set; }
    public AudioPresenter AudioPresenter { get; private set; }
    // Match parameters
    public State CurrentMatchState = State.NonStarted;
    public float MatchDuration = 180.0f;
    
    // Match time controls
    public float CurrentMatchDuration = 0.0f;

    void Awake()
    {
        // Set singleton refere
        GamePresenter._instance = this;
    }

    /// <summary>
    /// Only this player component must use the start and update methods
    /// </summary>
    void Start () {

	    // Get core presenters
        this.PlayerPresenter = this.GetComponentInChildren<PlayerPresenter>();
        this.EnvironmentPresenter = this.GetComponentInChildren<EnvironmentPresenter>();
        this.UiPresenter = this.GetComponentInChildren<GlobalUIPresenter>();
        this.AudioPresenter = this.GetComponentInChildren<AudioPresenter>();

        // Initialize core presenters
        this.EnvironmentPresenter.Initialize();
        this.UiPresenter.Initialize();
        this.PlayerPresenter.Initialize(this.EnvironmentPresenter);
        this.AudioPresenter.Initialize();

        // Display connected gamepad
        this.PrintGamepad();

        // Start game
        this.CurrentMatchState = State.Running;

    }
	
	// Update is called once per frame
	void Update ()
	{
        if (this.CurrentMatchState != GamePresenter.State.Running)
            return;

        this.UpdateMatchTimer();
	}

    // Ends the match
    public void EndMatch()
    {
        // Set end game state
        this.CurrentMatchState = State.Ended;

        // Get player with the highest score
        PlayerController winnerPlayer = this.PlayerPresenter.GetHighestScorePlayer();

        // Display winner
        if (winnerPlayer != null)
        {
            // Display winners and losers
            foreach (var playerPresenter in this.PlayerPresenter.Players)
            {
                // Check if the player is the winner
                if(playerPresenter == winnerPlayer)
                    playerPresenter.PlayerUIPresenter.DisplayEndGameMessage(EndGameMessage.Winner);
                // Check if the player was defeated
                else
                    playerPresenter.PlayerUIPresenter.DisplayEndGameMessage(EndGameMessage.Defeated);
            }
        }
        else
        {
            // Display draw players
            foreach (var playerPresenter in this.PlayerPresenter.Players)
                playerPresenter.PlayerUIPresenter.DisplayEndGameMessage(EndGameMessage.Draw);
        }

        // Display general message
        this.UiPresenter.DisplayEndGameLayout();
    }


    // Update match timer
    private void UpdateMatchTimer()
    {
        this.CurrentMatchDuration += Time.deltaTime;
        if (this.CurrentMatchDuration >= this.MatchDuration)
            this.EndMatch();
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

    public enum State
    {
        NonStarted,
        Running,
        Ended
    }

    public enum EndGameMessage
    {
        Winner,
        Defeated,
        Draw
    }
}
