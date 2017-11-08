using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class damageable : MonoBehaviour
{
    public int max_health;
    int health;
    public int getHealth() { return health; }
    
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

    public virtual void TakeDamage(Transform attacker, int hpLost, Vector3 dir, float force)
    {
        health -= hpLost;
        Debug.Log("Ow");
        if(health <= 0) { dead = true; }
        if(dead)
        {
            StopAllCoroutines();
            Die();
            return;
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
    public Animator anim;
    public EnemyData blueprint;
    NPCState currState;
    public int damage;
    public bool attacking;

    public Transform attackTarget;

    public virtual void Start()
    {
        setup();
    }

    public virtual void Update()
    {
        processMovement();
    }

    public virtual void setup()
    {
        attacking = false;
        rbody = GetComponent<Rigidbody>();
        blueprint.setup(this);
        currSpeed = baseSpeed;
        // setup currState
    }

    public virtual void processMovement()
    {
        if(currState != null && !attacking) { currState.Execute(); }
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

    public virtual void changeState(NPCState newState)
    {
        // if(currState != null) { currState.Exit(); }
        currState = newState;
        currState.Enter(this);
    }

    public virtual IEnumerator attack(Vector3 target)
    {
        attacking = true;
        float startTime = Time.time;
        // play attack animation
        anim.Play("Attack");
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return new WaitForEndOfFrame();
        }
        attacking = false;
        // get attack animation length
        // Do attack processing like hitbox, spell spawning, etc.
        // yield return new WaitForSeconds(1f); // set clip length here
    }
}

public abstract class NPCState
{
    public float idleTime;
    public float startIdle;
    public float sightRange;
    public Rigidbody rbody;
    public Movement myOwner;

    public Vector3 forward;
    public Vector3 currRotation;
    public Quaternion targetRotation;

    public float turnDurationTime;
    public float maxAngleChange = 60f;
    public float heading;

    public Animator anim;

    public virtual void Enter(Movement owner)
    {
        myOwner = owner;
        rbody = owner.GetComponent<Rigidbody>();
        idleTime = Random.Range(4f, 6f);
        startIdle = Time.time;
        targetRotation = Quaternion.Euler(0, 0, 0);
        turnDurationTime = Random.Range(1f, 2f);
        anim = myOwner.anim;
        maxAngleChange = 60f;
    }

    public virtual void Execute()
    {
        myOwner.Head.transform.localRotation = Quaternion.Slerp(myOwner.Head.localRotation, targetRotation, Time.deltaTime * turnDurationTime);
        myOwner.transform.eulerAngles = new Vector3(0, myOwner.transform.eulerAngles.y, 0);
    }

    public virtual void Exit()
    {
        // Perform last second actions...
    }
}