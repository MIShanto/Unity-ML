using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Brain3 : MonoBehaviour
{
    public int DNALength = 2;
    public float distanceTravelled = 0;
    public DNA2 dna;
    public GameObject eyes;

    bool alive = true;
    bool seeWall = true;

    Vector3 startPosition;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("dead"))
        {
            alive = false;
            distanceTravelled = 0;
        }
    }
    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        //initialize DNA
        // 0 forward
        // 1 angle turn
        dna = new DNA2(DNALength, 360);
        startPosition = this.transform.position;
    }

    private void Update()
    {
        if (!alive) return;

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 0.5f, Color.red);

        seeWall = false;
        RaycastHit rayHit;
        if (Physics.SphereCast(eyes.transform.position, 0.1f,  eyes.transform.forward, out rayHit, 0.5f))
        {
            if (rayHit.collider.CompareTag("wall"))
                seeWall = true;
        }
    }

    private void FixedUpdate()
    {
        if (!alive) return;

        //read DNA
        float turn, move;

        turn = 0; // in angle
        move = dna.GetGene(0);

        if (seeWall)
        {
            turn = dna.GetGene(1);
        }

        this.transform.Translate(0, 0, move * 0.001f);
        this.transform.Rotate(0, turn, 0);

        distanceTravelled = Vector3.Distance(startPosition, transform.position);
    }
}
