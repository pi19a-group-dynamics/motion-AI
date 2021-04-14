using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;


[System.Serializable]
public class Spider : MonoBehaviour, ICreature
{
    private float score;
    public GameObject[] botFalangeFirst = new GameObject[4];
    public GameObject[] botFalangeSecond = new GameObject[4];
    public GameObject[] topFalange = new GameObject[4];
    public GameObject body;
    public GameObject target;
    public GameObject spider;
    public NeuroNet net { get; set; }
    private GameObject currTarget;
    private int countInput, countOutput, countHidenLayer, countHidenNeuron;
    
    Vector3 startPoint;
    Vector3 endPoint;
    // Start is called before the first frame update
    void Start()
    {
        startPoint = this.transform.position;  
        score = 0;
        countInput = botFalangeFirst.Length+ botFalangeSecond.Length + topFalange.Length + 3;
        countOutput = botFalangeFirst.Length + botFalangeSecond.Length + topFalange.Length;
        countHidenLayer = 4;
        countHidenNeuron = botFalangeFirst.Length + botFalangeSecond.Length + topFalange.Length + 3;
        CreateNeuro();
        Spawn();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }

    public void CreateNeuro()
    {
        net = new NeuroNet(countInput, countOutput, countHidenLayer, countHidenNeuron);
    }

    public void CreateTarget()
    {
        endPoint = new Vector3(body.transform.position.x + Random.Range(-15f, 15f), body.transform.position.y, body.transform.position.z + Random.Range(-15f, 15f));
        currTarget = Instantiate(target, endPoint, new Quaternion(0, 0, 0, 0));
    }
    public void Spawn()
    {
        CreateTarget();
        for (int i = 0; i < botFalangeFirst.Length; i++)
        {
            HingeJoint joint = botFalangeFirst[i].GetComponent<HingeJoint>();
            JointSpring spring = joint.spring;
            spring.targetPosition = 10f;
            spring.spring = 5000f;
            joint.spring = spring;

            joint = botFalangeSecond[i].GetComponent<HingeJoint>();
            spring = joint.spring;
            spring.targetPosition = 10f;
            spring.spring = 5000f;
            joint.spring = spring;

            joint = topFalange[i].GetComponent<HingeJoint>();
            spring = joint.spring;
            spring.targetPosition = 10f;
            spring.spring = 5000f;
            joint.spring = spring;
        }
    }

    public GameObject Respawn()
    {
        score = 0;
        GameObject newObject = Instantiate(spider, this.transform.position, new Quaternion(0, 0, 0, 0));
        newObject.GetComponent<Spider>().net = new NeuroNet(net);
        Destroy(gameObject);
        Destroy(currTarget);
        return newObject;
    }

    public void Movement()
    {
        float[] input = new float[countInput];
        input[0] = Mathf.Sin(Time.time*10);
        for (int j = 1; j < botFalangeFirst.Length + 1; j++)
        {
            input[j] = botFalangeFirst[j-1].GetComponent<HingeJoint>().spring.targetPosition;
        }
        for (int j = botFalangeFirst.Length+1; j < botFalangeFirst.Length + botFalangeSecond.Length + 1; j++)
        {
            input[j] = botFalangeSecond[j - 1 - botFalangeFirst.Length].GetComponent<HingeJoint>().spring.targetPosition;
        }
        for (int j = botFalangeFirst.Length + botFalangeSecond.Length + 1; j < botFalangeFirst.Length + botFalangeSecond.Length + topFalange.Length +1; j++)
        {
            input[j] = topFalange[j - 1 - botFalangeFirst.Length - botFalangeSecond.Length].GetComponent<HingeJoint>().spring.targetPosition;
        }
        input[botFalangeFirst.Length + botFalangeSecond.Length + topFalange.Length + 1] = endPoint.x - body.transform.position.x;
        input[botFalangeFirst.Length + botFalangeSecond.Length + topFalange.Length + 2] = endPoint.z - body.transform.position.y;
        float[] output = new float[countOutput];
        output = net.GetOutput(input);
        for (int i = 0; i < botFalangeFirst.Length; i++)
        {
            HingeJoint joint = botFalangeFirst[i].GetComponent<HingeJoint>();
            JointSpring spring = joint.spring;
            if (spring.targetPosition < 60f && spring.targetPosition > -60f)
            {
                 spring.targetPosition = output[i] * 60f;
            }
            else
            {
                spring.targetPosition = 60f * Mathf.Sign(spring.targetPosition);
            }
            if (float.IsNaN(spring.targetPosition))
            {
                Debug.LogWarning("NaN");
                spring.targetPosition = 0f;
            }
            joint.spring = spring;
        }
        for (int i = 0; i < botFalangeSecond.Length; i++)
        {
            HingeJoint joint = botFalangeSecond[i].GetComponent<HingeJoint>();
            JointSpring spring = joint.spring;
            if (spring.targetPosition < 60f && spring.targetPosition > -60f)
            {
                spring.targetPosition = output[i + botFalangeFirst.Length] * 60f;
            }
            else
            {
                spring.targetPosition = 60f * Mathf.Sign(spring.targetPosition);
            }
            if (float.IsNaN(spring.targetPosition))
            {
                Debug.LogWarning("NaN");
                spring.targetPosition = 0f;
            }
            joint.spring = spring;
        }
        for (int i = 0; i < topFalange.Length; i++)
        {
            HingeJoint joint = topFalange[i].GetComponent<HingeJoint>();
            JointSpring spring = joint.spring;
            if (spring.targetPosition < 60f && spring.targetPosition > -60f)
            {
                spring.targetPosition = output[i + botFalangeFirst.Length + botFalangeSecond.Length] * 60f;
            }
            else
            {
                spring.targetPosition = 60f * Mathf.Sign(spring.targetPosition);
            }
            if (float.IsNaN(spring.targetPosition))
            {
                Debug.LogWarning("NaN");
                spring.targetPosition = 0f;
            }
            joint.spring = spring;
        }
        if (Vector3.Distance(currTarget.transform.position, body.transform.position)<2.5f)
        {
            score += 10f;
            Destroy(currTarget);
            CreateTarget();
        }
    }

    public float Score()
    {
        return 10f/Vector3.Distance(currTarget.transform.position, body.transform.position)+score;
    }
}
