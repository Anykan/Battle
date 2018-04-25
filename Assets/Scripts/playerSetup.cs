using UnityEngine;
using UnityEngine.Networking;

public class playerSetup : NetworkBehaviour {
    
    [SerializeField]
    Behaviour[] componentsToDisable;
    Camera sceneCamera;

    void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        else
        {
            //deaktiviert die Startcamera beim join
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
            
        }

    }

    //aktiviert die scenevcamera bei disconnecten
    void OnDisable()
    {
        sceneCamera = Camera.main;
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }
}
