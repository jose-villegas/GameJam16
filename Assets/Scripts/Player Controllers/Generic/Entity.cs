using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Entity : MonoBehaviour {
    // Player status
    [Header("Player Status")]
    public int HitPoints = 1;
    public PlayerType Type = PlayerType.Monster;

    public virtual void ChangeRole(PlayerType newRole, bool respawn = false)
    {
        // Set new role
        this.Type = newRole;
    }


    public virtual void Resurrect()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Kill()
    {
        this.gameObject.SetActive(false);
    }

    public enum PlayerType
    {
        Human,
        Monster
    }

}
