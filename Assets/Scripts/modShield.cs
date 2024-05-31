using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modShield : MonoBehaviour
{
    public GameObject shield;
    private bool toggleShield = false;
    private int useCount = 0;
    public modManager modManager;
    private void Update()
    {
        if (toggleShield == false && Input.GetKeyDown(KeyCode.I) && useCount<3)
        {
            toggleShield = true;
            shield.SetActive(true);
            useCount++;
            StartCoroutine(ShieldDuration(toggleShield));
        }
        if( toggleShield == true && Input.GetKeyDown(KeyCode.M)){
            toggleShield = false;
            shield.SetActive(false);
        }
        if (useCount == 3 && toggleShield == false)
        {
            StartCoroutine(modManager.ModDrop());
        }
    }

    private IEnumerator ShieldDuration(bool active)
    {
        print("entra corutina");
        if (active == true)
        {
            yield return new WaitForSeconds(5);
            toggleShield = false;
            shield.SetActive(false);
            print("Escudo agotado");
        }
    }
}
