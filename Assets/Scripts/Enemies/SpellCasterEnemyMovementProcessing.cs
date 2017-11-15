using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasterEnemyIdle : NPCState
{
    public override void Enter(Movement owner)
    {
        base.Enter(owner);
        myOwner.StartCoroutine(headTurn());
    }

    public override void Execute()
    {
        // Do some head turning
        // Some idle animations
        // Check if player is in sight

        if (myOwner.checkView())
        {
            // change state to aggro
            Debug.Log("I can see you!");
            myOwner.changeState(new SpellCasterEnemyAggro());
            return;
        }

        // Do some head turning
        base.Execute();
        if (Time.time - startIdle >= idleTime)
        {
            myOwner.changeState(new SpellCasterEnemyWander());
        }
    }

    public override void Exit()
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
        while (fracJourney < 1f)
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

public class SpellCasterEnemyWander : NPCState
{
    float wanderTime;
    float startWander;

    bool emergencyTurning = false;

    Vector3 targRotation;
    float maxHeadingChange = 90f;
    Coroutine headingGetter;

    public override void Enter(Movement owner)
    {
        myOwner = owner;
        startWander = Time.time;
        wanderTime = Random.Range(4f, 6f);
        rbody = owner.rbody;
        myOwner.currSpeed = myOwner.baseSpeed;
        headingGetter = myOwner.StartCoroutine(headingProcessing());
    }

    public override void Execute()
    {
        cliffCheck();
        myOwner.transform.eulerAngles = Vector3.Slerp(myOwner.transform.eulerAngles, targRotation, Time.deltaTime * turnDurationTime);
        Vector3 forward = myOwner.transform.TransformDirection(Vector3.forward);
        if (!emergencyTurning) { rbody.MovePosition(myOwner.transform.position + forward * myOwner.currSpeed * Time.deltaTime); }
        if (Time.time - startWander >= wanderTime) { myOwner.changeState(new SpellCasterEnemyIdle()); }
        if (myOwner.checkView())
        {
            // change state to aggro
            myOwner.changeState(new SpellCasterEnemyAggro());
            Debug.Log("I can see you!");
            return;
        }
    }

    public override void Exit()
    {
        if (headingGetter != null) { myOwner.StopCoroutine(headingGetter); }
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

    public IEnumerator headingProcessing()
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
        targRotation = new Vector3(0, heading, 0);
    }

    void emergencyTurn()
    {
        emergencyTurning = true;
        float heading = myOwner.transform.eulerAngles.y;
        int mod = 1;
        if (Random.value < 0.5f) { mod = -1; }
        heading = Mathf.Clamp(heading + 180 * mod, 0, 360f);
        targRotation = new Vector3(0, heading, 0);
    }
}

public class SpellCasterEnemyAggro : NPCState
{
    Transform attackTarget;
    Vector3 lastKnownLocation;
    float lostSightTime;
    float lostSightLimit;
    public override void Enter(Movement owner)
    {
        myOwner = owner;
        myOwner.currSpeed = myOwner.maxSpeed;
        myOwner.Head.forward = myOwner.transform.forward;
        attackTarget = myOwner.attackTarget;
        lastKnownLocation = attackTarget.position;
        lostSightTime = 0f;
        lostSightLimit = 3f;
    }

    public override void Execute()
    {
        float startTime = Time.time;
        lastKnownLocation = attackTarget.position;
        if (myOwner.checkView())
        {
            lostSightTime = 0f;
            Vector3 facingDir = attackTarget.position - myOwner.transform.position;
            facingDir.y = 0;
            myOwner.transform.forward = facingDir;
            myOwner.StartCoroutine(myOwner.attack(attackTarget.position));
            // attack the player's location
        }
        else if(lostSightTime < lostSightLimit)
        {
            /*
            Vector3 facingDir = attackTarget.position - myOwner.transform.position;
            facingDir.y = 0;
            myOwner.transform.forward = facingDir;
            myOwner.StartCoroutine(myOwner.attack(attackTarget.position));
            // attack the player's last known location
            // increment lost sight time
            lostSightTime = Time.time - startTime;
            Debug.Log(lostSightTime);
            */
            lostSightTime = 4f;
        }
        else
        {
            // walk over to last known location
            goToLastLocation();
        }
    }

    void processAttack()
    {

    }

    void goToLastLocation()
    {
        Vector3 myOwnerPos = new Vector3(myOwner.transform.position.x, 0, myOwner.transform.position.z);
        Vector3 lastKnownLoc = new Vector3(lastKnownLocation.x, 0, lastKnownLocation.z);
        Vector3 dir = (lastKnownLoc - myOwnerPos).normalized;
        Quaternion lookDir = Quaternion.LookRotation(dir);
        myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, lookDir, Time.deltaTime * 4f);
        myOwner.Head.forward = myOwner.transform.forward;
        myOwner.rbody.MovePosition(myOwner.transform.position + myOwner.transform.forward * myOwner.currSpeed * Time.deltaTime);
        if (Vector3.Distance(myOwner.transform.position, lastKnownLocation) < 0.4f) // If player is not seen, enter idling stage
        {
            Debug.Log("Guess I couldn't find you.");
            myOwner.changeState(new NPCIdle());
        }
    }

    public override void Exit()
    {
        myOwner.currSpeed = myOwner.baseSpeed;
    }
}

public class SpellCasterEnemySeduced : NPCState
{

}