using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyIdle : NPCStateMachine
{
    float idleTime;
    float startIdle;
    float sightRange;
    Rigidbody rbody;
    Transform attackTarget;
    Movement myOwner;

    public void Enter(Movement owner)
    {
        myOwner = owner;
        rbody = owner.GetComponent<Rigidbody>();
        idleTime = Random.Range(4f, 6f);
        startIdle = Time.time;
        attackTarget = owner.GetComponent<Movement>().attackTarget;
    }

    public void Execute()
    {
        // Do some head turning
        // Some idle animations
        // Check if player is in sight
        
        if(Time.time - startIdle < idleTime)
        {
            myOwner.changeState(new MeleeEnemyWander());
        }
        Debug.Log(myOwner.transform.name + " is Idling...");
    }

    public void Exit()
    {
        Debug.Log("Exiting idling phase...");
    }
}

public class MeleeEnemyWander : NPCStateMachine
{
    float idleTime;
    float startIdle;
    float turnDurationTime;
    float sightRange;
    Rigidbody rbody;
    Transform attackTarget;
    Movement myOwner;

    Vector3 targetRotation;
    float maxHeadingChange;

    public void Enter(Movement owner)
    {
        myOwner = owner;
        startIdle = Time.time; ;
        idleTime = Random.Range(4f, 6f);
        rbody = owner.rbody;
        attackTarget = myOwner.attackTarget;
    }

    public void Execute()
    {
        myOwner.transform.eulerAngles = Vector3.Slerp(myOwner.transform.eulerAngles, targetRotation, Time.deltaTime * turnDurationTime);
        Vector3 forward = myOwner.transform.TransformDirection(Vector3.forward);
        rbody.MovePosition(myOwner.transform.position + forward * myOwner.currSpeed);
    }

    public void Exit()
    {
        Debug.Log("Exiting wander...");
    }

    IEnumerator heading()
    {
        while (true)
        {
            calculateNewHeading();
            yield return new WaitForSeconds(turnDurationTime);
            turnDurationTime = Random.Range(3, 5);
        }
    }

    void calculateNewHeading()
    {
        float heading = myOwner.transform.eulerAngles.y;
        float floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
        targetRotation = new Vector3(0, heading, 0);
    }
}

public class MeleeEnemyChase : NPCStateMachine
{
    float sightRange;
    Rigidbody rbody;
    Transform attackTarget;
    Movement myOwner;

    public void Enter(Movement owner)
    {
        myOwner = owner;
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }
}

public class MeleeEnemyChaseLastPosition : NPCStateMachine
{
    public void Enter(Movement owner)
    {
        
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }
}
