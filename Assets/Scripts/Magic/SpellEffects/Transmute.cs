using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Spell Effect/Transmute")]
public class Transmute : SpellEffect
{
    public float durationLowerBound;
    public float durationUpperBound;

    public Transform projectile;
    public Transform deathFX;

    [Range(10f, 300f)]
    public float force;

    public List<GameObject> possibleResults = new List<GameObject>();
    public Dictionary<GameObject, GameObject> ongoingTransmutations = new Dictionary<GameObject, GameObject>();

    public override IEnumerator Die(Transform projFired)
    {
        Transform newExp = Instantiate(deathFX, projFired.position, projFired.rotation);
        Destroy(projFired.gameObject);
        Destroy(newExp.gameObject, 3f);
        yield return new WaitForEndOfFrame();
    }

    public override void primaryCast(Transform caster, int power)
    {
        Transform newMissile = Instantiate(projectile, caster.position + caster.forward * 0.5f, caster.rotation);
        ProjectileBehavior proj = newMissile.GetComponent<ProjectileBehavior>();
        proj.myCaster = caster;
        proj.myCasterBody = caster.parent;
        proj.mySpellEffect = this;
        proj.power = power;
        proj.isPrimary = true;
        Rigidbody rbody = newMissile.GetComponent<Rigidbody>();
        rbody.useGravity = false;
        rbody.AddForce(caster.forward * force, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
    }

    public override void primaryEffect(ProjectileBehavior projFired, Collider hit)
    {
        if (hit.GetComponent<Damageable>() != null)
        {
            GameObject replacement = possibleResults[UnityEngine.Random.Range(0, possibleResults.Count)];
            hit.GetComponent<Damageable>().InitiateTransmutation(projFired.power, replacement);
        }
        projFired.initiateDie();
    }

    public override void secondaryCast(Transform caster, int power)
    {
        Transform newMissile = Instantiate(projectile, caster.position + caster.forward * 0.5f, caster.rotation);
        ProjectileBehavior proj = newMissile.GetComponent<ProjectileBehavior>();
        proj.myCaster = caster;
        proj.myCasterBody = caster.parent;
        proj.mySpellEffect = this;
        proj.power = power;
        Rigidbody rbody = newMissile.GetComponent<Rigidbody>();
        rbody.useGravity = false;
        rbody.AddForce(caster.forward, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
    }

    public override void secondaryEffect(ProjectileBehavior projFired, Collider hit)
    {
        if (hit.GetComponent<Damageable>() != null)
        {
            Debug.Log("Hit transmutable target!");
            GameObject replacement = possibleResults[UnityEngine.Random.Range(0, possibleResults.Count)];
            hit.GetComponent<Damageable>().InitiateTransmutation(projFired.power, replacement);
        }
        projFired.initiateDie();
    }

    public override void setupSpellEffect(SpellBook spellBook)
    {
        spellBook.setPower((int)UnityEngine.Random.Range(durationLowerBound, durationUpperBound));
        spellBook.spellEffectDescription = spellDescription;
    }
}
