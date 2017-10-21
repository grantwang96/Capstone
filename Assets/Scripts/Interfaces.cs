using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    void TakeDamage(int damage, Vector3 dir, float force);
    GameObject getGameObject();
    void InitiateTransmutation(float duration, GameObject replacement);
    void setTransmutable(bool newBool);
    IEnumerator transmute(float duration, GameObject replacement);
    void vortexGrab(Transform center, float force, float duration);
    void fly(float force, float duration);
    void Seduce(float duration, GameObject target, SpellCaster owner);
    void setCurrentTarget(List<Damageable> targets, SpellCaster owner);
}

public interface Fighter
{
    void Blind(float duration, float severity);
    void Drunk(float duration);
    void Slow(float duration, float severity);
}

public interface SpellCaster
{
    void AddSpell(SpellBook newSpell);
    void DropSpell(SpellBook dropSpell);
    void addToSeductionList(Damageable loser);
    void removeFromSeductionList(Damageable loser);
    void initiateSeduction(float duration);
    void getHitList(List<Damageable> hitList, SpellCaster owner);
}

public interface Interactable
{
    void PickUp(Transform user, Fighter newFighter, Damageable newDamageable);
    void Drop();
}
