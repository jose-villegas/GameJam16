using UnityEngine;
using System.Collections;

public class PlayerPresenter : MonoBehaviour {

    // Player references
    private PlayerController[] players;
    
    // Player score references
    public float[] PlayerScores;

	// Use this for initialization
	public void Initialize () {
	    // Get player references
	    this.players = this.GetComponentsInChildren<PlayerController>();

        // Generate player score references
	    this.PlayerScores = new float[this.players.Length];

        // Initialize all players
	    foreach (var player in this.players)
	        player.Initialize();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Update player scores
        this.UpdatePlayerScores();
    }

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
}
