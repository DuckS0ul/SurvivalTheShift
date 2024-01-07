using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTriggerScript : MonoBehaviour
{
    private BearMovement bearMovement;

    private void Start()
    {
        bearMovement = GetComponentInParent<BearMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bearMovement.PlayerEnteredRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bearMovement.PlayerExitedRange();
        }
    }
}
