using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModTurbo : MonoBehaviour
{
    public float boost = 2500f;
    public Rigidbody carRB;
    private int useCount = 0;
    public modManager modManager;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && useCount< 3)
        {
            var force = carRB.transform.forward * boost;
            carRB.AddForce(force, ForceMode.Acceleration);
            useCount++;
            print("Turbo " + useCount);
        }

        if (useCount == 3)
        {
            StartCoroutine(modManager.ModDrop());
            print("Turbo agotado");
        }
    }
}
