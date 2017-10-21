using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour, Fighter, Damageable {

    #region Variables and Stuff
    CharacterController charCon;
    [SerializeField] public const int MAX_HEALTH = 100;
    [SerializeField] int health;
    public Image healthBar;
    public CameraMovement HeadMove;

    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    public const float MAX_JUMP_FORCE = 10f;
    public bool jumping;
    
    bool transmuted;
    bool transmutable;
    
    int drunkMod = 1;
    CameraMovement camScript;
    public GameObject DrunkHead;
    public Image shroud;

    bool inVortex;
    bool isFlying;

    #region Status Effect Coroutines

    Coroutine blindness;
    float blindnessSeverity = 0f;

    Coroutine drunkness;

    Coroutine slowness;
    float slownessSeverity = 1f;

    Coroutine flight;
    #endregion

    #endregion
    // Use this for initialization

    #region Start and Update functions
    void Start() {
        charCon = GetComponent<CharacterController>();
        health = MAX_HEALTH;
        inVortex = false;
        isFlying = false;
    }

    // Update is called once per frame
    void Update() {
        float horizontal = Input.GetAxis("Horizontal"); // Get player inputs
        float vertical = Input.GetAxis("Vertical"); // Get player inputs

        healthBar.fillAmount = (float)health / (float)MAX_HEALTH;

        if (!isFlying)
        {
            if (Input.GetKeyDown(KeyCode.Space) && charCon.isGrounded) { jump(); }

            charCon.Move(transform.forward * vertical * speed * Time.deltaTime * slownessSeverity * drunkMod);
            charCon.Move(transform.right * horizontal * speed * Time.deltaTime * slownessSeverity * drunkMod);
            charCon.Move(Vector3.up * jumpForce * Time.deltaTime);
            if (jumping) { applyGravity(); }
        }
    }

    #endregion

    #region Jump functions
    void jump()
    {
        jumpForce = MAX_JUMP_FORCE;
        jumping = true;
    }

    void applyGravity()
    {
        jumpForce += Time.deltaTime * Physics.gravity.y;
        if (jumpForce <= Physics.gravity.y)
        {
            jumping = false;
        }
    }
    #endregion

    #region Fighter Implementations
    public void Blind(float duration, float severity)
    {
        blindnessSeverity += severity;
        if(blindnessSeverity > 1) { blindnessSeverity = 1; }
        if(blindness != null) { StopCoroutine(blindness); }
        blindness = StartCoroutine(processBlindness(duration, blindnessSeverity));
    }

    IEnumerator processBlindness(float duration, float severity)
    {
        float startTime = Time.time;
        shroud.color = new Color(0, 0, 0, severity);
        while(Time.time - startTime < duration)
        {
            yield return new WaitForEndOfFrame();
        }
        blindnessSeverity = 0;
        StartCoroutine(recoverSight());
    }

    IEnumerator recoverSight()
    {
        float alpha = shroud.color.a;
        while(alpha > 0)
        {
            alpha -= Time.deltaTime * 0.5f;
            shroud.color = new Color(0, 0, 0, alpha);
            yield return new WaitForEndOfFrame();
        }
    }

    public void Drunk(float duration)
    {
        if(drunkness != null) { StopCoroutine(drunkness); }
        drunkness = StartCoroutine(processDrunk(duration));
    }
    IEnumerator processDrunk(float duration)
    {
        // drunkMod = -1;
        DrunkHead.SetActive(true);
        yield return new WaitForSeconds(duration);
        DrunkHead.SetActive(false);
        // drunkMod = 1;
    }
    public void Slow(float duration, float severity)
    {
        slownessSeverity -= severity;
        if(slownessSeverity <= 0f) { slownessSeverity = 0.25f; }
        if(slowness != null) { StopCoroutine(slowness); }
        slowness = StartCoroutine(processSlow(duration, slownessSeverity));
    }
    IEnumerator processSlow(float duration, float severity)
    {
        yield return new WaitForSeconds(duration);
        StartCoroutine(recoverSpeed());
    }
    IEnumerator recoverSpeed()
    {
        while(slownessSeverity < 1f)
        {
            slownessSeverity += Time.deltaTime * 0.5f;
            if(slownessSeverity > 1f) { slownessSeverity = 1f; }
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region Damageable Implementations

    public void TakeDamage(int damage, Vector3 dir, float force)
    {
        health -= damage;
        Debug.Log("OUCH! OH NO!");
        if(health <= 0)
        {
            StopAllCoroutines();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("Lose");
        }
        StartCoroutine(knockBack(dir));
    }

    IEnumerator knockBack(Vector3 dir)
    {
        float startTime = Time.time;
        jumping = true;
        jumpForce = 2f;
        while(Time.time - startTime < 0.3f)
        {
            charCon.Move(dir * speed * 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public GameObject getGameObject()
    {
        return gameObject;
    }

    public void InitiateTransmutation(float duration, GameObject replacement)
    {
        if (!transmuted)
        {
            StartCoroutine(transmute(duration, replacement));
        }
    }
    public void setTransmutable(bool newBool)
    {
        transmutable = newBool;
    }
    public IEnumerator transmute(float duration, GameObject replacement)
    {
        // Attach player script to replacement object
        // Attach character controller to replacement object
        // Store original player object in memory
        // GameObject temporary = Instantiate(replacement);
        transmuted = true;
        float startTime = Time.time;
        while(Time.time - startTime < duration)
        {
            // Display duration data
            yield return new WaitForEndOfFrame();
        }
        // Restore player scripts to controller
        // Move player to location of transmuted object
        // Destroy(temporary);
        transmuted = false;
    }

    public void vortexGrab(Transform center, float force, float duration)
    {
        Vector3 dir = (center.position - transform.position).normalized;
        charCon.Move(dir * force / 5f * Time.deltaTime);
        /*
        if (!inVortex)
        {
            inVortex = true;
            StartCoroutine(processVortexGrab(center, force, duration));
        }
        */
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
                yield return new WaitForEndOfFrame();
            }
        }
        inVortex = false;
    }

    public void fly(float force, float duration)
    {
        if (flight != null) { StopCoroutine(flight); }
        flight = StartCoroutine(processFlying(force, duration));
    }

    IEnumerator processFlying(float force, float duration)
    {
        float startTime = Time.time;
        isFlying = true;
        HeadMove.separateControl = false;
        while(Time.time - startTime < duration)
        {
            if (charCon.enabled)
            {
                charCon.Move(transform.forward * force * Time.deltaTime);
            }
            yield return new WaitForEndOfFrame();
        }
        HeadMove.separateControl = true;
        isFlying = false;
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }

    public void Seduce(float duration, GameObject target)
    {
        throw new NotImplementedException();
    }

    public void addToSeductionList(GameObject loser)
    {
        throw new NotImplementedException();
    }

    public void Seduce(bool aggro, GameObject target)
    {
        throw new NotImplementedException();
    }

    public void setCurrentTarget(List<Damageable> targets)
    {
        throw new NotImplementedException();
    }

    public void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        throw new NotImplementedException();
    }

    public void setCurrentTarget(List<Damageable> targets, SpellCaster owner)
    {
        throw new NotImplementedException();
    }
    #endregion
}
