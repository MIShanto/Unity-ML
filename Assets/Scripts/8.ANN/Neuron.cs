using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron
{
    public int numOfInput;
    public double bias; 
    public double output; 
    public double errorGradient;
    public List<double> weights = new List<double>();
    public List<double> inputs = new List<double>();

    public Neuron(int nuInputs)
    {
        bias = Random.Range(-1f, 1f);
        numOfInput = nuInputs;

        for (int i = 0; i < nuInputs; i++)
        {
            weights.Add(Random.Range(-1f, 1f));
        }
    }
}
