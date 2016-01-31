using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

    // Components
    private AudioSource audioSource;

    // Walking/ Running
    public float MovementThreshold = 0.1f;

    // Audio Clips
    public AudioClip WalkSFX;
    public AudioClip RunSFX;

	// Use this for initialization
	public void Initialize ()
	{
	    this.audioSource = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
    public void UpdateMovementInputSFX(float inputX, float inputY, bool running)
    {
        if (inputX > this.MovementThreshold || inputY > this.MovementThreshold)
        {
            this.audioSource.clip = !running ? this.WalkSFX : this.RunSFX;
        }
        else
        {
            this.audioSource.clip = null;

        }

        this.audioSource.loop = true;
        this.audioSource.Play();
    }

}
