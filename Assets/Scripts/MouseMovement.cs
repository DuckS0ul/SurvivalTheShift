using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float YRotation = 0f;

    public Transform cameraTransform;

    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(!InventorySystem.Instance.isOpen && !CraftingSystem.Instance.isOpen && !MenuManager.Instance.isMenuOpen && !DialogSystem.Instance.dialogUIActive && !QuestManager.Instance.isQuestMenuOpen && !StorageManager.Instance.storageUIOpen && !CampfireUIManager.Instance.isUiOpen)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            //control rotation around x axis (Look up and down)
            xRotation -= mouseY;

            //we clamp the rotation so we cant Over-rotate (like in real life)
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // 只旋转相机的上下
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}