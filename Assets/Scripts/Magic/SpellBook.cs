using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour, Interactable {

    public SpellEffect spellEffect;
    public SideEffect sideEffect;
    public Transform myUser;
    public Transform myUserBody;

    public Fighter myUserFighter;
    public Damageable myUserDamageable;

    public string spellEffectDescription;
    public string sideEffectDescription;
    public Transform spellDetailsPrefab;
    public Transform spellDetails;
    public Transform worldCanvas;

    [SerializeField] int power;
    [SerializeField] float lifeSpan = 15f;
    [SerializeField] float startTime;

    public float sideEffectSeverity;
    public float sideEffectDuration;

    public MeshRenderer[] myBodyParts;

    // Use this for initialization
    void Start () {
        spellEffect = SpellManager.Instance.spellEffects[UnityEngine.Random.Range(0, SpellManager.Instance.spellEffects.Count)];
        spellEffect.setupSpellEffect(this);
        sideEffect = SpellManager.Instance.sideEffects[UnityEngine.Random.Range(0, SpellManager.Instance.sideEffects.Count)];
        sideEffect.setupSpell(this);
        GetComponent<ParticleSystem>().startColor = spellEffect.spellColor;
        worldCanvas = GameObject.Find("WorldCanvas").transform;
        startTime = Time.time;
	}

    void Update()
    {
        if(Time.time - startTime >= lifeSpan && transform.parent == null
           && myUser == null)
        { StartCoroutine(Die()); }
    }

    IEnumerator Die()
    {
        GetComponent<Collider>().enabled = false;
        foreach(MeshRenderer mR in myBodyParts)
        {
            mR.enabled = false;
        }
        yield return new WaitForEndOfFrame();
        destroySpellDetails();
        Destroy(gameObject);
    }

    public void primaryCast()
    {
        spellEffect.primaryCast(myUser, power);
        sideEffect.sideEffect(myUserFighter, myUserDamageable, sideEffectDuration, sideEffectSeverity);
    }

    public void secondaryCast()
    {
        spellEffect.secondaryCast(myUser, power);
        sideEffect.sideEffect(myUserFighter, myUserDamageable, sideEffectDuration, sideEffectSeverity);
    }

    public void sideEffectCast()
    {
        if (sideEffect != null)
        {
            sideEffect.sideEffect(myUserFighter, myUserDamageable, sideEffectDuration, sideEffectSeverity);
        }
    }

    public void setPower(int newPower)
    {
        power = newPower;
    }

    public void PickUp(Transform user, Fighter newFighter, Damageable newDamageable)
    {
        transform.parent = user;
        destroySpellDetails();
        myUser = user;
        GetComponent<ParticleSystem>().Stop();
        if(user.GetComponent<SpellCaster>() != null) { user.GetComponent<SpellCaster>().AddSpell(this); }
        myUserFighter = newFighter;
        myUserDamageable = newDamageable;
        GetComponent<Collider>().enabled = false;
        transform.Find("Book").gameObject.SetActive(false);
    }

    public void Drop()
    {
        transform.parent = null;
        GetComponent<ParticleSystem>().Play();
        GetComponent<Collider>().enabled = true;
        myUser = null;
        myUserFighter = null;
        myUserDamageable = null;
        transform.Find("Book").gameObject.SetActive(true);
        startTime = Time.time;
    }
    
    public Transform createSpellDetails(Transform cameraHead)
    {
        Vector3 dir = transform.position - cameraHead.position;
        Quaternion lookDir = Quaternion.LookRotation(dir);
        spellDetails = Instantiate(spellDetailsPrefab, transform.position + Vector3.up, lookDir);
        spellDetails.SetParent(worldCanvas, false);
        spellDetails.Find("SpellEffectDescription").GetComponent<Text>().text = spellEffectDescription;
        spellDetails.Find("SideEffectDescription").GetComponent<Text>().text = sideEffectDescription;
        return spellDetails;
    }

    public void destroySpellDetails()
    {
        if(spellDetails != null)
        {
            Destroy(spellDetails.gameObject);
        }
        spellDetails = null;
    }
}
