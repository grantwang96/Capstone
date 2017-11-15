using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Side Effect/Blind")]
public class Blind : SideEffect {

    public float durationLowerBound;
    public float durationUpperBound;

    public float severityLowerBound;
    public float severityUpperBound;

    public override void sideEffect(damageable userDam, float duration, float severity)
    {
        userDam.Blind(severity, duration);
    }
    public override void setupSpell(SpellBook spellBook)
    {
        spellBook.sideEffectDuration = UnityEngine.Random.Range(durationLowerBound, durationUpperBound);
        spellBook.sideEffectSeverity = UnityEngine.Random.Range(severityLowerBound, severityUpperBound);
        spellBook.sideEffectDescription = sideEffectDescription;
        base.setupSpell(spellBook);
    }
}
