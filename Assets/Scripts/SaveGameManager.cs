using System.Collections;
using System.Collections.Generic;
//using Boo.Lang;
using UnityEngine;

public class SaveGameManager : MonoBehaviour {

    private static SaveGameManager instance;

    public List<SavableObjects> SaveableObject { get; private set; }

    public static SaveGameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<SaveGameManager>();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Awake() {
        SaveableObject = new List<SavableObjects>();
    }

    public void Save() {

        PlayerPrefs.SetInt("ObjectCount", SaveableObject.Count);

        for (int i = 0; i < SaveableObject.Count; i++)
        {
            SaveableObject[i].Save(i);
        }
    }

    public void Load() {
        int objectCount = PlayerPrefs.GetInt("ObjectCount");
        for (int i = 0; i < objectCount; i++)
        {
            string value = PlayerPrefs.GetString(i.ToString());
            Debug.Log(value);
        }
    }

    public Vector3 StringToVector(string value) {
        return Vector3.zero;
    }

    public Quaternion StringToQuaternion(string value)
    {
        return Quaternion.identity;
    }

}
