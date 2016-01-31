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

        // Initialize core presenters
        this.EnvironmentPresenter.Initialize();
        this.PlayerPresenter.Initialize(this.EnvironmentPresenter);

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
            // todo: display WINNER
            Debug.Log("WINNER: " + winnerPlayer.gameObject.name);
        }
        else
        {
            // todo: display DRAW
            Debug.Log("DRAW");
        }
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
}
