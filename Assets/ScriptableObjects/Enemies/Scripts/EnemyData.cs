using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ScriptableObject {

    [Range(5, 700)]
    public int health;
    public CombatType myType;

    public NPCStateMachine startingState;

    public enum CombatType
    {
        Melee,
        Ranged,
        Mixed,
        SpellCaster,
        Support,
    }

    public virtual void setup(Movement owner)
    {
        if(startingState != null) {
            owner.changeState(startingState);
        }
        owner.GetComponent<damageable>().max_health = health;
    }
}
