using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Save_Load : MonoBehaviour
{

    public Town[] towns;
    public WorldManager manager;
    public string path, filename;

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        towns = FindObjectsOfType<Town>();
        for (int i = 0; i < towns.Length; i++)
        {
            towns[i].Init();
        }
        path = $"{Path.Combine(Application.dataPath, @"..\")}/Saves/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    public void Save()
    {
        Debug.Log(Application.dataPath);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path + filename + ".emwrld");
        SaveData data = new SaveData();
        data.map = PlayerPrefs.GetInt("Map");
        data.pPos = new EmmiVector(manager.localPlayer.transform.position);
        data.pRot = new EmmiVector(manager.localPlayer.transform.localEulerAngles);


        for (int i = 0; i < towns.Length; i++)
        {
            if (towns[i].PlayersIn != 0)
            {
                EmmiTown town = new EmmiTown();
                town.townID = i;
                for (int j = 0; j < towns[i].dropSpawn.Count; j++)
                {
                    if (towns[i].dropSpawn[j].spawned == null)
                    {
                        town.items.Add(new EmiItem() { name = "NULL" });
                    }
                    else
                    {
                        var drop = towns[i].dropSpawn[j].spawned.GetComponent<Drop>();
                        town.items.Add(new EmiItem() { name = drop.item.name, bulletsIn = drop.item.bulletsIn, spawnid = j });
                    }
                }
                for (int j = 0; j < towns[i].carsSpawns.Count; j++)
                {
                    if (towns[i].carsSpawns[j].car == null)
                    {
                        town.cars.Add(new EmmiCar() { carName = "NULL" });
                    }
                    else
                    {
                        var car = towns[i].carsSpawns[j].car.GetComponent<SitInCar>();
                        if (car != null)
                        {
                            if (car.transform.parent != null)
                            {
                                town.cars.Add(new EmmiCar() { carName = car.transform.name, pos = new EmmiVector(car.transform.position), rot = new EmmiVector(car.transform.localEulerAngles), spawnID = j, fuel = car.fuel, hp = car.health, colour = car.material });
                            }
                        }
                    }
                }
                data.towns.Add(town);
            }
        }
        var usedCars = FindObjectsOfType<SitInCar>().ToList().FindAll(x => x.transform.parent == null);
        for (int i = 0; i < usedCars.Count; i++)
        {
            var car = usedCars[i];
            data.usedCars.Add(new EmmiCar() { carName = car.name, pos = new EmmiVector(car.transform.position), rot = new EmmiVector(car.transform.localEulerAngles), spawnID = -1, fuel = usedCars[i].fuel, hp = usedCars[i].health });
        }


        PlayerStats stats = manager.localPlayer.GetComponent<PlayerStats>();
        Attack attack = manager.localPlayer.GetComponent<Attack>();
        PlayerInventory inv = manager.localPlayer.GetComponent<PlayerInventory>();

        EmiPlayer emmiPlayer = new EmiPlayer()
        {
            energy = stats.energy,
            health = stats.health,
            hunger = stats.hunger,
            oxy = stats.oxy,
            rad = stats.radiation,
            water = stats.water,
            hat = new EmiItem(inv.hat),
            pants = new EmiItem(inv.pants),
            torso = new EmiItem(inv.torso),
            inHands = attack.item.name,
            main = new EmiItem(inv.main),
            second = new EmiItem(inv.second),
            inventory = EmiPlayer.toEmi(inv.items)

        };
        data.emmiPlayer = emmiPlayer;


        bf.Serialize(file, data);
        file.Close();

    }
    public void ClearWorld()
    {
        var drops = FindObjectsOfType<DropSpawner>();
        for (int i = 0; i < drops.Length; i++)
        {
            if (drops[i].spawned != null)
                Destroy(drops[i].spawned);
        }
        var cars = FindObjectsOfType<ProTruckController>();
        for (int i = 0; i < cars.Length; i++)
        {
            Destroy(cars[i].gameObject);
        }
    }
    public void Load()
    {

        if (File.Exists(path + filename + ".emwrld"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path + filename + ".emwrld", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            manager.localPlayer.transform.parent = null;
            ClearWorld();
            print("LOADED");
            manager.localPlayer.transform.position = data.pPos.ToVector();
            manager.localPlayer.transform.localEulerAngles = data.pRot.ToVector();


            for (int i = 0; i < data.towns.Count; i++)
            {
                for (int j = 0; j < data.towns[i].items.Count; j++)
                {
                    if (data.towns[i].items[j].name != "NULL")
                    {
                        towns[data.towns[i].townID].dropSpawn[data.towns[i].items[j].spawnid].Init(data.towns[i].items[j].name, data.towns[i].items[j].bulletsIn);
                    }
                }


                for (int j = 0; j < data.towns[i].cars.Count; j++)
                {
                    if (data.towns[i].cars[j].carName != "NULL")
                    {
                        towns[data.towns[i].townID].carsSpawns[data.towns[i].cars[j].spawnID].Init(data.towns[i].cars[j].carName, data.towns[i].cars[j].pos.ToVector(), data.towns[i].cars[j].rot.ToVector(), data.towns[i].cars[j].colour);
                    }
                }
            }
            for (int i = 0; i < data.usedCars.Count; i++)
            {
                SpawnCar(data.usedCars[i]);
            }



            PlayerStats stats = manager.localPlayer.GetComponent<PlayerStats>();
            Attack attack = manager.localPlayer.GetComponent<Attack>();
            PlayerInventory inv = manager.localPlayer.GetComponent<PlayerInventory>();

            var e = data.emmiPlayer;
            e.Init();
            stats.energy = e.energy;
            stats.health = e.health;
            stats.hunger = e.hunger;
            stats.water = e.water;
            stats.oxy = e.oxy;
            stats.radiation = e.rad;


            var main = new Item();
            var second = new Item();

            if (e.main.name != "NULL")
            {
                main = manager.GetByName(e.main.name);
                if (main != null)
                main.bulletsIn = e.main.bulletsIn;
            }
            if (e.second.name != "NULL")
            {
                second = manager.GetByName(e.second.name);
                if (second != null)
                    second.bulletsIn = e.second.bulletsIn;
            }



            if (e.hat.name != "NULL")
            {
                inv.hat = manager.GetByName(e.hat.name);
            }
            if (e.torso.name != "NULL")
            {
                inv.torso = manager.GetByName(e.torso.name);
            }
            if (e.pants.name != "NULL")
            {
                inv.pants = manager.GetByName(e.pants.name);
            }
            inv.main = main;
            inv.second = second;



            if (e.main.name == e.inHands)
            {
                attack.item = main;
            }
            if (e.second.name == e.inHands)
            {
                attack.item = second;
            }

            inv.items = EmiPlayer.fromEmi(e.inventory, manager);
            inv.GetComponent<PlayerUI>().UpdateInventory();
        }
    }


    public SaveData Load(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            return data;
        }
        return null;
    }



    public void SpawnCar(EmmiCar car)
    {
        var cr = Instantiate(manager.cars.Find(x => x.name == car.carName).gameObject, car.pos.ToVector(), Quaternion.identity);
        cr.GetComponent<SitInCar>().fuel = car.fuel;
        cr.GetComponent<SitInCar>().health = car.hp;
        cr.GetComponent<SitInCar>().material = car.colour;
        cr.name = car.carName;
        cr.transform.localEulerAngles = car.rot.ToVector();
        cr.transform.parent = null;
    }
    public void Delete(InputField s)
    {
        File.Delete(path + s.text + ".emwrld");
    }
}


