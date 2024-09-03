using UnityEngine;

namespace ArioSoren.GeneralUtility
{
    public class SplashScreenTechnicalLanunch : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("start");
            if (!PlayerPrefs.HasKey("Technical:Launch:FirstTime:SplashScreen"))
            {
            
                Debug.Log("tech events called");
                PlayerPrefs.SetInt("Technical:Launch:FirstTime:SplashScreen",1);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
