using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvironmentPresenter : MonoBehaviour
{
    // Human light references
    public Light[] HumanLights = new Light[1];

    // Rooms references
    private RoomPresenter[] rooms = new RoomPresenter[0];

    // Initial player power up
    public GameObject InitialPlayerPowerUp;

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
            Debug.LogError("Not enough player spawn points/rooms for the requested amount of Players");
            return null;
        }
        
        // Return shuffled array
        return this.Reshuffle( spawnTransforms.ToArray());
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
    Transform[] Reshuffle(Transform[] texts)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < texts.Length; t++)
        {
            Transform tmp = texts[t];
            int r = Random.Range(t, texts.Length);
            texts[t] = texts[r];
            texts[r] = tmp;
        }

        return texts;
    }
}
