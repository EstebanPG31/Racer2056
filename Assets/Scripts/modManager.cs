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
    private int[] ownedMods;
    private int owIndex = 0;
    private int maxIndex = 0;
    /*private int ownedMod1 = 10;
    private int ownedMod2 = 20;
    private int ownedMod3 = 30;*/


    private void Start()
    {
        ownedMods = new int[3];
    }

    private void Update()
    {
        
        if (modCount > 0)
        {
            slotController();
            
        }
        if (modCount > 1)
        {
            Debug.Log("Selector Activo");
            StartCoroutine(ModSelector());
        }
            
    }
    public void slotController()
    {
        if (0 <= currentMod && currentMod <2)
        {
            opener = 1;
            slotsAnimator.SetInteger("Slot", opener);
        }
        else if (currentMod == 2)
        {
            opener = 2;
            slotsAnimator.SetInteger("Slot", opener);
        }
        else
        {
            opener = 3;
            slotsAnimator.SetInteger("Slot", opener);
        }
    }

    public IEnumerator ModSelector()
    {
        Debug.Log("indice "+owIndex);
        if (Input.GetKeyDown(KeyCode.L) && owIndex < maxIndex)
        {
            Debug.Log("indice +1");
            modGameObjects[currentMod].gameObject.SetActive(false);
            owIndex++;
            currentMod = ownedMods[owIndex];
            slotController();
            yield return new WaitForSeconds(1);
            modGameObjects[currentMod].gameObject.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.L) && owIndex == maxIndex)
        {
            modGameObjects[currentMod].gameObject.SetActive(false);
            Debug.Log("indice maxeado a 0");
            owIndex=0;
            currentMod = ownedMods[owIndex];
            slotController();
            yield return new WaitForSeconds(1);
            modGameObjects[currentMod].gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.J) && owIndex > 0)
        {
            Debug.Log("indice --");
            modGameObjects[currentMod].gameObject.SetActive(false);
            owIndex--;
            currentMod = ownedMods[owIndex];
            slotController();
            yield return new WaitForSeconds(1);
            modGameObjects[currentMod].gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.J) & owIndex == 0)
        {
            Debug.Log("indice dropeado a maximo");
            modGameObjects[currentMod].gameObject.SetActive(false);
            owIndex=maxIndex;
            currentMod = ownedMods[owIndex];
            slotController();
            yield return new WaitForSeconds(1);
            modGameObjects[currentMod].gameObject.SetActive(true);
        }
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
        else
        {
            Debug.Log("No se asigna nuevo mod");
        }

        yield return new WaitForSeconds(5);
        other.gameObject.GetComponent<Animator>().SetBool("Available", true);
        other.gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    public IEnumerator ModAsign()
    {
        if (modCount == 0)
        {
            ownedMods[modCount] = Random.Range(0, modGameObjects.Length);
            currentMod = ownedMods[modCount];
            Debug.Log("Asignado 1");
            slotController();
            //activeMod1.sprite = modSprites[ownedMod1]
            yield return new WaitForSeconds(1);
            modGameObjects[currentMod].SetActive(true);
        }
        else if (modCount == 1)
        {
            ownedMods[modCount] = Random.Range(0, modGameObjects.Length);
            Debug.Log("Asignados 2 ");
            //activeMod2.sprite = modSprites[ownedMod2]
            yield return new WaitForSeconds(1);
        }
        else if(modCount == 2)
        {
            ownedMods[modCount] = Random.Range(0, modGameObjects.Length);
            Debug.Log("Asignados 3 ");
            //activeMod3.sprite = modSprites[ownedMod3]
            yield return new WaitForSeconds(1);
        }
        maxIndex = modCount;
        Debug.Log("maxIndex = " + maxIndex);
        modCount++;
    }
}
