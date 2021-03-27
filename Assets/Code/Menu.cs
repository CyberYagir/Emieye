using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Animator animator;
    public GameObject[] windows;
    public bool change;
    string oldAnim = "Idle";
    GameObject tochangeObj;
    public TMP_Dropdown maps;
    public Image mapPreview;
    public Slider graphSlider, waterSlider, volumeSlider;
    public Toggle drawgrass;
    public Toggle motionBlur;
    public Toggle vSync;
    public Toggle fog;
    public MonoBehaviour fogbeh;
    public GameObject loadscreen;
    public Transform zombie;
    [Space]
    public Transform holder, item;
    public Slider mapSelector;
    public List<SaveData> saveDates;
    public List<Transform> saveDatesItems;
    [Space]
    public TMP_InputField worldName;
    public Image playSurvivalB, loadSurvivalB, loadDelB;
    [Space]
    public AudioSource soundTest;
    public AudioClip[] soundTestClip;
    private void Start()
    {

        RenderSettings.fog = true;

        if (PlayerPrefs.HasKey("Volume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
            AudioListener.volume = volumeSlider.value;
        }
        if (PlayerPrefs.HasKey("Graphic"))
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Graphic"));
            graphSlider.value = PlayerPrefs.GetInt("Graphic");
        }
        else
        {
            QualitySettings.SetQualityLevel(5);
            graphSlider.value = 5;

        }
        if (PlayerPrefs.HasKey("WaterType"))
        {
            waterSlider.value = PlayerPrefs.GetInt("WaterType");
        }
        else
        {
            waterSlider.value = 1;
        }
        if (PlayerPrefs.HasKey("Fog"))
        {
            int gr = PlayerPrefs.GetInt("Fog");
            fog.isOn = gr == 1;
        }
        if (PlayerPrefs.HasKey("Blur"))
        {
            int gr = PlayerPrefs.GetInt("Blur");
            motionBlur.isOn = gr == 1;
        }
        if (PlayerPrefs.HasKey("VSync"))
        {
            int gr = PlayerPrefs.GetInt("VSync");
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync");
            vSync.isOn = gr == 1;
        }
        if (PlayerPrefs.HasKey("Grass"))
        {
            int gr = PlayerPrefs.GetInt("Grass");
            drawgrass.isOn = gr == 1;
            var terr = FindObjectsOfType<Terrain>();
            for (int i = 0; i < terr.Length; i++)
            {
                terr[i].drawTreesAndFoliage = drawgrass.isOn;
            }
        }
        zombie.transform.localEulerAngles = new Vector3(0, Random.Range(-30, 30), 0);
    }
    private void Update()
    {
        if (worldName.text.Replace(" ", "") == "")
        {
            playSurvivalB.GetComponent<Button>().enabled = false;
            playSurvivalB.color = new Color(playSurvivalB.color.r, playSurvivalB.color.g, playSurvivalB.color.b, 0.2f);
        }
        else
        {
            playSurvivalB.GetComponent<Button>().enabled = true;
            playSurvivalB.color = new Color(playSurvivalB.color.r, playSurvivalB.color.g, playSurvivalB.color.b, 1f);
        }
        if (saveDates.Count == 0)
        {
            loadSurvivalB.GetComponent<Button>().enabled = false;
            loadSurvivalB.color = new Color(loadSurvivalB.color.r, loadSurvivalB.color.g, loadSurvivalB.color.b, 0.2f);
            loadDelB.GetComponent<Button>().enabled = false;
            loadDelB.color = new Color(loadSurvivalB.color.r, loadSurvivalB.color.g, loadSurvivalB.color.b, 0.2f);
        }
        else
        {
            loadSurvivalB.GetComponent<Button>().enabled = true;
            loadSurvivalB.color = new Color(loadSurvivalB.color.r, loadSurvivalB.color.g, loadSurvivalB.color.b, 1f);
            loadDelB.GetComponent<Button>().enabled = true;
            loadDelB.color = new Color(loadSurvivalB.color.r, loadSurvivalB.color.g, loadSurvivalB.color.b, 1f);
        }


        if (change)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
            {
                if (oldAnim != "Open")
                {
                    for (int i = 0; i < windows.Length; i++)
                    {
                        windows[i].SetActive(false);
                    }
                    tochangeObj.SetActive(true);
                }
            }
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            oldAnim = "Open";
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Close"))
        {
            oldAnim = "Close";
        }
    }

    public void ChangeSound()
    {
        soundTest.Stop();
        soundTest.PlayOneShot(soundTestClip[Random.Range(0,soundTestClip.Length)]);
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        AudioListener.volume = volumeSlider.value;
    }

    public void DeleteSelected()
    {
        var path = $"{Path.Combine(Application.dataPath, @"..\")}/Saves/Trash";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.Delete(saveDatesItems[(int)mapSelector.value].GetComponent<LoadItem>().path);
        UpdateSaves();

    }
    public void UpdateSaves()
    {
        saveDates = new List<SaveData>();
        saveDatesItems = new List<Transform>();
        foreach (Transform item in holder)
        {
            Destroy(item.gameObject);
        }
        if (saveDates.Count == 0)
        {
            var path = $"{Path.Combine(Application.dataPath, @"..\")}/Saves/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var loads = Directory.GetFiles(path, "*.emwrld");

            for (int i = 0; i < loads.Length; i++)
            {

                SaveData load = null;
                if (File.Exists(loads[i]))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(Path.GetFullPath(loads[i]), FileMode.Open);
                    print(loads[i]);
                    load = (SaveData)bf.Deserialize(file);
                    file.Close();
                }
                if (load != null)
                {
                    print(load.map);
                    var it = Instantiate(item, holder);
                    it.localPosition = Vector3.zero;
                    it.GetComponent<LoadItem>().image.sprite = maps.options[load.map].image;
                    it.GetComponent<LoadItem>().nameT.text = Path.GetFileNameWithoutExtension(loads[i]);
                    it.GetComponent<LoadItem>().dateT.text = File.GetLastWriteTime(loads[i]).ToString();
                    it.GetComponent<LoadItem>().path = loads[i];
                    it.gameObject.SetActive(true);
                    saveDatesItems.Add(it);
                    saveDates.Add(load);
                }
            }
        }
        else
        {
            graphSlider.minValue = 0;
            graphSlider.maxValue = 0;
        }
        mapSelector.minValue = 0;
        mapSelector.maxValue = saveDates.Count - 1;
    }
    public void GoToLoad()
    {
        loadscreen.SetActive(true);
        PlayerPrefs.SetInt("Map", saveDates[(int)mapSelector.value].map);
        PlayerPrefs.SetString("File", Path.GetFileNameWithoutExtension(saveDatesItems[(int)mapSelector.value].GetComponent<LoadItem>().path));
        PlayerPrefs.SetString("Load", "yagir fool");
        Application.LoadLevelAsync(1);
    }
    public void SavesSlider()
    {
        for (int i = 0; i < saveDates.Count; i++)
        {
            print(i + " " + (int)mapSelector.value);
            if (i == (int)mapSelector.value)
            {
                saveDatesItems[i].gameObject.SetActive(true);
            }
            else
            {
                saveDatesItems[i].gameObject.SetActive(false);
            }
        }
    }
    public void ChangeMap()
    {
        mapPreview.sprite = maps.options[maps.value].image;
    }

    public void GraphicsChange()
    {
        PlayerPrefs.SetInt("Graphic", (int)graphSlider.value);
        QualitySettings.SetQualityLevel((int)graphSlider.value);
    }
    public void WaterChange()
    {
        PlayerPrefs.SetInt("WaterType", (int)waterSlider.value);
    }
    public void DrawGrassChange()
    {
        PlayerPrefs.SetInt("Grass", drawgrass.isOn ? 1 : 2);
        var terr = FindObjectsOfType<Terrain>();
        for (int i = 0; i < terr.Length; i++)
        {
            terr[i].drawTreesAndFoliage = drawgrass.isOn;
        }
    }
    public void SetVSync()
    {
        QualitySettings.vSyncCount = vSync.isOn ? 1 : 0;
        PlayerPrefs.SetInt("VSync", vSync.isOn ? 1 : 0);
    }
    public void MotionBlurChange()
    {
        PlayerPrefs.SetInt("Blur", motionBlur.isOn ? 1 : 2);
    }
    public void FogChange()
    {
        PlayerPrefs.SetInt("Fog", fog.isOn ? 1 : 2);
        fogbeh.enabled = fog.isOn;
    }
    public void GoTo()
    {
        loadscreen.SetActive(true);
        worldName.text.Replace("<", "");
        worldName.text.Replace(">", "");
        worldName.text.Replace("\"", "");
        worldName.text.Replace("/", "");
        worldName.text.Replace(@"\", "");
        worldName.text.Replace("|", "");
        worldName.text.Replace("?", "");
        worldName.text.Replace("*", "");
        PlayerPrefs.DeleteKey("Load");
        PlayerPrefs.SetInt("Map", maps.value);
        PlayerPrefs.SetString("File", worldName.text);
        Application.LoadLevelAsync(1);
    }
    public void OpenWindow(GameObject gm)
    {
        if (gm.active == false)
        {
            change = true;
            tochangeObj = gm;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
    public void ClickAnim(GameObject gm)
    {
        if (gm.active == false)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
            {
                animator.Play("Close");
            }
            else
            {
                animator.Play("Open");
            }
        }
    }
}
