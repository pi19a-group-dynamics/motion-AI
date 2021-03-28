using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainee : MonoBehaviour
{
    public GameObject worm;
    public List<GameObject> worms;
    // Start is called before the first frame update
    void Start()
    {
        Spawn(200);
        InvokeRepeating("Respawn", 10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(int countWorm)
    {
        for (int i = 0; i < countWorm; i++)
        {
            GameObject thisWorm = Instantiate(worm, new Vector3(0, 0, i*5), new Quaternion(0, 0, 0, 0));
            worms.Add(thisWorm);
        }
    }

    public void Respawn()
    {
        float averange = 0;
        List<float> scores = new List<float>();
        List<Worm> spawnersSorted = new List<Worm>();
        for (int i = 0; i < worms.Count; i++)
        {
            Worm thisWorm = worms[i].GetComponent<Worm>();
            float score = thisWorm.Score();
            averange += score;
            scores.Add(score);
            spawnersSorted.Add(thisWorm);
            
        }
        spawnersSorted.Sort((spawnerFirst, spawnerSecond) => spawnerFirst.Score().CompareTo(spawnerSecond.Score()));
        scores.Sort();
        averange /= worms.Count;
        Debug.Log("Среднее " + -averange);
        Debug.Log("Максимум " + -scores[0]);
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Worm newWorm = worms[i*10+j].GetComponent<Worm>();
                newWorm.Respawn();
                if (i!=j)
                {
                    NeuroNet wormA = spawnersSorted[i].net;
                    NeuroNet wormB = spawnersSorted[j].net;
                    newWorm.net = new NeuroNet(wormA, wormB);
                }
                else
                {
                    newWorm.net = spawnersSorted[i].net;
                }              
            }           
        }
        for (int i = 100; i < 200; i++)
        {
            Worm newWorm = worms[i].GetComponent<Worm>();
            newWorm.net = new NeuroNet(newWorm.bits.Count, newWorm.bits.Count - 1, 2, newWorm.bits.Count);
            newWorm.Respawn();
        }
    }
}
