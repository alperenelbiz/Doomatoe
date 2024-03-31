using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider healthSlider;
    public float maxHealth = 100f;
    public float health;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        Debug.Log(health);
    }

    public void takeDamage(float damage)
    {
        health -= damage;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            takeDamage(10);
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("selam");
        if (collision.gameObject.CompareTag("Bullet")) 
        {
            takeDamage(10); // Decrease the player's health by 10 (adjust as needed)
        }
    }*/
}
