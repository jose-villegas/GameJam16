using UnityEngine;
using System.Collections;

public class AudioPresenter : MonoBehaviour
{
    // Audio Sources
    public AudioSource SFXAudioSource;

    // Wolf Sfx
    public AudioClip[] WolfSFX = new AudioClip[1];

    // UI Sfx
    public AudioClip[] ClickSFX = new AudioClip[1];

	// Use this for initialization
	public void Initialize () {
	
	}
	
    // Wolf Sfx
    public void PlayHowlSFX()
    {
        this.SFXAudioSource.PlayOneShot(this.WolfSFX[Random.Range(1, this.WolfSFX.Length - 1)]);

    }

    //Menu SFX
    public void PlayClickSFX()
    {
        this.SFXAudioSource.PlayOneShot(this.ClickSFX[Random.Range(1, this.ClickSFX.Length -1 )]);
    }

}
