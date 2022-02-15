using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public int numNeurons;
    public List<Neuron> neurons = new List<Neuron>();

    // nNeuron = how many neurons are in this layer
    // numNeuronInputs = how many neurons were in previous layer (they are input for each neuron in current layer)
    public Layer(int nNeurons, int numNeuronInputs) 
    {
        numNeurons = nNeurons;

        for (int i = 0; i < nNeurons; i++)
        {
            neurons.Add(new Neuron(numNeuronInputs));
        }
    }
}
