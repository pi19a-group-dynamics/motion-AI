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
    public float timeK = 10f;
    private GameObject currTarget;
    private int countLeg, countInput, countOutput, countHidenLayer, countHidenNeuron;
    
    Vector3 startPoint;
    Vector3 endPoint;
    // Start is called before the first frame update
    void Start()
    {
        countLeg = botFalangeFirst.Length; 
        for (int i = 0; i < countLeg; i++)
        {
            Falange[0, i] = botFalangeFirst[i].GetComponent<HingeJoint>();
            Falange[0, i].axis = new Vector3(1, 0, 0);
            JointSpring spring = Falange[0, i].spring;
            Falange[0, i].axis = new Vector3(1, 0, 0);
            spring.targetPosition = 0f;
            spring.spring = 1000f;
            spring.damper = 100f;
            Falange[0, i].spring = spring;

            Falange[1, i] = botFalangeSecond[i].GetComponent<HingeJoint>();
            Falange[1, i].axis = new Vector3(1, 0, 0);
            spring = Falange[1, i].spring;
            spring.targetPosition = 0f;
            spring.spring = 1000f;
            spring.damper = 100f;
            Falange[1, i].spring = spring;

            Falange[2, i] = topFalange[i].GetComponent<HingeJoint>();
            Falange[2, i].axis = new Vector3(1, 0, 0);
            spring = Falange[2, i].spring;
            spring.targetPosition = 0f;
            spring.spring = 1000f;
            spring.damper = 100f;
            Falange[2, i].spring = spring;
        }
        startPoint = this.transform.position;  
        score = 0;
        countInput = 5;
        countOutput = 3;
        countHidenLayer = 1;
        countHidenNeuron = 10;
        if (currTarget == null)
        {
            CreateTarget();
        }
        if (net == null)
        {
            CreateNeuro();            
        }
    }

    // Update is called once per frame
    void Update()
    {
        //float[] input = new float[countInput];
        //for (int indexLeg = 0; indexLeg < countLeg; indexLeg++)
        //{
        //    input[4 * indexLeg + 0] = Falange[0, indexLeg].spring.targetPosition / 60f;
        //    input[4 * indexLeg + 1] = Falange[1, indexLeg].spring.targetPosition / 60f;
        //    input[4 * indexLeg + 2] = Falange[2, indexLeg].spring.targetPosition / 60f;
        //    input[4 * indexLeg + 3] = Vector2.Distance(new Vector2(body.transform.position.x, body.transform.position.z), new Vector2(currTarget.transform.position.x, currTarget.transform.position.z)) - Vector2.Distance(new Vector2(topFalange[indexLeg].transform.position.x, topFalange[indexLeg].transform.position.z), new Vector2(currTarget.transform.position.x, currTarget.transform.position.z));  
        //}
        //input[4 * countLeg] = Mathf.Sin(Time.time * 3);
        //float[] output = new float[countOutput];
        //output = net.GetOutput(input);
        //for (int indexLeg = 0; indexLeg < countLeg; indexLeg++)
        //{
        //    for (int falangeIndex = 0; falangeIndex < 3; falangeIndex++)
        //    {
        //        HingeJoint joint = Falange[falangeIndex, indexLeg];
        //        JointSpring spring = joint.spring;
        //        if (spring.targetPosition < 60f && spring.targetPosition > -60f)
        //        {
        //            spring.targetPosition = output[indexLeg*3+falangeIndex] * 60f;
        //        }
        //        else
        //        {
        //            spring.targetPosition = 60f * Mathf.Sign(spring.targetPosition);
        //        }
        //        if (float.IsNaN(spring.targetPosition))
        //        {
        //            spring.targetPosition = 0f;
        //        }
        //        joint.spring = spring;
        //    }

        for (int indexLeg = 0; indexLeg < countLeg; indexLeg++)
        {
            float[] input = new float[countInput];
            input[0] = Falange[0, indexLeg].spring.targetPosition / 60f;
            input[1] = Falange[1, indexLeg].spring.targetPosition / 60f;
            input[2] = Falange[2, indexLeg].spring.targetPosition / 60f;
            input[3] = Vector2.Distance(new Vector2(body.transform.position.x, body.transform.position.z), new Vector2(currTarget.transform.position.x, currTarget.transform.position.z)) - Vector2.Distance(new Vector2(topFalange[indexLeg].transform.position.x, topFalange[indexLeg].transform.position.z), new Vector2(currTarget.transform.position.x, currTarget.transform.position.z));
            input[4] = Mathf.Sin(Time.time * timeK);
            float[] output = new float[countOutput];
            output = net.GetOutput(input);
            for (int falangeIndex = 0; falangeIndex < 3; falangeIndex++)
            {
                HingeJoint joint = Falange[falangeIndex, indexLeg];
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
        if (Vector3.Distance(currTarget.transform.position, body.transform.position) < 2f)
        {
            score += 10f;
            CreateTarget();
        }
    }

    public void CreateNeuro()
    {
        net = new NeuroNet(countInput, countOutput, countHidenLayer, countHidenNeuron);
    }

    public void CreateTarget()
    {
        Destroy(currTarget);
        Vector2 targetPoint2D = Random.insideUnitCircle.normalized;
        targetPoint2D *= 5f;
        endPoint = new Vector3(body.transform.position.x + targetPoint2D.x, this.transform.position.y+5, body.transform.position.z + targetPoint2D.y);
        RaycastHit hit;
        Ray ray = new Ray(endPoint, -Vector3.up);
        Physics.Raycast(ray, out hit);
        endPoint = new Vector3(hit.point.x+1, hit.point.y + 1, hit.point.z + 1);
        currTarget = Instantiate(target, endPoint, new Quaternion(0, 0, 0, 0));
    }

    public GameObject Respawn()
    {
        GameObject newObject = Instantiate(spider, this.transform.position, new Quaternion(0, 0, 0, 0));
        newObject.GetComponent<Spider>().net = new NeuroNet(net);
        Destroy(gameObject);
        Destroy(currTarget);
        return newObject;
    }

    public float Score()
    {
        float d = 10-Vector3.Distance(body.transform.position,currTarget.transform.position);
        score+=d;
        return score * score * Mathf.Sign(score);
    }
}
