using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager4 : MonoBehaviour
{
    public GameObject botPrefab;
    public GameObject startingPos;
    public int populationSize = 50;

    List<GameObject> population = new List<GameObject>();

    public static float timeElapsed = 0;
    public float trialTime = 5;

    int generation = 1;

    GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", timeElapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.EndGroup();
    }

    private void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject bot = Instantiate(botPrefab, startingPos.transform.position, Quaternion.identity);
            bot.GetComponent<Brain3>().Init();
            population.Add(bot);
        }
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        GameObject offspring = Instantiate(botPrefab, startingPos.transform.position, Quaternion.identity);
        Brain3 b = offspring.GetComponent<Brain3>();

        if (Random.Range(0, 100) == 1)
        {
            b.Init();
            b.dna.Mutate();
        }
        else
        {
            b.Init();
            b.dna.Combine(parent1.GetComponent<Brain3>().dna, parent2.GetComponent<Brain3>().dna);
        }
        return offspring;
    }

    private void BreedNewPopulation()
    {
        List<GameObject> sortedList = population.OrderBy(o =>
                                        o.GetComponent<Brain3>().distanceTravelled ).ToList(); // sorted in ascending order..
        //List<GameObject> sortedList = population.OrderByDescending(o => o.GetComponent<DNA>().timeToDie).ToList(); // sorted in descending order..

        population.Clear();

        //breed upper half of sorted list..
        for (int i = (int)(sortedList.Count / 2f) - 1; i < sortedList.Count - 1; i++)
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

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= trialTime)
        {
            BreedNewPopulation();
            timeElapsed = 0;
        }
    }
}
