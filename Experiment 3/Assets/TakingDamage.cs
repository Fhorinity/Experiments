using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakingDamage : MonoBehaviour {

    public float health = 100f;
    private float maxHealth = 100f;
    private bool bulletHit = false;
    public Texture tex1;
    public Texture tex2;
    public Texture tex3;
    public Texture tex4;


    // Use this for initialization

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            bulletHit = true;
            Debug.Log("Hit by bullet");
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            bulletHit = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0 && bulletHit)
            health -= Time.deltaTime;
        if (health >= 0 && !bulletHit)
            health += Time.deltaTime;
        if (health > maxHealth)
            health = maxHealth;
    }
    void OnGUI()
    {
        if (health <= 100)
            GUI.DrawTexture(new Rect(0, 0,Screen.width, Screen.height), tex1);
        if (health <= 95)
            GUI.DrawTexture(new Rect(0, 0,Screen.width, Screen.height), tex2);
        if (health <= 90)
            GUI.DrawTexture(new Rect(0, 0,Screen.width, Screen.height), tex3);
        if (health <= 85)
            GUI.DrawTexture(new Rect(0, 0,Screen.width, Screen.height), tex4);
    }
}
