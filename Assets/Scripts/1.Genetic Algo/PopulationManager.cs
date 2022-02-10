using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public static float timeElapsed = 0;

    public GameObject personPrefab;
    public int populationSize;

    public int trialTime = 10;
    int generation = 1;

    List<GameObject> population = new List<GameObject>();

    GUIStyle gUIStyle = new GUIStyle();
    private void OnGUI()
    {
        gUIStyle.fontSize = 20;
        gUIStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(10, 10, 50, 5), "Generation: " + generation, gUIStyle);
        GUI.Label(new Rect(10, 65, 50, 5), "Trial Time: " + (int) timeElapsed, gUIStyle);
    }

    private void Start()
    {

        //generate population and set color values..
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-8, 8), UnityEngine.Random.Range(-4f, 5f), 0);
            GameObject obj = Instantiate(personPrefab, pos, Quaternion.identity);

            //Set RGB values to DNA

            obj.GetComponent<DNA>().red = UnityEngine.Random.Range(0, 1f);
            obj.GetComponent<DNA>().green = UnityEngine.Random.Range(0, 1f);
            obj.GetComponent<DNA>().blue = UnityEngine.Random.Range(0, 1f);
            obj.GetComponent<DNA>().scale = UnityEngine.Random.Range(0.1f, 0.3f);

            population.Add(obj);
        }
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed > trialTime)
        {
            BreedNewPopulation();
            timeElapsed = 0;
        }
    }

    private void BreedNewPopulation()
    {
        List<GameObject> newPopulation = new List<GameObject>();

        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<DNA>().timeToDie).ToList(); // sorted in ascending order..
        //List<GameObject> sortedList = population.OrderByDescending(o => o.GetComponent<DNA>().timeToDie).ToList(); // sorted in descending order..

        population.Clear();

        //breed upper half of sorted list..
        for (int i = (int) (sortedList.Count / 2f) - 1; i < sortedList.Count - 1; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }

        //destroy all parents and previous population
        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }

        generation++;

    }

    private GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-8, 8), UnityEngine.Random.Range(-4f, 5f), 0);
        GameObject offSpring = Instantiate(personPrefab, pos, Quaternion.identity);

        DNA dna1 = parent1.GetComponent<DNA>();
        DNA dna2 = parent2.GetComponent<DNA>();

        // Generate less mutated and more non mutated..
        if(UnityEngine.Random.Range(0,1000) > 5)
        {
            //swap parent dna
            offSpring.GetComponent<DNA>().red = UnityEngine.Random.Range(0, 10) < 5 ? dna1.red : dna2.red;
            offSpring.GetComponent<DNA>().green = UnityEngine.Random.Range(0, 10) < 5 ? dna1.green : dna2.green;
            offSpring.GetComponent<DNA>().blue = UnityEngine.Random.Range(0, 10) < 5 ? dna1.blue : dna2.blue;
            offSpring.GetComponent<DNA>().scale = UnityEngine.Random.Range(0, 10) < 5 ? dna1.scale : dna2.scale;
        }
        else
        {
            //Mutated offsprings...
            offSpring.GetComponent<DNA>().red = UnityEngine.Random.Range(0, 10);
            offSpring.GetComponent<DNA>().green = UnityEngine.Random.Range(0, 10);
            offSpring.GetComponent<DNA>().blue = UnityEngine.Random.Range(0, 10);
            offSpring.GetComponent<DNA>().scale = UnityEngine.Random.Range(0.1f, 0.3f);
        }
        

        return offSpring;
    }
}
