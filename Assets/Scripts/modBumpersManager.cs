using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modBumpersManager : MonoBehaviour
{
    public int hits = 0;
    public modManager modManager;

    // Update is called once per frame
    void Update()
    {
        print("hits = " + hits);
        if (hits == 3)
        {
            StartCoroutine(modManager.ModDrop());
        }
    }
}
