using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuroNet
{
    public static float mutationPower = 50f;
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

    public NeuroNet(NeuroNet netA)
    {
        this.countInput = netA.countInput;
        this.countOutput = netA.countOutput;
        this.countHidenLayer = netA.countHidenLayer;
        this.countHidenNeuron = netA.countHidenNeuron;
        inputLayer = new Layer(countInput);
        hidenLayers = new Layer[countHidenLayer];
        for (int i = 0; i < countHidenLayer; i++)
        {
            hidenLayers[i] = new Layer(netA.hidenLayers[i]);
        }
        outputLayer = new Layer(netA.outputLayer);
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
    public Layer(Layer layer)
    {
        neurons = new Neuron[layer.neurons.Length];
        prevLayer = layer.prevLayer;
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
                neurons[i].value += neurons[i].weight[j] * prevLayer.neurons[j].value;
            }
            neurons[i].value += neurons[i].byes;
            if (Mathf.Abs(neurons[i].value) > 1)
            {
                neurons[i].Activation();
            }
        }
    }

    public void Input(float[] inputs)
    {
        for (int i = 0; i < neurons.Length; i++)
        {
            neurons[i].value = inputs[i];
            if (Mathf.Abs(neurons[i].value) > 1)
            {
                neurons[i].Activation();
            }
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
        byes = Random.Range(-1f, 1f) * NeuroNet.mutationPower;
        weight = new float[countWeight];
        for (int i = 0; i < countWeight; i++)
        {
            weight[i] = Random.Range(-1f, 1f) * NeuroNet.mutationPower;
        }
    }

    public Neuron()
    {
        value = 0;
        byes = Random.Range(-1f, 1f) * NeuroNet.mutationPower;
        weight = null;
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
        float exp = Mathf.Exp(value/40);
        value = (exp - 1) / (exp + 1);
    }
}
