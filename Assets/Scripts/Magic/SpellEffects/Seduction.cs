using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Spell Effect/Seduction")]
public class Seduction : SpellEffect
{
    public Transform projectile;
    public Transform deathFX;
    public Transform dokiFX;

    public float radius;

    public int durationLowerBound;
    public int durationUpperBound;

    public float force;
    
    public override IEnumerator Die(Transform projFired, Vector3 point)
    {
        Transform newExp = Instantiate(deathFX, point, Quaternion.identity);
        Collider[] colls = Physics.OverlapSphere(projFired.transform.position, radius);
        ProjectileBehavior projData = projFired.GetComponent<ProjectileBehavior>();
        SpellCaster caster = projData.myCaster.GetComponent<SpellCaster>();
        foreach (Collider coll in colls)
        {
            damageable dam = coll.GetComponent<damageable>();
            if (dam != null && coll.transform != projData.myCaster && coll.transform != projData.myCasterBody)
            {
                dam.Seduce(projData.power, projData.myCasterBody.gameObject, caster);
                caster.addToSeductionList(dam);
                if(coll.transform.Find("Doki") == null)
                {
                    Transform newDokiEffect = Instantiate(dokiFX, coll.transform.position, Quaternion.identity);
                    newDokiEffect.name = "Doki";
                    newDokiEffect.parent = coll.transform;
                }
            }
        }
        projFired.GetComponent<Collider>().enabled = false;
        projFired.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        ParticleSystem projPart = projFired.GetComponent<ParticleSystem>();
        projPart.Stop();
        Destroy(newExp.gameObject, 3f);
        yield return new WaitForSeconds(projPart.startLifetime);
        Destroy(projFired.gameObject);
    }

    public override void primaryCast(Transform caster, Transform casterBody, int power)
    {
        Transform newMissile = Instantiate(projectile, caster.position + caster.forward * 0.5f, caster.rotation);
        ProjectileBehavior proj = newMissile.GetComponent<ProjectileBehavior>();
        proj.myCaster = caster;
        proj.myCasterBody = casterBody;
        proj.mySpellEffect = this;
        proj.power = power;
        Rigidbody rbody = newMissile.GetComponent<Rigidbody>();
        rbody.useGravity = false;
        rbody.mass = 1f;
        rbody.AddForce(caster.forward * force, ForceMode.Impulse);
        newMissile.GetComponent<ParticleSystem>().startColor = spellColor;
    }

    public override void primaryEffect(ProjectileBehavior projFired, Collision hit)
    {
        projFired.initiateDie(hit.contacts[0].point);
    }

    public override void secondaryCast(Transform caster, Transform casterBody, int power)
    {
        throw new NotImplementedException();
    }

    public override void secondaryEffect(ProjectileBehavior projFired, Collision hit)
    {
        throw new NotImplementedException();
    }

    public override void setupSpellEffect(SpellBook spellBook)
    {
        spellBook.setPower(UnityEngine.Random.Range(durationLowerBound, durationUpperBound));
        spellBook.spellEffectDescription = spellDescription;
        base.setupSpellEffect(spellBook);
    }
}
