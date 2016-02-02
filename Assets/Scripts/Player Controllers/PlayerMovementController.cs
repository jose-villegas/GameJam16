using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    // Component references
    private PlayerController mainPlayerController;

    //player movement speed amounts
    [Header("Default Movement Speed")]
	[Range(0, 100)]
    public float RunSpeed = 9.0f;
	[Range(0, 100)]
    public float WalkSpeed = 4.0f;
	[Range(0, 10)]
    public float JumpSpeed = 3.0f;
	[Range(0, 10)]
    public float ClimbSpeed = 4.0f;//speed that player moves upward when Climbing

    // player movement control variables
    private float speed = 6.0f; //combined axis speed of player movement
    private float limitStrafeSpeed = 0.0f;
    private float crouchSpeedAmt = 1.0f;
    private float speedAmtY = 1.0f; //current player speed per axis which is applied to rigidbody velocity vector
    private float speedAmtX = 1.0f;
    private float zoomSpeedAmt = 1.0f;

    // Diveeeee
    [Header("Dive Modifiers")]
    public float DiveForce = 15.0f;
    [Range(0,20)]
    public float DiveColdown = 4.0f;
    private float lastDiveTime;

    // player movement Multipliers
    [Header("Speed Modifiers")]
    [Range(0, 1)]
    public float AirSpeedMultiplier = 1.0f; //Multiplier of movement control  while moving in-air 
    [Range(0, 1)]
    public float BackwardSpeedMultiplier = 0.6f; //Multiplier to decrease movement speed while moving backwards
    [Range(0, 1)]
    public float CrouchSpeedMultiplier = 0.55f; //Multiplier to decrease movement speed while crouching
    [Range(0, 1)]
    public float ZoomSpeedMultiplier = 0.6f; //Multiplier to decrease movement speed while zooming
    [Range(0, 1)]
    public float SlidingSpeedMultiplier = 0.5f; //Multiplier of movement control while sliding in-air 

    // collider and rigidbody components
    [Header("Collision Parameters")]
    public LayerMask ClipMask;//mask for reducing the amount of objects that ray and capsule casts have to check
    [Range(0,2)]
    public float CollisionRadiusMult = 1.0f;
    private Rigidbody rigidBody;
    private CapsuleCollider capsule;

    //rigidbody physics settings
    public float SlopeGravityMultiplier= 3;
    public int SlopeLimit = 40;//the maximum allowed ground surface/normal angle that the player is allowed to climb
    private int maxVelocityChange = 5;//maximum rate that player velocity can change
    private bool rayTooSteep = false;//true when ray from capsule origin hits surface/normal angle greater than SlopeLimit, compared with capsuleTooSteep
    private Vector3 steepRayNormal = Vector3.zero;
    private bool capsuleTooSteep = false;//true when capsule cast hits surface/normal angle greater than SlopeLimit, compared with rayTooSteep
    private bool parentState = false;//only set parent once to prevent rapid parenting and de-parenting that breaks functionality

    // Gravity
    [Header("Gravity Parameters")]
    public Vector3 TargetGravity = new Vector3(0, -15, 0);//additional Gravity that is manually applied to the player rigidbody
    //public float GravityFlipColdown = 2.0f;
    private float lastFlipTime;

    // track player input
    public float InputXSmoothed { private set; get; }//binary inputs smoothed using lerps
    public float InputYSmoothed { private set; get; }

    // Wall Sliding
    [Header("Wall Sliding")]
    [Range(0, 180)]
    public float WallSlidingAngleThreshold = 10.0f;

    // basic rigidbody physics states
    [Header("Movement Status")]
    public bool Grounded;//true when capsule cast hits ground surface
    public bool Sliding = false;
    public bool Falling = false;//true when player is losing altitude
	public bool IsFlipping = false;

    public bool IsMoving
    {
        get
        {
            return this.rigidBody.velocity.magnitude > 0.1f;
        }
    }

    // Zooming
    public bool ZoomSpeed = false;//to control speed of movement while zoomed, handled by Ironsights script and true when zooming
    public bool Zoomed = false;

    //Climbing (ladders or other climbable surfaces)
    public bool Climbing; //true when playing is in contact with ladder trigger
    private float climbSpeedAmt = 4.0f;//actual rate that player is Climbing

    //In-air 
	[HideInInspector]
    public float AirTime = 0.0f;//total time that player is airborn
    private bool airTimeState = false;
    private float fallingDistance;//total distance that player has fallen
    private Vector3 fallStartLevel;//the coordinates that the player lost grounding and started to fall

    //Sprinting
    public bool CanRun = true;//true when player is allowed to sprint
    public bool SprintActive = false;//true when sprint button is ready
    private bool sprintBtnState = false;
	[HideInInspector]
    public bool CancelSprint = false;//true when sprint is canceled by other player input
	[HideInInspector]
    public float SprintStopTime = 0.0f;//track when sprinting stopped for control of item pickup time in FPSPlayer script 
    private bool sprintStopState = true;

    //Jumping
    public bool Jumping;//true when player is Jumping
    private float jumpTimer = 0.0f;//track the time player began jump
    private bool jumpfxstate = true;
    private bool jumpBtn = true;//to control jump button behavior
    public float LandStartTime { private set; get; }//time that player landed from jump

    //crouching	
    [HideInInspector]
    public float MidPos = 0.9f;//camera vertical position
    public bool Crouched = false;//true when player is crouching
    private bool crouchState = false;
    private bool crouchHit = false;//true when object above player prevents standing up from crouch
    
    // Miscellaneous
    [Header("Miscellaneous")]
    public bool AirMovementControl = true;
    public float FallingDamageThreshold = 5.5f;//Units that player can fall before taking damage
    public float AntiBunnyHopFactor = 0.35f;//to limit the time between player jumps

    // Camera control parameters (rotation)
    private float rotationX = 0.0f;
    private Quaternion originalLocalRotation;

    // Use this for initialization
    public void Initialize(PlayerController playerController)
    {
        // Store main player component reference
		this.mainPlayerController = playerController;

        // Get components
        this.rigidBody = this.GetComponent<Rigidbody>();
        this.capsule = this.GetComponent<CapsuleCollider>();

        //Initialize rigidbody
        this.rigidBody.freezeRotation = true;
        this.rigidBody.useGravity = false;

        //clamp movement modifier Multipliers
        this.BackwardSpeedMultiplier = Mathf.Clamp01(this.BackwardSpeedMultiplier);
        this.CrouchSpeedMultiplier = Mathf.Clamp01(this.CrouchSpeedMultiplier);
        this.ZoomSpeedMultiplier = Mathf.Clamp01(this.ZoomSpeedMultiplier);

        // Store variables
        this.originalLocalRotation = this.transform.localRotation;
    }

    #region Gravity Management

    private void UpdateCurrentGravity(InputInstance inputInstance)
    {
        // Rotate player object
        this.transform.up = -this.TargetGravity.normalized;
    }

    #endregion

    #region Update movement
    // FixedUpdate is called once per frame
    public void FixedUpdateMovement(InputInstance inputInstance)
    {
        // End game check
        if (GamePresenter.Instance.CurrentMatchState != GamePresenter.State.Running || !this.mainPlayerController.IsAlive)
        {
            this.rigidBody.velocity = Vector3.zero;
            return;
        }

        // Before doing anything, apply current gravity vector
        this.UpdateCurrentGravity(inputInstance);

        // Update player horizontal rotation
        this.UpdatePlayerRotation(inputInstance);

        // Set initial parameters
        RaycastHit hit = new RaycastHit();

        //set the vertical bounds of the capsule used to detect player collisions
        Vector3 p1 = this.transform.position;//bottom of player capsule
        Vector3 p2 = p1 - this.TargetGravity.normalized * capsule.height / 2;//top of player capsule

        //track rigidbody velocity
        Vector3 velocity = this.rigidBody.velocity;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Player Input
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        float inputY = 0, inputX = 0;

        //track movement buttons and set input vars
        //Input.Axis is not used here to have have more control over button states and to avoid glitches 
        //such as pressing opposite movement buttons simultaneously causing player to move very slow in one direction
        this.ProcessInputDirection(inputInstance, ref inputY, ref inputX);

        //Smooth our movement states using Mathf.Lerp
        this.InputXSmoothed = Mathf.Lerp(this.InputXSmoothed, inputX, Time.deltaTime * 6.0f);
        this.InputYSmoothed = Mathf.Lerp(this.InputYSmoothed, inputY, Time.deltaTime * 6.0f);

        // Send input for movement animation
        this.mainPlayerController.PlayerAvatarController.UpdateMovementAnimation(this.InputXSmoothed,this.InputYSmoothed, inputInstance.Sprint,this.Crouched);

        // Send input for movement sfx
        this.mainPlayerController.PlayerAudioController.UpdateMovementInputSFX(inputX, inputY, inputInstance.Sprint);

        //This is the start of the large block that performs all movement actions while Grounded	
        if (this.Grounded)
        {
            // Process player while landing
            this.FallingPlayerCalculations();

            // Sets or disables crouch mode
            this.CrouchModeHandling(inputInstance, p1, p2,hit);

            // Execute player sprinting
            this.ExecutePlayerSprinting(inputInstance, inputY);

            // Process player movement speed
            this.CalculatePlayerMovementSpeed(inputY, inputX);

            // Execute player jumping
            this.ExecutePlayerJumping(inputInstance, velocity);

            // Execute dive kick
            this.ExecuteDive(inputInstance);
        }
        else
        {
            //Player is airborn////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //keep track of the time that player lost grounding for air manipulation and moving gun while Jumping
            if (this.airTimeState)
            {
                this.AirTime = Time.time;
                this.airTimeState = false;
            }
            // Execute Falling player calculations
            this.ExecuteFalling();
        }

        // Execute player calculations while crouching
        this.ExecuteCrouching(inputInstance);

        // Execute player ground check
        this.ApplyPlayerGroundCheck(p1, p2);
        
        // Set player final velocity
        this.SetPlayerVelocity(inputX, inputY, velocity);
    }

    public void Stop()
    {
        StopCoroutine("EffectiveStop");
        StartCoroutine("EffectiveStop");
    }

    IEnumerator EffectiveStop()
    {
        this.rigidBody.velocity = Vector3.zero;
        this.rigidBody.isKinematic = true;
        yield return new WaitForSeconds(0.1f);
        this.rigidBody.isKinematic = false;
        this.rigidBody.velocity = Vector3.zero;
    }

    private void UpdatePlayerRotation(InputInstance inputInstance)
    {
        this.rotationX += inputInstance.HorizontalLook * this.mainPlayerController.CameraController.SensitivityX * Time.timeScale;
        transform.eulerAngles = new Vector3(0.0f, this.rotationX, 0.0f);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Dive Kick
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void ExecuteDive(InputInstance inputInstance)
    {
        if (inputInstance.Dive && (this.lastDiveTime + this.DiveColdown) < Time.time)
        {
            this.lastDiveTime = Time.time;

            // Execute dive kick
            Vector3 diveVector = this.transform.forward*this.DiveForce;

            // Execute final jump
            this.rigidBody.velocity = diveVector;

            // Play animation
            this.mainPlayerController.PlayerAvatarController.PlayDiveAnimation();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Landing
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void FallingPlayerCalculations()
    {
        //reset airTimeState var so that AirTime will only be set once when player looses grounding
        this.airTimeState = true;

        if (this.Falling)
        {
            //reset Falling state and perform actions if player has landed from a fall
            this.fallingDistance = 0;
            this.LandStartTime = Time.time; //track the time when player landed
            this.Falling = false;

            if ((Vector3.Distance(this.fallStartLevel, this.transform.position)) > 2.0f)
            {
                //play landing sound effect when Falling and not landing from jump
                if (!this.Jumping)
                {
                    /*
                    // play landing sound
					this.mainPlayerInput.AudioController.PlayLandSfx(
						this.mainPlayerInput.CameraController.MainCamera.transform.position);

                    //make camera jump when landing for better feeling of player weight	
					this.mainPlayerInput.CameraController.RequestPlayLandAnimation();
                    */
                }
            }

            //track the distance of the fall and apply damage if over Falling threshold
           // if (Vector3.Distance(this.fallStartLevel, this.transform.position) > this.FallingDamageThreshold)
			//	this.m.CalculateFallingDamage(Vector3.Distance(this.fallStartLevel, this.transform.position));
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Crouch Mode Handling
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void CrouchModeHandling(InputInstance inputInstance, Vector3 p1, Vector3 p2, RaycastHit hit)
    {
        //set Crouched variable that other scripts will access to check for crouching
        if (inputInstance.Crouch)
        {
            if (!this.crouchState)
            {
                // Check that we aren't already crouched
                if (!this.Crouched)
                {
                    this.Crouched = true;
                    this.SprintActive = false; //cancel sprint if crouch button is pressed
                }
                else
                {
                    // To disable crouch, first check we don't have anything above us
                    if (!this.CheckForCollidersAbove(p1))
                        this.Crouched = false;
                }
                this.crouchState = true;
            }
        }
        else
        {
            this.crouchState = false;
            if ((this.SprintActive || this.Climbing) && !this.CheckForCollidersAbove(p1))
            {
                this.Crouched = false; //cancel crouch if sprint button is pressed
            }
        }
        //cancel crouch if jump button is pressed
        if (inputInstance.Jump && this.Crouched &&
            !this.CheckForCollidersAbove(p1))
        {
            this.Crouched = false;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Player Movement Speeds
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void CalculatePlayerMovementSpeed(float inputY, float inputX)
    {
        //check that player can run and set speed 
        if (this.CanRun)
        {
            if (this.speed < this.RunSpeed)
            {
                this.speed += 12*Time.deltaTime; //gradually accelerate to run speed
            }
        }
        else
        {
            if (this.speed > this.WalkSpeed)
            {
                this.speed -= 16*Time.deltaTime; //gradually decelerate to walk speed
            }
        }

        //check if player is zooming and set speed 
        if (this.ZoomSpeed)
        {
            if (this.zoomSpeedAmt > this.ZoomSpeedMultiplier)
            {
                this.zoomSpeedAmt -= Time.deltaTime; //gradually decrease variable to zooming limit value
            }
        }
        else
        {
            if (this.zoomSpeedAmt < 1.0f)
            {
                this.zoomSpeedAmt += Time.deltaTime; //gradually increase variable to neutral
            }
        }

        //check that player can crouch and set speed
        //also check midpos because player can still be under obstacle when crouch button is released 
        if (this.Crouched || this.MidPos < 0.9f)
        {
            if (this.crouchSpeedAmt > this.CrouchSpeedMultiplier)
            {
                this.crouchSpeedAmt -= Time.deltaTime; //gradually decrease variable to crouch limit value
            }
        }
        else
        {
            if (this.crouchSpeedAmt < 1.0f)
            {
                this.crouchSpeedAmt += Time.deltaTime; //gradually increase variable to neutral
            }
        }

        //limit move speed if backpedaling
        if (inputY >= 0)
        {
            if (this.speedAmtY < 1.0f)
            {
                speedAmtY += Time.deltaTime; //gradually increase variable to neutral
            }
        }
        else
        {
            if (this.speedAmtY > this.BackwardSpeedMultiplier)
            {
                speedAmtY -= Time.deltaTime; //gradually decrease variable to backpedal limit value
            }
        }

        //allow limiting of move speed if strafing directly sideways and not diagonally
        if (this.speedAmtX < 1.0f)
        {
            this.speedAmtX += Time.deltaTime; //gradually increase variable to neutral
        }

    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Sprinting
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void ExecutePlayerSprinting(InputInstance inputInstance, float inputY)
    {
        // Hold sprint configuration
        this.SprintActive = inputInstance.Sprint;

        //determine if player can run 
        if (inputY != 0.0f
            && this.SprintActive
            && !this.Crouched
            && (!this.CancelSprint || (this.CancelSprint && this.Zoomed))
            && this.Grounded)
        {
            this.CanRun = true;
            this.Zoomed = false; //cancel zooming when sprinting
            this.sprintStopState = true;
        }
        else
        {
            if (this.sprintStopState)
            {
                this.SprintStopTime = Time.time;
                this.sprintStopState = false;
            }
            this.CanRun = false;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Jumping
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void ExecutePlayerJumping(InputInstance inputInstance, Vector3 velocity)
    {
        if (this.Jumping)
        {
            //play landing sound effect after landing from jump and reset jumpfxstate
            if (this.jumpTimer + 0.25f < Time.time)
            {
                //play landing sound
                /*
				this.mainPlayerInput.AudioController.PlayLandSfx(
					this.mainPlayerInput.CameraController.MainCamera.transform.position);

                //play landing animation
				this.mainPlayerInput.CameraController.RequestPlayLandAnimation();
                */

                // set control variable
                this.jumpfxstate = true;
            }
            //reset Jumping var (this check must be before Jumping var is set to true below)
            this.Jumping = false;

            //allow a small amount of time for capsule to become un-grounded before setting
            //jump button state to false to prevent continuous Jumping if jump button is held.
            if (this.jumpTimer + 0.25f < Time.time)
            {
                this.jumpBtn = false;
            }
        }

        //determine if player is Jumping and set Jumping variable
        if (inputInstance.Jump
            && !this.Zoomed
            && this.jumpBtn //check that jump button is not being held
            && !this.Crouched
            && this.LandStartTime + this.AntiBunnyHopFactor < Time.time //check for bunnyhop delay before Jumping
            )
        {
            //do not jump if ground normal is greater than SlopeLimit
            if (!this.Jumping)
            {
                this.Jumping = true;
                //track the time we began to jump
                this.jumpTimer = Time.time;
            }

            //apply the jump velocity to the player rigidbody
            Vector3 jumpVector;

            // Execute regular jump
            if (!this.rayTooSteep)
                jumpVector = -this.TargetGravity.normalized * Mathf.Sqrt(2 * this.JumpSpeed * Mathf.Abs(this.TargetGravity.magnitude));
            // Execute slope jump
            else
                jumpVector = this.steepRayNormal.normalized * Mathf.Sqrt(2 * this.JumpSpeed * Mathf.Abs(this.TargetGravity.magnitude));

            // Before executig the jump, neutralize the velocity of the direction we are going to jump to
            velocity = velocity - new Vector3(velocity.x * Mathf.Abs(this.TargetGravity.normalized.x),
                                              velocity.y * Mathf.Abs(this.TargetGravity.normalized.y),
                                              velocity.z * Mathf.Abs(this.TargetGravity.normalized.z));

            // Execute final jump
            this.rigidBody.velocity = new Vector3(velocity.x, velocity.y, velocity.z) + jumpVector;


            // Play animation
            this.mainPlayerController.PlayerAvatarController.PlayJumpAnimation();
        }

        //set jumpBtn to false to prevent continuous Jumping while holding jump button.
        if (!inputInstance.Jump && this.LandStartTime + this.AntiBunnyHopFactor < Time.time)
        {
            this.jumpBtn = true;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Crouching
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void ExecuteCrouching(InputInstance inputInstance)
    {
        if (Time.timeSinceLevelLoad > 0.5f)
        {
            //crouch
            if (this.Crouched)
            {
                if (this.MidPos > 0.45f)
                {
                    this.MidPos -= 5*Time.deltaTime;
                } //decrease camera height to crouch height
                if (this.capsule.height > 1.25f)
                {
                    this.capsule.height -= 5*Time.deltaTime;
                } //decrease capsule height to crouch height
            }
            else
            {
                if (this.MidPos < 0.9f)
                {
                    this.MidPos += 2.25f*Time.deltaTime;
                } //increase camera height to standing height
                if (this.capsule.height < 2.0f)
                {
                    this.capsule.height += 2.25f*Time.deltaTime;
                } //increase camera height to standing height
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Falling
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void ExecuteFalling()
    {
        //subtract height we began Falling from current position to get Falling distance
        this.fallingDistance = Vector3.Distance(this.fallStartLevel , this.transform.position);

        if (!this.Falling)
        {
            this.Falling = true;
            //start tracking altitude (y position) for fall check
            this.fallStartLevel = this.transform.position;

            //check jumpfxstate var to play Jumping sound only once
            if (this.Jumping && this.jumpfxstate)
            {
                //play Jumping sound
				//this.mainPlayerInput.AudioController.PlayJumpSfx(this.mainPlayerInput.CameraController.MainCamera.transform.position);
                this.jumpfxstate = false;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Player Ground Check
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void ApplyPlayerGroundCheck(Vector3 p1, Vector3 p2)
    {
        RaycastHit hit;
        //cast capsule shape down to see if player is about to hit anything or is resting on the ground
        if (
            Physics.CapsuleCast(p1, p2, this.capsule.radius*0.9f, -transform.up, out hit, 0.75f, this.ClipMask.value) ||
            this.Climbing)
        {
            this.Grounded = true;
        }
        else
        {
            this.Grounded = false;
        }
        
        //check that angle of the normal directly below the capsule center point is less than the movement slope limit 
        if (Physics.Raycast(this.transform.position, -transform.up, out hit, 2.6f, this.ClipMask.value))
        {
            if (Vector3.Angle(hit.normal, -this.TargetGravity.normalized) > this.SlopeLimit)
            {
                this.rayTooSteep = true;
                this.steepRayNormal = hit.normal;
            }
            else
            {
                this.rayTooSteep = false;
                this.steepRayNormal = Vector3.zero;
            }
        }
        else
        {
            this.rayTooSteep = false;
        }

    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Player Velocity
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void SetPlayerVelocity(float inputX, float inputY, Vector3 velocity)
    {

        //limit speed if strafing diagonally
        this.limitStrafeSpeed = (inputX != 0.0f && inputY != 0.0f) ? .7071f : 1.0f;

        // Check if the player is sliding
        this.Sliding = this.rayTooSteep && this.Grounded;

        //apply velocity to player rigidbody and allow a small amount of air manipulation
        //to allow Jumping up on obstacles when Jumping from stationary position with no forward velocity
        // todo: the main player check can be removed from here since we just shut down the movement from the player component when the player is dead
        if (((!this.Grounded && this.AirMovementControl) || this.Grounded || this.Climbing ||
             ((this.AirTime + 0.3f) > Time.time)) &&
            this.mainPlayerController.HitPoints > 0)
        {
            // We are grounded, so recalculate movedirection directly from axes	
            Vector3 moveDirection = new Vector3(this.InputXSmoothed * this.limitStrafeSpeed, 0.0f,
                                                this.InputYSmoothed * this.limitStrafeSpeed);

            // Get the movement direction relative to th camera
            Vector3 relativeMoveDirection = this.GetCameraRelativeDirection(moveDirection);

            //apply speed limits to moveDirection vector
            moveDirection = relativeMoveDirection * this.speed * this.speedAmtX * this.speedAmtY * this.crouchSpeedAmt *
                                    this.zoomSpeedAmt;

            // Apply air modifier if in-air
            if (this.AirMovementControl && !this.Grounded && !this.Climbing)
                moveDirection = Vector3.Lerp(velocity, moveDirection, this.AirSpeedMultiplier);

            //Check both capsule center point and capsule base slope angles to determine if the slope is too high to climb.
            //If so, bypass player control and apply some extra downward velocity to help capsule return to more level ground.
            Vector3 slopeSlidingForce = Vector3.zero;
            if (this.Sliding)
            {
                //If slope is too high below both the center and base contact point of capsule, apply some downward velocity to help
                //the capsule fall to more level ground. Check the slope angle at two points on the collider to prevent it from 
                //getting stuck when player control is bypassed and to have more control over the slope angle limit.
                slopeSlidingForce = this.TargetGravity.normalized * this.SlopeGravityMultiplier;

                moveDirection *= this.SlidingSpeedMultiplier;
            }

            //apply a force that attempts to reach target velocity
            Vector3 velocityChange = moveDirection - velocity;

            //limit max speed
            velocityChange.x = Mathf.Clamp(velocityChange.x, -this.maxVelocityChange, this.maxVelocityChange);
            velocityChange.y = Mathf.Clamp(velocityChange.y, -this.maxVelocityChange, this.maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -this.maxVelocityChange, this.maxVelocityChange);

            //disable velocity change on gravity vector
            velocityChange = this.NullifyVerticalMagnitude(velocityChange);

            //apply ladder climbing speed to velocityChange vector and set y velocity to zero if not climbing ladder
            // todo: implement relative gravity version of this
            if (this.Climbing && inputY > 0)
                velocityChange.y = this.climbSpeedAmt;

            // Check if there is a wall in the desired movement direction
            RaycastHit hit;
            if (!Physics.Raycast(this.mainPlayerController.MovementController.transform.position,
                velocityChange.normalized, out hit,
                this.capsule.radius*this.CollisionRadiusMult, this.ClipMask.value) ||
                Vector3.Angle(relativeMoveDirection, -hit.normal) < this.WallSlidingAngleThreshold)
            {
                Debug.DrawRay(this.transform.position, velocity, Color.blue);
                Debug.DrawRay(this.transform.position, slopeSlidingForce, Color.magenta);

                // If we are hitting a wall, neutralize forward force
                if (Vector3.Angle(relativeMoveDirection, -hit.normal) < this.WallSlidingAngleThreshold)
                    velocityChange = this.NullifyCustomMagnitude(velocityChange, -hit.normal);

                //finally, add movement velocity to player rigidbody velocity
                this.rigidBody.AddForce(velocityChange + slopeSlidingForce, ForceMode.VelocityChange);
            }
            else
            {
                // If there is a wall in the desired movement direction, we need to move along it.
                Vector3 wallsliding = Vector3.zero;

                // But first we need to check if the surface is inside the desired threshold
                if (Vector3.Angle(relativeMoveDirection, -hit.normal) > this.WallSlidingAngleThreshold)
                    // Formula: new_direction = (change cross plane_normal) cross plane_normal
                    wallsliding = -Vector3.Cross(Vector3.Cross(relativeMoveDirection, hit.normal), hit.normal).normalized * moveDirection.magnitude;

                //limit max speed
                Vector3 finalWallSliding = wallsliding - velocity;

                //disable velocity change on gravity vector
                finalWallSliding = this.NullifyVerticalMagnitude(finalWallSliding);
                Debug.DrawRay(this.transform.position, wallsliding, Color.red);
                Debug.DrawRay(this.transform.position, velocity, Color.blue);
                Debug.DrawRay(this.transform.position, velocityChange, Color.green);
                Debug.DrawRay(this.transform.position, slopeSlidingForce, Color.magenta);

                //finally, add movement velocity to player rigidbody velocity
                this.rigidBody.AddForce(finalWallSliding + slopeSlidingForce, ForceMode.VelocityChange);
            }
        }

        if (!this.Climbing)
        {
            //apply Gravity manually for more tuning control except when climbing a ladder to avoid unwanted downward movement
            Debug.DrawRay(this.transform.position, this.TargetGravity * this.rigidBody.mass, Color.cyan);
            this.rigidBody.AddForce(this.TargetGravity * this.rigidBody.mass);
        }
    }

    #endregion

    #region Utilities

    private bool CheckForCollidersAbove(Vector3 p1)
    {
        RaycastHit hit;
        return
            Physics.CapsuleCast(p1, p1 + this.capsule.height*this.TargetGravity.normalized, this.capsule.radius*0.9f,
                -this.TargetGravity.normalized, out hit, this.capsule.height, this.ClipMask.value);
    }

    private void ProcessInputDirection(InputInstance inputInstance, ref float inputY, ref float inputX)
    {
        Vector2 inputVector = new Vector2(inputInstance.StrafeRight, inputInstance.MoveForward);

        inputY = inputVector.normalized.y;
        inputX = inputVector.normalized.x;
    }


    private Vector3 GetCameraRelativeDirection(Vector3 moveDirection)
    {

        // Force camera to be parallel to the local XZ plane
		Vector3 tempLocalAngles = this.mainPlayerController.CameraController.transform.localEulerAngles;
		this.mainPlayerController.CameraController.transform.localEulerAngles =
            new Vector3(0,
				this.mainPlayerController.CameraController.transform.localEulerAngles.y,
				this.mainPlayerController.CameraController.transform.localEulerAngles.z);

        // realign moveDirection vector to camera relative space
		moveDirection = this.mainPlayerController.CameraController.transform.TransformDirection(moveDirection);

        //  Restore camera orientation
		this.mainPlayerController.CameraController.transform.localEulerAngles = tempLocalAngles;
        return moveDirection;

    }


    // Nullify the relative Y component of the input vector
    private Vector3 NullifyVerticalMagnitude(Vector3 input)
    {
        input = input - new Vector3(input.x*Mathf.Abs(this.TargetGravity.normalized.x),
                                    input.y*Mathf.Abs(this.TargetGravity.normalized.y),
                                    input.z*Mathf.Abs(this.TargetGravity.normalized.z));
        return input;
    }

    // Nullify the relative component of the input vector against another vector
    private Vector3 NullifyCustomMagnitude(Vector3 input,Vector3 custom)
    {
        input = input - new Vector3(input.x * Mathf.Abs(custom.normalized.x),
                                    input.y * Mathf.Abs(custom.normalized.y),
                                    input.z * Mathf.Abs(custom.normalized.z));
        return input;
    }


    #endregion

    #region Collisions
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Rigidbody Collisions
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void TrackCollision(Collision col)
    {
        //define a height of about a fourth of the capsule height to check for collisions with platforms
        float minimumHeight = (this.capsule.bounds.min.y + this.capsule.radius);
        //check the collision points within our predefined height range  
        foreach (ContactPoint c in col.contacts)
        {
            if (c.point.y < minimumHeight)
            {
                //check that we want to collide with this object (check for "PhysicsObject" tag) and that it's surface is not too steep 
                if (!this.parentState && col.gameObject.tag == "PhysicsObject" && Vector3.Angle(c.normal, -this.TargetGravity.normalized) < 70)
                {
                    //set player object parent to platform transform to inherit it's movement in addition to player movement
                    this.transform.parent = col.transform;
                    this.parentState = true;//only set parent once to prevent rapid parenting and de-parenting that breaks functionality
                }
                //check that angle of the surface that the capsule base is touching is less than the movement slope limit  
                if (Vector3.Angle(c.normal,this.TargetGravity.normalized) > this.SlopeLimit)
                {
                    this.capsuleTooSteep = true;
                }
                else
                {
                    this.capsuleTooSteep = false;
                }
            }
        }

    }

    void OnCollisionExit(Collision col)
    {
        //unparent if we are no longer standing on our parent
        this.transform.parent = this.transform.parent;
        //return parentState to false so we may check for collisions again
        parentState = false;
        capsuleTooSteep = false;
    }

    void OnCollisionStay(Collision col)
    {
        TrackCollision(col);
    }

    void OnCollisionEnter(Collision col)
    {
        TrackCollision(col);
    }
    #endregion

    #region Utilities
    // Function used to limit angles
    private float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
    #endregion
}
