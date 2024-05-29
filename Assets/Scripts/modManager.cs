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
    //public Image activeMod1;
    //public Image activeMod2;
    //public Image activeMod3;
    private int currentMod = 0;
    private int ownedMod1 = 10;
    private int ownedMod2 = 20;
    private int ownedMod3 = 30;

    private void Update()
    {
        Debug.Log(opener.ToString());
        if (modCount > 0)
        {
            slotController();
        }
            
    }
    public void slotController()
    {
        //opener = modGameObjects[currentMod].GetComponent
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Dron")
        {
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            other.gameObject.GetComponent<Animator>().SetBool("Available", false);
        }
        if (modCount < 3)
        {
            StartCoroutine(ModAsign());
        }

        yield return new WaitForSeconds(1);
        other.gameObject.GetComponent<Animator>().SetBool("Available", true);
        other.gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    public IEnumerator ModAsign()
    {
        if (modCount == 0)
        {
            ownedMod1 = Random.Range(0, modGameObjects.Length);
            currentMod = ownedMod1;
            //activeMod1.sprite = modSprites[ownedMod1]
            yield return new WaitForSeconds(1);
            modGameObjects[currentMod].SetActive(true);
        }
        else if (modCount == 1)
        {
            ownedMod2 = Random.Range(0, modGameObjects.Length);
            //activeMod2.sprite = modSprites[ownedMod2]
            yield return new WaitForSeconds(1);
        }
        else if(modCount == 2)
        {
            ownedMod3 = Random.Range(0, modGameObjects.Length);
            //activeMod3.sprite = modSprites[ownedMod3]
            yield return new WaitForSeconds(1);
        }
        modCount++;
    }
}
