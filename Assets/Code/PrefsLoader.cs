using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;
using UnityStandardAssets.Water;

public class PrefsLoader : MonoBehaviour
{

    bool loaded, loadsave;
    float wait;
    WorldManager manager;
    void Start()
    {
        
        manager = GetComponent<WorldManager>();
        for (int i = 0; i < manager.dotDestroyOnLoad.Length; i++)
        {
            DontDestroyOnLoad(manager.dotDestroyOnLoad[i]);
        }
        Application.LoadLevel(manager.maps[PlayerPrefs.GetInt("Map")].mapID);


    }

    // Update is called once per frame
    void Update()
    {
        if (loaded == false)
        {
            if (SceneManager.GetActiveScene().buildIndex == manager.maps[PlayerPrefs.GetInt("Map")].mapID)
            {
                if (PlayerPrefs.HasKey("Load"))
                {
                    manager.localPlayer.GetComponent<PlayerStats>().startResp = false;
                }
                for (int i = 0; i < manager.dotDestroyOnLoad.Length; i++)
                {
                    manager.dotDestroyOnLoad[i].SetActive(true);
                }
                GetComponent<Save_Load>().enabled = true;
                Prefs();
                
                loaded = true;
                return;
            }
        }
        if (loadsave == false)
        {
            if (SceneManager.GetActiveScene().buildIndex == manager.maps[PlayerPrefs.GetInt("Map")].mapID)
            {
                if (PlayerPrefs.HasKey("Load"))
                {
                    GetComponent<Save_Load>().filename = PlayerPrefs.GetString("File");
                    GetComponent<Save_Load>().Init();
                    GetComponent<Save_Load>().Load();
                    loadsave = true;
                }
            }
        }
    }
    public void Prefs()
    {
        RenderSettings.fog = true;

        if (PlayerPrefs.HasKey("Graphic"))
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Graphic"));
            if (PlayerPrefs.GetInt("Graphic") < 4)
            {
                FindObjectOfType<ReflectionProbe>().enabled = false;
            }
        }
        else
        {
            QualitySettings.SetQualityLevel(5);

        }
        if (PlayerPrefs.HasKey("Fog"))
        {
            int gr = PlayerPrefs.GetInt("Fog");
            FindObjectOfType<MotionBlur>().GetComponentInParent<PlayerInventory>().globalFog.enabled = gr == 1;
        }
        if (PlayerPrefs.HasKey("Blur"))
        {
            int gr = PlayerPrefs.GetInt("Blur");
            FindObjectOfType<MotionBlur>().enabled = gr == 1;
        }
        if (PlayerPrefs.HasKey("VSync"))
        {
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync");
            print("VSync: " + QualitySettings.vSyncCount);
        }
        if (PlayerPrefs.HasKey("Grass"))
        {
            int gr = PlayerPrefs.GetInt("Grass");
            bool act = gr == 1;
            var terr = FindObjectsOfType<Terrain>();
            for (int i = 0; i < terr.Length; i++)
            {
                terr[i].drawTreesAndFoliage = act;
            }
        }
        GetComponent<Save_Load>().filename = PlayerPrefs.GetString("File");
        if (PlayerPrefs.HasKey("WaterType"))
            FindObjectOfType<Water>().waterMode = (Water.WaterMode)PlayerPrefs.GetInt("WaterType");
    }
}
