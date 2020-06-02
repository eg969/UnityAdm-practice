using UnityEngine;

using static AudioBlockInterface;
using static AudioBlockObjects;
using static AudioBlockHoa;
using static AudioBlockWrapper;
using UnityEditor;

public class UnityAdm : MonoBehaviour
{

    public GameObject objectVisualisation;
    public GameObject hoaVisualisation;
    public AudioClip ambisonicsClip;

    void Awake()
    {
        // TODO - hard-coded paths are temp
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
        {
            readFile("C:\\Users\\matthewf\\Desktop\\Edgars\\TestFiles\\panned_noise_adm.wav");
        }
        else if(SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
        {
            readFile("/Users/edgarsg/Desktop/hoa_4ch_1stOrderAmbix_commondef.wav");
        }

        /*string path = AssetDatabase.GetAssetPath(ambisonicsClip);
        AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
        audioImporter.ambisonic = true;
        AssetDatabase.ImportAsset(path);*/
    }

    void Update()
    {
        updateAudioBlockObjects(objectVisualisation);
        updateAudioBlockHoa(hoaVisualisation);
    }

}
