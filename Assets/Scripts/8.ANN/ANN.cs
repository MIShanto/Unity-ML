using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANN
{
    public int numInputs;
    public int numOutputs;
    public int numHidden;
    public int numNeuronPerHidden;

    public double alpha; // learning rate..

    List<Layer> layers = new List<Layer>();

    public ANN(int nI, int nO, int nH, int nNPH, double a)
    {
        numInputs = nI;
        numOutputs = nO;
        numHidden = nH;
        numNeuronPerHidden = nNPH;
        alpha = a;

        if (numHidden > 0)
        {
            layers.Add(new Layer(numNeuronPerHidden, numInputs)); // input layer

            for (int i = 0; i < numHidden - 1; i++)
            {
                layers.Add(new Layer(numNeuronPerHidden, numNeuronPerHidden)); // hidden layers
            }

            layers.Add(new Layer(numOutputs, numNeuronPerHidden)); // output layer
        }
        else
        {
            layers.Add(new Layer(numOutputs, numInputs)); // with only 1 layer (simillar to perceptron)
        }
    }

    public List<double> Go(List<double> inputValues, List<double> desiredOutput)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        //handling error/exception
        if (inputValues.Count != numInputs)
        {
            Debug.LogError("Error: num of inputs must be" + numInputs);
            return outputs;
        }

        //put inputs into the list.
        inputs = new List<double>(inputValues);

        //process input and do forward propagation..
        for (int i = 0; i < numHidden + 1 ; i++) // loop through all layers
        {
            if (i > 0) // for other than input layer, inputs = previous outputs
            {
                inputs = new List<double>(outputs);
            }
            outputs.Clear();

            // process all the neurons of a particular layer..
            for (int j = 0; j < layers[i].numNeurons; j++)
            {
                double singleNeuronResultBeforeActivation  = 0;
                layers[i].neurons[j].inputs.Clear(); // clear all inputs first..

                //process all the input,weights, bias of a particular neuron..
                for (int k = 0; k < layers[i].neurons[j].numOfInput; k++)
                {
                    //adding inputs to the list..
                    layers[i].neurons[j].inputs.Add(inputs[k]);

                    // Perceptron calculation logic. (DOT product. i.e: weight * input)
                    singleNeuronResultBeforeActivation += layers[i].neurons[j].weights[k] * inputs[k];
                }

                // subtracting the bias
                singleNeuronResultBeforeActivation -= layers[i].neurons[j].bias;

                if(i == numHidden) // for out layer
                    layers[i].neurons[j].output = ActivationFunctionO(singleNeuronResultBeforeActivation);
                else
                    layers[i].neurons[j].output = ActivationFunction(singleNeuronResultBeforeActivation);

                outputs.Add(layers[i].neurons[j].output);
            }
        }

        UpdateWeights(outputs, desiredOutput);

        return outputs;
    }

    private void UpdateWeights(List<double> outputs, List<double> desiredOutput)
    {
        double error;
        //process input and do back propagation..
        for (int i = numHidden; i >= 0; i--) // loop through all layers 
        {
            for (int j = 0; j < layers[i].numNeurons; j++) // for all the neurons of that layer
            {

                /* calc error.. */

                if (i == numHidden) // for output layer
                {
                    error = desiredOutput[j] - outputs[j];

                    //calculate error gradient using delta rule. (search google for more..)
                    layers[i].neurons[j].errorGradient = outputs[j] * (1 - outputs[j]) * error;
                }
                else
                {
                    //calculate error gradient using delta rule. (search google for more..)
                    layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * (1 - layers[i].neurons[j].output);

                    double errorGradSum = 0;

                    // calculate from next/following layers neuron
                    for (int m = 0; m < layers[i+1].numNeurons; m++)
                    {
                        //errorGradSum = SUM(error grad * weights)
                        errorGradSum += layers[i + 1].neurons[m].errorGradient * layers[i+1].neurons[m].weights[j];
                    }
                    layers[i].neurons[j].errorGradient *= errorGradSum;
                }

                /*Update weight*/

                // process all the neurons of a particular layer..
                for (int k = 0; k < layers[i].neurons[j].numOfInput; k++)
                {
                    if (i == numHidden) // for output layer
                    {
                        error = desiredOutput[j] - outputs[j];
                        layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * error;
                    }
                    else
                    {
                        layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
                    }
                }

                // update bias..
                layers[i].neurons[j].bias += alpha * -1 * layers[i].neurons[j].errorGradient;

            }
        }
    }

    #region Activation Functions..
    double ActivationFunction(double value)
    {
        return Relu(value);
    }
    double ActivationFunctionO(double value)
    {
        return Sigmoid(value);
    }

    private double TanH(double value)
    {
        return (2 * (Sigmoid(2 * value)) - 1);
    }

    double Step(double value)
    {
        if (value < 0) return 0;
        else return 1;
    }

    double Sigmoid(double value)
    {
        double k = (double)System.Math.Exp(value);

        return k / (1.0f + k);
    }
    double Relu(double value)
    {
        if (value > 0) return value;
        else return 0;
    }
    double LeakyRelu(double value)
    {
        if (value < 0) return 0.01*value;
        else return value;
    }


    #endregion
}
