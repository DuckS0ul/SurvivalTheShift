using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTriggerScript : MonoBehaviour
{
    private BearMovement bearMovement;

    private float declineInterval = 0.5f;
    private float curTime = 0;
    private bool playerInRange = false;

    private void Start()
    {
        bearMovement = GetComponentInParent<BearMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if(playerInRange)
        {
            curTime += Time.deltaTime;
            if(curTime >= declineInterval)
            {
                curTime = 0;
                bearMovement.AttackPlayer();
            }
        }
    }


}
