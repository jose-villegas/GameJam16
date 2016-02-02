using UnityEngine;
using System.Collections;

public class AudioPresenter : MonoBehaviour
{
    // Audio Sources
    public AudioSource SFXAudioSource;

    // Random creepy noises
    public AudioClip[] CreepySFX = new AudioClip[1];
    public float MinCreepy = 5.0f;
    public float MaxCreepy = 15.0f;

    // Wolf Sfx
    public AudioClip[] WolfSFX = new AudioClip[1];

    // Human SFX
    public AudioClip[] HumanCapturedSFX = new AudioClip[1];

    // UI Sfx
    public AudioClip[] ClickSFX = new AudioClip[1];

    // Narration
    public AudioSource NarrationSFX;

	// Use this for initialization
	public void Initialize (bool enableCreepy = false)
	{
	    if (enableCreepy)
	        StartCoroutine("CreepynessSFX");
	}

    IEnumerator CreepynessSFX()
    {
        Debug.Log(this.CreepySFX.Length);
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(this.MinCreepy, this.MaxCreepy));

            this.SFXAudioSource.PlayOneShot(this.CreepySFX[Random.Range(0, this.CreepySFX.Length - 1)]);
        }
    }

    public void ManageNarrationSFX(bool state)
    {
        if(state)
            this.NarrationSFX.Play();
        else
            this.NarrationSFX.Stop();
    }

    public void PlayCapturedHumanSFX()
    {
        this.SFXAudioSource.PlayOneShot(this.HumanCapturedSFX[Random.Range(0, this.HumanCapturedSFX.Length - 1)]);

    }

    // Wolf Sfx
    public void PlayHowlSFX()
    {
        this.SFXAudioSource.PlayOneShot(this.WolfSFX[Random.Range(0, this.WolfSFX.Length - 1)]);

    }

    //Menu SFX
    public void PlayClickSFX()
    {
        this.SFXAudioSource.PlayOneShot(this.ClickSFX[Random.Range(0, this.ClickSFX.Length -1 )]);
    }

}
