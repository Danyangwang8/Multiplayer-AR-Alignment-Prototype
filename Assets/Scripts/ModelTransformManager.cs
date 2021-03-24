using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ModelTransformManager : NetworkBehaviour
{
    public Transform m_transform;
    public SelectQRCode selectQR;

    [SyncVar(hook = nameof(OnPosChanged))]
    public Vector3 current_Pos;
    [SyncVar(hook = nameof(OnRotChanged))]
    public Quaternion current_Rot;

    void OnPosChanged(Vector3 oldPos, Vector3 newPos)
    {

       // m_transform.position = newPos;
    }

    void OnRotChanged(Quaternion oldRot, Quaternion newRot)
    {
       // m_transform.rotation = newRot;
    }
}
