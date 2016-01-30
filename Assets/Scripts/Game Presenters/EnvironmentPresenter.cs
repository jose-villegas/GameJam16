using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public Transform[] GetSeparateSpawnTransforms(int amount)
    {

        List<Transform> spawnTransforms = new List<Transform>();

        foreach (var room in this.rooms)
        {
            foreach (var spawnLocation in room.SpawnLocations)
            {
                spawnTransforms.Add(spawnLocation);
            }
        }

        // Safety check
        if (spawnTransforms.Count < amount)
        {
            Debug.LogError("Not enough player spawn points/rooms for the requested amount of players");
            return null;
        }
        
        // Return shuffled array
        return this.ShuffleArray( spawnTransforms.ToArray());
    }

    // Returns a random spawn location in a random room
    public Transform GetValidPlayerSpawnTransform()
    {
        if (this.rooms.Length <= 0)
        {
            Debug.LogError("No room detected, please insert rooms as childrens of this gameobject");
            return this.transform;
        }
        else
        {
            return this.rooms[Random.Range(0, this.rooms.Length - 1)].GetRandomSpawnLocation();
        }
    }

    // Shuffle lists
    public T[] ShuffleArray<T>(T[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int r = Random.Range(0, i);
            T tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }

        return arr;
    }
}
