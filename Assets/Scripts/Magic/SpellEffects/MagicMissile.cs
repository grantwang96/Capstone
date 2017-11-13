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

    public override void primaryEffect(ProjectileBehavior projFired, Collision hit)
    {
        projFired.initiateDie(hit.contacts[0].point);
    }

    public override void secondaryEffect(ProjectileBehavior projFired, Collision hit)
    {
        projFired.initiateDie(hit.contacts[0].point);
    }

    public override void setupSpellEffect(SpellBook spellBook)
    {
        spellBook.setPower(UnityEngine.Random.Range(damageLowerBound, damageUpperBound));
        spellBook.spellEffectDescription = spellDescription;
        base.setupSpellEffect(spellBook);
    }

    public override void primaryCast(Transform caster, Transform casterBody, int power)
    {
        Transform newMissile = Instantiate(projectile, caster.position + caster.forward * 0.5f, caster.rotation);
        ProjectileBehavior proj = newMissile.GetComponent<ProjectileBehavior>();
        proj.myCaster = caster;
        proj.myCasterBody = casterBody;
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

    public override void secondaryCast(Transform caster, Transform casterBody, int power)
    {
        Transform newMissile = Instantiate(projectile, caster.position + caster.forward * 0.5f, caster.rotation);
        ProjectileBehavior proj = newMissile.GetComponent<ProjectileBehavior>();
        proj.myCaster = caster;
        proj.myCasterBody = casterBody;
        proj.mySpellEffect = this;
        proj.power = power;
        proj.isPrimary = false;
        Rigidbody rbody = newMissile.GetComponent<Rigidbody>();
        rbody.useGravity = false;
        rbody.AddForce(caster.forward * force, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
    }

    public override IEnumerator Die(Transform projFired,Vector3 point)
    {
        // Spawn Explosion
        ProjectileBehavior proj = projFired.GetComponent<ProjectileBehavior>();
        Transform newExp = Instantiate(deathFX, point, Quaternion.identity);
        Collider[] colls = Physics.OverlapSphere(projFired.transform.position, radius);
        SpellCaster caster = projFired.GetComponent<ProjectileBehavior>().mySpellCaster;
        if(colls.Length > 0)
        {
            List<damageable> potentialTargets = new List<damageable>();
            for (int i = 0; i < colls.Length; i++)
            {
                if(colls[i].transform == proj.myCasterBody) { continue; }
                damageable dam = colls[i].GetComponent<damageable>();
                if (dam != null)
                {
                    dam.TakeDamage(proj.myCasterBody, proj.power, (colls[i].transform.position - proj.myCasterBody.position).normalized, knockbackForce);
                    potentialTargets.Add(dam);
                }
                else if (colls[i].attachedRigidbody != null)
                {
                    colls[i].attachedRigidbody.AddExplosionForce(knockbackForce, projFired.transform.position, radius);
                }
            }
            if(potentialTargets.Count != 0) { hitList(potentialTargets, caster); }
        }
        projFired.GetComponent<Collider>().enabled = false;
        projFired.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        ParticleSystem projPart = projFired.GetComponent<ParticleSystem>();
        projPart.Stop();
        Destroy(newExp.gameObject, 3f);
        yield return new WaitForSeconds(projPart.startLifetime);
        Destroy(projFired.gameObject);
    }

    void hitList(List<damageable> targets, SpellCaster caster)
    {
        caster.getHitList(targets, caster);
    }
}
