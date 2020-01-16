using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioBlockWrapper;
using System.Text;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //print("Random Number: " + RandomNumberWrapper.GetRandNum());

        for (int i = 0; i < 3; i++)
        {
            UnityAudioBlock nextUnityBlock = getBlock(i);

            print("Name: " + Encoding.ASCII.GetString(nextUnityBlock.name));
                //string(nextUnityBlock.name));
            print("Azimuth: " + nextUnityBlock.azimuth);
            print("Elevation: " + nextUnityBlock.elevation);
            print("Distance: " + nextUnityBlock.distance);
        }

    }

}
