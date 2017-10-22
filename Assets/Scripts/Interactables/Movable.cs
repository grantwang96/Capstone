﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour, Damageable
{
    bool transmuted = false;
    bool transmutable = true;

    Coroutine seduction;
    Transform attackTarget;
    SpellCaster myOwner;

    public GameObject getGameObject()
    {
        return gameObject;
    }

    public void TakeDamage(int damage, Vector3 dir, float force)
    {
        GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.Impulse);
    }

    public void InitiateTransmutation(float duration, GameObject replacement)
    {
        if (!transmuted && transmutable)
        {
            StartCoroutine(transmute(duration, replacement));
        }
    }

    public IEnumerator transmute(float duration, GameObject replacement)
    {
        transmuted = true;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<MeshRenderer>().enabled = false;
        GameObject myReplacement = Instantiate(replacement, transform.position, transform.rotation);
        if (myReplacement.GetComponent<Rigidbody>() != null)
        {
            Debug.Log("Boosh!");
            myReplacement.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
            myReplacement.GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere * 5f;
        }
        myReplacement.GetComponent<Damageable>().setTransmutable(false);
        yield return new WaitForSeconds(duration);
        Destroy(myReplacement);
        transform.position = myReplacement.transform.position;
        transform.rotation = myReplacement.transform.rotation;
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<MeshRenderer>().enabled = true;
        transmuted = false;
        GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
        GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere * 5f;
    }

    public void setTransmutable(bool newBool)
    {
        transmutable = newBool;
    }

    public void vortexGrab(Transform center, float force, float duration)
    {
        Vector3 dir = (center.position - transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(dir * force);
    }

    public void fly(float force, float duration)
    {

    }

    public void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        myOwner = owner;
        if(seduction != null) { StopCoroutine(seduction); }
        seduction = StartCoroutine(processSeduction(duration, target, owner));
    }

    IEnumerator processSeduction(float duration, GameObject target, SpellCaster owner)
    {
        attackTarget = target.transform;
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            if (target.transform == attackTarget) { processSeducedMovement(target); }
            else { processSeducedAttack(); }
            yield return new WaitForEndOfFrame();
        }
        myOwner.removeFromSeductionList(this.GetComponent<Damageable>());
        myOwner = null;
        attackTarget = null;
        Transform dokiFX = transform.Find("Doki");
        if (dokiFX != null)
        {
            ParticleSystem dFX = dokiFX.GetComponent<ParticleSystem>();
            dFX.Stop();
            dokiFX.parent = null;
            Destroy(dokiFX.gameObject, dFX.startLifetime);
        }
    }

    void processSeducedMovement(GameObject target)
    {
        float loverDist = Vector3.Distance(transform.position, target.transform.position);
        Vector3 moveDir = (target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir);
        if (loverDist > 5f)
        {
            GetComponent<Rigidbody>().velocity = moveDir * 10f;
        }
    }

    void processSeducedAttack()
    {
        if (attackTarget == null) { attackTarget = GameObject.Find("Player_Rbody").transform; }
        float loverDist = Vector3.Distance(transform.position, attackTarget.transform.position);
        Vector3 moveDir = (attackTarget.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir);
        GetComponent<Rigidbody>().velocity = moveDir * 20f;
    }

    public void setCurrentTarget(List<Damageable> targets, SpellCaster owner)
    {
        myOwner = owner;
        List<Damageable> goodTargets = new List<Damageable>();
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != GetComponent<Damageable>()) { goodTargets.Add(targets[i]); }
        }
        if (goodTargets.Count > 0)
        {
            Damageable target = goodTargets[UnityEngine.Random.Range(0, goodTargets.Count)];
            attackTarget = target.getGameObject().transform;
        }
    }
}