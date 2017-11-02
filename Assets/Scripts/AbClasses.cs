using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class damageable : MonoBehaviour
{
    public int max_health;
    int health;
    
    public bool transmutable;

    public bool hurt;
    public float hurtTime;
    public bool dead;

    public Rigidbody rbody;
    
    public virtual void Start()
    {
        Debug.Log("Oh my god!");
        rbody = GetComponent<Rigidbody>();
        hurt = false;
        transmutable = true;
        health = max_health;
    }

    public virtual void Update()
    {
        
    }

    public virtual void TakeDamage(Transform attacker, int damage, Vector3 dir, float force)
    {
        health -= damage;
        if(health <= 0) { dead = true; }
        if(dead)
        {
            StopAllCoroutines();
            Die();
        }
        else
        {
            knockBack(dir, force);
        }
    }

    public virtual void Heal(int recover)
    {
        health += recover;
        if(health > max_health) { health = max_health; }
    }

    public virtual void knockBack(Vector3 dir, float force)
    {
        rbody.AddForce(dir * force, ForceMode.Impulse);
    }

    public virtual void Fly(float force, float duration)
    {

    }

    public virtual void InitiateTransmutation(float duration, GameObject replacement)
    {
        
    }

    public virtual IEnumerator processTransmutation(float duration, GameObject replacement)
    {
        yield return new WaitForSeconds(duration);
    }

    public virtual void setTransmutable(bool newBool)
    {
        transmutable = newBool;
    }

    public virtual void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        
    }

    public virtual void vortexGrab(Transform center, float force, float duration)
    {
        Vector3 dir = (center.position - transform.position).normalized;
        rbody.AddForce(dir * force);
        Vector3 dirEdit = new Vector3(-dir.x, 0, -dir.z);
        transform.rotation = Quaternion.LookRotation(dirEdit);
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}

public abstract class fighter : MonoBehaviour
{
    public virtual void Blind(float duration, float severity)
    {
        Movement myOwnerMove = GetComponent<Movement>();
        myOwnerMove.sightRange = 0f;
        myOwnerMove.changeState(new MeleeEnemyIdle());
    }
    public virtual void Drunk(float duration)
    {
        // myOwnerMove.changeState(new drunkState());
    }
    public virtual void Slow(float duration, float severity)
    {
        Movement myOwnerMove = GetComponent<Movement>();
        myOwnerMove.currSpeed *= 0.5f;
    }
}

public abstract class Movement : MonoBehaviour
{
    public float baseSpeed;
    public float maxSpeed;
    public float currSpeed;
    public float sightRange;
    public float sightAngle;

    public Transform Head;

    public Rigidbody rbody;
    public EnemyData blueprint;
    NPCStateMachine currState;

    public Transform attackTarget;

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        processMovement();
    }

    public virtual void setup()
    {
        blueprint.setup(this);
        rbody = GetComponent<Rigidbody>();
        currSpeed = baseSpeed;
        // setup currState
    }

    public virtual void processMovement()
    {
        if(currState != null) { currState.Execute(); }
    }

    public virtual bool checkView()
    {
        if(attackTarget == null) { return false; }
        float dist = Vector3.Distance(transform.position, attackTarget.position);
        float angle = Vector3.Angle(Head.forward, attackTarget.position - Head.position);
        if(dist <= sightRange && angle <= sightAngle)
        {
            RaycastHit rayHit;
            if (Physics.Raycast(transform.position, (attackTarget.position - transform.position).normalized, out rayHit, sightRange))
            {
                if (rayHit.collider.transform == attackTarget)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public virtual void changeState(NPCStateMachine newState)
    {
        if(currState != null) { currState.Exit(); }
        currState = newState;
        currState.Enter(this);
    }
}

public abstract class NPCState
{
    public virtual void Enter()
    {

    }

    public virtual void Execute()
    {

    }

    public virtual void Exit()
    {

    }
}
