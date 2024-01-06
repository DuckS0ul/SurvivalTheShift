using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    public Checkpoint reachVillage_Xxx;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            reachVillage_Xxx.isCompleted = true;
        }
    }
}
