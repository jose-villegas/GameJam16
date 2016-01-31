using UnityEngine;
using System.Collections;
using System.Linq.Expressions;

public class PlayerCameraController : MonoBehaviour {

    // Component references
    private CameraLightController mainCamera;
    public Camera UiCamera;
    private PlayerController mainPlayerController;

    // General camera speeds
    [Range(0.5f, 8.0f)]
    public float SensitivityY = 4.0f;
    [Range(0.5f, 8.0f)]
    public float SensitivityX = 2.0f;
    public float SmoothSpeed = 0.35f;

    // Camera collisions
    public LayerMask CollisionLayerMask;
    [Range(0,1)]
    public float CollisionThreshold = 0.1f;
    private float originalCameraDistance;

    // Camera control parameters (rotation)
    private float rotationY = 0.0f;
    private Quaternion originalLocalRotation;

    // Rotation constraints
    [Range(-90, 90)]
    public float MinimumY = -60f;
    [Range(-90, 90)]
    public float MaximumY = 60f;

    // Use this for initialization
    public void Initialize (PlayerController playerController, Vector4 cameraConfiguration) {
        // Store main player component reference
        this.mainCamera = this.GetComponentInChildren<CameraLightController>();
        this.mainPlayerController = playerController;

        // Set camera configuration
        Camera camera = this.mainCamera.GetComponentInChildren<Camera>();
        camera.rect = new Rect(cameraConfiguration.x, cameraConfiguration.y, cameraConfiguration.z, cameraConfiguration.w);
        UiCamera.rect = camera.rect;

        // Initialize light camera
        this.mainCamera.Initialize(playerController);

        // Store variables
        this.originalLocalRotation = this.transform.localRotation;

        // Store camera original distance
        this.originalCameraDistance = Vector3.Distance(this.transform.parent.position, this.mainCamera.transform.position);
    }


    // Update is called once per frame
    public void UpdateCamera(InputInstance inputInstance)
    {
        // Execute camera rotation
        this.ExecuteCameraVerticalRotation(inputInstance);

        // Execute camera collisions
        this.ExecuteCameraCollisions();

    }

    private void ExecuteCameraCollisions()
    {
        // Get collision direction
        Vector3 collisionDirection = this.mainCamera.transform.position - this.transform.position;

        // Detect camera collisions
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, collisionDirection.normalized);
        if (Physics.SphereCast(ray, this.CollisionThreshold, out hit,
            this.originalCameraDistance, this.CollisionLayerMask))
        {
            // Set collision distance
            float collisionDistance = Vector3.Distance(this.transform.parent.position, hit.point);

            // Set new camera position
            this.mainCamera.transform.position = this.transform.position +
                                                 collisionDirection.normalized*
                                                 collisionDistance;
        }
        else
        {
            // Set regular camera position
            this.mainCamera.transform.position = this.transform.position +
                                                 collisionDirection.normalized*
                                                 originalCameraDistance;
        }
    }

    private void ExecuteCameraVerticalRotation(InputInstance inputInstance)
    {
        // Get camera vertical rotation
        this.rotationY += inputInstance.VerticalLook*this.SensitivityY*Time.timeScale;
        this.rotationY = this.ClampAngle(this.rotationY, MinimumY, MaximumY);
        Quaternion yQuaternion = Quaternion.AngleAxis(this.rotationY, -Vector3.right);

        // Apply smoothed rotation on Y
        Quaternion localEulerRotation = Quaternion.Slerp(this.transform.localRotation,
            this.originalLocalRotation*yQuaternion,
            this.SmoothSpeed*Time.smoothDeltaTime*60/Time.timeScale);

        // Apply final camera rotation
        this.transform.localRotation = localEulerRotation;
    }

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
