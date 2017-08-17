using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    private float health = 100f;   
    public bool bulletHit;
    public GameObject health30;
    public GameObject health70;
    public GameObject health100;
    private float waitTimer = 0;

    void Start()
    {
        health100.SetActive(false);
        health70.SetActive(false);
        health30.SetActive(false);
    }
    void Update()
    {
        if (!bulletHit)
        {
            waitTimer = 0;
            health++;
            if (health > 100)
            {
                health = 100;           
            }
            if (health > 50)
            {
                health100.SetActive(false);
                health70.SetActive(true);
            }
            if (health > 70)
            {
                health70.SetActive(false);
                health30.SetActive(true);
            }
        }
        if (health == 100)
        {
            health30.SetActive(false);
        }    
        if (bulletHit)
        {
            
            waitTimer++;
        } 
        if (waitTimer > 100)
        {
            waitTimer = 100;
            bulletHit = false;
        } 
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Hit by Bullet");
        health -= amount;
        waitTimer = 0;
        if (health < 100)
        {
            health30.SetActive(true);
        }
        if (health < 70)
        {
            health70.SetActive(true);
            health30.SetActive(false);
        }
        if (health < 50)
        {
            health100.SetActive(true);
            health70.SetActive(false);
        }
        if (health <= 0)
        {
            health = 0;
        }
    }  
}
