using UnityEngine;
using System.Collections;

public class RoomPresenter : MonoBehaviour
{
    public Transform[] SpawnLocations = new Transform[0];

	// Use this for initialization
	public void Initialize () {
	
	}

    // Returns a random spawn location
    public Transform GetRandomSpawnLocation()
    {
        if (this.SpawnLocations.Length <= 0)
            return this.transform;
        else
        {
            int spawnIndex = Random.Range(0, SpawnLocations.Length - 1);

            Debug.Log(this.gameObject.name + " in Spawn Point: " + spawnIndex);
            
            return SpawnLocations[spawnIndex];
        }

    }
}
