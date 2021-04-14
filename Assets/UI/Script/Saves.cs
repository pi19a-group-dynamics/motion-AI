using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Saves : MonoBehaviour
{
    public string SaveLoadPath;
    public Trainee manager;
    public string path;

    public void Save()
    {
        string json = JsonUtility.ToJson(new Buba());
        Debug.Log(json);
    }


    void Start()
    {
        
    }


    void Update()
    {

    }
}

public class Buba {
    public int k = 12;
    public int[] stuka = { 1, 2, 3, 4, 6 };
}
