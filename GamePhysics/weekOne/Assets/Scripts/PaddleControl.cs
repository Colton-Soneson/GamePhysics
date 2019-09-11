using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleControl : MonoBehaviour
{
    public GameObject PaddleOne;
    public GameObject PaddleTwo;

    Quaternion paddleOneStartMark;
    Quaternion paddleTwoStartMark;

    Quaternion paddleOneEndMark;
    Quaternion paddleTwoEndMark;

    public float speed = 1.0F;
    private float startTime;

    private float journeyLengthP1;
    private float journeyLengthP2;

    // Start is called before the first frame update
    void Start()
    {
        //startTime = Time.time;

        startTime = 0.0f;

        paddleOneStartMark = PaddleOne.GetComponent<Transform>().rotation;
        paddleTwoStartMark = PaddleTwo.GetComponent<Transform>().rotation;

 

        paddleOneEndMark = new Quaternion(37.334f, -29.231f, -18.746f, PaddleOne.GetComponent<Transform>().rotation.w);
        paddleTwoEndMark = new Quaternion(37.453f, 28.786f, 18.476f, PaddleTwo.GetComponent<Transform>().rotation.w);

        //journeyLengthP1 = Quaternion.Slerp(paddleOneStartMark, paddleOneEndMark);
        //journeyLengthP2 = Quaternion.Distance(paddleTwoStartMark, paddleTwoEndMark);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            PaddleOne.GetComponent<Transform>().rotation = Quaternion.Slerp(paddleOneStartMark, paddleOneEndMark, startTime);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            PaddleTwo.GetComponent<Transform>().rotation = Quaternion.Slerp(paddleTwoStartMark, paddleTwoEndMark, startTime);
        }

        startTime = startTime + Time.deltaTime;
    }
}
