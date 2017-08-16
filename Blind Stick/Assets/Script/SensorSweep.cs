using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorSweep : MonoBehaviour
{
    private float sonarRange;
    private float fov = 15;
    public Camera upwardsCamera;
    public Camera downwardsCamera;
    public Camera frontalCamera;
    public Camera crossingCamera;
    public float angle;
    //Changes the spread of raycasts
    private int segments = 30;
    //Segments equals number of lines however incorrect number of segments showing for lines
    private Vector3 uTargetPos;
    private Vector3 fTargetPos;
    private Vector3 dTargetPos;
    private Vector3 cTargetPos;
    private float pitch;
    private bool shortRangeMode;
    private bool audioOutput;
    private bool hapticOutput;
    private int modeCounter;
    public AudioClip hapticPulse1;
    public AudioClip hapticPulse2;
    public AudioClip hapticPulse3;
    public AudioClip hapticPulse4;

    public AudioClip audioPulse1;
    public AudioClip audioPulse2;
    public AudioClip audioPulse3;
    public AudioClip audioPulse4;
    private Renderer rend;

    public Transform rig;
    public Transform headset;
    private Vector2 axis = Vector2.zero;
    public AudioSource hapticPulse;
    public AudioSource audioPulse;
    
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId applicationMenu = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchPad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    void Start()
    {
        audioPulse = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        hapticPulse = GameObject.FindGameObjectWithTag("Haptic").GetComponent<AudioSource>();
        uTargetPos = Vector3.zero;
        fTargetPos = Vector3.zero;
        dTargetPos = Vector3.zero;
        cTargetPos = Vector3.zero;
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
            if (modeCounter > 2)
            {
                modeCounter = 1;
            }
        }
        if (modeCounter == 0)
        {
            Debug.Log("Dual Output Mode");
            hapticOutput = false;
            audioOutput = false;
        }
        if (modeCounter == 1)
        {
            Debug.Log("Haptic Output Mode");
            hapticOutput = true;
            audioOutput = false;
        }
        if (modeCounter == 2)
        {
            Debug.Log("Audio Output Mode");

            hapticOutput = false;
            audioOutput = true;
        }
        if (controller.GetPressDown(gripButton))
        {
            shortRangeMode = !shortRangeMode;
            Debug.Log(shortRangeMode);
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
    IEnumerator HapticRumble(float length, ushort strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            controller.TriggerHapticPulse(strength);
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
                Ray uRay = upwardsCamera.ViewportPointToRay(new Vector3(0.5f, 0, 0.5f));
                Ray dRay = downwardsCamera.ViewportPointToRay(new Vector3(0.5f, 0, 0.5f));
                Ray cRay = crossingCamera.ViewportPointToRay(new Vector3(0.5f, 0, 0.5f));
                Ray fRay = frontalCamera.ViewportPointToRay(new Vector3(0.5f, 0, 0.5f));
                float segmentIndex = Mathf.Abs(i);
                angle = Mathf.Lerp(-fov / 2, fov / 2, segmentIndex / segments);
                yield return null;
                uTargetPos = (Quaternion.Euler(0, angle, 0) * transform.forward + transform.up).normalized * sonarRange;
                fTargetPos = (Quaternion.Euler(0, angle, 0) * transform.forward).normalized * sonarRange;
                dTargetPos = (Quaternion.Euler(0, angle, 0) * -transform.up).normalized * 2f;
                cTargetPos = (Quaternion.Euler(0, angle, 0) * transform.forward + -transform.up + transform.forward + transform.forward).normalized * sonarRange;

                if (shortRangeMode)
                {
                    sonarRange = 3f;
                }
                if (!shortRangeMode)
                {
                    sonarRange = 15f;
                }
                if (Physics.Raycast(dRay.origin, dTargetPos, out hit))
                {
                    Debug.DrawRay(dRay.origin, dTargetPos, Color.yellow, 0.5f);
                    if (hit.collider.tag != "Ground")
                    {
                        audioPulse.clip = audioPulse1;
                        hapticPulse.clip = hapticPulse1;
                        Debug.Log("Kerb or Downward Stairs");
                        if (audioOutput)
                        {
                            hapticPulse.Stop();
                            audioPulse.Play();
                        }
                        if (hapticOutput)
                        {
                            hapticPulse.Play();
                            audioPulse.Stop();
                            StartCoroutine(HapticRumble(1, 500));
                        }
                        if (!audioOutput && !hapticOutput)
                        {
                            hapticPulse.Play();
                            audioPulse.Play();
                            StartCoroutine(HapticRumble(1, 500));
                        }
                    }
                    else
                    {
                        audioPulse.Stop();
                        hapticPulse.Stop();
                        StopCoroutine(HapticRumble(1, 500));
                    }
                }
                if (Physics.Raycast(fRay.origin, fTargetPos, out hit))
                {
                    Debug.DrawRay(fRay.origin, fTargetPos, Color.red, 0.5f);
                    if (hit.collider.tag == "Frontal_Obstruction")
                    {
                        audioPulse.clip = audioPulse2;
                        hapticPulse.clip = hapticPulse2;
                        Debug.Log("Frontal Obstruction");
                        if (audioOutput)
                        {
                            hapticPulse.Stop();
                            audioPulse.Play();
                        }
                        if (hapticOutput)
                        {
                            hapticPulse.Play();
                            audioPulse.Stop();
                            StartCoroutine(HapticRumble(1, 3999));
                        }
                        if (!audioOutput && !hapticOutput)
                        {
                            hapticPulse.Play();
                            audioPulse.Play();
                            StartCoroutine(HapticRumble(1, 3999));
                        }
                    }
                    else
                    {
                        hapticPulse.Stop();
                        audioPulse.Stop();
                        StopCoroutine(HapticRumble(1, 3999));
                    }
                }
                if (Physics.Raycast(cRay.origin, cTargetPos, out hit))
                {
                    Debug.DrawRay(cRay.origin, cTargetPos, Color.green, 0.5f);

                    if (hit.collider.tag == "Pedestrian_Crossing")
                    {
                        audioPulse.clip = audioPulse3;
                        hapticPulse.clip = hapticPulse3;
                        Debug.Log("Pedestrian Crossing");
                        if (hit.collider.gameObject.GetComponent<Renderer>().material.color == Color.red)
                        {
                            Debug.Log("Traffic light = Red. Do not Cross");
                        }
                        if (hit.collider.gameObject.GetComponent<Renderer>().material.color == Color.green)
                        {
                            Debug.Log("Traffic light = Green. Safe to Cross");
                            if (audioOutput)
                            {
                                hapticPulse.Stop();
                                audioPulse.Play();
                            }
                            if (hapticOutput)
                            {
                                hapticPulse.Play();
                                audioPulse.Stop();
                                StartCoroutine(HapticRumble(1, 1999));
                            }
                            if (!audioOutput && !hapticOutput)
                            {
                                hapticPulse.Play();
                                audioPulse.Play();
                                StartCoroutine(HapticRumble(1, 1999));
                            }
                        }                     
                    }
                    if (hit.collider.tag == "Pelican_Crossing")
                    {
                        Debug.Log("Pelican Crossing");
                        if (hit.collider.tag == "Pelican_Crossing" && hit.collider.tag == "Car")
                        {
                            Debug.Log("Not safe to cross. Car in way");
                        }
                    }
                    else
                    {
                        audioPulse.Stop();
                        hapticPulse.Stop();
                    }
                }
                if (Physics.Raycast(uRay.origin, uTargetPos, out hit))
                {
                    Debug.DrawRay(uRay.origin, uTargetPos, Color.blue, 0.5f);
                    if (hit.collider.tag == "Overhanging_Obstruction")
                    {
                        audioPulse.clip = audioPulse4;
                        hapticPulse.clip = hapticPulse4;
                        Debug.Log("Overhanging Obstruction");
                        if (audioOutput)
                        {
                            audioPulse.Play();
                            hapticPulse.Stop();
                        }
                        if (hapticOutput)
                        {
                            audioPulse.Stop();
                            hapticPulse.Play();
                            StartCoroutine(HapticRumble(1, 20));
                        }
                        if (!audioOutput && !hapticOutput)
                        {
                            hapticPulse.Play();
                            audioPulse.Play();
                            StartCoroutine(HapticRumble(1, 20));
                        }
                    }
                    else
                    {
                        hapticPulse.Stop();
                        audioPulse.Stop();
                        StopCoroutine(HapticRumble(1, 20));
                    }
                }
            }
        }
    }
}

