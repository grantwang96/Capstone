using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Side Effect/Flying")]
public class Flying : SideEffect
{
    public float durationLowerBound;
    public float durationUpperBound;

    public float force;

    public override void setupSpell(SpellBook spellBook)
    {
        spellBook.sideEffectDuration = UnityEngine.Random.Range(durationLowerBound, durationUpperBound);
        spellBook.sideEffectSeverity = force;
        spellBook.sideEffectDescription = sideEffectDescription;
    }

    public override void sideEffect(Fighter userFight, Damageable userDam, float duration, float severity)
    {
        userDam.fly(severity, duration);
    }
}
