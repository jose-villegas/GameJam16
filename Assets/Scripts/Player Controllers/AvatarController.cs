using System;
using UnityEngine;
using System.Collections;

public class AvatarController : MonoBehaviour {

    // Component references
    private PlayerController mainPlayerController;

    // Possible Avatars
    public Animator HumanAvatar;
    public Animator MonsterAvatar;
    private Animator currentAvatar; 

    // Particle system
    public GameObject MagicalGirlFX;
    public float PFXDuration = 3.0f;

    // Use this for initialization
    public void Initialize (PlayerController playerController) {
        // Store main player component reference
        this.mainPlayerController = playerController;

        // Set initial avatar
        this.ChangePlayerAvatar(this.mainPlayerController.Type);
    }

    #region Avatar Animation

    public void ExecuteParticleSystem()
    {
        this.MagicalGirlFX.SetActive(false);

        StopCoroutine("DelayedPFXDeactivation");
        StartCoroutine("DelayedPFXDeactivation");
    }

    IEnumerator DelayedPFXDeactivation()
    {
        this.MagicalGirlFX.SetActive(true);
        yield return new WaitForSeconds(this.PFXDuration);
        this.MagicalGirlFX.SetActive(false);
    }

    public void SetCrouchAnimation(bool crouch)
    {
        this.currentAvatar.SetBool("Crunch", crouch);
    }

    public void UpdateMovementAnimation(float inputX, float inputY, bool running,bool currentCrouch)
    {
        this.currentAvatar.SetBool("Run",running);

        this.currentAvatar.SetFloat("InputX", inputX);
        this.currentAvatar.SetFloat("InputY", inputY);

        this.SetCrouchAnimation(currentCrouch);
    }

    public void PlayDiveAnimation()
    {
        this.currentAvatar.SetTrigger("Dive");
    }

    public void PlayJumpAnimation()
    {
        this.currentAvatar.SetTrigger("Jump");
    }

    #endregion


    #region Avatar Management
    public void ChangePlayerAvatar(Entity.PlayerType playerType)
    {
        if (this.MonsterAvatar == null || this.HumanAvatar == null)
            return;

        switch (playerType)
        {
            case Entity.PlayerType.Human:
                this.HumanAvatar.gameObject.SetActive(true);
                this.MonsterAvatar.gameObject.SetActive(false);

                this.currentAvatar = this.HumanAvatar;

                this.ExecuteParticleSystem();
                break;
            case Entity.PlayerType.Monster:
                this.HumanAvatar.gameObject.SetActive(false);
                this.MonsterAvatar.gameObject.SetActive(true);

                this.currentAvatar = this.MonsterAvatar;
                break;
        }
    }
    #endregion
}
