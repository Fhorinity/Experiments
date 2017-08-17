using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage = 10f;
    public Camera gunCam;
    private float range = 100f;
    private float impactForce = 30f;
    public ParticleSystem muzzleFlash;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    public AudioClip hitClip;
    public AudioSource hitPulse;
    private Vector3 targetPos;
    private Target target;

	void Start ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        hitPulse = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();	
	}

	void Update ()
    {
		if (Input.GetButtonDown("Fire1") || controller.GetPressDown(triggerButton))
        {
            Shoot();
            hitPulse.clip = hitClip;
            hitPulse.Play();
            target.bulletHit = true;
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        Ray ray = gunCam.ViewportPointToRay(new Vector3(0.5f, 0, 0.5f));
        targetPos = (transform.forward).normalized * range;
        Debug.DrawRay(ray.origin, targetPos, Color.yellow);
        if (Physics.Raycast(ray.origin, targetPos, out hit))
        {
            Debug.Log(hit.transform.name);
            target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);            
            }         
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
        }

    }
}
