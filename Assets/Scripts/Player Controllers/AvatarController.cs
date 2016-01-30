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

    // Use this for initialization
    public void Initialize (PlayerController playerController) {
        // Store main player component reference
        this.mainPlayerController = playerController;
    }

    #region Avatar Animation
    public void PlayDiveAnimation()
    {
        // todo:
    }

    public void PlayJumpAnimation()
    {
        // todo:
    }

    #endregion


    #region Avatar Management
    public void ChangePlayerAvatar(Entity.PlayerType playerType)
    {
        switch (playerType)
        {
            case Entity.PlayerType.Human:
                this.HumanAvatar.gameObject.SetActive(true);
                this.MonsterAvatar.gameObject.SetActive(false);

                this.currentAvatar = this.HumanAvatar;
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
