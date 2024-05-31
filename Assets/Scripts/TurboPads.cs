using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboPads : MonoBehaviour
{
    public float boost;
    //public Rigidbody carRB;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var force = other.transform.forward * boost;
            other.GetComponent<Rigidbody>().AddForce(force, ForceMode.Acceleration);
        }
    }
}
