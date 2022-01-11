using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour
{
    //gene for color
    public float red;
    public float green;
    public float blue;

    bool isDead = false;

    public float timeToDie = 0f;

    SpriteRenderer sr;
    Collider2D col;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        sr.color = new Color(red, green, blue);
    }

    private void OnMouseDown()
    {
        isDead = true;

        timeToDie = PopulationManager.timeElapsed;

        Debug.Log("Dead at: " + timeToDie);

        sr.enabled = false;
        col.enabled = false;
    }
}
