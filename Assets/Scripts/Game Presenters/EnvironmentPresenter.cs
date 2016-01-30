using UnityEngine;
using System.Collections;

public class EnvironmentPresenter : MonoBehaviour
{
    private RoomPresenter[] rooms = new RoomPresenter[0];

	// Use this for initialization
	public void Initialize () {

        // Get all rooms references
	    this.rooms = this.GetComponentsInChildren<RoomPresenter>();

        // Initialize all rooms
	    foreach (var room in this.rooms)
	    {
	        room.Initialize();
	    }	
	}

    // Returns a random spawn location in a random room
    public Vector3 GetValidPlayerSpawnPosition()
    {
        if (this.rooms.Length <= 0)
            return Vector3.zero;
        else
        {
            return this.rooms[Random.Range(0, this.rooms.Length - 1)].GetRandomSpawnLocation();
        }
    }
}
