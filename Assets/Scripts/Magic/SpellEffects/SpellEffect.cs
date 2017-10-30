using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellEffect : ScriptableObject{

    public Color spellColor;
    public string spellDescription;
    public int ammo;

    public abstract void primaryCast(Transform caster, Transform casterbody, int power);
    public abstract void secondaryCast(Transform caster, Transform casterbody, int power);
    public abstract void primaryEffect(ProjectileBehavior projFired, Collision hit);
    public abstract void secondaryEffect(ProjectileBehavior projFired, Collision hit);
    public virtual void setupSpellEffect(SpellBook spellBook)
    {
        spellBook.maxAmmo += ammo;
        MeshRenderer mr = spellBook.transform.Find("Book").Find("Cover").GetComponent<MeshRenderer>();
        mr.material.color = spellColor;
    }
    public abstract IEnumerator Die(Transform projFired, Vector3 point);
}
