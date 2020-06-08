using UnityEngine;

using static AudioBlockInterface;
using static AudioBlockObjects;
using static AudioBlockHoa;
using System.Reflection;
using UnityEditor;
using System.Text;

public class Main : MonoBehaviour
{

    public GameObject objectInstance;


    void Awake()
    {
        readFileForUnityAudio("/Users/edgarsg/Desktop/demo.wav");
    }

void Update()
    {
       updateAudioBlockObjects(objectInstance);
    }

}
