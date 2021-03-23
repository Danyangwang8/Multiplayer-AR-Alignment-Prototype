using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    GameObject[] objectsToDisable;
    Camera sceneCamera;

    void Start()
    {
        //Diable components and objects that should only be
        //active on the player that we control
        if(!isLocalPlayer)
        {
            DisableComponents();
            DisableObjects();
        }
        else
        {
            //we are the local player: Disable the scene camera
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    void DisableObjects()
    {
        for (int j = 0; j < objectsToDisable.Length; j++)
        {
            objectsToDisable[j].SetActive(false);
        }
    }
}
