using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour, ICreature
{
    public GameObject bit;
    public int countBit = 20;
    private float score;
    public List<GameObject> bits;
    private Rigidbody rb;
    public NeuroNet net { get; set; }
    private int countInput, countOutput, countHidenLayer, countHidenNeuron;


    // Start is called before the first frame update
    void Start()
    {      
        score = 0;
        bits = new List<GameObject>();
        rb = new Rigidbody();
        Spawn();
        countInput = bits.Count;
        countOutput = bits.Count - 1;
        countHidenLayer = 2;
        countHidenNeuron = bits.Count;
        CreateNeuro();
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

    public void Spawn()
    {
        for (int i = 0; i < countBit; i++)
        {
            GameObject thisBit = Instantiate(bit, new Vector3(i * 1.2f+ this.transform.position.x+10, this.transform.position.y, this.transform.position.z), new Quaternion(0, 0, 0, 0), this.transform);
            bits.Add(thisBit);
            if (i > 0)
            {
                HingeJoint joint = bits[i].AddComponent<HingeJoint>();
                JointSpring spring = joint.spring;
                if (i % 2 == 0)
                {
                    joint.axis = new Vector3(0, 0, 1);
                }
                else
                {
                    joint.axis = new Vector3(0, 1, 0);
                }
                joint.useSpring = true;
                joint.connectedBody = rb;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = new Vector3(-0.51f, 0f, 0f);
                joint.connectedAnchor = new Vector3(0.51f, 0f, 0f);
                spring.spring = 1000f;
                spring.damper = 100f;
                spring.targetPosition = 0f;
                joint.enableCollision = false;
                joint.spring = spring;
            }
            rb = bits[i].GetComponent<Rigidbody>();
        }
    }

    public GameObject Respawn()
    {
        for (int i = 0; i < countBit; i++)
        {
            Destroy(bits[i]);
        }
        bits.Clear();
        Spawn();
        return gameObject;
    }

    public void Movement()
    {
        float[] input = new float[bits.Count];
        float[] velocity = new float[bits.Count - 1];
        input[0] = Mathf.Sin(Time.time/10);
        for (int j = 1; j < bits.Count; j++)
        {
            //input[j] = bits[j].GetComponent<HingeJoint>().spring.targetPosition;
            if (j % 2 == 0)
            {
                input[j] = bits[j].transform.position.y - bits[0].transform.position.y;
            }
            else
            {
                input[j] = bits[j].transform.position.z - bits[0].transform.position.z;
            }      
            velocity[j - 1] = 0f;
        }
        float[] output = new float[bits.Count - 1];
        output = net.GetOutput(input);
        
        for (int i = 0; i < bits.Count; i++)
        {
            rb = bits[i].GetComponent<Rigidbody>();
            if (i > 0)
            {
                try
                {
                    HingeJoint joint = bits[i].GetComponent<HingeJoint>();
                    JointSpring spring = joint.spring;
                    if (Mathf.Abs(output[i - 1]) > 1f)
                    {
                        spring.targetPosition = 30f * Mathf.Sign(output[i - 1]);
                        velocity[i - 1] = 0f;
                    }
                    else
                    {
                        spring.targetPosition = output[i - 1] * 30f;
                    }
                    if (float.IsNaN(spring.targetPosition))
                    {
                        Debug.LogWarning("NaN");
                        spring.targetPosition = 0f;
                    }
                    //if (Mathf.Abs(spring.targetPosition) > 60f)
                    //{
                    //    spring.targetPosition = 60f * Mathf.Sign(spring.targetPosition);
                    //}
                    //if (Mathf.Abs(spring.targetPosition) > 60f)
                    //{
                    //    spring.targetPosition = 60f * Mathf.Sign(spring.targetPosition);
                    //}
                    joint.spring = spring;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex);
                    Debug.LogWarning(output[i - 1]);
                    throw;
                }
                
            }
        }
    }

    public float Score()
    {
        return bits[0].transform.position.x - GetComponentInParent<Transform>().position.x;
    }
}

