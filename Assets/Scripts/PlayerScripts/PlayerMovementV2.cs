using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementV2 : Movement {

    public int drunkMod;
    public float slownessSeverity;

    public float jumpForce;
    Vector3 moveDir;

	// Use this for initialization
	public override void Start () {
        setup();
	}

    // Update is called once per frame
    public override void Update () {
        processMovement();
	}

    void FixedUpdate()
    {
        if (moveDir != Vector3.zero)
        {
            rbody.MovePosition(rbody.position + moveDir * Time.deltaTime * slownessSeverity * drunkMod);
            // transform.position += moveDir * Time.deltaTime * slownessSeverity;
        }
    }

    public override void setup()
    {
        // Do NOT give this a state machine!
        // Do NOT run base.setup() on this!
        rbody = GetComponent<Rigidbody>();
        currSpeed = maxSpeed;
    }

    public override void processMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // Get player inputs
        float vertical = Input.GetAxis("Vertical"); // Get player inputs

        if (Input.GetKeyDown(KeyCode.Space)) { Jump(); }
        moveDir = ((transform.forward * vertical * currSpeed) + (transform.right * horizontal * currSpeed));
    }

    void Jump()
    {
        RaycastHit rayHit = new RaycastHit();
        float dist = GetComponent<Collider>().bounds.extents.y + 0.25f;
        Debug.DrawLine(transform.position, transform.position + Vector3.down * dist);
        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, dist))
        {
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
