﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWellVortex : MonoBehaviour {

    public float force;
    public float lifeTime;
    public float range;
    public float rangeIncreaseFactor;
    public float maxRange;

    public float speed;

    public ParticleSystem effects;
    public SphereCollider rangeFinder;

    float startTime;
    List<Transform> trapped = new List<Transform>();
    public Transform explosionPrefab;

	// Use this for initialization
	void Start () {
        range = 0.01f;
        rangeFinder = GetComponent<SphereCollider>();
        effects = GetComponent<ParticleSystem>();
        rangeFinder.radius = range;
        ParticleSystem.ShapeModule shapeModule = effects.shape;
        shapeModule.radius = range;
        effects.startSpeed = -range;
        startTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, speed * Time.deltaTime, 0);
        if (Time.time - startTime >= lifeTime) {
            Die();
        }
        if (range < maxRange)
        {
            range += Time.deltaTime * rangeIncreaseFactor;
            if(range > maxRange)
            {
                range = maxRange;
                startTime = Time.time;
            }
            rangeFinder.radius = range;
            ParticleSystem.ShapeModule shapeModule = effects.shape;
            shapeModule.radius = range;
            effects.startSpeed = -range;
        }
        if(trapped.Count > 0)
        {
            foreach(Transform loser in trapped)
            {
                Damageable dam = loser.GetComponent<Damageable>();
                dam.vortexGrab(transform, force, Time.time - startTime);
            }
        }
	}

    void OnTriggerEnter(Collider coll)
    {
        if(coll.GetComponent<Damageable>() != null)
        {
            coll.transform.parent = transform;
            trapped.Add(coll.transform);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (trapped.Contains(coll.transform))
        {
            coll.transform.parent = null;
            trapped.Remove(coll.transform);
        }
    }

    void Die()
    {
        foreach(Transform loser in trapped) { loser.parent = null; }
        trapped.Clear();
        // Small explosion to send all objects up
        Collider[] colls = Physics.OverlapSphere(transform.position, 3f);
        Transform newExp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        foreach(Collider coll in colls)
        {
            Damageable dam = coll.GetComponent<Damageable>();
            if(dam != null)
            {
                Vector3 dir = (coll.transform.position - transform.position).normalized;
                dam.TakeDamage(0, dir, force);
            }
        }
        Destroy(newExp.gameObject, 3f);
        Destroy(gameObject);
    }
}
