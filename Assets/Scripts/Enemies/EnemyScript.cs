using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

    public int MAX_HEALTH = 6;
    public int health;

    bool dead;
    public bool getDead() { return dead; }
    public void setDead(bool newBool) { dead = newBool; }

    // Use this for initialization
    void Start () {
        health = MAX_HEALTH;
        dead = false;
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        health -= damage;
    }

    public void Heal(int damage)
    {
        health += damage;
        if(health > MAX_HEALTH) { health = MAX_HEALTH; }
    }
}
