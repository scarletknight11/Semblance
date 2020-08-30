using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour {

   public void AvatarDisplay()
    {
        SceneManager.LoadScene("Visual Avatar");
    }
    public void AvatarScan()
    {
        SceneManager.LoadScene("08_fullbody_legacy_sample_cloud");
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
