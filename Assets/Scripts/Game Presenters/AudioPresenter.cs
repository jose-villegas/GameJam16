using UnityEngine;
using System.Collections;

public class AudioPresenter : MonoBehaviour
{
    // Audio Sources
    public AudioSource SFXAudioSource;

    // UI Sfx
    public AudioClip[] ClickSFX = new AudioClip[1];

	// Use this for initialization
	public void Initialize () {
	
	}
	

    //Menu SFX
    public void PlayClickSFX()
    {
        this.SFXAudioSource.PlayOneShot(this.ClickSFX[Random.Range(1, this.ClickSFX.Length -1 )]);
    }

}
