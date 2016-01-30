using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the player status
/// </summary>
public class PlayerController : Entity
{
    // Main controllers
    public PlayerInput InputController { get; private set; }
    public PlayerMovementController MovementController { get; private set; }
    public PlayerCameraController CameraController { get; private set; }

    // Collisions
    public LayerMask GameplayLayerMask;

    /// <summary>
    /// Only this player component must use the start and update methods
    /// </summary>
    public void Start()
	{
        // Set initial player variables
        this.InputController = this.GetComponentInChildren<PlayerInput>();
        this.MovementController = this.GetComponentInChildren<PlayerMovementController>();
        this.CameraController = this.GetComponentInChildren<PlayerCameraController>();

        // Initialize player components
        this.InputController.Initialize(this);
        this.MovementController.Initialize(this);
        this.CameraController.Initialize(this);
	}

    #region Player Components Update
    // Update is called once per frame
    void Update()
    {
        // Update player input given the current input key configuration
        this.InputController.UpdateInput();

        // Update regular controllers
        this.UpdateControllers();
    }

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        // Update physics related controllers
        this.FixedUpdateControllers();
    }

    // Update player controllers every frame
    private void UpdateControllers()
    {

        // Update controlers using updated player input if required
        this.CameraController.UpdateCamera(this.InputController.InputInstance);
    }

    // Update player controllers every physics frame
    private void FixedUpdateControllers()
    {
        // Update controlers using updated player input if required
        // note: there is no need to update the input here since it only changes between update frames
        this.MovementController.FixedUpdateMovement(this.InputController.InputInstance);
    }
    #endregion

    #region Player Events

    public override void ChangeRole(PlayerType newRole, bool respawn = false)
    {
        base.ChangeRole(newRole, respawn);

        // todo: change player avatar
    }

    public override void Kill()
    {
        base.Kill();

        // todo: request player respawn
    }

    #endregion

    #region Collisions
    public void ReactGameplayCollision(Entity entity)
    {
        // Reactions as monster (only the monster reacts to gameplay collisions)
        if (this.Type == PlayerType.Monster)
        {
            switch (entity.Type)
            {
                case PlayerType.Human:
                    // Eat the human and take its role
                    entity.Kill();
                    this.ChangeRole(PlayerType.Human);
                    break;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.collider.gameObject.layer) & this.GameplayLayerMask) != 0)
        {
            // Check if it's an entity
            Entity entity = collision.collider.GetComponent<Entity>();
            if (entity != null)
                this.ReactGameplayCollision(entity);
        }
    }

    #endregion
}