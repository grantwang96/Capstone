using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell Effect/Fireblast")]
public class Fireblast : SpellEffect {

    public Transform projectile;
    public Transform pillarOfDoom;
    public Transform tinyFlame;
    public Transform deathFX;
    
    public float force;
    public float delay;
    public float knockbackForce;

    public int damageLowerBound;
    public int damageUpperBound;

    [Range(0f, 10f)]
    public float mass;

    public int shrapnelCountLowerBound;
    public int shrapnelCountUpperBound;

    [Range(0.2f, 20f)]
    public float blastRadius;
    public float radius;

    public override IEnumerator Die(Transform projFired, Vector3 point)
    {
        ProjectileBehavior proj = projFired.GetComponent<ProjectileBehavior>();
        proj.GetComponent<ParticleSystem>().Stop();
        if (proj.mainShot)
        {
            Transform newFlame = Instantiate(tinyFlame, point, Quaternion.identity);
            ParticleSystem flamePartSys = newFlame.GetComponent<ParticleSystem>();
            float startTime = Time.time;
            proj.GetComponent<Collider>().enabled = false;
            proj.GetComponent<Rigidbody>().isKinematic = true;
            proj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            while(Time.time - startTime < delay)
            {
                float startLife = flamePartSys.startLifetime;
                if(startLife > 0) { flamePartSys.startLifetime -= Time.deltaTime / delay; }
                yield return new WaitForEndOfFrame();
            }
            mainBlast(proj);
            Destroy(newFlame.gameObject);
            Destroy(projFired.gameObject);
        }
        else
        {
            Transform newExp = Instantiate(deathFX, projFired.position, Quaternion.identity);
            Collider[] colls = Physics.OverlapSphere(projFired.transform.position, radius);
            List<Damageable> potentialTargets = new List<Damageable>();
            SpellCaster caster = projFired.GetComponent<ProjectileBehavior>().mySpellCaster;
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
            if (potentialTargets.Count != 0) { hitList(potentialTargets, caster); }
            Destroy(newExp.gameObject, 3f);
            Destroy(projFired.gameObject);
            yield return new WaitForEndOfFrame();
        }
    }

    public override void primaryCast(Transform caster, int power)
    {
        Transform newMissile = Instantiate(projectile, caster.position + caster.forward * 0.5f, caster.rotation);
        ProjectileBehavior proj = newMissile.GetComponent<ProjectileBehavior>();
        proj.myCaster = caster;
        proj.myCasterBody = caster.parent;
        proj.mySpellEffect = this;
        proj.mySpellCaster = caster.GetComponent<SpellCaster>();
        proj.power = power;
        proj.isPrimary = true;
        proj.mainShot = true;
        Rigidbody rbody = newMissile.GetComponent<Rigidbody>();
        rbody.useGravity = true;
        rbody.mass = mass;
        rbody.AddForce(caster.forward * force, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
        Debug.Log("Hit target for " + power + " damage!");
        Debug.Log("Bounce Count = " + proj.bounceCount);
    }

    public override void primaryEffect(ProjectileBehavior projFired, Collision hit)
    {
        projFired.initiateDie(hit.contacts[0].point);
    }

    public override void secondaryCast(Transform caster, int power)
    {
        throw new NotImplementedException();
    }

    public override void secondaryEffect(ProjectileBehavior projFired, Collision hit)
    {
        throw new NotImplementedException();
    }

    public override void setupSpellEffect(SpellBook spellBook)
    {
        spellBook.setPower(UnityEngine.Random.Range(damageLowerBound, damageUpperBound));
        spellBook.spellEffectDescription = spellDescription;
    }

    void mainBlast(ProjectileBehavior projFired)
    {
        Transform newPillarOfDoom = Instantiate(pillarOfDoom, projFired.transform.position, Quaternion.identity);
        newPillarOfDoom.GetComponent<PillarOfDoom>().damage = projFired.power;
        int shrapCount = UnityEngine.Random.Range(shrapnelCountLowerBound, shrapnelCountUpperBound);
        float angInterval = 360 / shrapCount;
        for (int i = 0; i < shrapCount; i++)
        {
            float ang = angInterval * i;
            Vector3 offset;
            offset.x = projFired.transform.position.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            offset.y = projFired.transform.position.y + 1f;
            offset.z = projFired.transform.position.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            subBlast(offset, newPillarOfDoom.position, projFired.mySpellCaster);
        }
    }

    void subBlast(Vector3 position, Vector3 startingPos, SpellCaster caster)
    {
        Transform newSubBlast = Instantiate(projectile, position, Quaternion.identity);
        newSubBlast.forward = position - startingPos;
        ProjectileBehavior newproj = newSubBlast.GetComponent<ProjectileBehavior>();
        newproj.mySpellCaster = caster;
        newproj.mySpellEffect = this;
        newproj.power = UnityEngine.Random.Range(damageLowerBound, damageUpperBound) / 2;
        newproj.isPrimary = true;
        Rigidbody rbody = newSubBlast.GetComponent<Rigidbody>();
        rbody.useGravity = true;
        rbody.mass = mass;
        rbody.AddForce(newSubBlast.forward * UnityEngine.Random.Range(6, 12), ForceMode.Impulse);
        newproj.GetComponent<ParticleSystem>().startColor = spellColor;
        newproj.mainShot = false;
    }
    
    void hitList(List<Damageable> targets, SpellCaster caster)
    {
        caster.getHitList(targets, caster);
    }
}
