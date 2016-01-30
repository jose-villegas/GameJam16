using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerPresenter : MonoBehaviour {

    // Player references
    private PlayerController[] players;
    
    // Player score references
    public float[] PlayerScores;
    
    // Player respawn variables
    [Range(0,10)]
    public float RespawnDelay = 2.0f;                

	// Use this for initialization
	public void Initialize () {
	    // Get player references
	    this.players = this.GetComponentsInChildren<PlayerController>();

        // Generate player score references
	    this.PlayerScores = new float[this.players.Length];

        // Initialize all players
	    foreach (var player in this.players)
	        player.Initialize(this);
	}
	
	// Update is called once per frame
	void Update ()
    {
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

        // todo: move player to valid respawn location

        // Restore player stats
        player.Resurrect();

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
    #endregion
}
