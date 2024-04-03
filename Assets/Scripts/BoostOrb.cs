using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoostOrb : MonoBehaviour
{
    public BoostManager.BoostType orbType;
        
    public Color healColor = Color.green;
    public Color speedColor = Color.yellow;
    public Color damageColor = Color.red;
    private void Start()
    {
        SetOrb();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BoostManager.instance.ActivateBoost(orbType);
            Destroy(gameObject); // Destroy the orb after triggering the boost
        }
    }

    private void SetOrb()
    {
        orbType = (BoostManager.BoostType)Random.Range(0, System.Enum.GetValues(typeof(BoostManager.BoostType)).Length);
        switch (orbType)
        {
            case BoostManager.BoostType.Speed:
                gameObject.GetComponent<Renderer>().material.color = speedColor;
                break;
            case BoostManager.BoostType.Damage:
                gameObject.GetComponent<Renderer>().material.color = damageColor;
                break;
        }
    }
}

