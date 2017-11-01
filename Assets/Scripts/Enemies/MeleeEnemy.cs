using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour, Damageable, Fighter {

    const int MAX_HEALTH = 20;
    public int health;

    CharacterController charCon;
    public Transform player;

    bool freeMove;

    public int damage;
    public float speed;

    bool transmuted = false;
    bool transmutable = true;
    float force;

    // Use this for initialization
    void Start()
    {
        charCon = GetComponent<CharacterController>();
        health = MAX_HEALTH;
        damage = UnityEngine.Random.Range(5, 10);
        player = GameObject.Find("Player_Rbody").transform;
        freeMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = new Vector3(player.position.x, 0, player.position.z);
        Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
        if (charCon.enabled && freeMove)
        {
            transform.rotation = Quaternion.LookRotation(playerPos - myPos);
            charCon.Move(transform.forward * speed * Time.deltaTime);
            charCon.Move(Vector3.up * Physics.gravity.y * Time.deltaTime);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit coll)
    {
        if(coll.collider.transform == player)
        {
            Vector3 dir = player.transform.position - transform.position;
            player.GetComponent<Damageable>().TakeDamage(transform, damage, dir, force);
        }else if(coll.collider.attachedRigidbody != null)
        {
            Vector3 dir = coll.collider.transform.position - transform.position;
            coll.collider.attachedRigidbody.AddForce(dir * 5);
        }
    }

    public GameObject getGameObject()
    {
        return gameObject;
    }

    public void InitiateTransmutation(float duration, GameObject replacement)
    {
        if (!transmuted && transmutable)
        {
            StartCoroutine(transmute(duration, replacement));
        }
    }

    public void setTransmutable(bool newBool)
    {
        transmutable = newBool;
    }

    public void TakeDamage(Transform attacker, int damage, Vector3 dir, float force)
    {
        health -= damage;
        StartCoroutine(knockBack(dir));
        
        if (health <= 0)
        {
            GameObject.Find("Scorekeeper").GetComponent<ScoreKeeper>().incrementScore();
            Destroy(gameObject);
        }
    }

    IEnumerator knockBack(Vector3 dir)
    {
        float startTime = Time.time;
        while (Time.time - startTime < 0.3f)
        {
            charCon.Move(dir * speed * 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator transmute(float duration, GameObject replacement)
    {
        transmuted = true;
        Debug.Log("OH NO, I'VE BEEN TRANSFORMED!");
        charCon.enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        transform.Find("Cube").GetComponent<MeshRenderer>().enabled = false;
        GameObject myReplacement = Instantiate(replacement, transform.position, transform.rotation);
        myReplacement.GetComponent<Damageable>().setTransmutable(false);
        yield return new WaitForSeconds(duration);
        Destroy(myReplacement);
        transform.position = myReplacement.transform.position;
        transform.rotation = myReplacement.transform.rotation;
        charCon.enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        transform.Find("Cube").GetComponent<MeshRenderer>().enabled = true;
        transmuted = false;
    }

    public void vortexGrab(Transform center, float force, float duration)
    {
        if (freeMove)
        {
            freeMove = false;
            StartCoroutine(processVortexGrab(center, force, duration));
        }
    }

    IEnumerator processVortexGrab(Transform center, float force, float duration)
    {
        float startTime = Time.time;
        while(Time.time - startTime < duration)
        {
            if(center != null)
            {
                Vector3 dir = (center.position - transform.position).normalized;
                charCon.Move(dir * force / 5f * Time.deltaTime);
                Vector3 dirEdit = new Vector3(-dir.x, 0, -dir.z);
                transform.rotation = Quaternion.LookRotation(dirEdit);
            }
            yield return new WaitForEndOfFrame();
        }
        freeMove = true;
    }

    public void Blind(float duration, float severity)
    {
        throw new NotImplementedException();
    }

    public void Drunk(float duration)
    {
        throw new NotImplementedException();
    }

    public void Slow(float duration, float severity)
    {
        throw new NotImplementedException();
    }

    public void fly(float force, float duration)
    {

    }

    public void addToSeductionList(GameObject loser)
    {
        throw new NotImplementedException();
    }

    public void Seduce(bool aggro, GameObject target)
    {
        throw new NotImplementedException();
    }

    public void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        throw new NotImplementedException();
    }

    public void setCurrentTarget(List<Damageable> targets)
    {
        throw new NotImplementedException();
    }

    public void setCurrentTarget(List<Damageable> targets, SpellCaster owner)
    {
        throw new NotImplementedException();
    }

    public float getSightRange()
    {
        throw new NotImplementedException();
    }
}
