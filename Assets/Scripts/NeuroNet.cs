using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuroNet
{
    public static float mutationPower = 50f;
    public static float mutationProb = 0.2f;
    public static float weightPower = 1f;
    public static float byesPower = 1f;
    int countInput, countOutput, countHidenLayer, countHidenNeuron;
    Layer inputLayer;
    Layer[] hidenLayers;
    Layer outputLayer;

    public NeuroNet()
    {
        this.countInput = 0;
        this.countOutput = 0;
        this.countHidenLayer = 0;
        this.countHidenNeuron = 0;
    }

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
        inputLayer.Gybrid(netA.inputLayer, netB.inputLayer);
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

    public NeuroNet(NeuroNet netA)
    {
        this.countInput = netA.countInput;
        this.countOutput = netA.countOutput;
        this.countHidenLayer = netA.countHidenLayer;
        this.countHidenNeuron = netA.countHidenNeuron;
        inputLayer = new Layer(netA.inputLayer, inputLayer);
        hidenLayers = new Layer[countHidenLayer];
        hidenLayers[0] = new Layer(netA.hidenLayers[0], inputLayer);
        for (int i = 1; i < countHidenLayer; i++)
        {
            hidenLayers[i] = new Layer(netA.hidenLayers[i], hidenLayers[i-1]);
        }
        outputLayer = new Layer(netA.outputLayer, hidenLayers[countHidenLayer-1]);
    }

    public float[] GetOutput(float[] inputs)
    {
        inputLayer.Input(inputs);
        for (int i = 0; i < hidenLayers.Length; i++)
        {
            hidenLayers[i].Calc();
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
    public Layer(Layer layer, Layer prev)
    {
        neurons = new Neuron[layer.neurons.Length];
        prevLayer = prev;
        for (int i = 0; i < neurons.Length; i++)
        {
            neurons[i] = new Neuron(layer.neurons[i]);
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
                neurons[i].value += neurons[i].weight[j] * prevLayer.neurons[j].value * NeuroNet.weightPower;
            }
            neurons[i].value += neurons[i].byes * NeuroNet.byesPower;
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
            weight[i] = Random.Range(-1f, 1f);
        }
    }

    public Neuron()
    {
        value = 0;
        byes = Random.Range(-1f, 1f) * NeuroNet.mutationPower;
        weight = new float[0];
    }

    public Neuron(Neuron neuron)
    {
        value = neuron.value;
        byes = neuron.byes;
        weight = new float[neuron.weight.Length];
        for (int i = 0; i < weight.Length; i++)
        {
            weight[i] = neuron.weight[i];
        }
    }

    public Neuron(Neuron neuronA, Neuron neuronB)
    {
        weight = new float[neuronA.weight.Length];
        if (Random.value > 0.5)
        {
            byes = neuronA.byes;
        }
        else
        {
            byes = neuronB.byes;
        }
        if (Random.value < NeuroNet.mutationProb)
        {
            byes += Random.Range(-1f, 1f) * NeuroNet.mutationPower;
        }
        for (int i = 0; i < weight.Length; i++)
        {
            if (Random.value > 0.5)
            {
                weight[i] = neuronA.weight[i];
            }
            else
            {
                weight[i] = neuronB.weight[i];
            }
            if (Random.value < NeuroNet.mutationProb)
            {
                weight[i] += Random.Range(-1f, 1f) * NeuroNet.mutationPower;
            }
        }
    }

    public void Activation()
    {
        value = Mathf.Atan(value)/Mathf.PI*2;
    }
}
