using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool i;
    public Dictionary<string, List<GameObject>> objectLists = new Dictionary<string, List<GameObject>>();  
    public Dictionary<string, ObjectData> objectLookup = new Dictionary<string, ObjectData>();  

    [SerializeField] List<ObjectData> objectList = new List<ObjectData>();
    public Dictionary<string, ObjectListData> objectListLookup = new Dictionary<string, ObjectListData>();
    public List<ObjectData> ObjectList => objectList;
    [SerializeField] int initSpawnAmount = 100;
    List<GameObject> activeObjects;

    private void Awake() 
    {
        i = this;
    }
    void Start()
    {
        Init();
    }
    void Init()
    {
        foreach (ObjectData data in ObjectList)
        {
            InitObject(data.name);
        }

        objectLookup = objectList.ToDictionary(x => x.name);
        foreach (ObjectData data in ObjectList)
        {
            SpawnObject(data.name, initSpawnAmount);
        }

    }

    public void ResetPool() 
    {   
        foreach (ObjectListData listData in objectListLookup.Values)
        {
            for (int j = 0; j < listData.list.Count; j++) 
            {
                if (listData.list[j].activeInHierarchy
                    || listData.list[j].GetComponent<Entity>().State == EntityState.Active) 
                    listData.list[j].SetActive(false);
            }
        }
    }

    void InitObject(string name, int amount = 1)
    {
        if (!objectListLookup.ContainsKey(name)) 
        { 
            List<GameObject> newList = new List<GameObject>();
            ObjectListData newData = new ObjectListData(name, newList);
            objectListLookup.Add(name, newData); 
        }
    }
    void SpawnObject(string name, int amount = 1)
    {
        for (int i = 1; i <= amount; i++)
        {
            GameObject spawn = Instantiate(objectLookup[name].prefab);
            spawn.SetActive(false);
            objectListLookup[name].list.Add(spawn);
        }
    }
    public void ClearPool() 
    {
        foreach(ObjectListData data in objectListLookup.Values)
        {
            if (data.list.Count > 0) 
            {
                for (int i = 0; i < data.list.Count; i++) 
                {
                    Destroy(data.list[i]);
                }
            }
            data.list.Clear();
        }
    }
    public GameObject GetObjectByName(string name) 
    {
        
        if (objectListLookup.ContainsKey(name)) 
        {  
            for (int j = 0; j < objectListLookup[name].list.Count; j++) 
            {
                if (!objectListLookup[name].list[j].activeInHierarchy) 
                    return objectListLookup[name].list[j];
            }
        }
        else 
        {
            List<GameObject> newList = new List<GameObject>();
            ObjectListData newData = new ObjectListData(name, newList);
            objectListLookup.Add(name, newData);
            return objectListLookup[name].list[0];
            
        }
        GameObject obj = Instantiate(objectLookup[name].prefab);
        obj.SetActive(false);
        objectListLookup[name].list.Add(obj);
        return obj;

    }  
    public void RemoveFromPool(string name, GameObject obj)
    {
        for (int j = 0; j < objectListLookup[name].list.Count; j++) 
        {
            if (objectListLookup[name].list[j] == obj) 
                objectListLookup[name].list.Remove(obj);
        }
    }
    public void AddToPool(string name, GameObject obj)
    {
        objectListLookup[name].list.Add(obj);
    }
    public GameObject GetObject(string name) 
    {
        for (int j = 0; j < objectListLookup[name].list.Count; j++) 
        {
            if (!objectListLookup[name].list[j].activeInHierarchy
                || objectListLookup[name].list[j].GetComponent<Entity>().State == EntityState.Inactive) 
                return objectListLookup[name].list[j];
        }

        GameObject obj = GetObjectByName(name);
        return obj;
    }
}

[System.Serializable]
public class ObjectData
{
    public string name;
    public GameObject prefab;
}
[System.Serializable]
public class ObjectListData
{
    public string name;
    public List<GameObject> list = new List<GameObject>();
    public ObjectListData(string name, List<GameObject> list)
    {
        this.name = name;
        this.list = list;
    }
}