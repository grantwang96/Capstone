using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovementRBody : MonoBehaviour, Damageable, Fighter {

    #region Variables and Stuff
    #region Important Stuff
    [SerializeField] public const int MAX_HEALTH = 100;
    [SerializeField] int health;
    public Image healthBar;
    public CameraMovement HeadMove;
    public PlayerAttack myAttackScript;

    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    public const float MAX_JUMP_FORCE = 10f;
    public bool jumping;

    bool hurting;
    public float hurtTime;

    bool transmutable;

    int drunkMod = 1;
    CameraMovement camScript;
    public GameObject DrunkHead;
    public GameObject statusEffectBar;
    public Image statusEffectPrefab;
    public Image shroud;

    Rigidbody rbody;
    #endregion

    #region Important Lists
    public Sprite[] statusEffectIcons;
    #endregion

    #region Status Effects
    bool transmuted;
    bool inVortex;
    bool isFlying;
    bool isSeduced;
    #endregion

    #region Status Effect Coroutines

    Coroutine blindness;
    float blindnessSeverity = 0f;

    Coroutine drunkness;

    Coroutine slowness;
    float slownessSeverity = 1f;

    Coroutine flight;
    Coroutine Seduced;
    SpellCaster myOwner;
    #endregion

    #endregion
    // Use this for initialization

    #region Start and Update functions
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        health = MAX_HEALTH;
        inVortex = false;
        isFlying = false;
        hurting = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Physics.autoSyncTransforms = true;
        float horizontal = Input.GetAxis("Horizontal"); // Get player inputs
        float vertical = Input.GetAxis("Vertical"); // Get player inputs

        healthBar.fillAmount = (float)health / (float)MAX_HEALTH;

        if (!isFlying)
        {
            if (Input.GetKeyDown(KeyCode.Space)) { Jump(); }
            Vector3 moveDir = ((transform.forward * vertical * speed) + (transform.right * horizontal * speed));
            if(moveDir != Vector3.zero)
            {
                rbody.MovePosition(rbody.position + moveDir * Time.deltaTime * slownessSeverity * drunkMod);
                // transform.position += moveDir * Time.deltaTime * slownessSeverity;
            }
        }
    }

    #endregion

    #region Jump functions
    
    void Jump()
    {
        RaycastHit rayHit = new RaycastHit();
        float dist = GetComponent<Collider>().bounds.extents.y + 0.25f;
        Debug.DrawLine(transform.position, transform.position + Vector3.down * dist);
        if(Physics.Raycast(transform.position, Vector3.down, out rayHit, dist))
        {
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    #endregion

    #region Collision and Trigger Functions

    void OnCollisionStay(Collision coll)
    {
        
    }

    void OnCollisionExit(Collision coll)
    {
        
    }

    #endregion

    #region Fighter Implementations
    public void Blind(float duration, float severity)
    {
        blindnessSeverity += severity;
        if (blindnessSeverity > 1) { blindnessSeverity = 1; }
        if (blindness != null) { StopCoroutine(blindness); }
        blindness = StartCoroutine(processBlindness(duration, blindnessSeverity));
    }

    IEnumerator processBlindness(float duration, float severity)
    {
        float startTime = Time.time;
        shroud.color = new Color(0, 0, 0, severity);
        while (Time.time - startTime < duration)
        {
            yield return new WaitForEndOfFrame();
        }
        blindnessSeverity = 0;
        StartCoroutine(recoverSight());
    }

    IEnumerator recoverSight()
    {
        float alpha = shroud.color.a;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * 0.5f;
            shroud.color = new Color(0, 0, 0, alpha);
            yield return new WaitForEndOfFrame();
        }
    }

    public void Drunk(float duration)
    {
        if (drunkness != null) { StopCoroutine(drunkness); }
        drunkness = StartCoroutine(processDrunk(duration));
    }
    IEnumerator processDrunk(float duration)
    {
        float startTime = Time.time;
        DrunkHead.SetActive(true);
        HeadMove.drunk = true;
        Transform statEffectObject = statusEffectBar.transform.Find("drunk");
        Image newStatusEffect;
        if(statEffectObject == null)
        {
            newStatusEffect = Instantiate(statusEffectPrefab);
            newStatusEffect.transform.name = "drunk";
            newStatusEffect.sprite = statusEffectIcons[1];
            newStatusEffect.transform.SetParent(statusEffectBar.transform, false);
        }
        else
        {
            newStatusEffect = statEffectObject.GetComponent<Image>();
        }
        HeadMove.normalMove = -1;
        drunkMod = -1;
        while (Time.time - startTime < duration)
        {
            newStatusEffect.fillAmount = 1f - (Time.time - startTime) / duration;
            yield return new WaitForEndOfFrame();
        }
        HeadMove.normalMove = 1;
        drunkMod = 1;
        DrunkHead.SetActive(false);
        HeadMove.drunk = false;
        Destroy(newStatusEffect.gameObject);
        drunkness = null;
        Debug.Log("Done Recovering Speed");
        // drunkMod = 1;
    }
    public void Slow(float duration, float severity)
    {
        if (slowness != null) { StopCoroutine(slowness); }
        slowness = StartCoroutine(processSlow(duration, severity));
    }
    IEnumerator processSlow(float duration, float severity)
    {
        slownessSeverity *= severity;
        if(slownessSeverity < 0.25f) { slownessSeverity = 0.25f; }
        float startTime = Time.time;
        Transform statEffectObj = statusEffectBar.transform.Find("Slow");
        Image newStatusEffect;
        if(statEffectObj == null)
        {
            newStatusEffect = Instantiate(statusEffectPrefab);
            newStatusEffect.transform.name = "Slow";
            newStatusEffect.sprite = statusEffectIcons[2];
            newStatusEffect.transform.SetParent(statusEffectBar.transform, false);
        }
        else
        {
            newStatusEffect = statEffectObj.GetComponent<Image>();
        }
        float originSeverity = slownessSeverity;
        float full = 1f - slownessSeverity;
        while (Time.time - startTime < duration)
        {
            slownessSeverity = originSeverity + ((Time.time - startTime) / duration) * full;
            newStatusEffect.fillAmount = 1f - (slownessSeverity - originSeverity)/ full;
            yield return new WaitForEndOfFrame();
        }
        Destroy(newStatusEffect.gameObject);
        slownessSeverity = 1f;
        slowness = null;
    }
    IEnumerator recoverSpeed(float duration)
    {
        Debug.Log("Recovering Speed");
        Image newStatusEffect = Instantiate(statusEffectPrefab);
        newStatusEffect.sprite = statusEffectIcons[2];
        newStatusEffect.transform.SetParent(statusEffectBar.transform, false);
        while (slownessSeverity < 1f)
        {
            slownessSeverity += Time.deltaTime/* / (duration - slownessSeverity)*/;
            newStatusEffect.fillAmount = 1f - slownessSeverity;
            yield return new WaitForEndOfFrame();
        }
        Destroy(newStatusEffect.gameObject);
        slownessSeverity = 1f;
        slowness = null;
    }

    public float getSightRange()
    {
        return 0f;
    }
    #endregion

    #region Damageable Implementations

    public void TakeDamage(Transform attacker, int damage, Vector3 dir, float force)
    {
        if (hurting) { return; }
        hurting = true;
        health -= damage;
        if(attacker != transform && attacker != null)
        {
            List<Damageable> hitList = new List<Damageable>();
            if (attacker.GetComponent<Damageable>() != null) { hitList.Add(attacker.GetComponent<Damageable>()); }
            if (hitList.Count > 0) {
                myAttackScript.GetComponent<SpellCaster>().getHitList(hitList, myAttackScript.GetComponent<SpellCaster>()); }
        }
        if (health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("Lose");
        }
        knockBack(dir, force);
        StartCoroutine(hurtFrames());
    }

    IEnumerator hurtFrames()
    {
        hurting = true;
        yield return new WaitForSeconds(hurtTime);
        hurting = false;
    }

    void knockBack(Vector3 dir, float force)
    {
        rbody.AddForce(dir * force, ForceMode.Impulse);
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
        while (Time.time - startTime < duration)
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
        rbody.AddForce(dir * force);
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
        while (Time.time - startTime < duration)
        {
            if (center != null)
            {
                Vector3 dir = (center.position - transform.position).normalized;
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
        rbody.constraints = RigidbodyConstraints.None;
        Transform statEffectObj = statusEffectBar.transform.Find("Flight");
        Image newStatusEffect;
        if(statEffectObj == null) {
            newStatusEffect = Instantiate(statusEffectPrefab);
            newStatusEffect.transform.name = "Flight";
            newStatusEffect.sprite = statusEffectIcons[0];
            newStatusEffect.transform.SetParent(statusEffectBar.transform, false);
        }
        else
        {
            newStatusEffect = statEffectObj.GetComponent<Image>();
        }
        if (rbody.useGravity)
        {
            rbody.useGravity = false;
            rbody.AddForce(Vector3.up * force * 0.5f, ForceMode.Impulse);
        }
        rbody.angularDrag = 1f;
        // rbody.angularVelocity = new Vector3(Random.value, Random.value, Random.value) * Random.Range(-force, force);
        while(Time.time - startTime < duration)
        {
            float vertical = Input.GetAxis("Vertical"); // Get player inputs
            float horizontal = Input.GetAxis("Horizontal"); // Get player inputs
            rbody.AddForce(HeadMove.transform.forward * vertical * speed / 2 + (HeadMove.transform.right * horizontal * speed / 2));
            if(rbody.velocity.magnitude > speed / 2) { rbody.velocity = rbody.velocity.normalized * speed / 2; }
            /*
            Vector3 locVel = transform.InverseTransformDirection(rbody.velocity);
            if (locVel.z > speed / 2)
            {
                rbody.velocity = transform.forward * vertical * speed / 2;
            }*/
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
        isFlying = false;
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        Destroy(newStatusEffect.gameObject);
        flight = null;
    }

    public void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        if(Seduced != null) { StopCoroutine(Seduced); }
        Seduced = StartCoroutine(processSeduction(duration, target));
        /*
        Vector3 targetPos = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetDir = (targetPos - myPos).normalized;
        rbody.velocity = targetDir * speed;
        */
    }

    IEnumerator processSeduction(float duration, GameObject target)
    {
        SpellCaster caster = target.GetComponent<SpellCaster>();
        myAttackScript.enabled = false;
        float startTime = Time.time;
        while(Time.time - startTime < duration)
        {
            Vector3 targetPos = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetDir = (targetPos - myPos).normalized;
            rbody.velocity = targetDir * speed;
            yield return new WaitForEndOfFrame();
        }
        myAttackScript.enabled = true;
        caster.removeFromSeductionList(this.GetComponent<Damageable>());
    }

    public void setCurrentTarget(List<Damageable> targets, SpellCaster owner)
    {
        
    }
    #endregion
}
