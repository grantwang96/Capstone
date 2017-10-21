using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SideEffect : ScriptableObject {

    public string sideEffectDescription;

    public abstract void sideEffect(Fighter userFight, Damageable userDam, float duration, float severity);
    public abstract void setupSpell(SpellBook spellBook);
}
