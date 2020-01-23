using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioBlockWrapper;
using System.Text;

public class Test : MonoBehaviour
{
    public GameObject objectInstance;
    public GameObject objectPositionInstance;
    private List<GameObject> audioObjects = new List<GameObject>();

    int i = 0;
    private float lerpTime = 1f;
    private float currentLerpTime = 0f;

    UnityAudioBlock previousUnityBlock;
    UnityAudioBlock nextUnityBlock;
    //List<UnityAudioBlock> blocks = new List<UnityAudioBlock>();
    List<List<UnityAudioBlock>> channelFormats = new List<List<UnityAudioBlock>>();
    float prec = 0f;

    Vector3 start;
    Vector3 end;

    void Start()
    {

        int j = 0;

        while (queryBlock(j, 0))
        {
            List<UnityAudioBlock> blocks = new List<UnityAudioBlock>();
            int k = 0;
            GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
            audioObjects.Add(audioObjectInstance);

            while (queryBlock(j, k))
            {
                UnityAudioBlock nextBlock = getBlock(j, k);
                blocks.Add(nextBlock);
                GameObject objectPosition = Instantiate(objectPositionInstance) as GameObject;

                objectPosition.transform.position = new Vector3(nextBlock.x, nextBlock.y, nextBlock.z);
                k++;
            }

            channelFormats.Add(blocks);
            j++;
        }
        //StartCoroutine(TransportBlocks());
    }

    IEnumerator TransportBlocks()
    {  
        int j = 0;

        while (queryBlock(j, 0))
        {
            List<UnityAudioBlock> blocks = new List<UnityAudioBlock>();
            int k = 0;
            GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
            audioObjects.Add(audioObjectInstance);

            while (queryBlock(j, k))
            {
                UnityAudioBlock nextBlock = getBlock(j, k);
                blocks.Add(nextBlock);
                GameObject objectPosition = Instantiate(objectPositionInstance) as GameObject;

                objectPosition.transform.position = new Vector3(nextBlock.x, nextBlock.y, nextBlock.z);
                k++;
            }

            channelFormats.Add(blocks);
            j++;

            yield return new WaitForSeconds(3);
        }
    }

    void Update()
    {
        for (int y = 0; y < channelFormats.Count; y++)
        {
            i = 0;
            if (i + 1 < channelFormats[y].Count)
            {
                previousUnityBlock = channelFormats[y][i];

                nextUnityBlock = channelFormats[y][i + 1];

                lerpTime = nextUnityBlock.rTime - previousUnityBlock.rTime;
                if (lerpTime == 0f) lerpTime = 1f;

                start = new Vector3(previousUnityBlock.x, previousUnityBlock.y, previousUnityBlock.z);
                end = new Vector3(nextUnityBlock.x, nextUnityBlock.y, nextUnityBlock.z);

                if (prec == 1f)
                {
                    currentLerpTime = 0f;
                    i++;
                }

            }

            currentLerpTime += Time.deltaTime;

            if (currentLerpTime >= lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            prec = (currentLerpTime / lerpTime);

            audioObjects[y].transform.position = Vector3.Lerp(start, end, prec);

        }
    }

}
