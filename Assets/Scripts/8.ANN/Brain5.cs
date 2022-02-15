using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain5 : MonoBehaviour
{
    ANN ann;

    double sumSquareError = 0;

    void Start()
    {
        ann = new ANN(2, 1, 1, 2, 0.8);

        List<double> result;


        #region Train
        // XOR test..
        for (int i = 0; i < 1000; i++) // 1000 epoch
	    {
            sumSquareError = 0;

            result = Train(1, 1, 0); // input: 1,1. expected out: 0
            // get squared error: (out - desiredOut)^2
            sumSquareError += Mathf.Pow((float)result[0] - 0, 2);
            
            result = Train(1, 0, 1); 
            sumSquareError += Mathf.Pow((float)result[0] - 1, 2);

            result = Train(0, 1, 1);
            sumSquareError += Mathf.Pow((float)result[0] - 1, 2);

            result = Train(0, 0, 0);
            sumSquareError += Mathf.Pow((float)result[0] - 0, 2);
        }

        #endregion
        // we expect that to become smaller and smaller..
        Debug.Log($"SSE: {sumSquareError}");

        #region Test

        result = Train(1, 1, 0);
        Debug.Log($"1 1 {result[0]}");

        result = Train(1, 0, 1);
        Debug.Log($"1 0 {result[0]}");

        result = Train(0, 1, 1);
        Debug.Log($"0 1 {result[0]}");

        result = Train(0, 0, 0);
        Debug.Log($"0 0 {result[0]}");

        #endregion

    }

    private List<double> Train(int i1, int i2, int o)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        inputs.Add(i1);
        inputs.Add(i2);

        outputs.Add(o);

        return (ann.Go(inputs, outputs));
    }
}
