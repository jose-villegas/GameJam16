using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraLightController : MonoBehaviour {

    // Component references
    private PlayerController mainPlayerController;

    public void Initialize(PlayerController playerController)
    {
        // Store references
        this.mainPlayerController = playerController;
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
