using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{
    public GameObject bit;
    public int countBit = 8;
    private float score;
    public List<GameObject> bits;
    private Rigidbody rb;
    public NeuroNet net;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        bits = new List<GameObject>();
        rb = new Rigidbody();
        Spawn();
        net = new NeuroNet(bits.Count, bits.Count - 1, 2, bits.Count);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }

    public void Spawn()
    {
        for (int i = 0; i < countBit; i++)
        {
            GameObject thisBit = Instantiate(bit, new Vector3(i * 1.2f, 0, this.transform.position.z), new Quaternion(90, 0, 0, 0), this.transform);
            bits.Add(thisBit);
            if (i > 0)
            {
                HingeJoint joint = bits[i].AddComponent<HingeJoint>();
                JointSpring spring = joint.spring;
                joint.axis = new Vector3(0, 0, 1);
                joint.connectedBody = rb;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = new Vector3(-0.51f, 0f, 0f);
                joint.connectedAnchor = new Vector3(0.51f, 0f, 0f);
                joint.useSpring = true;
                spring.spring = 1000f;
                spring.damper = 100f;
                spring.targetPosition = 0f;
                joint.enableCollision = false;
                joint.spring = spring;
            }
            rb = bits[i].GetComponent<Rigidbody>();
        }
    }

    public void Respawn()
    {
        for (int i = 0; i < countBit; i++)
        {
            Destroy(bits[i]);
        }
        bits.Clear();
        Spawn();
    }

    public void Movement()
    {
        float[] input = new float[bits.Count];
        float[] velocity = new float[bits.Count - 1];
        input[0] = Mathf.Sin(Time.time/10);
        for (int j = 1; j < bits.Count; j++)
        {
            input[j] = bits[j].GetComponent<HingeJoint>().spring.targetPosition;
            velocity[j - 1] = 0f;
        }
        float[] output = new float[bits.Count - 1];
        output = net.GetOutput(input);
        
        for (int i = 0; i < bits.Count; i++)
        {
            rb = bits[i].GetComponent<Rigidbody>();
            if (i > 0)
            {
                HingeJoint joint = bits[i].GetComponent<HingeJoint>();
                JointSpring spring = joint.spring;
                spring.targetPosition = Mathf.SmoothDamp(spring.targetPosition, output[i - 1] * 600f, ref velocity[i - 1], 0.05f);
                if (Mathf.Abs(spring.targetPosition) > 60f)
                {
                    spring.targetPosition = 60f * Mathf.Sign(spring.targetPosition);
                }
                //if (Mathf.Abs(spring.targetPosition) > 60f)
                //{
                //    spring.targetPosition = 60f * Mathf.Sign(spring.targetPosition);
                //}
                joint.spring = spring;
            }
        }
    }

    public float Score()
    {
        return bits[0].transform.position.x;
    }
}

public class NeuroNet
{
    public static float mutationPower = 10f;
    public static float mutationProb = 0.2f;
    int countInput, countOutput, countHidenLayer, countHidenNeuron;
    Layer inputLayer;
    Layer[] hidenLayers;
    Layer outputLayer;

    public NeuroNet(int countInput, int countOutput, int countHidenLayer, int countHidenNeuron)
    {
        this.countInput = countInput;
        this.countOutput = countOutput;
        this.countHidenLayer = countHidenLayer;
        this.countHidenNeuron = countHidenNeuron;
        inputLayer = new Layer(countInput);
        hidenLayers = new Layer[countHidenLayer];
        hidenLayers[0] = new Layer(countHidenNeuron, inputLayer);
        for (int i = 1; i < countHidenLayer; i++)
        {
            hidenLayers[i] = new Layer(countHidenNeuron, hidenLayers[i - 1]);
        }
        outputLayer = new Layer(countOutput, hidenLayers[countHidenLayer - 1]);
    }

