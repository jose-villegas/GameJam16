using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Scene loading
    public string MainSceneString = "MainMenu";
    public string GameSceneString = "GameScene";

    // Load main scene
    public void LoadMainScene()
    {
        SceneManager.LoadScene(this.MainSceneString);
    }

    // Load game scene
    public void LoadGameScene()
    {
        SceneManager.LoadScene(this.GameSceneString);
    }
}
