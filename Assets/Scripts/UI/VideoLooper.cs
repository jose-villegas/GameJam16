using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VideoLooper : MonoBehaviour
{
    public RawImage Image;

	// Use this for initialization
	void Start () {
        var video = (MovieTexture)this.Image.mainTexture;
        video.loop = true;
        video.Play();
	}
	

}
