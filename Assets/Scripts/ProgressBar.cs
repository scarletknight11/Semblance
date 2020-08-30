using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

    public GameObject bar;
   
    public void Progress()
    {
        bar.SetActive(true);
    }

    public void CloseProgress()
    {
        bar.SetActive(false);
    }

}
