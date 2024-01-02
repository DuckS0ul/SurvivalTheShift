using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HydrationBar : MonoBehaviour
{
    private Slider slider;
    public TextMeshProUGUI HydrationCounter;


    public GameObject playerState;

    private float currentHydrationPercent, maxHydrationPercent;




    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHydrationPercent = playerState.GetComponent<PlayerState>().currentHydrationPercent;
        maxHydrationPercent = playerState.GetComponent<PlayerState>().maxHydrationPercent;

        float fillValue = currentHydrationPercent / maxHydrationPercent;
        slider.value = fillValue;

        HydrationCounter.text = currentHydrationPercent + "%";


    }
}
