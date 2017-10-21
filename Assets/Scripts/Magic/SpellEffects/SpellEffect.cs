using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellEffect : ScriptableObject{

    public Color spellColor;
    public string spellDescription;

    public abstract void primaryCast(Transform caster, int power);
    public abstract void secondaryCast(Transform caster, int power);
    public abstract void primaryEffect(ProjectileBehavior projFired, Collider hit);
    public abstract void secondaryEffect(ProjectileBehavior projFired, Collider hit);
    public abstract void setupSpellEffect(SpellBook spellBook);
    public abstract IEnumerator Die(Transform projFired);
}
