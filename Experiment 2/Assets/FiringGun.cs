using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringGun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera cam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public float impactForce = 30f;
    public float fireRate = 15f;
    public bool ve;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    public AudioClip hapticclip;
    private float nextTimeToFire = 0f;
    public AudioSource audioPulse;
    

	// Use this for initialization
	void Start ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        audioPulse = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
	}
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") || controller.GetPressDown(triggerButton)) // && Time.time >= nextTimeToFire)
        {
            //   nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            audioPulse.clip = hapticclip;
            audioPulse.Play();
        }
    }
    void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if (!ve)
        {
            if (Physics.Raycast(transform.position, cam.transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);


                Target target = hit.transform.GetComponent<Target>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }
                GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGo, 2f);
            }
        }
        if (ve)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);
                Target target = hit.transform.GetComponent<Target>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }
                GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGo, 2f);
            }
        }
    }
}
