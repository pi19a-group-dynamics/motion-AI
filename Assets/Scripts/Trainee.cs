using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Trainee: MonoBehaviour
{
    public GameObject creature;
    public GameObject plane;
    public GameObject pullPlane;
    public System.Type type;
    public List<List<GameObject>> branches = new List<List<GameObject>>();
    public List<GameObject> pullCreature = new List<GameObject>();
    List<List<float>> averageData = new List<List<float>>();
    List<List<float>> maxData = new List<List<float>>();
    public int countBranch;
    public int countBest;
    public float distansBetweenCreature = 20f;
    public float mutationPower;
    public float mutationProb;
    private int countSpecimen;
    public int countHidenLayer;
    private int numGen;
    // Start is called before the first frame update
    void Start()
    {
        numGen = 0;
        NeuroNet.mutationPower = mutationPower;
        NeuroNet.mutationProb = mutationProb;
        using (StreamWriter streamBr = new StreamWriter("branch.txt", false, System.Text.Encoding.Default))
        {
            streamBr.Write(countBranch);
        }
        using (StreamWriter streamAv = new StreamWriter("averageData.txt", false, System.Text.Encoding.Default)) 
        { 
        }
        using (StreamWriter streamMax = new StreamWriter("maxData.txt", false, System.Text.Encoding.Default)) 
        { 
        }
        countSpecimen = countBest * countBest;
        for (int i = 0; i < countBranch; i++)
        {
            averageData.Add(new List<float>());
            maxData.Add(new List<float>());
            GameObject CurrPlane = Instantiate(plane, new Vector3(300*i, 0, 0), new Quaternion(0, 0, 0, 0));
            CurrPlane.transform.localScale = new Vector3(200f, 1f, countSpecimen*distansBetweenCreature + distansBetweenCreature);
            SpawnBranch(CurrPlane);
        }
        GameObject pullBranch = Instantiate(pullPlane, new Vector3(300 * countBranch, 0, 0), new Quaternion(0, 0, 0, 0));
        pullBranch.transform.localScale = new Vector3(200f, 1f, countSpecimen * distansBetweenCreature + distansBetweenCreature);
        SpawnPullBranch(pullBranch);
        InvokeRepeating("Respawn", 20, 20);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBranch(GameObject plane)
    {
        List<GameObject> creatures = new List<GameObject>(); 
        for (int i = 0; i < countSpecimen; i++)
        {
            GameObject thisCreature = Instantiate(creature, new Vector3(plane.transform.position.x, 5, -countSpecimen * distansBetweenCreature/2 + i* distansBetweenCreature), new Quaternion(0, 0, 0, 0));
            creatures.Add(thisCreature);
        }
        branches.Add(creatures);
    }

    public void SpawnPullBranch(GameObject plane)
    {
        for (int i = 0; i < countSpecimen; i++)
        {
            GameObject thisCreature = Instantiate(creature, new Vector3(plane.transform.position.x, 5, -countSpecimen * distansBetweenCreature / 2 + i * distansBetweenCreature), new Quaternion(0, 0, 0, 0));
            pullCreature.Add(thisCreature);
        }
    }

    public void Respawn()
    {
        numGen++;
        for (int indexBranch = 0; indexBranch < branches.Count; indexBranch++)
        {
            float average = 0;
            List<(NeuroNet, float)> spawnersSorted = new List<(NeuroNet, float)>();
            for (int i = 0; i < branches[indexBranch].Count; i++)
            {

                ICreature thisCreature = branches[indexBranch][i].GetComponent<ICreature>();
                float score = thisCreature.Score();
                spawnersSorted.Add((new NeuroNet(thisCreature.net),score));
            }
            for (int i = 0; i < pullCreature.Count; i++)
            {
                ICreature thisCreature = pullCreature[i].GetComponent<ICreature>();
                float score = thisCreature.Score();
                spawnersSorted.Add((new NeuroNet(thisCreature.net), score));
            }
            spawnersSorted.Sort((spawnerFirst, spawnerSecond) => spawnerFirst.Item2.CompareTo(spawnerSecond.Item2));
            spawnersSorted.Reverse();
            for (int i = 0; i < countBest; i++)
            {
                average += spawnersSorted[i].Item2;
            }
            average /= countBest;
            averageData[indexBranch].Add(average);
            maxData[indexBranch].Add(spawnersSorted[0].Item2);
            using (StreamWriter streamAv = new StreamWriter("averageData.txt", true, System.Text.Encoding.Default))
            {
                streamAv.Write(" " + average);
            }
            using (StreamWriter streamMax = new StreamWriter("maxData.txt", true, System.Text.Encoding.Default))
            {
                streamMax.Write(" " + spawnersSorted[0].Item2);
            }
            Debug.Log("Поколение " + numGen);
            for (int i = 0; i < countBest; i++)
            {
                for (int j = 0; j < countBest; j++)
                {
                    ICreature newCreature = branches[indexBranch][i * countBest + j].GetComponent<ICreature>();
                    NeuroNet wormA = spawnersSorted[i].Item1;
                    NeuroNet wormB = spawnersSorted[j].Item1;
                    newCreature.net = new NeuroNet(wormA, wormB);
                    branches[indexBranch][i * countBest + j] = newCreature.Respawn();                  
                }
            }
        }
        for (int i = 0; i < pullCreature.Count; i++)
        {
            ICreature thisCreature = pullCreature[i].GetComponent<ICreature>();
            pullCreature[i] = thisCreature.Respawn();
            thisCreature.CreateNeuro();
        }
        using (StreamWriter streamAv = new StreamWriter("averageData.txt", true, System.Text.Encoding.Default))
        {
            streamAv.WriteLine();
        }
        using (StreamWriter streamMax = new StreamWriter("maxData.txt", true, System.Text.Encoding.Default))
        {
            streamMax.WriteLine();
        }
    }

    public void Save()
    {

    }
}