[System.Serializable]
public class EmiPlayer {
    public EmiItem main;
    public EmiItem second;
    public EmiItem hat, torso, pants;
    public List<EmiItem> inventory;
    public string inHands;

    public float health, hunger, water, rad, oxy, energy;
    public void Init()
    {
        if (main == null)
        {
            main = new EmiItem(null);
        }
        if (second == null)
        {
            second = new EmiItem(null);
        }
        if (hat == null)
        {
            hat = new EmiItem(null);
        }
        if (torso == null)
        {
            torso = new EmiItem(null);
        }
        if (pants == null)
        {
            pants = new EmiItem(null);
        }
        if (inventory == null)
        {
            inventory = new List<EmiItem>();
        }
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = new EmiItem(null);
            }
        }
    }

    public static List<EmiItem> toEmi(List<Item> items)
    {
        List<EmiItem> emiItems = new List<EmiItem>();
        for (int i = 0; i < items.Count; i++)
        {
            emiItems.Add(new EmiItem(items[i]));
        }
        return emiItems;
    }
    public static List<Item> fromEmi(List<EmiItem> items, WorldManager manager)
    {
        List<Item> from = new List<Item>();
        for (int i = 0; i < items.Count; i++)
        {
            var it = manager.GetByName(items[i].name);
            if (it != null)
            {
                it.bulletsIn = items[i].bulletsIn;
                from.Add(it);
            }
        }
        return from;
    }
}


[System.Serializable]
public class SaveData
{
    public int map;
    public EmmiVector pPos, pRot;
    public List<EmmiTown> towns = new List<EmmiTown>();
    public List<EmmiCar> usedCars = new List<EmmiCar>();
    public EmiPlayer emmiPlayer = new EmiPlayer();
}
[System.Serializable]
public class EmmiCar
{
    public int spawnID, colour;
    public string carName;
    public EmmiVector rot, pos;
    public float fuel, hp;
}



[System.Serializable]
public class EmmiTown
{
    public int townID;
    public List<EmiItem> items = new List<EmiItem>();
    public List<EmmiCar> cars = new List<EmmiCar>();
}

[System.Serializable]
public class EmiItem
{
    public string name;
    public int spawnid;
    public int bulletsIn;
    public EmiItem()
    {
    }
    public EmiItem(Item item)
    {
        if (item == null)
        {
            name = "NULL";
            return;
        }
        name = item.name;
        bulletsIn = item.bulletsIn;
    }
}


[System.Serializable]
public class EmmiVector
{
    public float x, y, z;

    public EmmiVector()
    {
        x = 0;
        y = 0;
        z = 0;
    }

    public EmmiVector(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector()
    {
        return new Vector3(x, y, z);
    }

}

