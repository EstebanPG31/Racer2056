using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modBumpers : MonoBehaviour
{
    public float push = 1500f;
    public Collider bumper;
    //public Collider rbumper;

    
    private void OnTriggerEnter(Collider other)
    { 
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.right * push, ForceMode.Acceleration);
            hitCount();
        }
        
    }

    private void hitCount()
    {
        GameObject mod = transform.parent.gameObject;
        modBumpersManager parentScript = mod.GetComponent<modBumpersManager>();
        parentScript.hits++;
    }
}
