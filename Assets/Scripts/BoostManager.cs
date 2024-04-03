using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoostManager : MonoBehaviour
{
    public static BoostManager instance;

    public float dropRate = 25;
    
    [Header("Heal")]
    //Leave 0 for max health
    public float healAmount = 0f;
    
    [Header("Speed")]
    public float speedBoost = 1.5f;
    public float speedBoostDuration = 5f;
    
    [Header("Damage")]
    public float damageBoost = 1.5f;
    public float damageBoostDuration = 5f;
    
    [Header("Active Boosts")]
    public float currentSpeedBoost = 1f;
    public float remainingSpeedBoost = 0f;
    
    public float currentDamageBoost = 1f;
    public float remainingDamageBoost = 0f;

    public enum BoostType
    {
        //Heal,
        Speed,
        Damage
    }

    public GameObject boostOrb;
    private void Awake()
    {
        instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (remainingSpeedBoost > 0f)
        {
            remainingSpeedBoost -= Time.deltaTime;
        }
        else
        {
            currentSpeedBoost = 1f;
        }

        if (remainingDamageBoost > 0f)
        {
            remainingDamageBoost -= Time.deltaTime;
        }
        else
        {
            currentDamageBoost = 1f;
        }
    }
    
    public void ActivateBoost(BoostType type)    //Add heal, life steal, shield, energy, etc.
    {
        switch (type)
        {
            /*case BoostType.Heal:
                break;*/
            case BoostType.Speed:
                currentSpeedBoost = speedBoost;
                remainingSpeedBoost = speedBoostDuration;
                break;
            case BoostType.Damage:
                currentDamageBoost = damageBoost;
                remainingDamageBoost = damageBoostDuration;
                break;
        }
    }
}


