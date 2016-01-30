using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Entity : MonoBehaviour {
    // Player status
    [Header("Player Status")]
    public int HitPoints = 1;
    public PlayerType Type = PlayerType.Monster;

    public enum PlayerType
    {
        Human,
        Monster
    }

}
