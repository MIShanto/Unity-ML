using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainingSet2
{
	public double[] input;
	public double output;
}

public class Perceptron2 : MonoBehaviour
{

	public TrainingSet2[] ts;
	double[] weights = { 0, 0 };
	double bias = 0;
	double totalError = 0;

	[SerializeField] SimpleGrapher sg;
	double DotProductBias(double[] v1, double[] v2)
	{
		if (v1 == null || v2 == null)
			return -1;

		if (v1.Length != v2.Length)
			return -1;

		double d = 0;
		for (int x = 0; x < v1.Length; x++)
		{
			d += v1[x] * v2[x];
		}

		d += bias;

		return d;
	}

	double CalcOutput(int i)
	{
		double dp = DotProductBias(weights, ts[i].input);
		if (dp > 0) return (1);
		return (0);
	}

	void InitialiseWeights()
	{
		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = Random.Range(-1.0f, 1.0f);
		}
		bias = Random.Range(-1.0f, 1.0f);
	}

	void UpdateWeights(int j)
	{
		double error = ts[j].output - CalcOutput(j);
		totalError += Mathf.Abs((float)error);
		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = weights[i] + error * ts[j].input[i];
		}
		bias += error;
	}

	double CalcOutput(double i1, double i2)
	{
		double[] inp = new double[] { i1, i2 };
		double dp = DotProductBias(weights, inp);
		if (dp > 0) return (1);
		return (0);
	}

	void Train(int epochs)
	{
		InitialiseWeights();

		for (int e = 0; e < epochs; e++)
		{
			totalError = 0;
			for (int t = 0; t < ts.Length; t++)
			{
				UpdateWeights(t);
				Debug.Log("W1: " + (weights[0]) + " W2: " + (weights[1]) + " B: " + bias);
			}
			Debug.Log("TOTAL ERROR: " + totalError);
		}
	}

	void DrawAllPoints()
	{
		for (int i = 0; i < ts.Length; i++)
		{
			if (ts[i].output == 0)
			{
				sg.DrawPoint((float)ts[i].input[0], (float)ts[i].input[1], Color.magenta);
			}
			else
				sg.DrawPoint((float)ts[i].input[0], (float)ts[i].input[1], Color.green);

		}
	}
	void Start()
	{
		Train(300);
		DrawAllPoints();
		sg.DrawRay((float)(-(bias / weights[1]) / (bias / weights[0])), (float)(-bias / weights[1]), Color.red);

		//Test set...
		//1st parameter = how sharp it is
		//2nd parameter = how edible it is
		if (CalcOutput(0.3f, 0.9f) == 0)
			sg.DrawPoint(0.3f, 0.9f, Color.red);
		else
			sg.DrawPoint(0.3f, 0.9f, Color.yellow);
		if (CalcOutput(0.8f, 0.1f) == 0)
			sg.DrawPoint(0.8f, 0.1f, Color.red);
		else
			sg.DrawPoint(0.8f, 0.1f, Color.yellow);
	}
}
