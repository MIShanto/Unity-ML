using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ANNDrive : MonoBehaviour
{
    ANN2 ann;
    public float visibleDistance = 200f;
    public int epochs = 1000;
    public float moveSpeed = 80.0f;
    public float rotationSpeed = 120.0f;
    public double initialAlpha = 0.5;

    bool trainingDone = false;
    float trainingProgress = 0;
    double sse = 0; // sum of squares error
    double lastSSE = 1;

    public float translation;
    public float rotation;

    public bool loadFromFile = false;

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + lastSSE);
        GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + ann.alpha);
        GUI.Label(new Rect(25, 55, 250, 30), "Trained: " + trainingProgress);
    }
    private void Start()
    {
        ann = new ANN2(5, 2, 1, 10, initialAlpha);

        //StartCoroutine(LoadTrainingSet());

        if (loadFromFile)
        {
            LoadWeightsFromFile();
            trainingDone = true;
        }
        else
            StartCoroutine(LoadTrainingSet());
    }

    IEnumerator LoadTrainingSet()
    {
        string path = Application.dataPath + "/trainingData.txt";
        string line;

        if (File.Exists(path))
        {
            int lineCount = File.ReadAllLines(path).Length;
            StreamReader trainingDataFile = File.OpenText(path);

            List<double> calcOutputs = new List<double>();
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            for (int i = 0; i < epochs; i++)
            {
                sse = 0;
                trainingDataFile.BaseStream.Position = 0;

                string currentWeights = ann.PrintWeights();

                while ((line = trainingDataFile.ReadLine()) != null)
                {
                    string[] data = line.Split(',');

                    float thisError = 0;

                    if (System.Convert.ToDouble(data[5]) != 0 && System.Convert.ToDouble(data[6]) != 0) //if output values arent zero do action..
                    {
                        inputs.Clear();
                        outputs.Clear();

                        inputs.Add(System.Convert.ToDouble(data[0]));
                        inputs.Add(System.Convert.ToDouble(data[1]));
                        inputs.Add(System.Convert.ToDouble(data[2]));
                        inputs.Add(System.Convert.ToDouble(data[3]));
                        inputs.Add(System.Convert.ToDouble(data[4]));

                        double output_1 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[5]));
                        outputs.Add(output_1);

                        double output_2 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[6]));
                        outputs.Add(output_2);

                        calcOutputs = ann.Train(inputs, outputs);

                        thisError = ((Mathf.Pow((float)(outputs[0] - calcOutputs[0]), 2) +
                                        Mathf.Pow((float)(outputs[1] - calcOutputs[1]), 2))) / 2.0f;
                    }

                    sse += thisError;
                }

                trainingProgress = (float)i / (float)epochs;
                sse /= lineCount;
                //lastSSE = sse;

                //We will use preserved weight here which found a decent local optimum. 
                // we r using it not to increase sse again...

                //if sse isn't better then reload previous weights
                // and decrease alpha.....

                if (lastSSE < sse)
                {
                    ann.LoadWeights(currentWeights);
                    ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f, 0.01f, 0.9f);
                }
                else // increase alpha
                {
                    ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f, 0.01f, 0.9f);
                    lastSSE = sse;
                }

                yield return null;
            }
        }

        trainingDone = true;
        SaveWeightsToFile();
    }

    void SaveWeightsToFile()
    {
        string path = Application.dataPath + "/ANNweights.txt";
        StreamWriter wf = File.CreateText(path);
        wf.WriteLine(ann.PrintWeights());
        wf.Close();
    }

    void LoadWeightsFromFile()
    {
        string path = Application.dataPath + "/ANNweights.txt";
        StreamReader wf = File.OpenText(path);

        if (File.Exists(path))
        {
            string line = wf.ReadLine();
            ann.LoadWeights(line);
        }
    }

    float Map(float newfrom, float newto, float origfrom, float origto, float value)
    {
        if (value <= origfrom)
            return newfrom;
        else if (value >= origto)
            return newto;
        return (newto - newfrom) * ((value - origfrom) / (origto - origfrom)) + newfrom;
    }

    float Round(float value)
    {
        return ((float)System.Math.Round(value, System.MidpointRounding.AwayFromZero) / 2.0f);
    }

    private void Update()
    {
        if (!trainingDone) return;

        float forwardDist = 0, rightDist = 0, leftDist = 0,
              right45Dist = 0, left45Dist = 0;

        List<double> calcOutputs = new List<double>();
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        //Raycasts
        RaycastHit hit;

        //forward
        if (Physics.Raycast(transform.position, this.transform.forward, out hit, visibleDistance))
        {
            // we will keep farthest value to small(0) and closest value to big(1)
            forwardDist = 1 - Round(hit.distance / visibleDistance); // normalized and rounded
        }

        //right
        if (Physics.Raycast(transform.position, this.transform.right, out hit, visibleDistance))
        {
            // we will keep farthest value to small(0) and closest value to big(1)
            rightDist = 1 - Round(hit.distance / visibleDistance); // normalized and rounded
        }

        //left
        if (Physics.Raycast(transform.position, -this.transform.right, out hit, visibleDistance))
        {
            // we will keep farthest value to small(0) and closest value to big(1)
            leftDist = 1 - Round(hit.distance / visibleDistance); // normalized and rounded
        }

        //right45
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.right, out hit, visibleDistance))
        {
            // we will keep farthest value to small(0) and closest value to big(1)
            right45Dist = 1 - Round(hit.distance / visibleDistance); // normalized and rounded
        }

        //left45
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(45, Vector3.up) * -transform.right, out hit, visibleDistance))
        {
            // we will keep farthest value to small(0) and closest value to big(1)
            left45Dist = 1 - Round(hit.distance / visibleDistance); // normalized and rounded
        }

        inputs.Add(forwardDist);
        inputs.Add(rightDist);
        inputs.Add(leftDist);
        inputs.Add(right45Dist);
        inputs.Add(left45Dist);

        outputs.Add(0);
        outputs.Add(0);

        calcOutputs = ann.CalcOutput(inputs, outputs);

        float translationInput = Map(-1, 1, 0, 1, (float)calcOutputs[0]);
        float rotationInput = Map(-1, 1, 0, 1, (float)calcOutputs[1]);

        translation = translationInput * moveSpeed * Time.deltaTime;
        rotation = rotationInput * rotationSpeed * Time.deltaTime;

        this.transform.Translate(0, 0, translation);
        this.transform.Rotate(0, rotation, 0);
    }



}
