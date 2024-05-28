using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class modManager : MonoBehaviour
{
    public Animator slotsAnimator;
    private int opener = 0;

    private int modCount = 0;
    public GameObject[] modGameObjects;
    //public Sprite[] modSprites;   
    //public Image activeMod;

    private void Update()
    {
        Debug.Log(opener.ToString());
        slotController();
    }
    private void slotController()
    {
        if (Input.GetKeyDown(KeyCode.L) && opener<4)
        {
            opener += 1;
            if (opener > 3)
                opener = 1;
            slotsAnimator.SetInteger("Slot", opener);
        }
        if (Input.GetKeyDown(KeyCode.J) && opener > 0)
        {
            opener -= 1;
            if (opener < 1)
                opener = 3;
            slotsAnimator.SetInteger("Slot", opener);
        }
    }

    /*private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Dron")
        {
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            other.gameObject.GetComponent<Animator>().SetBool("Available", false);
        }
    }*/
}
