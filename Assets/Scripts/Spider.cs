using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour, ICreature
{
    private float score;
    public GameObject[] botFalangeFirst = new GameObject[4];
    public GameObject[] botFalangeSecond = new GameObject[4];
    public GameObject[] topFalange = new GameObject[4];
    public HingeJoint[,] Falange = new HingeJoint[3,4];
    public GameObject body;
    public GameObject target;
    public GameObject spider;
    public NeuroNet net { get; set; }
    private GameObject currTarget;
    private int countLeg, countInput, countOutput, countHidenLayer, countHidenNeuron;
    
    Vector3 startPoint;
    Vector3 endPoint;
    // Start is called before the first frame update
    void Start()
    {
        countLeg = botFalangeFirst.Length;
        //CreateTarget();
        for (int i = 0; i < countLeg; i++)
        {
            Falange[0, i] = botFalangeFirst[i].GetComponent<HingeJoint>();
            JointSpring spring = Falange[0, i].spring;
            spring.targetPosition = 10f;
            spring.spring = 5000f;
            Falange[0, i].spring = spring;

            Falange[1, i] = botFalangeSecond[i].GetComponent<HingeJoint>();
            spring = Falange[1, i].spring;
            spring.targetPosition = 10f;
            spring.spring = 5000f;
            Falange[1, i].spring = spring;

            Falange[2, i] = topFalange[i].GetComponent<HingeJoint>();
            spring = Falange[2, i].spring;
            spring.targetPosition = 10f;
            spring.spring = 5000f;
            Falange[2, i].spring = spring;
        }
        startPoint = this.transform.position;  
        score = 0;
        countInput = 5;
        countOutput = 2;
        countHidenLayer = 1;
        countHidenNeuron = 5;
        CreateNeuro();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < countLeg; i++)
        {
            float[] input = new float[countInput];
            input[0] = Falange[0, i].spring.targetPosition / 60f;
            input[1] = Falange[1, i].spring.targetPosition / 60f;
            input[2] = Mathf.Sign(topFalange[i].transform.position.x - body.transform.position.x);
            input[3] = Mathf.Sign(topFalange[i].transform.position.z - body.transform.position.z);
            input[4] = Mathf.Sin(Time.time*3);
            float[] output = new float[countOutput];
            output = net.GetOutput(input);
            for (int falangeIndex = 0; falangeIndex < 2; falangeIndex++)
            {
                HingeJoint joint = Falange[falangeIndex, i];
                JointSpring spring = joint.spring;
                if (spring.targetPosition < 60f && spring.targetPosition > -60f)
                {
                    spring.targetPosition = output[falangeIndex] * 60f;
                }
                else
                {
                    spring.targetPosition = 60f * Mathf.Sign(spring.targetPosition);
                }
                if (float.IsNaN(spring.targetPosition))
                {
                    spring.targetPosition = 0f;
                }
                joint.spring = spring;
            }
            
        }  
        //input[0] = Mathf.Sin(Time.time * 10);
        //input[countLeg * 3 + 1] = (body.transform.rotation.eulerAngles.y)/360f;
        //var direction = (target.transform.position - transform.position).normalized;
        //direction.y = 0f;
        //input[countLeg * 3 + 1] = Quaternion.FromToRotation(target.transform.forward, direction).eulerAngles.y;
        //input[countLeg * 3 + 2] = Mathf.Sign(endPoint.x - body.transform.position.x);
        //input[countLeg * 3 + 3] = Mathf.Sign(endPoint.z - body.transform.position.y);
        //if (Vector3.Distance(currTarget.transform.position, body.transform.position) < 2.5f)
        //{
        //    score += 10f;
        //    Destroy(currTarget);
        //    CreateTarget();
        //}
    }

    public void CreateNeuro()
    {
        net = new NeuroNet(countInput, countOutput, countHidenLayer, countHidenNeuron);
    }

    public void CreateTarget()
    {
        //Vector2 targetPoint2D = Random.insideUnitCircle.normalized;
        //targetPoint2D *= 7f;
        endPoint = new Vector3(body.transform.position.x + 10f, this.transform.position.y-4f, body.transform.position.z);
        currTarget = Instantiate(target, endPoint, new Quaternion(0, 0, 0, 0));
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

    public float Score()
    {
        return 5*(body.transform.position.x - this.transform.position.x) - Mathf.Abs(body.transform.position.z- this.transform.position.z);
    }
}
