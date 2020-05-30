using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteTaking : MonoBehaviour
{

    public GameObject note;


    // Start is called before the first frame update
    void Start()
    {
        note.SetActive(false);
    }

    public void Display()
    {
        note.SetActive(true);
    }
}
