using System.Collections;
using UnityEngine;

public class modManager : MonoBehaviour
{
    public Animator slotsAnimator;
    private int opener = 0;

    public int modCount = 0;
    public GameObject[] modGameObjects;
    //public Sprite[] modSprites;   
    //public Image activeMod1;
    //public Image activeMod2;
    //public Image activeMod3;
    private int currentMod = 0;
    private int[] ownedMods;
    [Range(0, 2)] private int owIndex = 0;
    [Range(0, 2)] private int maxIndex = 0;

    #region Unity functions

    private void Start()
    {
        
        ownedMods = new int[3];
    }

    private void Update()
    {
        
        if (modCount > 0 && Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(ModDrop());
        }
        if (modCount > 1)
        {
            Debug.Log("Selector Activo");
            StartCoroutine(ModSelector());
        }
            
    }

    #endregion

    #region Mods Interactions

    public void SlotController()
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
            modGameObjects[currentMod].gameObject.SetActive(false);
            owIndex++;
            currentMod = ownedMods[owIndex];
            SlotController();
            yield return new WaitForSeconds(1);
            modGameObjects[currentMod].gameObject.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.L) && owIndex == maxIndex)
        {
            modGameObjects[currentMod].gameObject.SetActive(false);
            owIndex=0;
            currentMod = ownedMods[owIndex];
            SlotController();
            yield return new WaitForSeconds(1);
            modGameObjects[currentMod].gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.J) && owIndex > 0)
        {
            modGameObjects[currentMod].gameObject.SetActive(false);
            owIndex--;
            currentMod = ownedMods[owIndex];
            SlotController();
            yield return new WaitForSeconds(1);
            modGameObjects[currentMod].gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.J) & owIndex == 0)
        {
            modGameObjects[currentMod].gameObject.SetActive(false);
            owIndex=maxIndex;
            currentMod = ownedMods[owIndex];
            SlotController();
            yield return new WaitForSeconds(1);
            modGameObjects[currentMod].gameObject.SetActive(true);
        }
    }

    public IEnumerator ModDrop()
    {
        if (modCount == 1)
        {
            modGameObjects[currentMod].gameObject.SetActive(false);
            ownedMods[(modCount - 1)] = 0;
            opener = 0;
            slotsAnimator.SetInteger("Slot", opener);
        }
        else if (modCount == 2) { 
            if (owIndex == maxIndex)
            {
                modGameObjects[currentMod].gameObject.SetActive(false);
                ownedMods[maxIndex] = 0;
                owIndex--;
                maxIndex--;
                currentMod = ownedMods[owIndex];
                SlotController();
                yield return new WaitForSeconds(1);
                modGameObjects[currentMod].gameObject.SetActive(true);
            }
            else if(owIndex == 0)
            {
                modGameObjects[currentMod].gameObject.SetActive(false);
                currentMod = ownedMods[owIndex +1];
                maxIndex--;
                SlotController();
                //ownedMods[owIndex+1] = 0;
                ownedMods[owIndex] = currentMod;
                yield return new WaitForSeconds(1);
                modGameObjects[currentMod].gameObject.SetActive(true);
            }
        }
        else if (modCount == 3)
        {
            if (owIndex == maxIndex)
            {
                modGameObjects[currentMod].gameObject.SetActive(false);
                ownedMods[maxIndex] = 0;
                owIndex--;
                maxIndex--;
                currentMod = ownedMods[owIndex];
                SlotController();
                yield return new WaitForSeconds(1);
                modGameObjects[currentMod].gameObject.SetActive(true);
            }
            else if (owIndex == 1)
            {

                modGameObjects[currentMod].gameObject.SetActive(false);
                currentMod = ownedMods[owIndex + 1];
                SlotController();
                maxIndex--;
                //ownedMods[owIndex + 1] = 0;
                ownedMods[owIndex] = currentMod;
                yield return new WaitForSeconds(1);
                modGameObjects[currentMod].gameObject.SetActive(true);
            }
            else if (owIndex == 0)
            {
                modGameObjects[currentMod].gameObject.SetActive(false);
                currentMod = ownedMods[owIndex + 1];
                SlotController();
                ownedMods[owIndex] = currentMod;
                ownedMods[owIndex+1] = ownedMods[owIndex+2];
                //ownedMods[owIndex + 2] = 0;
                maxIndex--;
                yield return new WaitForSeconds(1);
                modGameObjects[currentMod].gameObject.SetActive(true);
            }
        }
        print("owIndex = " + owIndex);
        modCount--;
        print("Mod dropeado, cuenta " + modCount);
    }

    public IEnumerator ModAsign()
    {
        if (modCount == 0)
        {
            ownedMods[modCount] = Random.Range(0, modGameObjects.Length);
            currentMod = ownedMods[modCount];
            Debug.Log("Asignado 1");
            SlotController();
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

    #endregion


    #region Dron Collider Interactor

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dron"))
        {
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            other.gameObject.GetComponent<Animator>().SetBool("Available", false);
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
    }
    #endregion
}

