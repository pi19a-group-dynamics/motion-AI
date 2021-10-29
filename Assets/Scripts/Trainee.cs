using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Trainee: MonoBehaviour
{
    public SaveInfo info;
    public Graph GraphAv;
    public Graph GraphMax;

    public GameObject creature;
    public GameObject plane;
    public GameObject pullPlane;
    public List<List<GameObject>> branches = new List<List<GameObject>>();
    List<GameObject> pullCreature = new List<GameObject>();
    List<List<float>> averageData = new List<List<float>>();
    List<List<float>> maxData = new List<List<float>>();
    public float timeToNextGen;
    public int countBranch;
    public int countBest;
    public float distansBetweenCreature;
    public float mutationPower;
    public float mutationProb;
    public float weightPower;
    public float byesPower;
    public float timeK;
    public int genToMigration;
    public bool pullBranch;
    public bool migration;
    private int countSpecimen;
    private int numGen;
    // Start is called before the first frame update
    void Start()
    {
        numGen = 0;
        NeuroNet.mutationPower = mutationPower;
        NeuroNet.mutationProb = mutationProb;
        NeuroNet.weightPower = weightPower;
        NeuroNet.byesPower = byesPower;


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
            SpawnBranch(i);
        }
        if (pullBranch)
        {
            SpawnPullBranch(countBranch);
        }   
        InvokeRepeating("Respawn", timeToNextGen, timeToNextGen);

        info.Pocolenie.text = (0).ToString();
        info.CountBranch.text = (countBranch).ToString();
        info.CountCharacter.text = (countSpecimen).ToString();
    }

    public void SpawnBranch(int num)
    {
        List<GameObject> creatures = new List<GameObject>(); 
        for (int i = 0; i < countSpecimen; i++)
        {
            GameObject currPlane = Instantiate(plane, new Vector3(distansBetweenCreature * num, 0, i * distansBetweenCreature), new Quaternion(0, 0, 0, 0));
            currPlane.transform.localScale = new Vector3(distansBetweenCreature - 10f, 1f, distansBetweenCreature - 10f);
            GameObject thisCreature = Instantiate(creature, new Vector3(currPlane.transform.position.x, 2, currPlane.transform.position.z), new Quaternion(0, 0, 0, 0));
            creatures.Add(thisCreature);
        }
        branches.Add(creatures);
    }

    public void SpawnPullBranch(int num)
    {
       for (int i = 0; i < countSpecimen; i++)
        {
            GameObject currPlane = Instantiate(pullPlane, new Vector3(distansBetweenCreature * num, 0, i * distansBetweenCreature), new Quaternion(0, 0, 0, 0));
            currPlane.transform.localScale = new Vector3(distansBetweenCreature - 10f, 1f, distansBetweenCreature - 10f);
            GameObject thisCreature = Instantiate(creature, new Vector3(currPlane.transform.position.x, 2, currPlane.transform.position.z), new Quaternion(0, 0, 0, 0));
            pullCreature.Add(thisCreature);
        }
    }

    public void Respawn()
    {
        numGen++;
        info.Pocolenie.text = numGen.ToString();
        for (int indexBranch = 0; indexBranch < branches.Count; indexBranch++)
        {
            float sumScore = 0;
            float average = 0;
            List<(NeuroNet, float)> spawnersSorted = new List<(NeuroNet, float)>();
            for (int i = 0; i < branches[indexBranch].Count; i++)
            {
                ICreature thisCreature = branches[indexBranch][i].GetComponent<ICreature>();
                float score = thisCreature.Score();
                if (score < 0)
                {
                    score = 0.01f;
                }
                sumScore += score;
                spawnersSorted.Add((new NeuroNet(thisCreature.net),score));
            }
            for (int i = 0; i < pullCreature.Count; i++)
            {
                ICreature thisCreature = pullCreature[i].GetComponent<ICreature>();
                float score = thisCreature.Score();
                if (score < 0)
                {
                    score = 0.01f;
                }
                sumScore += score;
                spawnersSorted.Add((new NeuroNet(thisCreature.net), score));
            }
            spawnersSorted.Sort((spawnerFirst, spawnerSecond) => spawnerFirst.Item2.CompareTo(spawnerSecond.Item2));
            spawnersSorted.Reverse();
            average = sumScore/countSpecimen;
            averageData[indexBranch].Add(average);
            maxData[indexBranch].Add(spawnersSorted[0].Item2);

            GraphAv.AddDot((int)average);
            GraphMax.AddDot((int)spawnersSorted[0].Item2);

            using (StreamWriter streamAv = new StreamWriter("averageData.txt", true, System.Text.Encoding.Default))
            {
                streamAv.Write(" " + average);
            }
            using (StreamWriter streamMax = new StreamWriter("maxData.txt", true, System.Text.Encoding.Default))
            {
                streamMax.Write(" " + spawnersSorted[0].Item2);
            }

            //отбор методом рулетки
            NeuroNet wormA = new NeuroNet();
            NeuroNet wormB = new NeuroNet();
            for (int i = 0; i < countBest; i++)
            {
                ICreature newCreature = branches[indexBranch][i].GetComponent<ICreature>();
                newCreature = branches[indexBranch][i].GetComponent<ICreature>();
                newCreature.net = new NeuroNet(spawnersSorted[i].Item1);
                branches[indexBranch][i] = newCreature.Respawn();
            }
            for (int i = countBest; i < countSpecimen; i++)
            {
                float currValue = 0;
                (float, float) randomValue = (Random.Range(0, sumScore), Random.Range(0, sumScore));
                for (int j = 0; j < spawnersSorted.Count; j++)
                {
                    if (currValue + spawnersSorted[j].Item2 >= randomValue.Item1 && currValue < randomValue.Item1)
                    {
                        wormA = spawnersSorted[j].Item1;
                    }
                    if (currValue + spawnersSorted[j].Item2 >= randomValue.Item2 && currValue < randomValue.Item2)
                    {
                        wormB = spawnersSorted[j].Item1;
                    }
                    currValue += spawnersSorted[j].Item2;  
                }
                ICreature newCreature = branches[indexBranch][i].GetComponent<ICreature>();               
                newCreature = branches[indexBranch][i].GetComponent<ICreature>();
                newCreature.net = new NeuroNet(wormA, wormB);
                branches[indexBranch][i] = newCreature.Respawn();
            }

            //выбор десяти лучших (закомментирован)
            //for (int i = 0; i < countBest; i++)
            //{
            //    for (int j = 0; j < countBest; j++)
            //    {
            //        ICreature newCreature = branches[indexBranch][i * countBest + j].GetComponent<ICreature>();
            //        NeuroNet wormA = spawnersSorted[i].Item1;
            //        NeuroNet wormB = spawnersSorted[j].Item1;
            //        newCreature.net = new NeuroNet(wormA, wormB);
            //        branches[indexBranch][i * countBest + j] = newCreature.Respawn();                  
            //    }
            //}
        }
        if (migration)
        {
            if (numGen % genToMigration == 0)
            {
                List<List<GameObject>> newBranches = new List<List<GameObject>>(branches);
                for (int i = 0; i < branches.Count; i++)
                {
                    for (int j = 0; j < branches[i].Count; j++)
                    {
                        if (Random.value < 0.1f)
                        {
                            newBranches[Random.Range(0, newBranches.Count)][Random.Range(countBest, countBest * countBest)].GetComponent<ICreature>().net = new NeuroNet(branches[i][j].GetComponent<ICreature>().net);
                        }
                    }
                }
                branches = newBranches;
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
}
