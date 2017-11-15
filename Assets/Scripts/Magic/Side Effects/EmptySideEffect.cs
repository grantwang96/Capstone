using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Side Effect/None")]
public class EmptySideEffect : SideEffect
{
    public override void setupSpell(SpellBook spellBook)
    {
        
    }

    public override void sideEffect(damageable userDam, float duration, float severity)
    {
        
    }
}
