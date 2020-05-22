using UnityEngine;

using static AudioBlockInterface;
using static AudioBlockObjects;
using static AudioBlockHoa;
using UnityEditor;

public class Test : MonoBehaviour
{

    public GameObject objectInstance;
    public AudioClip ambisonicsClip;

    void Awake()
    {
        readFile("/Users/edgarsg/Desktop/hoa_4ch_1stOrderAmbix_commondef.wav");

        /*string path = AssetDatabase.GetAssetPath(ambisonicsClip);
        AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
        audioImporter.ambisonic = true;
        AssetDatabase.ImportAsset(path);*/
    }

    void Update()
    {
        updateAudioBlockObjects(objectInstance);
        updateAudioBlockHoa(objectInstance);
    }

}
