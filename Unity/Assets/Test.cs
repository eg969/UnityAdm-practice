using UnityEngine;

using static AudioBlockInterface;
using static AudioBlockObject;

public class Test : MonoBehaviour
{

    public GameObject objectInstance;
    public AudioClip ambisonicsClip;

    void Awake()
    {
        readFile("/Users/edgarsg/Desktop/panned_noise_adm.wav");

        /*string path = AssetDatabase.GetAssetPath(ambisonicsClip);
        AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
        audioImporter.ambisonic = true;
        AssetDatabase.ImportAsset(path);*/
    }

    void Update()
    {
        updateAudioBlockObjects(objectInstance);
    }

}
