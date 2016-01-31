using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerPresenter : MonoBehaviour {

    // Core component references
    private EnvironmentPresenter environmentPresenter;

    // Player references
    private PlayerController[] players;
    
    // Player score references
    public float[] PlayerScores;
    
    // Player respawn variables
    [Range(0,2)]
    public float SafetyOffset = 1.0f;
    [Range(0,10)]
    public float RespawnDelay = 2.0f;                

	// Use this for initialization
	public void Initialize (EnvironmentPresenter environmentPresenter) {
        // Store environment presenters
	    this.environmentPresenter = environmentPresenter;

	    // Get player references
	    this.players = this.GetComponentsInChildren<PlayerController>();

        // Generate player score references
	    this.PlayerScores = new float[this.players.Length];

        // Initialize all players
	    foreach (var player in this.players)
	        player.Initialize(this);

        // Set initial random player positions
	    Transform[] initialTransforms = this.environmentPresenter.GetSeparateSpawnTransforms(this.players.Length);
	    if (initialTransforms != null && this.players.Length <= initialTransforms.Length)
	    {
	        for (int index = 0; index < this.players.Length; index++)
	        {
	            var player = this.players[index];
                this.SetPlayerPosition(player, initialTransforms[index]);
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(GamePresenter.Instance.CurrentMatchState == GamePresenter.State.Running)
            // Update player scores
            this.UpdatePlayerScores();
    }

    #region Respawing
    public void RequestPlayerRespawn(PlayerController playerToRespawn)
    {
        // Check if the player is registered
        if (this.players.Contains(playerToRespawn))
            StartCoroutine("DelayedRespawn", playerToRespawn);
    }

    IEnumerator DelayedRespawn(PlayerController player)
    {
        // Wait the respawn coldown
        yield return new WaitForSeconds(this.RespawnDelay);

        this.RespawnPlayer(player);
    }

    private void RespawnPlayer(PlayerController player)
    {
        Debug.Log("Respawning Player: " + player.name);

        // Move player to valid respawn location
        Transform spawnTransform = this.environmentPresenter.GetValidPlayerSpawnTransform();
        this.SetPlayerPosition(player, spawnTransform);

        // Restore player stats
        player.Resurrect();

    }

    private void SetPlayerPosition(PlayerController player, Transform spawnTransform)
    {
        player.transform.position = spawnTransform.position;
        player.transform.rotation = spawnTransform.rotation;
        player.transform.position += Vector3.up*this.SafetyOffset;
    }

    #endregion

    #region Scoring
    // Update the human player score
    private void UpdatePlayerScores()
    {
        for (var index = 0; index < this.players.Length; index++)
        {
            var player = this.players[index];
            // If the player is an human, update his score
            if (player.Type == Entity.PlayerType.Human)
            {
                this.PlayerScores[index] += Time.deltaTime;
            }
        }
    }

    public PlayerController GetHighestScorePlayer()
    {
        float maxScore = this.PlayerScores.Concat(new float[] {0}).Max();

        List<int> maxPlayerControllerIndexes = new List<int>();
        for (var index = 0; index < this.PlayerScores.Length; index++)
        {
            var playerScore = this.PlayerScores[index];
            if (playerScore >= maxScore)
                maxPlayerControllerIndexes.Add(index);
        }

        // If there is more than one max score player, return draw
        if (maxPlayerControllerIndexes.Count > 1)
            return null;
        else
            return this.players[maxPlayerControllerIndexes[0]];

    }

    #endregion
}
