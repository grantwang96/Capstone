using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "EnemyData/BasicMeleeEnemy")]
public class BasicMeleeEnemy : EnemyData {
    
    public override void setup(Movement owner)
    {
        startingState = new MeleeEnemyIdle();
        owner.baseSpeed = baseSpeed;
        owner.maxSpeed = maxSpeed;
        damageable ownerDam = owner.GetComponent<damageable>();
        ownerDam.max_health = health;
        owner.attackTarget = GameObject.FindGameObjectWithTag("Player").transform;
        base.setup(owner);
    }
}
