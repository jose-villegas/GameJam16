using UnityEngine;
using System.Collections;

public class PlayerCameraController : MonoBehaviour {

    // Component references
    private PlayerController mainPlayerController;

    // General camera speeds
    [Range(0.5f, 8.0f)]
    public float SensitivityY = 4.0f;
    [Range(0.5f, 8.0f)]
    public float SensitivityX = 2.0f;
    public float SmoothSpeed = 0.35f;

    // Camera control parameters (rotation)
    private float rotationY = 0.0f;
    private Quaternion originalLocalRotation;

    // Rotation constraints
    [Range(-90, 90)]
    public float MinimumY = -60f;
    [Range(-90, 90)]
    public float MaximumY = 60f;


    // Use this for initialization
    public void Initialize (PlayerController playerController) {
        // Store main player component reference
        this.mainPlayerController = playerController;

        // Store variables
        this.originalLocalRotation = this.transform.localRotation;
    }

    // Update is called once per frame
    public void UpdateCamera(InputInstance inputInstance)
    {
        // Get camera vertical rotation
        this.rotationY += inputInstance.VerticalLook   * this.SensitivityY * Time.timeScale;
        this.rotationY = this.ClampAngle(this.rotationY, MinimumY, MaximumY);
        Quaternion yQuaternion = Quaternion.AngleAxis(this.rotationY, -Vector3.right);

        // Apply smoothed rotation on Y
        Quaternion localEulerRotation = Quaternion.Slerp(this.transform.localRotation,
                                                     this.originalLocalRotation * yQuaternion,
                                                     this.SmoothSpeed * Time.smoothDeltaTime * 60 / Time.timeScale);
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
