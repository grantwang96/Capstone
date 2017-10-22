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

    public List<Damageable> seducedObjects = new List<Damageable>();
    public Coroutine seductionEffect;

    public delegate void HitTarget(List<Damageable> targets, SpellCaster owner);
    public event HitTarget OnHitTarget;

    // Use this for initialization
    void Start () {
        currSpell = 0;
	}
	
	// Update is called once per frame
	void Update () {
        mouseScroll();
        if (Input.GetMouseButtonDown(0) && spellInventory.Count > 0)
        {
            spellInventory[currSpell].primaryCast();
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
            hoverSpellDetails = touchedSpell.createSpellDetails(transform);
            hoverSpellDetails.GetComponent<RectTransform>().forward = transform.forward;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if(coll.GetComponent<SpellBook>() == touchedSpell && hoverSpellDetails != null)
        {
            hoverSpellDetails.GetComponent<RectTransform>().forward = transform.forward;
        }
        if (Input.GetKeyDown(KeyCode.E) && coll.GetComponent<Interactable>() != null)
        {
            Interactable interact = coll.GetComponent<Interactable>();
            interact.PickUp(transform, myBody.GetComponent<Fighter>(), myBody.GetComponent<Damageable>());
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.GetComponent<SpellBook>() == touchedSpell)
        {
            Debug.Log("Goodbye");
            touchedSpell.destroySpellDetails();
            if(hoverSpellDetails != null) { hoverSpellDetails = null; }
            // SpellBook touchSpell = coll.GetComponent<SpellBook>();
            // hoverWeaponSlot.gameObject.SetActive(false);
        }
    }

    public void AddSpell(SpellBook newSpell)
    {
        if(spellInventory.Count == 3)
        {
            SpellBook temp = spellInventory[currSpell];
            spellInventory[currSpell] = newSpell;
            temp.Drop();
        }
        else { spellInventory.Add(newSpell); }
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
        heldWeaponSlot.transform.Find("Spell Effect").GetComponent<Text>().text =
            spellInventory[currSpell].spellEffectDescription;
        heldWeaponSlot.transform.Find("Side Effect").GetComponent<Text>().text =
            spellInventory[currSpell].sideEffectDescription;
    }

    public void addSeductionObject(Damageable loser)
    {
        seducedObjects.Add(loser);
    }

    public void removeFromSeductionList(Damageable loser)
    {
        seducedObjects.Remove(loser);
        OnHitTarget -= loser.setCurrentTarget;
    }

    public void addToSeductionList(Damageable loser)
    {
        if (!seducedObjects.Contains(loser))
        {
            seducedObjects.Add(loser);
            OnHitTarget += loser.setCurrentTarget;
        }
    }

    public void initiateSeduction(float duration)
    {
        /*
        if(seductionEffect == null) { seductionEffect = StartCoroutine(processSeducedObjects(duration)); }
        Debug.Log("Seducing!");
        */
    }

    public void getHitList(List<Damageable> hitList, SpellCaster owner)
    {
        if(OnHitTarget != null)
        {
            OnHitTarget(hitList, owner);
        }
    }
}
