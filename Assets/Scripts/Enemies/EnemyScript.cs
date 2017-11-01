using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

    public int MAX_HEALTH = 6;
    public int health;

    public Transform attackTarget;

    public bool freeMove;
    public int damage;
    public float speed;
    public float sightRange;

    bool dead;
    public bool getDead() { return dead; }
    public void setDead(bool newBool) { dead = newBool; }

    public Rigidbody rbody;
    public Collider myColl;

    public SpellCaster myOwner;
    public NPCStateMachine currState;

    // Use this for initialization
    void Start () {
        rbody = GetComponent<Rigidbody>();
        myColl = GetComponent<Collider>();
        health = MAX_HEALTH;
        damage = UnityEngine.Random.Range(5, 10);
        freeMove = true;
        dead = false;
    }
	
	// Update is called once per frame
	void Update () {
		if(currState != null) {
            currState.Execute();
        }
	}

    public void changeStates(NPCStateMachine newState)
    {
        if(currState != null) { currState.Exit(); }
        currState = newState;
    }
}
