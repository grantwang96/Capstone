using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarOfDoom : MonoBehaviour {
    
    public int damage; // over time
    public float radius;
    public float force = 10f;
    float startTime;
    float duration;

    ParticleSystem partSys;
    public CameraMovement playerHead;
    public float shakeForce;

	// Use this for initialization
	void Start () {
        playerHead = GameObject.Find("PlayerHead").GetComponent<CameraMovement>();
        partSys = GetComponent<ParticleSystem>();
        duration = partSys.startLifetime;
        radius = partSys.shape.radius;
        StartCoroutine(burn());
        StartCoroutine(playerHead.shakeCamera(shakeForce));
	}

    IEnumerator burn()
    {
        startTime = Time.time;
        while(Time.time - startTime < duration)
        {
            float dist = partSys.startSpeed * (Time.time - startTime);
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, radius, Vector3.up, dist);
            foreach(RaycastHit hit in rayHits)
            {
                Damageable dam = hit.collider.GetComponent<Damageable>();
                if(dam != null)
                {
                    Vector3 dir = (hit.collider.transform.position - transform.position).normalized;
                    dam.TakeDamage(damage, dir, force);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        if (partSys.isPlaying) {
            partSys.Stop();
        }
        Destroy(gameObject, duration);
    }
}
