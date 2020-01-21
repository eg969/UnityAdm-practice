using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioBlockWrapper;
using System.Text;

public class Test : MonoBehaviour
{
    public GameObject audioObject;
    public GameObject audioObjectInstance;
    int i = 0;
    private float lerpTime = 1f;
    private float currentLerpTime = 0f;

    UnityAudioBlock previousUnityBlock;
    UnityAudioBlock nextUnityBlock;
    List<UnityAudioBlock> blocks = new List<UnityAudioBlock>();
    Vector3 start;
    Vector3 end;
    float prec = 1f;

    void Start()
    {
        /*reviousUnityBlock = getBlock(0);
        nextUnityBlock = getBlock(1);
        start = new Vector3(previousUnityBlock.x, previousUnityBlock.y, previousUnityBlock.z);
        end = new Vector3(nextUnityBlock.x, nextUnityBlock.y, nextUnityBlock.z);*/

        for (int k = 0; k < 7; k++)
        {
            UnityAudioBlock nextBlock = getBlock(k);
            blocks.Add(nextBlock);
            GameObject a = Instantiate(audioObjectInstance) as GameObject;

            a.transform.position = new Vector3(nextBlock.x, nextBlock.y, nextBlock.z);
        }

    }

    void Update()
    {
        if (prec == 1f)
        {
            if (i == 6)
            {
                i = 0;
            }

            currentLerpTime = 0f;

            previousUnityBlock = blocks[i];
            nextUnityBlock = blocks[i + 1];
            i++;

            start = new Vector3(previousUnityBlock.x, previousUnityBlock.y, previousUnityBlock.z);
            end = new Vector3(nextUnityBlock.x, nextUnityBlock.y, nextUnityBlock.z);

        }


        currentLerpTime += Time.deltaTime;

        if (currentLerpTime >= lerpTime)
        {
            currentLerpTime = lerpTime;
        }

        prec = (currentLerpTime / lerpTime);

        audioObject.transform.position = Vector3.Lerp(start, end, prec);
    }

}
