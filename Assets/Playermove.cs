using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playmove : MonoBehaviour
{
    // Start is called before the first frame update
    public float feed;
    public CharacterController characterController;

    // Update is called once per frame
    void Update()
    {
        float x, z;
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        Vector3 move;
        move = transform.right * x + transform.forward * z;
        characterController.Move(move * feed * Time.deltaTime);
    }
}