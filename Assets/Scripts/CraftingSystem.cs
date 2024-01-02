using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI;
    public GameObject survivalScreenUI;
    public GameObject refineScreenUI;
    public GameObject constructionScreenUI;

    public List<string> inventoryItemList;

    //Category Buttons
    Button toolsBTN, survivalBTN, refineBTN, constructionBTN;

    //Craft Buttons
    Button craftAxeBTN, craftPlankBTN, craftWallBTN, craftFoundationBTN;

    //Requirement Text
    TextMeshProUGUI AxeReq1, AxeReq2, PlankReq1, WallReq1, FoundationReq1;

    public bool isOpen;

    //All Blueprint
    public Blueprint AxeBLP = new Blueprint("Axe", 1, 2, "Stone", 2, "Stick", 2);

    public Blueprint PlankBLP = new Blueprint("Plank", 2, 1, "Log", 1, "", 0);

    public Blueprint WallBLP = new Blueprint("Wall", 1, 1, "Plank", 2, "", 0);

    public Blueprint FoundationBLP = new Blueprint("Foundation", 1, 1, "Plank", 4, "", 0);




    public static CraftingSystem Instance { get; set; }


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
        isOpen = false;

        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });

        survivalBTN = craftingScreenUI.transform.Find("SurvivalButton").GetComponent<Button>();
        survivalBTN.onClick.AddListener(delegate { OpenSurvivalCategory(); });

        refineBTN = craftingScreenUI.transform.Find("RefineButton").GetComponent<Button>();
        refineBTN.onClick.AddListener(delegate { OpenRefineCategory(); });

        constructionBTN = craftingScreenUI.transform.Find("ConstructionButton").GetComponent<Button>();
        constructionBTN.onClick.AddListener(delegate { OpenConstructionCategory(); });

        //Axe
        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<TextMeshProUGUI>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<TextMeshProUGUI>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });


        //Plank
        PlankReq1 = refineScreenUI.transform.Find("Plank").transform.Find("req1").GetComponent<TextMeshProUGUI>();

        craftPlankBTN = refineScreenUI.transform.Find("Plank").transform.Find("Button").GetComponent<Button>();
        craftPlankBTN.onClick.AddListener(delegate { CraftAnyItem(PlankBLP); });


        //Wall
        WallReq1 = constructionScreenUI.transform.Find("Wall").transform.Find("req1").GetComponent<TextMeshProUGUI>();

        craftWallBTN = constructionScreenUI.transform.Find("Wall").transform.Find("Button").GetComponent<Button>();
        craftWallBTN.onClick.AddListener(delegate { CraftAnyItem(WallBLP); });


        //Foundation
        FoundationReq1 = constructionScreenUI.transform.Find("Foundation").transform.Find("req1").GetComponent<TextMeshProUGUI>();

        craftFoundationBTN = constructionScreenUI.transform.Find("Foundation").transform.Find("Button").GetComponent<Button>();
        craftFoundationBTN.onClick.AddListener(delegate { CraftAnyItem(FoundationBLP); });


    }


    void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        constructionScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
    }


    void OpenSurvivalCategory()
    {
        craftingScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        constructionScreenUI.SetActive(false);
        survivalScreenUI.SetActive(true);
    }



    void OpenRefineCategory()
    {
        craftingScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        constructionScreenUI.SetActive(false);
        refineScreenUI.SetActive(true);
    }


    void OpenConstructionCategory()
    {
        craftingScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        constructionScreenUI.SetActive(true);
    }



    void CraftAnyItem(Blueprint blueprintToCraft)
    {

        SoundManager.Instance.PlaySound(SoundManager.Instance.craftingSound);


        StartCoroutine(craftedDelayForSound(blueprintToCraft));


        //remove resources from inventory
        if (blueprintToCraft.numOfRequirements == 1 ) 
        {
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
        }
        else if (blueprintToCraft.numOfRequirements == 2 )
        {
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2amount);
        }





        //refresh list
        StartCoroutine(calculate());

        //RefreshNeededItems();







    }


    public IEnumerator calculate()
    {
        yield return 0;
        InventorySystem.Instance.RecalculateList();
        RefreshNeededItems();
    }

    IEnumerator craftedDelayForSound(Blueprint blueprintToCraft)
    {
        yield return new WaitForSeconds(0);

        // Produce item according to blueprint number of items to produce
        for (var i = 0; i < blueprintToCraft.numberOfItemsProduce; i++)
        {
            InventorySystem.Instance.AddToInventory(blueprintToCraft.itemName);
        }
    }




    // Update is called once per frame
    void Update()
    {

        //RefreshNeededItems();

        if (Input.GetKeyDown(KeyCode.C) && !isOpen && !ConstructionManager.Instance.inConstructionMode)
        {

            craftingScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;

            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            survivalScreenUI.SetActive(false);
            refineScreenUI.SetActive(false);
            constructionScreenUI.SetActive(false);

            if (!InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SelectionManager.Instance.EnableSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
            }

            isOpen = false;
        }
    }


    public void RefreshNeededItems()
    {
        int stone_count = 0;
        int stick_count = 0;
        int log_count = 0;
        int plank_count = 0;

        inventoryItemList = InventorySystem.Instance.itemList;

        foreach (string itemName in inventoryItemList)
        {
            switch (itemName)
            {
                case "Stone":
                    stone_count += 1;
                    break;
                case "Stick":
                    stick_count += 1;
                    break;
                case "Log":
                    log_count += 1;
                    break;
                case "Plank":
                    plank_count += 1;
                    break;
            }
        }

        //---- Axe ----//
        AxeReq1.text = "2 Stones [" + stone_count + "]";
        AxeReq2.text = "2 Sticks [" + stick_count + "]";

        if(stone_count >= 2 && stick_count >= 2 && InventorySystem.Instance.CheckSlotsAvailable(1))
        {
            craftAxeBTN.gameObject.SetActive(true);
        }
        else
        {
            craftAxeBTN.gameObject.SetActive(false);
        }


        //---- Plank ----//
        PlankReq1.text = "1 Log [" + log_count + "]";

        if (log_count >= 1 && InventorySystem.Instance.CheckSlotsAvailable(2))
        {
            craftPlankBTN.gameObject.SetActive(true);
        }
        else
        {
            craftPlankBTN.gameObject.SetActive(false);
        }

        //---- Wall ----//
        WallReq1.text = "2 Planks [" + plank_count + "]";

        if (plank_count >= 2 && InventorySystem.Instance.CheckSlotsAvailable(1))
        {
            craftWallBTN.gameObject.SetActive(true);
        }
        else
        {
            craftWallBTN.gameObject.SetActive(false);
        }

        //---- Foundation ----//
        FoundationReq1.text = "4 Planks [" + plank_count + "]";

        if (plank_count >= 4 && InventorySystem.Instance.CheckSlotsAvailable(1))
        {
            craftFoundationBTN.gameObject.SetActive(true);
        }
        else
        {
            craftFoundationBTN.gameObject.SetActive(false);
        }


    }




}
