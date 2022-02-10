using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class Brain : MonoBehaviour
{
    public int DNALength = 1;
    public float timeAlive;
    public DNA2 dna;

    ThirdPersonCharacter m_character;

    Vector3 m_move;

    bool m_jump;
    bool alive = true;

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
        // 1 back
        // 2 left
        // 3 right
        // 4 jump
        // 5 crouch

        dna = new DNA2(DNALength, 6);
        m_character = GetComponent<ThirdPersonCharacter>();
        timeAlive = 0;
        alive = true;

    }

    private void FixedUpdate()
    {
        //read DNA
        float h, v;
        h = v = 0;

        bool crouch = false;

        if (dna.GetGene(0) == 0) v = 1; // forward..
        else if (dna.GetGene(0) == 1) v = -1; // back
        else if (dna.GetGene(0) == 2) h = -1; // left
        else if (dna.GetGene(0) == 3) h = 1; // right
        else if (dna.GetGene(0) == 4) m_jump = true; // jump
        else if (dna.GetGene(0) == 5) crouch = true; // crouch

        m_move = v * Vector3.forward + h * Vector3.right;

        m_character.Move(m_move, crouch, m_jump);

        m_jump = false;
        if (alive)
            timeAlive += Time.deltaTime;

    }
}
