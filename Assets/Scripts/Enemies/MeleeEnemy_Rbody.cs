using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy_Rbody : MonoBehaviour, Damageable, Fighter {
    public int MAX_HEALTH = 10;
    public int health;
    public float hurtTime;
    bool hurt;
    
    public Transform attackTarget;

    bool freeMove;

    public int damage;
    public float speed;
    bool ishurting;
    bool dead;

    bool transmuted = false;
    bool transmutable = true;
    bool isSeduced = false;

    Rigidbody rbody;
    Collider myColl;

    #region Status Effect Coroutines

    Coroutine seduced;
    SpellCaster myOwner;

    #endregion

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        myColl = GetComponent<Collider>();
        health = MAX_HEALTH;
        damage = UnityEngine.Random.Range(5, 10);
        attackTarget = GameObject.Find("Player_Rbody").transform;
        freeMove = true;
        hurt = false;
        dead = false;
        ishurting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(attackTarget == null) { attackTarget = GameObject.Find("Player_Rbody").transform; }
        Vector3 playerPos = new Vector3(attackTarget.position.x, 0, attackTarget.position.z);
        Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
        if (/*freeMove && */!isSeduced && !transmuted)
        {
            transform.forward = (playerPos - myPos);
            rbody.MovePosition(rbody.position + transform.forward * speed * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        isSeduced = false;
    }

    void OnCollisionStay(Collision coll)
    {
        Damageable dam = coll.collider.GetComponent<Damageable>();
        if (dam != null && coll.collider.transform == attackTarget)
        {
            Vector3 dir = (attackTarget.position - transform.position).normalized + Vector3.up * 0.25f;
            attackTarget.GetComponent<Damageable>().TakeDamage(transform, damage, dir, 10f);
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
        if (hurt) { return; }
        health -= damage;
        knockBack(dir, force);
        if (health <= 0)
        {
            dead = true;
            GameObject.Find("Scorekeeper").GetComponent<ScoreKeeper>().incrementScore();
            StopAllCoroutines();
            // if(myOwner != null) { myOwner.removeFromSeductionList(GetComponent<Damageable>()); }
            Transform dokiFX = transform.Find("Doki");
            if (dokiFX != null)
            {
                ParticleSystem dFX = dokiFX.GetComponent<ParticleSystem>();
                dFX.Stop();
                dokiFX.parent = null;
                Destroy(dokiFX.gameObject, dFX.startLifetime);
            }
            if(UnityEngine.Random.value < 0.7f) { SpellManager.Instance.dropSpellBook(transform.position); }
            Destroy(gameObject);
        }
        StartCoroutine(recoveryTime());
    }

    IEnumerator tempHurt()
    {
        yield return new WaitForSeconds(0.3f);
    }

    IEnumerator recoveryTime()
    {
        hurt = true;
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        Color originColor = mr.material.color;
        mr.material.color = Color.red;
        yield return new WaitForSeconds(hurtTime);
        mr.material.color = originColor;
        hurt = false;
    }

    void knockBack(Vector3 dir, float force)
    {
        rbody.AddForce(dir * force, ForceMode.Impulse);
    }

    public IEnumerator transmute(float duration, GameObject replacement)
    {
        transmuted = true;
        GetComponent<MeshRenderer>().enabled = false;
        transform.Find("Cube").GetComponent<MeshRenderer>().enabled = false;
        GameObject myReplacement = Instantiate(replacement, transform.position, transform.rotation);
        if(myReplacement.GetComponent<Rigidbody>() != null)
        {
            Debug.Log("Boosh!");
            myReplacement.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }
        myReplacement.GetComponent<Damageable>().setTransmutable(false);
        yield return new WaitForSeconds(duration);
        Destroy(myReplacement);
        transform.position = myReplacement.transform.position;
        transform.rotation = myReplacement.transform.rotation;
        GetComponent<MeshRenderer>().enabled = true;
        transform.Find("Cube").GetComponent<MeshRenderer>().enabled = true;
        transmuted = false;
    }

    public void vortexGrab(Transform center, float force, float duration)
    {
        /*
        if (freeMove){
            StartCoroutine(processVortexGrab(center, force, duration));
        }*/
        Vector3 dir = (center.position - transform.position).normalized;
        rbody.AddForce(dir * force);
        Vector3 dirEdit = new Vector3(-dir.x, 0, -dir.z);
        transform.rotation = Quaternion.LookRotation(dirEdit);
    }

    IEnumerator processVortexGrab(Transform center, float force, float duration)
    {
        float startTime = Time.time;
        freeMove = false;
        transform.parent = center;
        while (Time.time - startTime < duration)
        {
            if (center != null)
            {
                Vector3 dir = (center.position - transform.position).normalized;
                rbody.AddForce(dir * force);
                Vector3 dirEdit = new Vector3(-dir.x, 0, -dir.z);
                transform.rotation = Quaternion.LookRotation(dirEdit);
            }
            yield return new WaitForEndOfFrame();
        }
        freeMove = true;
    }

    public void Blind(float duration, float severity)
    {
        
    }

    public void Drunk(float duration)
    {
        
    }

    public void Slow(float duration, float severity)
    {
        
    }

    public void fly(float force, float duration)
    {

    }

    public void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        myOwner = owner;
        if(seduced != null) { StopCoroutine(seduced); }
        seduced = StartCoroutine(processSeduction(duration, target, owner));
        /*
        isSeduced = true;
        if (aggro) { processSeducedAttack(); }
        else { processSeducedMovement(target); }
        */
    }

    IEnumerator processSeduction(float duration, GameObject target, SpellCaster owner)
    {
        freeMove = false;
        float startTime = Time.time;
        GetComponent<MeshRenderer>().material.color = new Color(255, 0, 155);
        while(Time.time - startTime < duration)
        {
            if(gameObject == null) {
                // myOwner.removeFromSeductionList(this.GetComponent<Damageable>());
                yield break;
            }
            if(target.transform == attackTarget) { processSeducedMovement(target); }
            else { processSeducedAttack(); }
            yield return new WaitForEndOfFrame();
        }
        freeMove = true;
        isSeduced = false;
        // myOwner.removeFromSeductionList(this.GetComponent<Damageable>());
        myOwner = null;
        attackTarget = GameObject.Find("Player_Rbody").transform;
        Transform dokiFX = transform.Find("Doki");
        if(dokiFX != null)
        {
            ParticleSystem dFX = dokiFX.GetComponent<ParticleSystem>();
            dFX.Stop();
            dokiFX.parent = null;
            Destroy(dokiFX.gameObject, dFX.startLifetime);
        }
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    void processSeducedMovement(GameObject target)
    {
        float loverDist = Vector3.Distance(transform.position, target.transform.position);
        Vector3 targetPos = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 moveDir = (targetPos - myPos).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir);
        if (loverDist > 5f)
        {
            rbody.velocity = moveDir * speed;
        }
    }

    void processSeducedAttack()
    {
        if(attackTarget == null) { attackTarget = GameObject.Find("Player_Rbody").transform; }
        float loverDist = Vector3.Distance(transform.position, attackTarget.transform.position);
        Vector3 targetPos = new Vector3(attackTarget.transform.position.x, 0, attackTarget.transform.position.z);
        Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 moveDir = (targetPos - myPos).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir);
        rbody.velocity = moveDir * speed;
    }

    public void setCurrentTarget(List<Damageable> targets, SpellCaster owner)
    {
        myOwner = owner;
        List<Damageable> goodTargets = new List<Damageable>();
        for(int i = 0; i < targets.Count; i++)
        {
            if(targets[i] != GetComponent<Damageable>()) { goodTargets.Add(targets[i]); }
        }
        if(goodTargets.Count > 0)
        {
            Damageable target = goodTargets[UnityEngine.Random.Range(0, goodTargets.Count)];
            attackTarget = target.getGameObject().transform;
        }
    }

    public float getSightRange()
    {
        throw new NotImplementedException();
    }
}
