using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{

    public static PlayerState Instance { get; set; }

    public GameObject deathUI;

    // ---- Player Health ----//
    public float currentHealth;
    public float maxHealth;




    // ---- Player Calories ----//
    public float currentCalories;
    public float maxCalories;

    float distanceTravelled = 0;
    Vector3 lastPosition;

    public GameObject playerBody;


    // ---- Player Hydration ----//
    public float currentHydrationPercent;
    public float maxHydrationPercent;

    public bool isHydrationActive;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydrationPercent = maxHydrationPercent;


        StartCoroutine(decreaseHydration());
    }





    IEnumerator decreaseHydration()
    {
        while (true)
        {
            currentHydrationPercent -= 1;
            yield return new WaitForSeconds(8);
        }
    }






    // Update is called once per frame
    void Update()
    {
        distanceTravelled += Vector3.Distance(playerBody.transform.position, lastPosition);
        lastPosition = playerBody.transform.position;

        if(distanceTravelled >= 10)
        {
            distanceTravelled = 0;
            currentCalories -= 1;
        }



        //Testing the HealthBar
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentHealth -= 10;
        }

        if (currentHealth <= 0)
        {
            Die();
        }


    }

    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }


    public void setCalories(float newCalories)
    {
        currentCalories = newCalories;
    }



    public void setHydration(float newHydration)
    {
        currentHydrationPercent = newHydration;
    }



    private void Die()
    {
        deathUI.SetActive(true); // ��ʾ����UI����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SelectionManager.Instance.DisableSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;                         // ������ҿ��ƽű�������еĻ���
                                                                                                            // playerController.enabled = false; // ��������һ����ΪplayerController�Ŀ��ƽű�
    }


    public void LoadMainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // ������������Ϊ "MainScene"
    }



}
