using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSensor : MonoBehaviour {

    public float distance = 5f;
    private float fov = 15;
    public Camera cam;
    public float angle;
    //Changes the spread of raycasts
    private int segments = 30;
    //Segments equals number of lines however incorrect number of segments showing for lines
    public bool enemyHit = false;
    private Vector3 targetPos;

    void Start()
    {
        targetPos = Vector3.zero;
        StartCoroutine("RaySonar");
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
                targetPos = (Quaternion.Euler(0, angle, 0) * transform.forward).normalized * distance;
                if (Physics.Raycast(ray.origin, targetPos, out hit))
                {
                    if (hit.collider.tag == "Pedestrian_Crossing")
                    {
                        Debug.Log("Pedestrian Crossing");
                        enemyHit = true;
                    }
                    if (hit.collider.tag == "Pelican_Crossing")
                    {
                        Debug.Log("Pelican Crossing");
                        enemyHit = true;
                    }
                }
                Debug.DrawRay(ray.origin, targetPos, Color.green, 0.5f);
            }
        }
    }
}
