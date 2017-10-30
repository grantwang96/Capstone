using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Side Effect/Slow")]
public class Slow : SideEffect
{
    public float durationLowerBound;
    public float durationUpperBound;

    public float severityLowerBound;
    public float severityUpperBound;

    public override void setupSpell(SpellBook spellBook)
    {
        spellBook.sideEffectDuration = UnityEngine.Random.Range(durationLowerBound, durationUpperBound);
        spellBook.sideEffectSeverity = UnityEngine.Random.Range(severityLowerBound, severityUpperBound);
        spellBook.sideEffectDescription = sideEffectDescription;
        base.setupSpell(spellBook);
    }

    public override void sideEffect(Fighter userFight, Damageable userDam, float duration, float severity)
    {
        userFight.Slow(duration, severity);
    }
}
