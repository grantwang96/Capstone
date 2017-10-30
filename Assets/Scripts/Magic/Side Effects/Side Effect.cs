using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SideEffect : ScriptableObject {

    public int ammoBonus;
    public Color sideEffectColor;
    public string sideEffectDescription;
    public Sprite affectIcon;

    public abstract void sideEffect(Fighter userFight, Damageable userDam, float duration, float severity);
    public virtual void setupSpell(SpellBook spellBook)
    {
        spellBook.maxAmmo += ammoBonus;
        ParticleSystem.MainModule main = spellBook.GetComponent<ParticleSystem>().main;
        main.startColor = sideEffectColor;
    }
}
