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

    // public Fighter myUserFighter;
    public damageable myUserDamageable;

    public string spellEffectDescription;
    public string sideEffectDescription;
    public string spellTitle;
    public Transform spellDetailsPrefab;
    public Transform spellDetails;
    public Transform worldCanvas;

    [SerializeField] int power;
    [SerializeField] float lifeSpan = 15f;
    [SerializeField] float startTime;

    public float sideEffectSeverity;
    public float sideEffectDuration;

    public MeshRenderer[] myBodyParts;
    public Transform dieEffect;
    public Transform pickUpEffect;
    public bool dead;
    public bool dying;
    public bool defaultSpell;

    public Vector3 startPos;

    public int maxAmmo;
    [SerializeField] private int ammo;
    public void setAmmo(int newAmmo) { ammo = newAmmo; }
    public int getAmmo() { return ammo; }
    public void useAmmo() { ammo -= 1; }

    // Use this for initialization
    void Start () {
        if(spellEffect == null)
        {
            spellEffect = SpellManager.Instance.spellEffects[UnityEngine.Random.Range(0, SpellManager.Instance.spellEffects.Count)];
            sideEffect = SpellManager.Instance.sideEffects[UnityEngine.Random.Range(0, SpellManager.Instance.sideEffects.Count)];
        }
        spellEffect.setupSpellEffect(this);
        sideEffect.setupSpell(this);
        // GetComponent<ParticleSystem>().startColor = spellEffect.spellColor;
        worldCanvas = GameObject.Find("WorldCanvas").transform;
        startPos = transform.position;
        startTime = Time.time;
        if (defaultSpell) {
            Transform user = GameObject.Find("PlayerHead").transform;
            Transform userbody = GameObject.Find("Player_Rbody").transform;
            PickUp(user, userbody, userbody.GetComponent<damageable>());
        }
        dead = false;
        dying = false;
        ammo = maxAmmo;
	}

    void Update()
    {
        if (ammo <= 0 && !dying)
        {
            dying = true;
            StartCoroutine(Die());
        }
    }

    public void hovered()
    {
        if (!dead)
        {
            transform.Rotate(new Vector3(0, 30f * Time.deltaTime, 0));
            float height = startPos.y + Mathf.PerlinNoise(Time.time * 1f, 0f) - 0.5f;
            Vector3 pos = transform.position;
            pos.y = height;
            transform.position = pos;
        }
    }

    IEnumerator Die()
    {
        dead = true;
        myUser.GetComponent<SpellCaster>().DropSpell(this);
        float dieTime = 2f;
        float startTime = Time.time;
        GetComponent<Collider>().enabled = false;
        Transform newDieEffect = Instantiate(dieEffect, transform.position, Quaternion.identity);
        newDieEffect.parent = transform;
        destroySpellDetails();
        foreach (MeshRenderer mR in myBodyParts)
        {
            mR.enabled = false;
        }
        while(Time.time - startTime < dieTime)
        {
            transform.position += Vector3.up * Time.deltaTime * 0.3f;
            yield return new WaitForEndOfFrame();
        }
        newDieEffect.GetComponent<ParticleSystem>().Stop();
        newDieEffect.parent = null;
        Destroy(newDieEffect.gameObject, 3f);
        Destroy(gameObject);
    }

    public void primaryCast()
    {
        spellEffect.primaryCast(myUser, myUserBody, power);
        sideEffect.sideEffect(myUserDamageable, sideEffectDuration, sideEffectSeverity);
    }

    public void secondaryCast()
    {
        spellEffect.secondaryCast(myUser, myUserBody, power);
        sideEffect.sideEffect(myUserDamageable, sideEffectDuration, sideEffectSeverity);
    }

    public void sideEffectCast()
    {
        if (sideEffect != null)
        {
            sideEffect.sideEffect(myUserDamageable, sideEffectDuration, sideEffectSeverity);
        }
    }

    public void setPower(int newPower)
    {
        power = newPower;
    }

    public void PickUp(Transform user, Transform userbody, damageable newDamageable)
    {
        if (dead) { return; }
        transform.parent = user;
        destroySpellDetails();
        myUser = user;
        myUserBody = userbody;
        GetComponent<ParticleSystem>().Stop();
        myUserDamageable = newDamageable;
        GetComponent<Collider>().enabled = false;
        dead = true;
        StartCoroutine(pickUpAnimation(user));
    }

    IEnumerator pickUpAnimation(Transform user)
    {
        float startTime = Time.time;
        while(Time.time - startTime < 0.2f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, 10f * Time.deltaTime);
            transform.Rotate(new Vector3(0, 45f, 0));
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = Vector3.zero;
        if (user.GetComponent<SpellCaster>() != null) { user.GetComponent<SpellCaster>().AddSpell(this); }
        transform.Find("Book").gameObject.SetActive(false);
    }

    public void Drop()
    {
        transform.parent = null;
        GetComponent<ParticleSystem>().Play();
        GetComponent<Collider>().enabled = true;
        myUser = null;
        myUserDamageable = null;
        transform.Find("Book").gameObject.SetActive(true);
        dead = false;
        startTime = Time.time;
        // StartCoroutine(Die());
    }
    
    public Transform createSpellDetails(Transform cameraHead)
    {
        if (dead) { return null; }
        /*
        Vector3 dir = transform.position - cameraHead.position;
        Quaternion lookDir = Quaternion.LookRotation(dir);
        spellDetails = Instantiate(spellDetailsPrefab, transform.position + Vector3.up, lookDir);
        spellDetails.SetParent(worldCanvas, false);
        spellDetails.Find("SpellEffectDescription").GetComponent<Text>().text = spellEffectDescription;
        spellDetails.Find("SideEffectDescription").GetComponent<Text>().text = sideEffectDescription;
        return spellDetails;
        */
        return null;
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
