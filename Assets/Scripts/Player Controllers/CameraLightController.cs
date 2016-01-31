using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(Camera))]
public class CameraLightController : MonoBehaviour {

    // Component references
    private PlayerController mainPlayerController;

    // Camera height
    public float HumanHeight = 1.0f;
    public float MonsterHeight = 2.5f;

    // Noise effect
    public NoiseAndGrain NoiseVFX;
    [Range(0,10)]
    public float MaxNoise = 9.0f;

    public float MaxNoiseRange = 15.0f;
    public float MinNoiseRange = 3.0f;

    public void Initialize(PlayerController playerController)
    {
        // Store references
        this.mainPlayerController = playerController;
    }

    // Set special vfxs
    void Update()
    {
        switch (this.mainPlayerController.Type)
        {
            case Entity.PlayerType.Human:
                this.transform.localPosition = new Vector3(this.transform.localPosition.x,this.HumanHeight, this.transform.localPosition.z);

                this.ManageNoiseVFX();
                break;
            case Entity.PlayerType.Monster:
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.MonsterHeight, this.transform.localPosition.z);

                this.ManageNoiseVFX(false);
                break;
        }
    }

    // Manage noise vfx
    private void ManageNoiseVFX(bool useVFX = true)
    {
        this.NoiseVFX.enabled = useVFX;

        // Manage noise intensity
        if (this.NoiseVFX.enabled)
        {
            // Get closest player
            PlayerController closestMonsterController =
                GamePresenter.Instance.PlayerPresenter.GetClosestMonsterPlayer(this.mainPlayerController);

            // If there is no close player, disable the effect
            if (closestMonsterController == null)
            {
                this.NoiseVFX.intensityMultiplier = 0;
                return;
            }

            // Check if the distance is within the target distance
            float currentDistance = Vector3.Distance(this.mainPlayerController.transform.position,
                closestMonsterController.transform.position);

            // Set final noise intensity
            this.NoiseVFX.intensityMultiplier = this.MaxNoise * Mathf.Lerp(1,0, currentDistance / MaxNoiseRange);
        }

    }

    void OnPreCull()
    {
        if (this.mainPlayerController == null)
            return;

        switch (this.mainPlayerController.Type)
        {
            case Entity.PlayerType.Human:
                break;
            case Entity.PlayerType.Monster:
                // If monster, disable human lights
                foreach (var light in GamePresenter.Instance.EnvironmentPresenter.HumanLights)
                    light.enabled = false;
                break;
        }
    }

    void OnPostRender()
    {
        // Reenable disabled lights
        foreach (var light in GamePresenter.Instance.EnvironmentPresenter.HumanLights)
        {
            light.enabled = true;
        }
    }
}
