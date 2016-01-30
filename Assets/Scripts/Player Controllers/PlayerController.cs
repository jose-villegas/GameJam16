using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the player status
/// </summary>
public class PlayerController : MonoBehaviour
{
	// Player status
	[Header("Player Status")]
	public int HitPoints = 1;

    // Main controllers
    public PlayerInput InputController { get; private set; }
    public PlayerMovementController MovementController { get; private set; }

    /// <summary>
    /// Only this player component must use the start and update methods
    /// </summary>
    public void Start()
	{
        // Set initial player variables
        this.InputController = this.GetComponentInChildren<PlayerInput>();
        this.MovementController = this.GetComponentInChildren<PlayerMovementController>();

        // Initialize player components
        this.InputController.Initialize(this);
        this.MovementController.Initialize(this);
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
        // Update player status
        // todo:

        // Update controlers using updated player input if required
        // todo:
    }

    // Update player controllers every physics frame
    private void FixedUpdateControllers()
    {
        // Update controlers using updated player input if required
        // note: there is no need to update the input here since it only changes between update frames
        this.MovementController.FixedUpdateMovement(this.InputController.InputInstance);
    }
    #endregion
}