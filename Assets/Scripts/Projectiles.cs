using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class misil : MonoBehaviour
{
    public GameObject explosionEffect;
    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionEffect, collision.contacts[0].point, Quaternion.identity);
        if(collision.gameObject.CompareTag("Destruible"))
        {
            Destroy(collision.gameObject);
        }
        Destroy(this.gameObject);
    }
}
