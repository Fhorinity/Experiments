using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public float health = 100f;
    [HideInInspector]
    public bool bulletHit = false;

    void Start()
    {
      
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Hit by Bullet");
        health -= amount;
        if (health < 0)
        {
            Die();
        }
      
    }

    void Die()
    {
        Destroy(gameObject);
    }
    
}