    public NeuroNet(NeuroNet netA, NeuroNet netB)
    {
        this.countInput = netA.countInput;
        this.countOutput = netA.countOutput;
        this.countHidenLayer = netA.countHidenLayer;
        this.countHidenNeuron = netA.countHidenNeuron;
        inputLayer = new Layer(countInput);
        hidenLayers = new Layer[countHidenLayer];
        hidenLayers[0] = new Layer(countHidenNeuron, inputLayer);
        for (int i = 1; i < countHidenLayer; i++)
        {
            hidenLayers[i] = new Layer(countHidenNeuron, hidenLayers[i - 1]);
        }
        outputLayer = new Layer(countOutput, hidenLayers[countHidenLayer - 1]);
        for (int i = 0; i < countHidenLayer; i++)
        {
            hidenLayers[i].Gybrid(netA.hidenLayers[i], netB.hidenLayers[i]);
        }
        outputLayer.Gybrid(netA.outputLayer, netB.outputLayer);
    }

    public float[] GetOutput(float[] inputs)
    {
        inputLayer.Input(inputs);
        foreach (Layer currentLayer in hidenLayers)
        {
            currentLayer.Calc();
        }
        outputLayer.Calc();
        return outputLayer.GetValues();
    }
}

public class Layer
{
    Layer prevLayer;
    Neuron[] neurons;
    public Layer(int count)
    {
        neurons = new Neuron[count];
        prevLayer = this;
        for (int i = 0; i < neurons.Length; i++)
        {
            neurons[i] = new Neuron();
        }
    }
    public Layer(int count, Layer prev)
    {
        neurons = new Neuron[count];
        prevLayer = prev;
        for (int i = 0; i < neurons.Length; i++)
        {
            neurons[i] = new Neuron(prevLayer.neurons.Length);
        }
    }

    public void Gybrid(Layer layerA, Layer layerB)
    {
        for (int i = 0; i < neurons.Length; i++)
        {
            neurons[i] = new Neuron(layerA.neurons[i], layerB.neurons[i]);
        }
    }


    public void Calc()
    {
        for (int i = 0; i < neurons.Length; i++)
        {
            neurons[i].value = 0;
            for (int j = 0; j < prevLayer.neurons.Length; j++)
            {
                neurons[i].value += neurons[i].weight[j] * prevLayer.neurons[j].value;
            }
            neurons[i].value += neurons[i].byes;
            neurons[i].Activation();
        }
    }

    public void Input(float[] inputs)
    {
        for (int i = 0; i < neurons.Length; i++)
        {
            neurons[i].value = inputs[i];
            neurons[i].Activation();
        }
    }

    public float[] GetValues()
    {
        float[] result = new float[neurons.Length];
        for (int i = 0; i < neurons.Length; i++)
        {
            result[i] = neurons[i].value;
        }
        return result;
    }
}

public class Neuron
{
    public float[] weight { get; set; }
    public float value;
    public float byes;
    public Neuron(int countWeight)
    {
        value = 0;
        byes = Random.Range(-1f, 1f);
        weight = new float[countWeight];
        for (int i = 0; i < countWeight; i++)
        {
            weight[i] = Random.Range(-10f, 10f);
        }
    }

    public Neuron()
    {
        value = 0;
        byes = Random.Range(-1f, 1f);
        weight = null;
    }

    public Neuron(Neuron neuronA, Neuron neuronB)
    {
        weight = new float[neuronA.weight.Length];
        if (Random.Range(0f, 1f) > 0.5)
        {
            byes = neuronA.byes;
        }
        else
        {
            byes = neuronB.byes;
        }
        if (Random.Range(0f, 1f) < NeuroNet.mutationProb)
        {
            byes += Random.Range(-1f, 1f) * NeuroNet.mutationPower;
        }
        for (int i = 0; i < neuronA.weight.Length; i++)
        {
            if (Random.Range(0f, 1f) > 0.5)
            {
                weight[i] = neuronA.weight[i];
            }
            else
            {
                weight[i] = neuronB.weight[i];
            }
            if (Random.Range(0f, 1f) < NeuroNet.mutationProb)
            {
                weight[i] += Random.Range(-1f, 1f) * NeuroNet.mutationPower;
            }
        }
    }

    public void Activation()
    {       
        float exp = Mathf.Exp(value/10);
        //Debug.Log("exp " + exp);
        value = (exp - 1) / (exp + 1);
        //Debug.Log("val " + value);
    }
}