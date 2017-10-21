using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Spell Effect/MagicMissile")]
public class MagicMissile : SpellEffect
{
    public int damageLowerBound;
    public int damageUpperBound;

    public Transform projectile;
    public Transform deathFX;

    public float radius;

    [Range(20f, 300f)]
    public float force;

    [Range(0f, 300f)]
    public float knockbackForce;

    public override void primaryEffect(ProjectileBehavior projFired, Collider hit)
    {
        /*
        Collider[] colls = Physics.OverlapSphere(projFired.transform.position, radius);
        foreach(Collider coll in colls)
        {
            Damageable dam = coll.GetComponent<Damageable>();
            if(dam != null)
            {
                dam.TakeDamage(projFired.power, coll.transform.position - projFired.transform.position, knockbackForce);
            }
            else if(coll.attachedRigidbody != null)
            {
                coll.attachedRigidbody.AddExplosionForce(knockbackForce, projFired.transform.position, radius);
            }
        }
        */

        projFired.initiateDie();
    }

    public override void secondaryEffect(ProjectileBehavior projFired, Collider hit)
    {
        projFired.initiateDie();
    }

    public override void setupSpellEffect(SpellBook spellBook)
    {
        spellBook.setPower(UnityEngine.Random.Range(damageLowerBound, damageUpperBound));
        spellBook.spellEffectDescription = spellDescription;
    }

    public override void primaryCast(Transform caster, int power)
    {
        Transform newMissile = Instantiate(projectile, caster.position + caster.forward * 0.5f, caster.rotation);
        ProjectileBehavior proj = newMissile.GetComponent<ProjectileBehavior>();
        proj.myCaster = caster;
        proj.myCasterBody = caster.parent;
        proj.mySpellCaster = caster.GetComponent<SpellCaster>();
        proj.mySpellEffect = this;
        proj.power = power;
        proj.isPrimary = true;
        Rigidbody rbody = newMissile.GetComponent<Rigidbody>();
        rbody.useGravity = false;
        rbody.AddForce(caster.forward * force, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
        Debug.Log("Hit target for " + power + " damage!");
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
        rbody.useGravity = false;
        rbody.AddForce(caster.forward * force, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
    }

    public override IEnumerator Die(Transform projFired)
    {
        // Spawn Explosion
        Transform newExp = Instantiate(deathFX, projFired.position, Quaternion.identity);
        Collider[] colls = Physics.OverlapSphere(projFired.transform.position, radius);
        SpellCaster caster = projFired.GetComponent<ProjectileBehavior>().mySpellCaster;
        if(colls.Length > 0)
        {
            List<Damageable> potentialTargets = new List<Damageable>();
            for (int i = 0; i < colls.Length; i++)
            {
                Damageable dam = colls[i].GetComponent<Damageable>();
                if (dam != null)
                {
                    dam.TakeDamage(projFired.GetComponent<ProjectileBehavior>().power, colls[i].transform.position - projFired.transform.position, knockbackForce);
                    potentialTargets.Add(dam);
                }
                else if (colls[i].attachedRigidbody != null)
                {
                    colls[i].attachedRigidbody.AddExplosionForce(knockbackForce, projFired.transform.position, radius);
                }
            }
            if(potentialTargets.Count != 0) { hitList(potentialTargets, caster); }
        }
        Destroy(newExp.gameObject, 3f);
        Destroy(projFired.gameObject);
        yield return new WaitForEndOfFrame();
    }

    void hitList(List<Damageable> targets, SpellCaster caster)
    {
        caster.getHitList(targets, caster);
    }
}
