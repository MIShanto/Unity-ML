using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain6 : MonoBehaviour
{
    /*          Inputs                 Outputs
         *BallX,             |      Paddle Velocity Y
         *BallY,             |
         *Ball velocity X,   |
         *Ball velocity Y,   |
         *Paddle X,          |
         *Paddle Y           |
     */

    public GameObject paddle;
    public GameObject ball;

    Rigidbody2D ball_rb;

    float yVelocity;
    float paddleMinY = 8.8f;
    float paddleMaxY = 17.4f;
    float paddleMaxSpeed = 15;

    public float numSaved = 0;
    public float numMissed = 0;

    ANN2 ann;

    //[SerializeField] LayerMask layerMask;

    private void Start()
    {
        ball_rb = ball.GetComponent<Rigidbody2D>();

        ann = new ANN2(6, 1, 1, 4, 0.11);
    }

    List<double> Run(double bx, double by, double bvx, double bvy, double px, double py, double pv, bool train)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        inputs.Add(bx);
        inputs.Add(by);
        inputs.Add(bvx);
        inputs.Add(bvy);
        inputs.Add(px);
        inputs.Add(py);

        outputs.Add(pv);

        if (train)
            return (ann.Train(inputs, outputs));
        else
            return (ann.CalcOutput(inputs, outputs));
    }

    private void Update()
    {
        float posy = Mathf.Clamp(paddle.transform.position.y + (yVelocity * Time.deltaTime * paddleMaxSpeed),
            paddleMinY, paddleMaxY);

        if (float.IsNaN(posy))
            posy = transform.position.y;

        paddle.transform.position = new Vector3(paddle.transform.position.x, posy, paddle.transform.position.z);

        List<double> output = new List<double>();

        int layerMask = 1 << 10;
        RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, ball_rb.velocity, 1000, layerMask);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("tops")) // reflect off top
            {
                Vector3 reflection = Vector3.Reflect(ball_rb.velocity, hit.normal);
                hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
            }

            if (hit.collider != null && hit.collider.gameObject.CompareTag("backwall"))
            {
                //the error..
                float error = (hit.point.y - paddle.transform.position.y);

                output = Run(ball.transform.position.x,
                                ball.transform.position.y,
                                ball_rb.velocity.x, ball_rb.velocity.y,
                                paddle.transform.position.x,
                                paddle.transform.position.y,
                                error, true);

                yVelocity = (float)output[0];
            }
        }
        else
            yVelocity = 0;
    }
}
