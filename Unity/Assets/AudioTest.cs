using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public GameObject objectInstance;
    private float lerpTime = 1f;
    private float currentLerpTime = 0f;



    Vector3 start;
    Vector3 end;

    Vector3 p1;
    Vector3 p2;
    Vector3 p3;
    Vector3 p4;

    int i = 0;

    float prec = 1f;
    void Start()
    {
        p1 = new Vector3(0, 0, 10);
        p2 = new Vector3(-10, 0, 0);
        p3 = new Vector3(0, 0, -10);
        p4 = new Vector3(10, 0, 0);

        start = p1;
        end = p2;

    }

    // Update is called once per frame
    void Update()
    {
        currentLerpTime += 0.5f;//Time.deltaTime;

        if (currentLerpTime >= lerpTime)
        {
            currentLerpTime = lerpTime;
            i++;
        }

        prec = currentLerpTime/lerpTime;

        objectInstance.transform.position = Vector3.Slerp(start, end, prec);


        if (prec == 1f)
        {
            currentLerpTime = 0f;
        }

        if (i == 1)
        {
            start = p2;
            end = p3;
        }
        else if (i == 2)
        {
            start = p3;
            end = p4;
        }
        else if (i == 3)
        {
            start = p4;
            end = p1;
        }
        else if (i == 4)
        {
            start = p1;
            end = p2;

            i = 0;
        }
    }
}
