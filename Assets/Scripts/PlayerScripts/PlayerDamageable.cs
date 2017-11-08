using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageable : damageable {

    Coroutine flight;
    Coroutine seduced;
    public Transform playerCanvas;
    public Transform playerCanvasPrefab;

    public Image healthBar;

	// Use this for initialization
	public override void Start () {
        base.Start();
        playerCanvas = Instantiate(playerCanvasPrefab);
	}
	
	// Update is called once per frame
	public override void Update () {
		// update healthbar
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
        // reset game;
    }

    public override void Fly(float force, float duration)
    {
        if(flight != null) { StopCoroutine(flight); }
        flight = StartCoroutine(processFlying(force, duration));
    }

    IEnumerator processFlying(float force, float duration)
    {
        float startTime = Time.time;
        Movement myOwnerMove = GetComponent<Movement>();
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
            yield return new WaitForEndOfFrame();
        }
        flight = null;
    }

    public override void InitiateTransmutation(float duration, GameObject replacement)
    {
        base.InitiateTransmutation(duration, replacement);
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
