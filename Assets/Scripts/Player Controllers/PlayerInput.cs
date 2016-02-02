using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the player input and sends it to the player components
/// </summary>
public class PlayerInput : MonoBehaviour
{
    // Component references
    private PlayerController mainPlayerController;

    // Player configuration
    [Header("Player Controls")]
    public InputInstance.InputConfiguration InputConfiguration;

    public InputInstance InputInstance { get; private set; }

    public void Initialize(PlayerController playerController)
    {
        // Store references
        this.mainPlayerController = playerController;

        // Initialize input class
        this.InputInstance = new InputInstance();
    }

    public void UpdateInput()
    {
        // Update input
        this.InputInstance.Update(this.InputConfiguration);
    }
}

/// <summary>
/// This class manages the player input
/// </summary>
public class InputInstance
{
    public InputConfiguration Config;

    public float MoveForward;
    public float StrafeRight;

    public float VerticalLook;
    public float HorizontalLook;

    public bool Sprint;
    public bool Jump;
    public bool Crouch;
    public bool Dive;

    public void Update(InputConfiguration config)
    {
        this.Config = config;

        // General input
        this.HorizontalLook = Input.GetAxisRaw(config.HorizontalInput);
        this.VerticalLook = Input.GetAxisRaw(config.VerticalInput);

        // Keyboard input
        if (this.Config.TControlType == ControlType.Keyboard)
        {
            this.MoveForward = Input.GetKey(config.MoveForward) ? 1 : 0;
            this.MoveForward = Input.GetKey(config.MoveBack) ? -1 : this.MoveForward;

            this.StrafeRight = Input.GetKey(config.StrafeRight) ? 1 : 0;
            this.StrafeRight = Input.GetKey(config.StrafeLeft) ? -1 : this.StrafeRight;
            
            this.Sprint = !Input.GetKey(config.Sprint);
            this.Crouch = Input.GetKey(config.Crouch);
            this.Jump = Input.GetKeyDown(config.Jump);
            this.Dive = Input.GetKeyDown(config.Dive);
        }
        // Gamepad input
        else
        {
            this.MoveForward = Input.GetAxisRaw(config.GamepadVerticalInput);
            this.StrafeRight = Input.GetAxisRaw(config.GamepadHorizontalInput);

            this.Sprint = !Input.GetKey(config.GamepadSprint);
            this.Crouch = Input.GetKey(config.GamepadCrouch);
            this.Jump = Input.GetKeyDown(config.GamepadJump);
            this.Dive = Input.GetKeyDown(config.GamepadDive);
        }
    }

    [System.Serializable]
    public class InputConfiguration
    {
        [Header("General")]
        public ControlType TControlType = ControlType.Keyboard;

        [Header("Directional Movement")]
        public KeyCode MoveForward = KeyCode.W;
        public KeyCode MoveBack = KeyCode.S;
        public KeyCode StrafeLeft = KeyCode.A;
        public KeyCode StrafeRight = KeyCode.D;

        public string GamepadSprint = "joystick 1 button 0";
        public string GamepadCrouch = "joystick 1 button 1";
        public string GamepadJump = "joystick 1 button 2";
        public string GamepadDive = "joystick 1 button 3";

        [Header("Gamepad Movement Axes")]
        public string GamepadHorizontalInput = "Mouse X";
        public string GamepadVerticalInput = "Mouse Y";

        [Header("Controller Camera Axes")]
        public string HorizontalInput = "Mouse X";
        public string VerticalInput = "Mouse Y";

        [Header("Special Movement Actions")]
        public KeyCode Sprint = KeyCode.LeftShift;
        public KeyCode Crouch = KeyCode.LeftControl;
        public KeyCode Jump   = KeyCode.Space;
        public KeyCode Dive = KeyCode.Mouse0;
    }

    public enum ControlType
    {
        Keyboard,
        Gamepad
    }
}
