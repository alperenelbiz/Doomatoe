using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZiplineFollower : MonoBehaviour
{
    public GameObject player;
    [SerializeField] GameObject[] waypoints;
    int currentWaypointIndex = 0;

    [SerializeField] float speed = 1f;


    bool isPlayerOnZipline = false;
    bool canMove = true;

    void Update()
    {
        if (isPlayerOnZipline && canMove)
        {
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < .1f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    canMove = false;
                    currentWaypointIndex = 0;
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, speed * Time.deltaTime);
        }

        if(isPlayerOnZipline && Input.GetKeyDown(KeyCode.Space))
        {
            player.transform.parent = null;
            isPlayerOnZipline = false;
            canMove = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isPlayerOnZipline = true;
            other.gameObject.transform.parent = transform;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isPlayerOnZipline = false;
            other.gameObject.transform.parent = null;
        }
    }
}
