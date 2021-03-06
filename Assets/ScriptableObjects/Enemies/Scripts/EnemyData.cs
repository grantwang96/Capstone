﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ScriptableObject {

    [Range(5, 700)]
    public int health;
    public float baseSpeed;
    public float maxSpeed;

    [Range(0, 100)] public float sightRange;
    [Range(0, 90)] public float sightAngle;

    public int damage;

    public CombatType myType;

    public NPCState startingState;

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
        owner.sightRange = sightRange;
        owner.sightAngle = sightAngle;
        owner.myType = myType;
    }
}
