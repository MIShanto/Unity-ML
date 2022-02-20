using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Drive : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float visibleDistance = 200.0f;

    List<string> collectedTrainingData = new List<string>();

    StreamWriter trainingDataFile;
    private void Start()
    {
        string path = Application.dataPath + "/trainingData.txt";
        trainingDataFile = File.CreateText(path);
    }

    private void OnApplicationQuit()
    {
        foreach (string trainingData in collectedTrainingData)
        {
            trainingDataFile.WriteLine(trainingData);
        }

        trainingDataFile.Close();
    }

    float Round(float value)
    {
        return ((float) System.Math.Round(value, System.MidpointRounding.AwayFromZero) / 2.0f);
    }
    void Update()
    {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");

        // Make it move 10 meters per second instead of 10 meters per frame...
        float translation = Time.deltaTime * speed * translationInput;
        float rotation = Time.deltaTime * rotationSpeed * rotationInput;

        // Move translation along the object's z-axis
        transform.Translate(0, 0, translation);

        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);

        Debug.DrawRay(transform.position, this.transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, this.transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, -this.transform.right * visibleDistance, Color.red);
        //look left
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(45, Vector3.up) * -this.transform.right * visibleDistance, Color.green);
        //look right
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * this.transform.right * visibleDistance, Color.green);

        //Raycasts
        RaycastHit hit;
        float forwardDist = 0, rightDist = 0, leftDist = 0, 
              right45Dist = 0, left45Dist = 0;

        //forward
        if (Physics.Raycast(transform.position, this.transform.forward, out hit, visibleDistance))
        {
            // we will keep farthest value to small(0) and closest value to big(1)
            forwardDist =1 - Round(hit.distance/visibleDistance); // normalized and rounded
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

        //we will feed these input to ANN
        string trainingData = forwardDist + "," + rightDist + "," + leftDist + ","  + right45Dist + "," +
            left45Dist + "," + Round(translationInput) + "," + Round(rotationInput);

        if(!collectedTrainingData.Contains(trainingData))
            collectedTrainingData.Add(trainingData);
    }
}
