using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class SpecificObject : SavableObjects {

    private float speed;

    private float strength;



    // Update is called once per frame
    void Update() {
        GameObject gm = null;

        gm.GetComponent<SaveGameManager>().Save();
    }

    public override void Save(int id)
    {
        base.Save(id);
    }

    public override void Load(string[] values)
    {
        base.Load(values);
    }

}
