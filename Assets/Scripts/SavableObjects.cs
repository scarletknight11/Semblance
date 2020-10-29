using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ObjectType {Crage, Mushroom, Stone, male_suit}


public abstract class SavableObjects : MonoBehaviour {

    protected string save;

    private ObjectType objectType;

    // Start is called before the first frame update

    private void Start()
    {
        SaveGameManager.Instance.SaveableObject.Add(this);
    }


    public virtual void Save(int id)
    {
        PlayerPrefs.SetString(id.ToString(), transform.position.ToString());
    }

    public virtual void Load(string[] values)
    {

    }

    public void DestroySaveable()
    {

    }

}
