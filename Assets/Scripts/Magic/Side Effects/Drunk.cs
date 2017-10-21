using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Side Effect/Drunk")]
public class Drunk : SideEffect {

    public float durationLowerBound;
    public float durationUpperBound;

    public override void setupSpell(SpellBook spellBook)
    {
        spellBook.sideEffectDuration = UnityEngine.Random.Range(durationLowerBound, durationUpperBound);
        spellBook.sideEffectDescription = sideEffectDescription;
    }

    public override void sideEffect(Fighter userFight, Damageable userDam, float duration, float severity)
    {
        userFight.Drunk(duration);
    }
}
