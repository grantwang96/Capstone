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

    public override IEnumerator Die(Transform projFired)
    {
        /*
        projFired.GetComponent<Collider>().enabled = false;
        projFired.GetComponent<Rigidbody>().isKinematic = true;
        projFired.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        float duration = projFired.GetComponent<ProjectileBehavior>().power;
        float startDeathTime = Time.time;
        while(Time.time - startDeathTime < duration)
        {
            Collider[] colls = Physics.OverlapSphere(projFired.position, Range, combatLayer);
            foreach(Collider coll in colls)
            {
                Rigidbody collRbody = coll.GetComponent<Rigidbody>();
                Vector3 suckDir = (projFired.position - coll.transform.position).normalized;
                if (coll.GetComponent<Damageable>() != null)
                {
                    coll.GetComponent<Damageable>().vortexGrab(projFired, suckForce, duration - (Time.time - startDeathTime));
                }
                else if(collRbody != null && !collRbody.isKinematic && collRbody.transform != projFired)
                {
                    collRbody.AddForce(suckDir * suckForce);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        */
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
        rbody.useGravity = true;
        rbody.mass = weight;
        rbody.AddForce((caster.forward * throwForce) + Vector3.up, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
    }

    public override void primaryEffect(ProjectileBehavior projFired, Collider hit)
    {
        Transform newGravWell = Instantiate(gravWell, projFired.transform.position, Quaternion.identity);
        GravityWellVortex gravScript = newGravWell.GetComponent<GravityWellVortex>();
        gravScript.force = suckForce;
        gravScript.maxRange = Range;
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
        proj.isPrimary = false;
        Rigidbody rbody = newMissile.GetComponent<Rigidbody>();
        rbody.useGravity = true;
        rbody.mass = weight;
        rbody.AddForce((caster.forward * throwForce) + Vector3.up, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
    }

    public override void secondaryEffect(ProjectileBehavior projFired, Collider hit)
    {
        projFired.initiateDie();
    }

    public override void setupSpellEffect(SpellBook spellBook)
    {
        spellBook.setPower((int)UnityEngine.Random.Range(durationLowerBound, durationUpperBound));
        spellBook.spellEffectDescription = spellDescription;
    }
}
