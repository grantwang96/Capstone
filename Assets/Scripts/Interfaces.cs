using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public interface Damageable
{
    void TakeDamage(Transform attacker, int damage, Vector3 dir, float force);
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

    float getSightRange();
}
*/
public interface SpellCaster
{
    void AddSpell(SpellBook newSpell);
    void DropSpell(SpellBook dropSpell);
    void addToSeductionList(damageable loser);
    void removeFromSeductionList(damageable loser);
    void initiateSeduction(float duration);
    void getHitList(List<damageable> hitList, SpellCaster owner);
}

public interface Interactable
{
    void PickUp(Transform user, Transform userbody, damageable newDamageable);
    void Drop();
}

public interface NPCStateMachine
{
    void Enter(Movement owner);
    void Execute();
    void Exit();
}
