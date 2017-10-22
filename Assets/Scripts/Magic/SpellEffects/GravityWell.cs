using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Gravity Well")]
public class GravityWell : SpellEffect
{
    public float durationLowerBound;
    public float durationUpperBound;
    public float maximumRange;

    [Range(1f, 10f)]
    public float Range;
    [Range(1f, 100f)]
    public float suckForce;
    [Range(1f, 200f)]
    public float throwForce;
    [Range(0f, 10f)]
    public float weight;

    public Transform projectile;
    public Transform gravWell;
    public Transform deathFX;

    public LayerMask combatLayer;

    public override IEnumerator Die(Transform projFired, Vector3 point)
    {
        Transform newExp = Instantiate(deathFX, point, projFired.rotation);
        projFired.GetComponent<Collider>().enabled = false;
        projFired.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        ParticleSystem projPart = projFired.GetComponent<ParticleSystem>();
        projPart.Stop();
        Destroy(newExp.gameObject, 3f);
        yield return new WaitForSeconds(projPart.startLifetime);
        Destroy(projFired.gameObject);
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
        rbody.useGravity = true;
        rbody.mass = weight;
        rbody.AddForce((caster.forward * throwForce) + Vector3.up, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
    }

    public override void primaryEffect(ProjectileBehavior projFired, Collision hit)
    {
        Transform newGravWell = Instantiate(gravWell, projFired.transform.position, Quaternion.identity);
        GravityWellVortex gravScript = newGravWell.GetComponent<GravityWellVortex>();
        gravScript.force = suckForce;
        gravScript.maxRange = Range;
        projFired.initiateDie(hit.contacts[0].point);
    }

    public override void secondaryCast(Transform caster, int power)
    {
        Transform newMissile = Instantiate(projectile, caster.position + caster.forward * 0.5f, caster.rotation);
        ProjectileBehavior proj = newMissile.GetComponent<ProjectileBehavior>();
        proj.myCaster = caster;
        proj.myCasterBody = caster.parent;
        proj.mySpellEffect = this;
        proj.power = power;
        proj.isPrimary = false;
        Rigidbody rbody = newMissile.GetComponent<Rigidbody>();
        rbody.useGravity = true;
        rbody.mass = weight;
        rbody.AddForce((caster.forward * throwForce) + Vector3.up, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
    }

    public override void secondaryEffect(ProjectileBehavior projFired, Collision hit)
    {
        projFired.initiateDie(hit.contacts[0].point);
    }

    public override void setupSpellEffect(SpellBook spellBook)
    {
        spellBook.setPower((int)UnityEngine.Random.Range(durationLowerBound, durationUpperBound));
        spellBook.spellEffectDescription = spellDescription;
    }
}
