using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] GameObject[] waypoints;
    int currentIndex = 0;
    bool playerOnPlatform = false;

    [SerializeField] float speed = 1f;

    void FixedUpdate()
    {
        if(playerOnPlatform)
        {
            MoveToNextWaypoint();
        }
    }

    void MoveToNextWaypoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentIndex].transform.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, waypoints[currentIndex].transform.position) < .1f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = 0;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    { 
        if (collision.gameObject.name == "Player")
        {
            playerOnPlatform = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerOnPlatform = false;
        }
    }
}
