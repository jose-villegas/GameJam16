using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the player status
/// </summary>
public class PlayerController : Entity
{
    // Core presenter
    private PlayerPresenter playerPresenter;

    // Main player identifier
    public ID PlayerId = ID.Player1;

    // Flashlight
    public GameObject FlashLight;

    // Main controllers
    public PlayerInput InputController { get; private set; }
    public PlayerMovementController MovementController { get; private set; }
    public PlayerCameraController CameraController { get; private set; }
    public AvatarController PlayerAvatarController { get; private set; }

    // Collisions
    public LayerMask GameplayLayerMask;

    // Initialize this class
    public void Initialize(PlayerPresenter playerPresenter,int index, Vector4 cameraConfiguration)
	{
        // Set player identifier
        this.PlayerId = (ID) index;
        this.transform.name = this.PlayerId.ToString();

        // Set initial player variables
        this.playerPresenter = playerPresenter;

        this.InputController = this.GetComponentInChildren<PlayerInput>();
        this.MovementController = this.GetComponentInChildren<PlayerMovementController>();
        this.CameraController = this.GetComponentInChildren<PlayerCameraController>();
        this.PlayerAvatarController = this.GetComponentInChildren<AvatarController>();
                                                    
        // Initialize player components
        this.InputController.Initialize(this);
        this.MovementController.Initialize(this);
        this.CameraController.Initialize(this, cameraConfiguration);
        this.PlayerAvatarController.Initialize(this);
	}

    #region Player Components Update
    // Update is called once per frame
    void Update()
    {
        if (GamePresenter.Instance.CurrentMatchState != GamePresenter.State.Running)
            return;

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
        // Update flashlight
        this.FlashLight.gameObject.SetActive(this.Type == PlayerType.Monster);

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

    public override void Resurrect()
    {
        base.Resurrect();

        // Stop player
        this.MovementController.Stop();
    }

    public override void ChangeRole(PlayerType newRole, bool respawn = false)
    {
        base.ChangeRole(newRole, respawn);

        // todo: change player avatar
    }

    public override void Kill()
    {
        // Stop player
        this.MovementController.Stop();

        // Change player role to monster again
        this.ChangeRole(PlayerType.Monster);

        // Kill unit
        base.Kill();

        // Request player respawn
        this.playerPresenter.RequestPlayerRespawn(this);
    }

    #endregion

    #region Collisions
    public void ReactGameplayCollision(Entity entity)
    {
        // Only react to the entity if it's alive
        if (!entity.IsAlive || !this.IsAlive || GamePresenter.Instance.CurrentMatchState != GamePresenter.State.Running)
            return;
        
        // Reactions as monster (only the monster reacts to gameplay collisions)
        if (this.Type == PlayerType.Monster)
        {
            switch (entity.Type)
            {
                case PlayerType.Human:
                    // Eat the human and take its role
                    Debug.Log(this.gameObject.name+"("+this.Type+") Kills human: "+ entity.gameObject.name+"(" + entity.Type + ")");
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

    public enum ID
    {
        Player1,
        Player2,
        Player3,
        Player4
    }

}