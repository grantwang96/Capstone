using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageable : damageable {

    Coroutine flight;
    Coroutine drunkness;
    Coroutine slowness;
    Coroutine seduced;

    public Transform playerCanvas;
    public Transform playerCanvasPrefab;
    public CameraMovement HeadMove;
    public GameObject DrunkHead;

    public Sprite drunkIcon;
    public Sprite slowIcon;
    public Sprite floatIcon;
    public Image statusEffectPrefab;
    public Transform statusEffectBar;

    public Image healthBar;

	// Use this for initialization
	public override void Start () {
        base.Start();
        playerCanvas = Instantiate(playerCanvasPrefab);
        healthBar = playerCanvas.Find("HealthBar").GetComponent<Image>();
	}
	
	// Update is called once per frame
	public override void Update () {
        // update healthbar
        healthBar.fillAmount = (float)getHealth() / max_health;
	}

    public override void TakeDamage(Transform attacker, int hpLost, Vector3 dir, float force)
    {
        if (hurt) { return; }
        // Visual hurt effects
        base.TakeDamage(attacker, hpLost, dir, force);
        Debug.Log("Player HP: " + getHealth());
        StartCoroutine(hurtFrames());
    }

    IEnumerator hurtFrames()
    {
        hurt = true;
        yield return new WaitForSeconds(hurtTime);
        hurt = false;
    }

    public override void Die()
    {
        
    }

    public override void Fly(float force, float duration)
    {
        if(flight != null) {
            StopCoroutine(flight);
            myMovement.hamper--;
        }
        flight = StartCoroutine(processFlying(force, duration));
    }

    IEnumerator processFlying(float force, float duration)
    {
        Debug.Log("Duration is: " + duration);
        float startTime = Time.time;
        Movement myOwnerMove = GetComponent<Movement>();
        myOwnerMove.hamper++;
        HeadMove.separateControl = false;
        Transform statEffectObj = statusEffectBar.transform.Find("Flight");
        Image newStatusEffect;
        if (statEffectObj == null) {
            newStatusEffect = Instantiate(statusEffectPrefab);
            newStatusEffect.transform.name = "Flight";
            newStatusEffect.sprite = floatIcon;
            newStatusEffect.transform.SetParent(statusEffectBar.transform, false);
        }
        else {
            newStatusEffect = statEffectObj.GetComponent<Image>();
        }
        // UI handling
        rbody.constraints = RigidbodyConstraints.None;
        if (rbody.useGravity)
        {
            rbody.useGravity = false;
            rbody.AddForce(Vector3.up * force * 0.5f, ForceMode.Impulse);
        }
        rbody.angularDrag = 1f;
        while (Time.time - startTime < duration)
        {
            float vertical = Input.GetAxis("Vertical"); // Get player inputs
            float horizontal = Input.GetAxis("Horizontal"); // Get player inputs
            rbody.AddForce(myOwnerMove.Head.transform.forward * vertical * myOwnerMove.currSpeed / 2 + (myOwnerMove.Head.transform.right * horizontal * myOwnerMove.currSpeed / 2));
            if (rbody.velocity.magnitude > myOwnerMove.currSpeed / 2) { rbody.velocity = rbody.velocity.normalized * myOwnerMove.currSpeed / 2; }
            newStatusEffect.fillAmount = 1f - (Time.time - startTime) / duration;
            yield return new WaitForEndOfFrame();
        }
        rbody.useGravity = true;
        rbody.angularDrag = 0.05f;
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        rbody.angularVelocity = Vector3.zero;
        rbody.constraints = RigidbodyConstraints.FreezeRotation;
        HeadMove.separateControl = true;
        HeadMove.transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        Destroy(newStatusEffect.gameObject);
        flight = null;
        myOwnerMove.hamper--;
    }

    public override void Drunk(float duration)
    {
        if(drunkness != null) { StopCoroutine(drunkness); }
        drunkness = StartCoroutine(processDrunk(duration));
    }

    IEnumerator processDrunk(float duration)
    {
        PlayerMovementV2 myMove = GetComponent<PlayerMovementV2>();
        float startTime = Time.time;
        DrunkHead.SetActive(true);
        HeadMove.drunk = true;
        Transform statEffectObject = statusEffectBar.transform.Find("drunk");
        Image newStatusEffect;
        if (statEffectObject == null)
        {
            newStatusEffect = Instantiate(statusEffectPrefab);
            newStatusEffect.transform.name = "drunk";
            newStatusEffect.sprite = drunkIcon;
            newStatusEffect.transform.SetParent(statusEffectBar.transform, false);
        }
        else
        {
            newStatusEffect = statEffectObject.GetComponent<Image>();
        }
        HeadMove.normalMove = -1;
        myMove.drunkMod = -1;

        while (Time.time - startTime < duration)
        {
            newStatusEffect.fillAmount = 1f - (Time.time - startTime) / duration;
            yield return new WaitForEndOfFrame();
        }
        HeadMove.normalMove = 1;
        myMove.drunkMod = 1;
        DrunkHead.SetActive(false);
        HeadMove.drunk = false;
        Destroy(newStatusEffect.gameObject);
        drunkness = null;
    }

    public override void Slow(float duration, float severity)
    {
        if (slowness != null) { StopCoroutine(slowness); }
        slowness = StartCoroutine(processSlowness(duration, severity));
    }

    IEnumerator processSlowness(float duration, float severity)
    {
        PlayerMovementV2 myMove = GetComponent<PlayerMovementV2>();
        myMove.slownessSeverity *= severity;
        if (myMove.slownessSeverity < 0.25f) { myMove.slownessSeverity = 0.25f; }
        float startTime = Time.time;
        Transform statEffectObj = statusEffectBar.transform.Find("Slow");
        Image newStatusEffect;
        if (statEffectObj == null)
        {
            newStatusEffect = Instantiate(statusEffectPrefab);
            newStatusEffect.transform.name = "Slow";
            newStatusEffect.sprite = slowIcon;
            newStatusEffect.transform.SetParent(statusEffectBar.transform, false);
        }
        else
        {
            newStatusEffect = statEffectObj.GetComponent<Image>();
        }
        float originSeverity = myMove.slownessSeverity;
        float full = 1f - myMove.slownessSeverity;
        while (Time.time - startTime < duration)
        {
            myMove.slownessSeverity = originSeverity + ((Time.time - startTime) / duration) * full;
            newStatusEffect.fillAmount = 1f - (myMove.slownessSeverity - originSeverity) / full;
            yield return new WaitForEndOfFrame();
        }
        Destroy(newStatusEffect.gameObject);
        myMove.slownessSeverity = 1f;
        slowness = null;
    }

    public override void InitiateTransmutation(float duration, GameObject replacement)
    {
        StartCoroutine(processTransmutation(duration, replacement));
    }

    public override IEnumerator processTransmutation(float duration, GameObject replacement)
    {
        Collider myColl = GetComponent<Collider>();
        myColl.enabled = false;
        // Deactivate player attack?
        GameObject newBody = Instantiate(replacement, transform);
        newBody.GetComponent<Rigidbody>().isKinematic = true;
        newBody.GetComponent<Rigidbody>().useGravity = false;
        newBody.GetComponent<damageable>().parentHit = this;
        yield return new WaitForSeconds(duration);
        Destroy(newBody);
        myColl.enabled = true;
    }

    public override void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        base.Seduce(duration, target, owner);
    }

    public override void knockBack(Vector3 dir, float force)
    {
        base.knockBack(dir, force);
    }

    public override void vortexGrab(Transform center, float force, float duration)
    {
        base.vortexGrab(center, force, duration);
    }
}
