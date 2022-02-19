using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour 
{

	Rigidbody2D rb;
	[SerializeField] AudioSource blip;
	[SerializeField] AudioSource blop;

	Vector3 startPos;

	[SerializeField] float force = 400f;
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		startPos = transform.position;
		ResetBall();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("backwall"))
			blop.Play();
		else
			blip.Play();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			ResetBall();
	}

	private void ResetBall()
	{
		transform.position = startPos;
		rb.velocity = Vector2.zero;

		Vector3 ballMoveDir = new Vector3(Random.Range(100, 300), Random.Range(-100, 100), 0).normalized;
		rb.AddForce(ballMoveDir * force);
	}
}
