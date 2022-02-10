using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager3 : MonoBehaviour
{
    public GameObject botPrefab;
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
            Vector3 startingPos = new Vector3(this.transform.position.x + Random.Range(-2, 2),
                                                this.transform.position.y, this.transform.position.z + Random.Range(-2, 2));
            GameObject bot = Instantiate(botPrefab, startingPos, Quaternion.identity);
            bot.GetComponent<Brain2>().Init();
            population.Add(bot);
        }
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 startingPos = new Vector3(this.transform.position.x + Random.Range(-2, 2),
                                                this.transform.position.y, this.transform.position.z + Random.Range(-2, 2));
        GameObject offspring = Instantiate(botPrefab, startingPos, Quaternion.identity);
        Brain2 b = offspring.GetComponent<Brain2>();

        if (Random.Range(0, 100) == 1)
        {
            b.Init();
            b.dna.Mutate();
        }
        else
        {
            b.Init();
            b.dna.Combine(parent1.GetComponent<Brain2>().dna, parent2.GetComponent<Brain2>().dna);
        }
        return offspring;
    }

    private void BreedNewPopulation()
    {
        List<GameObject> sortedList = population.OrderBy(o => 
                                        (o.GetComponent<Brain2>().timeWalking * 5 + o.GetComponent<Brain2>().timeAlive)).ToList(); // sorted in ascending order..
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
