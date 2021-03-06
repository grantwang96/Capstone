﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "EnemyData/BasicMeleeEnemy")]
public class BasicMeleeEnemy : EnemyData {
    
    public override void setup(Movement owner)
    {
        startingState = new NPCWander();
        owner.baseSpeed = baseSpeed;
        owner.maxSpeed = maxSpeed;
        damageable ownerDam = owner.GetComponent<damageable>();
        ownerDam.max_health = health;
        owner.damage = damage;
        owner.attackTarget = GameObject.FindGameObjectWithTag("Player").transform;
        base.setup(owner);
    }
}
