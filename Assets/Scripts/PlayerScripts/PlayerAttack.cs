using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour, SpellCaster {

    public Transform myBody;
    public List<SpellBook> spellInventory = new List<SpellBook>();
    int currSpell;

    public Image heldWeaponSlot;
    public Image hoverWeaponSlot;
    public Transform hoverSpellDetails;
    public SpellBook touchedSpell;

    public List<damageable> seducedObjects = new List<damageable>();
    public Coroutine seductionEffect;

    public delegate void HitTarget(List<damageable> targets, SpellCaster owner);
    public event HitTarget OnHitTarget;

    // Use this for initialization
    void Start () {
        currSpell = 0;
	}
	
	// Update is called once per frame
	void Update () {
        mouseScroll();
        keyInputs();
        if (Input.GetMouseButtonDown(0) && spellInventory.Count > 0)
        {
            spellInventory[currSpell].primaryCast();
            spellInventory[currSpell].useAmmo();
            updateCurrSpellInfo();
        }
	}

    void mouseScroll()
    {
        if (spellInventory.Count > 0)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                currSpell++;
                if (currSpell >= spellInventory.Count)
                {
                    currSpell = 0;
                }
                updateCurrSpellInfo();
            }
            else if (scroll < 0f)
            {
                currSpell--;
                if (currSpell < 0)
                {
                    currSpell = spellInventory.Count - 1;
                }
                updateCurrSpellInfo();
            }
        }
    }

    void keyInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && spellInventory.Count > 0)
        {
            currSpell = 0;
            updateCurrSpellInfo();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && spellInventory.Count > 1)
        {
            currSpell = 1;
            updateCurrSpellInfo();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && spellInventory.Count > 2)
        {
            currSpell = 2;
            updateCurrSpellInfo();
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        /*
        if (coll.GetComponent<SpellBook>() != null)
        {
            SpellBook touchSpell = coll.GetComponent<SpellBook>();
            hoverWeaponSlot.gameObject.SetActive(true);
            hoverWeaponSlot.transform.Find("Spell Effect").GetComponent<Text>().text =
                touchSpell.spellEffectDescription;
            hoverWeaponSlot.transform.Find("Side Effect").GetComponent<Text>().text =
                touchSpell.sideEffectDescription;
        }
        */
        if (coll.GetComponent<SpellBook>() != null)
        {
            touchedSpell = coll.GetComponent<SpellBook>();
            // hoverSpellDetails = touchedSpell.createSpellDetails(transform);
            // hoverSpellDetails.GetComponent<RectTransform>().forward = transform.forward;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if(coll.GetComponent<SpellBook>() == touchedSpell && touchedSpell != null)
        {
            touchedSpell.hovered();
        }
        if (Input.GetMouseButtonDown(1) && coll.GetComponent<Interactable>() != null)
        {
            Interactable interact = coll.GetComponent<Interactable>();
            interact.PickUp(transform, myBody, myBody.GetComponent<Fighter>(), myBody.GetComponent<Damageable>());
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.GetComponent<SpellBook>() == touchedSpell)
        {
            if(touchedSpell != null) {
                touchedSpell.destroySpellDetails();
            }
            if(hoverSpellDetails != null) { hoverSpellDetails = null; }
            // SpellBook touchSpell = coll.GetComponent<SpellBook>();
            // hoverWeaponSlot.gameObject.SetActive(false);
        }
    }

    public void AddSpell(SpellBook newSpell)
    {
        if (spellInventory.Count == 3)
        {
            SpellBook temp = spellInventory[currSpell];
            spellInventory[currSpell] = newSpell;
            temp.Drop();
        }
        else
        {
            spellInventory.Add(newSpell);
            currSpell = spellInventory.Count - 1;
            updateCurrSpellInfo();
        }
        updateCurrSpellInfo();
        hoverWeaponSlot.gameObject.SetActive(false);
    }

    public void DropSpell(SpellBook dropSpell)
    {
        spellInventory.Remove(dropSpell);
        dropSpell.Drop();
        updateCurrSpellInfo();
    }

    void updateCurrSpellInfo()
    {
        if (spellInventory.Count == 0)
        {
            currSpell = 0;
            heldWeaponSlot.transform.Find("Title").GetComponent<Text>().text = "Nothing held";
            heldWeaponSlot.transform.Find("Spell Effect").GetComponent<Text>().text = "-None";
            heldWeaponSlot.transform.Find("Side Effect").GetComponent<Text>().text = "-None";
            heldWeaponSlot.transform.Find("Ammo").GetComponent<Text>().text = "0/0";
            return;
        }
        if (currSpell >= spellInventory.Count) { currSpell = spellInventory.Count - 1; }
        heldWeaponSlot.transform.Find("Spell Effect").GetComponent<Text>().text =
            spellInventory[currSpell].spellEffectDescription;
        heldWeaponSlot.transform.Find("Side Effect").GetComponent<Text>().text =
            spellInventory[currSpell].sideEffectDescription;
        heldWeaponSlot.transform.Find("Ammo").GetComponent<Text>().text = spellInventory[currSpell].getAmmo() + "/"
            + spellInventory[currSpell].maxAmmo;
    }

    public void addSeductionObject(damageable loser)
    {
        seducedObjects.Add(loser);
    }

    public void removeFromSeductionList(damageable loser)
    {
        seducedObjects.Remove(loser);
        // OnHitTarget -= loser.setCurrentTarget;
    }

    public void addToSeductionList(damageable loser)
    {
        /*
        if (!seducedObjects.Contains(loser))
        {
            seducedObjects.Add(loser);
            OnHitTarget += loser.setCurrentTarget;
        }
        */
    }

    public void initiateSeduction(float duration)
    {
        /*
        if(seductionEffect == null) { seductionEffect = StartCoroutine(processSeducedObjects(duration)); }
        Debug.Log("Seducing!");
        */
    }

    public void getHitList(List<damageable> hitList, SpellCaster owner)
    {
        if(OnHitTarget != null)
        {
            OnHitTarget(hitList, owner);
        }
    }
}
