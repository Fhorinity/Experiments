using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorSweep : MonoBehaviour
{
    public float sonarRange = 5f;
    private float fov = 15;
    public Camera cam;
    public float angle;
    //Changes the spread of raycasts
    private int segments = 30;
    //Segments equals number of lines however incorrect number of segments showing for lines
    private Vector3 targetPos;
    private float pitch;
    private bool shortRangeMode;
    private bool audioOutput;
    private bool hapticOutput;
    private bool dualOutput;
    private int modeCounter;
    public Transform rig;
    public Transform headset;
    private Vector2 axis = Vector2.zero;
    public AudioSource hapticPulse;
    public AudioSource audioPulse;
    public GameObject[] objects;
    public GameObject closest = null;
    private string[] tagsToCheck = { "Frontal_Obstruction", "Overhanging Obstruction" };
    
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId applicationMenu = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchPad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    void Start()
    {
        foreach (string tag in tagsToCheck)
        {
            objects = GameObject.FindGameObjectsWithTag(tag);
        }
        audioPulse = GetComponent<AudioSource>();
        hapticPulse = GetComponent<AudioSource>();
        targetPos = Vector3.zero;
        StartCoroutine("RaySonar");
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        if (controller.GetPressDown(applicationMenu))
        {
            modeCounter = 0;
        }
        if (controller.GetPressDown(triggerButton))
        {
            modeCounter++;
        }
        if (modeCounter == 1)
        {
            audioOutput = true;
            hapticOutput = false;
        }
        if (modeCounter == 2)
        {
            audioOutput = false;
            hapticOutput = true;
        }
        if (modeCounter >= 2)
        {
            modeCounter = 1;
        }
        if (modeCounter == 0)
        {
            audioOutput = false;
            hapticOutput = false;
        }
        if (controller.GetPressDown(gripButton))
        {
            shortRangeMode = !shortRangeMode;
        }
        if (controller.GetPress(touchPad))
        {
            axis = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
            rig.position += (headset.transform.right * axis.x + headset.transform.forward * axis.y) * Time.deltaTime;
        }
        if (controller.GetTouch(touchPad))
        {
            axis = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
            rig.position += (headset.transform.right * axis.x + headset.transform.forward * axis.y) * Time.deltaTime;
        }
    }

    IEnumerator HapticRumble(float length, float strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            controller.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
            yield return null;
        }
    }

    IEnumerator RaySonar()
    {
        while (true)
        {
            for (int i = -segments; i < segments; i++)
            {
                RaycastHit hit;
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0, 0.5f));
                float segmentIndex = Mathf.Abs(i);
                angle = Mathf.Lerp(-fov / 2, fov / 2, segmentIndex / segments);
                yield return null;
                targetPos = (Quaternion.Euler(0, angle, 0) * transform.forward).normalized * sonarRange;

                float closestDistance = Mathf.Infinity; // Don't know if this works
                foreach (GameObject obj in objects)
                {
                    var distance = Vector3.Distance(obj.transform.position, transform.position);
                    if (distance < closestDistance)
                    {
                        closest = obj;
                        closestDistance = distance;
                    }
                } //Don't know if this area works.
              //  distance = Vector3.Distance(targetPos.transform.position, closest.transform.position); // I doubt this will work

                if (Physics.Raycast(ray.origin, targetPos, out hit))
                {
                    if (shortRangeMode)
                    {
                        sonarRange = 10f;
                    }
                    if (!shortRangeMode)
                    {
                        sonarRange = 20f;
                    }
                    if (cam.tag == "Frontal")
                    {
                        Debug.DrawRay(ray.origin, targetPos, Color.red, 0.5f);
                        if (hit.collider.tag == "Frontal_Obstruction")
                        {
                            Debug.Log("Frontal Obstruction");
                            if (audioOutput)
                            {
                                pitch = 1f;
                            }
                            if (hapticOutput)
                            {
                                StartCoroutine(HapticRumble(1, 3000));
                            }
                            if (!audioOutput && !hapticOutput)
                            {
                                pitch = 1f;
                                StartCoroutine(HapticRumble(1, 3000));
                            }
                        }
                        else
                        {
                            StopCoroutine(HapticRumble(1, 3000));
                        }
                    }
                    if (cam.tag == "Upward")
                    {
                        Debug.DrawRay(ray.origin, targetPos, Color.blue, 0.5f);
                        if (hit.collider.tag == "Overhanging_Obstruction")
                        {
                            Debug.Log("Overhanging Obstruction");
                            if (audioOutput)
                            {
                                pitch = 1.4f;
                            }
                            if (hapticOutput)
                            {
                                StartCoroutine(HapticRumble(1, 2000));
                            }
                            if (!audioOutput && !hapticOutput)
                            {
                                pitch = 1.4f;
                                StartCoroutine(HapticRumble(1, 2000));
                            }
                        }
                        else
                        {
                            StopCoroutine(HapticRumble(1, 2000));
                        }
                    }
                    if (cam.tag == "Camera")
                    {
                        Debug.DrawRay(ray.origin, targetPos, Color.green, 0.5f);
                        if (hit.collider.tag == "Pedestrian_Crossing")
                        {
                            Debug.Log("Pedestrian Crossing");
                            if (hit.transform.gameObject.GetComponent<Renderer>().material.color == Color.red)
                            {
                                Debug.Log("Traffic light = Red. Do not Cross");
                            }
                            if (hit.transform.gameObject.GetComponent<Renderer>().material.color == Color.green)
                            {
                                Debug.Log("Traffic light = Green. Safe to Cross");
                            }
                        }
                        if (hit.collider.tag == "Pelican_Crossing")
                        {
                            Debug.Log("Pelican Crossing");
                        }
                        else
                        {
                        }
                    }
                    if (cam.tag == "Downward")
                    {
                        Debug.DrawRay(ray.origin, targetPos, Color.yellow, 0.5f);
                        if (hit.collider.tag != "Ground")
                        {
                            Debug.Log("Kerb or Downward Stairs");
                            if (audioOutput)
                            {
                                pitch = 1.8f;
                            }
                            if (hapticOutput)
                            {
                                StartCoroutine(HapticRumble(1, 1000));
                            }
                            if (!audioOutput && !hapticOutput)
                            {
                                pitch = 1.8f;
                                StartCoroutine(HapticRumble(1, 1000));
                            }
                        }
                        else
                        {
                            StopCoroutine(HapticRumble(1, 1000));
                        }
                    }
                }
            }
        }
    }
}

