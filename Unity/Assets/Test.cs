using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioBlockWrapper;
using System.Text;
using System;
using System.Threading;
using UnityEditor;

public class Test : MonoBehaviour
{
    public GameObject objectInstance;
    Thread newThread;
    Dictionary<int, GameObject> audioObjects = new Dictionary<int, GameObject>();

    void Start()
    {
        newThread = new Thread(new ThreadStart(getNewBlock));
        newThread.Start();  
    }

    void addAnimations()
    {
        for(int i = 0; i < channelFormats.Count; i++)
        {
            if (!audioObjects.ContainsKey(channelFormats[i].cfId))
            {
                GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
                audioObjects.Add(channelFormats[i].cfId, audioObjectInstance);
               
            }
            if (channelFormats[i].audioBlocks.Count > 1 && !channelFormats[i].moivng)
            {

                Vector3 start = new Vector3(channelFormats[i].audioBlocks[0].x, channelFormats[i].audioBlocks[0].y, channelFormats[i].audioBlocks[0].z);
                Vector3 end = new Vector3(channelFormats[i].audioBlocks[1].x, channelFormats[i].audioBlocks[1].y, channelFormats[i].audioBlocks[1].z);

                float duration;

                channelFormats[i].startPos = start;
                channelFormats[i].endPos = end;
                channelFormats[i].currentLerpTime = 0f;
                duration = channelFormats[i].audioBlocks[1].rTime - channelFormats[i].audioBlocks[0].rTime;

                if (Time.time < channelFormats[i].audioBlocks[1].rTime)
                {
                    channelFormats[i].currentLerpTime = Time.time - channelFormats[i].audioBlocks[0].rTime;
                }
                else
                {
                    duration = 0;
                }


                channelFormats[i].lerpTime = duration;
                channelFormats[i].moivng = true;

                channelFormats[i].audioBlocks.RemoveAt(0);
            }     
        }
    }

    void moveObjects()
    {
        for (int i = 0; i < channelFormats.Count; i++)
        {
            if (channelFormats[i].moivng)
            {
                float prec;

                if (channelFormats[i].currentLerpTime >= channelFormats[i].lerpTime)
                {
                    channelFormats[i].currentLerpTime = channelFormats[i].lerpTime;
                    channelFormats[i].moivng = false;
                }

                if (channelFormats[i].lerpTime == 0)
                {
                    channelFormats[i].moivng = false;
                    prec = 1f;
                }
                else
                {
                    prec = channelFormats[i].currentLerpTime / channelFormats[i].lerpTime;
                }

               
                Vector3 newPos = Vector3.Lerp(channelFormats[i].startPos, channelFormats[i].endPos, prec);
                audioObjects[channelFormats[i].cfId].transform.position = newPos;
                channelFormats[i].currentLerpTime += Time.deltaTime;
            }
        }
    }
    void Update()
    {
        lock (lockObject)
        {
            addAnimations();
        }
        moveObjects();
    }

}
