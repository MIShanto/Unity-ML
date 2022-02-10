using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Brain2 : MonoBehaviour
{
    public int DNALength = 2;
    public float timeAlive;
    public float timeWalking;
    public DNA2 dna;
    public GameObject eyes;

    public GameObject ethanPrefab;
    GameObject ethan;

    bool seeGround = true;
    bool alive = true;

    private void OnDestroy()
    {
        Destroy(ethan);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("dead"))
        {
            alive = false;
        }
    }

    public void Init()
    {
        //initialize DNA
        // 0 forward
        // 1 left
        // 2 right

        dna = new DNA2(DNALength, 3);
        timeAlive = 0;
        timeWalking = 0;
        alive = true;

        ethan = Instantiate(ethanPrefab, transform.position, Quaternion.identity);
        ethan.GetComponent<AICharacterControl>().target = this.transform;
    }

    private void Update()
    {
        if (!alive) return;

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10, Color.white, 10);

        seeGround = false;
        RaycastHit rayHit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out rayHit))
        {
            if (rayHit.collider.CompareTag("platform"))
                seeGround = true;
        }

        timeAlive += PopulationManager3.timeElapsed;

        //read DNA
        float turn, move;
        turn = move = 0;

        if (seeGround)
        {
            if (dna.GetGene(0) == 0)
            {
                move = 1; // move forward..
                timeWalking += 1;
            }
            else if (dna.GetGene(0) == 1) turn = -90; // turn left
            else if (dna.GetGene(0) == 2) turn = 90; // turn right
        }
        else
        {
            if (dna.GetGene(1) == 0)
            {
                move = 1; // move forward..
                timeWalking += 1;
            }
            else if (dna.GetGene(1) == 1) turn = -90; // turn left
            else if (dna.GetGene(1) == 2) turn = 90; // turn right
        }

        this.transform.Translate(0, 0, move * 0.1f);
        this.transform.Rotate(0, turn , 0);
    }
}
