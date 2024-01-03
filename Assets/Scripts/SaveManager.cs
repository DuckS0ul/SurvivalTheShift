using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{

    public static SaveManager Instance { get; set; }

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

        DontDestroyOnLoad(gameObject);
    }


    // Json Project Save Path
    string jsonPathProject;

    // Json External/Real Save Path
    string jsonPathPersistent;

    // Binary Project Save Path
    string binaryPath;


    string fileName = "SaveGame";

    public bool isSavingToJson;

    public bool isLoading;

    public Canvas loadingScreen;

    private void Start()
    {
        jsonPathProject = Application.dataPath + Path.AltDirectorySeparatorChar;

        jsonPathPersistent = Application.persistentDataPath + Path.AltDirectorySeparatorChar;

        binaryPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
    }

    #region || --------------- General Section --------------- ||


    #region || --------------- Saving --------------- ||
    public void SaveGame(int slotNumber)
    {
        AllGameData data = new AllGameData();
        
        data.playerData = GetPlayerData();

        data.environmentData = GetEnvironmentData();

        SavingTypeSwitch(data, slotNumber);
    }

    private EnvironmentData GetEnvironmentData()
    {
        List<string> itemsPickedup = InventorySystem.Instance.itemsPickedup;

        return new EnvironmentData(itemsPickedup);
    }


    private PlayerData GetPlayerData()
    {
        float[] playerStats = new float[3];
        playerStats[0] = PlayerState.Instance.currentHealth;
        playerStats[1] = PlayerState.Instance.currentCalories;
        playerStats[2] = PlayerState.Instance.currentHydrationPercent;

        float[] playerPosAndRot = new float[6];
        playerPosAndRot[0] = PlayerState.Instance.playerBody.transform.position.x;
        playerPosAndRot[1] = PlayerState.Instance.playerBody.transform.position.y;
        playerPosAndRot[2] = PlayerState.Instance.playerBody.transform.position.z;

        playerPosAndRot[3] = PlayerState.Instance.playerBody.transform.rotation.x;
        playerPosAndRot[4] = PlayerState.Instance.playerBody.transform.rotation.y;
        playerPosAndRot[5] = PlayerState.Instance.playerBody.transform.rotation.z;

        string[] inventory = InventorySystem.Instance.itemList.ToArray();

        string[] quickSlots = GetQuickSLotsContent();

        return new PlayerData(playerStats, playerPosAndRot, inventory, quickSlots);
    }

    private string[] GetQuickSLotsContent()
    {
        List<string> temp = new List<string>();

        foreach (GameObject slot in EquipSystem.Instance.quickSlotsList)
        {
            if (slot.transform.childCount != 0)
            {
                string name = slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string cleanName = name.Replace(str2, "");
                temp.Add(cleanName);
            }
           
        }

        return temp.ToArray();
    }


    public void SavingTypeSwitch(AllGameData gameData, int slotNumber)
    {
        if (isSavingToJson)
        {
            SaveGameDataToJsonFile(gameData, slotNumber);
        }
        else
        {
            SaveGameDataToBinaryFile(gameData, slotNumber);
        }
    }

    #endregion

    #region || --------------- Loading --------------- ||

    public AllGameData LoadingTypeSwitch(int slotNumber)
    {
        if (isSavingToJson)
        {
            AllGameData gameData = LoadGameDataFromJsonFile(slotNumber);
            return gameData;
        }
        else
        {
            AllGameData gameData = LoadGameDataFromBinaryFile(slotNumber);
            return gameData;
        }
    }


    public void LoadGame(int slotNumber)
    {
        //Player Data
        SetPlayerData(LoadingTypeSwitch(slotNumber).playerData);

        //Environment Data
        SetEnvironmentData(LoadingTypeSwitch(slotNumber).environmentData);

        isLoading = false;

        DisableLoadingScreen();
    }

    private void SetEnvironmentData(EnvironmentData environmentData)
    {
        foreach (Transform itemType in EnvironmentManager.Instance.allItems.transform)
        {
            foreach (Transform item in itemType.transform)
            {
                if (environmentData.pickedupItems.Contains(item.name))
                {
                    Destroy(item.gameObject);
                }
            }
        }

        InventorySystem.Instance.itemsPickedup = environmentData.pickedupItems;
    }



    private void SetPlayerData(PlayerData playerData)
    {
        // Setting Player Stats
        PlayerState.Instance.currentHealth = playerData.playerStats[0];
        PlayerState.Instance.currentCalories = playerData.playerStats[1];
        PlayerState.Instance.currentHydrationPercent = playerData.playerStats[2];

        // Setting Player Position
        Vector3 loadedPosition;
        loadedPosition.x = playerData.playerPositionAndRotation[0];
        loadedPosition.y = playerData.playerPositionAndRotation[1];
        loadedPosition.z = playerData.playerPositionAndRotation[2];

        PlayerState.Instance.playerBody.transform.position = loadedPosition;

        // Setting Player Rotation
        Vector3 loadedRotation;
        loadedRotation.x = playerData.playerPositionAndRotation[3];
        loadedRotation.y = playerData.playerPositionAndRotation[4];
        loadedRotation.z = playerData.playerPositionAndRotation[5];

        PlayerState.Instance.playerBody.transform.rotation = Quaternion.Euler(loadedRotation);

        // Setting Player Inventory Content
        foreach (string item in playerData.inventoryContent)
        {
            InventorySystem.Instance.AddToInventory(item);
        }

        foreach (string item in playerData.quickSlotsContent)
        {
            GameObject availableSlot = EquipSystem.Instance.FindNextEmptySlot();

            var itemToAdd = Instantiate(Resources.Load<GameObject>(item));

            itemToAdd.transform.SetParent(availableSlot.transform, false);
        }

    }

    public void StartLoadedGame(int slotNumber)
    {
        ActivateLoadingScreen();

        isLoading = true;

        SceneManager.LoadScene("GameScene");

        StartCoroutine(DelayedLoading(slotNumber));
    }

    private IEnumerator DelayedLoading(int slotNumber)
    {
        yield return new WaitForSeconds(1f);

        LoadGame(slotNumber);
    }

    #endregion

    #endregion


    #region || --------------- To Binary Section --------------- ||

    public void SaveGameDataToBinaryFile(AllGameData gameData, int slotNumber)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(binaryPath + fileName + slotNumber + ".bin", FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Data Saved to " + binaryPath + fileName + slotNumber + ".bin");

    }

    public AllGameData LoadGameDataFromBinaryFile(int slotNumber)
    {

        if (File.Exists(binaryPath + fileName + slotNumber + ".bin"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(binaryPath + fileName + slotNumber + ".bin", FileMode.Open);

            AllGameData data = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            print("Data Loaded from " + binaryPath);

            return data;
        }
        else
        {
            return null;
        }
    }

    #endregion


    #region || --------------- To Json Section --------------- ||

    public void SaveGameDataToJsonFile(AllGameData gameData, int slotNumber)
    {
        string json = JsonUtility.ToJson(gameData);

        //string encrypted = EncryptionDecryption(json);

        using (StreamWriter writer = new StreamWriter(jsonPathProject + fileName + slotNumber + ".json"))
        {
            writer.Write(json);
            print("Saved Game to Json file at :" + jsonPathProject + fileName + slotNumber + ".json");
        };

    }

    public AllGameData LoadGameDataFromJsonFile(int slotNumber)
    {
        using (StreamReader reader = new StreamReader(jsonPathProject + fileName + slotNumber + ".json"))
        {
            string json = reader.ReadToEnd();

            //string decrypted = EncryptionDecryption(json);

            AllGameData data = JsonUtility.FromJson<AllGameData>(json);
            return data;
        };
    }

    #endregion


    #region || --------------- Settings Section --------------- ||

    #region || --------------- Volume Settings --------------- ||
    [System.Serializable]
    public class VolumeSettings
    {
        public float music;
        public float effects;
        public float master;
    }

    public void SaveVolumeSettings(float _music, float _effects, float _master)
    {
        VolumeSettings volumeSettings = new VolumeSettings()
        {
            music = _music,
            effects = _effects,
            master = _master
        };

        PlayerPrefs.SetString("Volume", JsonUtility.ToJson(volumeSettings));
        PlayerPrefs.Save();

        print("Saved to Player Pref");
    }

    public VolumeSettings LoadVolumeSettings()
    {
        return JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("Volume"));
    }

    #endregion





    #endregion


    #region || --------------- Encryption --------------- ||

    public string EncryptionDecryption(string jsonString)
    {
        string keyword = "1234567";
        string result = "";

        for (int i = 0; i < jsonString.Length; i++)
        {
            result += (char)(jsonString[i] ^ keyword[i % keyword.Length]);
        }

        return result;
    }


    #endregion


    #region || --------------- Utility --------------- ||

    public bool DoseFileExists(int slotNumber)
    {
        if (isSavingToJson)
        {
            if (System.IO.File.Exists(jsonPathProject + fileName + slotNumber + ".json"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (System.IO.File.Exists(binaryPath + fileName + slotNumber + ".bin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }


    public bool IsSlotEmpty(int slotNumber)
    {
        if (DoseFileExists(slotNumber))
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public void DeselectButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }



    #endregion

    #region || --------------- Loading Section --------------- ||

    public void ActivateLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void DisableLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(false);
    }

    #endregion
}
