using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {

    public SpellEffect mySpellEffect;
    public int power;
    public bool isPrimary;

    public int bounceCount = 0;
    public bool mainShot;

    float maxLifeTime = 8f;
    float startTime;

    public Transform myCaster;
    public SpellCaster mySpellCaster;
    public Transform myCasterBody;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - startTime > maxLifeTime)
        {
            Destroy(this.gameObject);
        }
	}

    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform != myCaster && coll.transform != myCasterBody)
        {
            if (isPrimary) { mySpellEffect.primaryEffect(this, coll); }
            else { mySpellEffect.secondaryEffect(this, coll); }
        }
    }

    public void initiateDie(Vector3 point)
    {
        StartCoroutine(mySpellEffect.Die(transform, point));
    }
}
