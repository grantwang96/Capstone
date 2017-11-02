using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyIdle : NPCStateMachine
{
    float idleTime;
    float startIdle;
    float sightRange;
    Rigidbody rbody;
    Movement myOwner;

    Vector3 forward;
    Vector3 currRotation;
    Quaternion targetRotation;

    float turnDurationTime;
    float maxAngleChange = 60f;
    float heading;

    public void Enter(Movement owner)
    {
        myOwner = owner;
        rbody = owner.GetComponent<Rigidbody>();
        idleTime = Random.Range(4f, 6f);
        startIdle = Time.time;
        targetRotation = Quaternion.Euler(0, 0, 0);
        turnDurationTime = Random.Range(1f, 2f);
        myOwner.StartCoroutine(headTurn());
    }

    public void Execute()
    {
        // Do some head turning
        // Some idle animations
        // Check if player is in sight

        if (myOwner.checkView()) {
            // change state to aggro
            Debug.Log("I can see you!");
            myOwner.changeState(new MeleeEnemyChase());
            return;
        }

        // Do some head turning
        myOwner.Head.transform.localRotation = Quaternion.Slerp(myOwner.Head.localRotation, targetRotation, Time.deltaTime * turnDurationTime);
        myOwner.transform.eulerAngles = new Vector3(0, myOwner.transform.eulerAngles.y, 0);
        if (Time.time - startIdle >= idleTime)
        {
            myOwner.changeState(new MeleeEnemyWander());
        }
    }

    public void Exit()
    {
        myOwner.StartCoroutine(returnHeadPos());
    }

    public IEnumerator returnHeadPos()
    {
        Quaternion startRotation = myOwner.Head.transform.localRotation;
        targetRotation = Quaternion.Euler(0, 0, 0);
        float startTime = Time.time;
        float totalJourney = 0.25f;
        float fracJourney = 0;
        while(fracJourney < 1f)
        {
            fracJourney = (Time.time - startTime) / totalJourney;
            myOwner.Head.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, fracJourney);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator headTurn()
    {
        while (true)
        {
            yield return new WaitForSeconds(turnDurationTime);
            getHeadDir();
            turnDurationTime = Random.Range(1f, 2f);
        }
    }

    void getHeadDir()
    {
        heading = myOwner.Head.transform.localEulerAngles.y;
        float floor = Mathf.Clamp(heading - maxAngleChange, -maxAngleChange, maxAngleChange);
        float ceil = Mathf.Clamp(heading + maxAngleChange, -maxAngleChange, maxAngleChange);
        heading = Random.Range(floor, ceil);
        targetRotation = Quaternion.Euler(0, heading, 0);
    }
}

public class MeleeEnemyWander : NPCStateMachine
{
    float wanderTime;
    float startWander;
    float turnDurationTime;
    float sightRange;
    Rigidbody rbody;
    Movement myOwner;

    bool emergencyTurning = false;

    Vector3 targetRotation;
    float maxHeadingChange = 90f;
    Coroutine headingGetter;

    public void Enter(Movement owner)
    {
        myOwner = owner;
        startWander = Time.time;
        wanderTime = Random.Range(4f, 6f);
        rbody = owner.rbody;
        headingGetter = myOwner.StartCoroutine(heading());
    }

    public void Execute()
    {
        cliffCheck();
        myOwner.transform.eulerAngles = Vector3.Slerp(myOwner.transform.eulerAngles, targetRotation, Time.deltaTime * turnDurationTime);
        Vector3 forward = myOwner.transform.TransformDirection(Vector3.forward);
        if (!emergencyTurning) { rbody.MovePosition(myOwner.transform.position + forward * myOwner.currSpeed * Time.deltaTime); }
        if(Time.time - startWander >= wanderTime) { myOwner.changeState(new MeleeEnemyIdle()); }
        if (myOwner.checkView())
        {
            // change state to aggro
            myOwner.changeState(new MeleeEnemyChase());
            Debug.Log("I can see you!");
            return;
        }
    }

    public void Exit()
    {
        if(headingGetter != null) { myOwner.StopCoroutine(headingGetter); }
    }

    void cliffCheck()
    {
        bool problem = false;
        Vector3 dir = (myOwner.transform.forward - myOwner.transform.up);
        Ray ray = new Ray(myOwner.transform.position, dir);
        RaycastHit[] rayHits = Physics.RaycastAll(ray, 3f);
        Debug.DrawRay(myOwner.transform.position, dir);
        if (rayHits.Length > 0)
        {
            foreach (RaycastHit hit in rayHits)
            {
                if (hit.collider.tag == "Wall")
                {
                    problem = true;
                    emergencyTurn();
                    Debug.Log("That's a wall");
                }
            }
        }
        else {
            problem = true;
            Debug.Log("That's a cliff");
        }
        if (problem) { emergencyTurn(); }
        else { emergencyTurning = false; }
    }

    public IEnumerator heading()
    {
        while (true)
        {
            calculateNewHeading();
            yield return new WaitForSeconds(turnDurationTime);
            turnDurationTime = Random.Range(2f, 3f);
        }
    }

    void calculateNewHeading()
    {
        emergencyTurning = false;
        float heading = myOwner.transform.eulerAngles.y;
        float floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        float ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
        targetRotation = new Vector3(0, heading, 0);
    }

    void emergencyTurn()
    {
        emergencyTurning = true;
        float heading = myOwner.transform.eulerAngles.y;
        int mod = 1;
        if(Random.value < 0.5f) { mod = -1; }
        heading = Mathf.Clamp(heading + 180 * mod, 0, 360f);
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
        myOwner.currSpeed = myOwner.maxSpeed;
        myOwner.Head.forward = myOwner.transform.forward;
    }

    public void Execute()
    {
        Debug.Log("I'm chasing you...");
        if (myOwner.checkView())
        {
            Vector3 myOwnerPos = new Vector3(myOwner.transform.position.x, 0, myOwner.transform.position.z);
            Vector3 targetPos = new Vector3(myOwner.attackTarget.position.x, 0, myOwner.attackTarget.position.z);
            Vector3 dir = (targetPos - myOwnerPos).normalized;
            Quaternion lookDir = Quaternion.LookRotation(dir);
            // myOwner.transform.forward = dir;
            myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, lookDir, Time.deltaTime * 4f);
            myOwner.Head.forward = myOwner.transform.forward;
            myOwner.rbody.MovePosition(myOwner.transform.position + myOwner.transform.forward * myOwner.currSpeed * Time.deltaTime);
        }
        else
        {
            myOwner.changeState(new MeleeEnemyIdle());
        }
    }

    public void Exit()
    {
        myOwner.currSpeed = myOwner.baseSpeed;
    }
}
