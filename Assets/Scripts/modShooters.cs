using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float force;
    public GameObject launcher;
    private GameObject tmpprojectile;
    public modManager modManager;
    private int useCount=0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && useCount < 3)
        {
            Launch();
            useCount++;
        }

        if (useCount == 3)
        {
            StartCoroutine(modManager.ModDrop());
        }
    }

    void Launch()
    {
        tmpprojectile = Instantiate(projectilePrefab, launcher.transform.position, Quaternion.identity);
        tmpprojectile.transform.forward = launcher.transform.forward;
        tmpprojectile.GetComponent<Rigidbody>().AddForce(launcher.transform.forward * force, ForceMode.Impulse);
    }
}
