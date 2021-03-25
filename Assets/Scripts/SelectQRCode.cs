﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SelectQRCode : NetworkBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    [SerializeField]
    private Camera cam;
    private Transform m_transform;

    private GameObject selectedQRCodeObj;
    private string selectedCubeTransform;
    private Vector3 m_QRCubePos;
    private Quaternion m_QRCubeRot;
    private Vector3 worldPos;
    private Quaternion worldRot;

    private GameObject ModelTransform;
    private GameObject ModelChilds;
    [SerializeField]
    private GameObject TransformTools;
    private GameObject go;
    [SerializeField]
    private LayerMask mask;

    //------UI--------//
    private DataScript dataScript;
    private string dataCubeName;

    private void Awake()
    {
        dataScript = FindObjectOfType<DataScript>();
    }

    public override void OnStartLocalPlayer()
    {
        dataScript.selectQR = this;

        if (cam == null)
        {
            Debug.LogError("SelectQRCode: No Camera referenced!");
            this.enabled = false;
        }
        m_transform = gameObject.GetComponent<Transform>();
        ModelTransform = GameObject.FindGameObjectWithTag("ModelToAlign");
        ModelChilds = GameObject.FindGameObjectWithTag("ModelQRCodes");
        csvController.GetInstance().loadFile(Application.dataPath + "/Resources", "Metadata.csv");

    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Input.GetMouseButtonDown(0))
        {
            Selecting();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CmdSendDataToServer();
        }
    }
    

    void Selecting()
    {
        mask = LayerMask.GetMask("QRCode");
        if (Physics.Raycast(ray, out hit, mask) && hit.collider.transform.name == "Panel")
        {
            Debug.DrawLine(m_transform.position, hit.point, Color.red);
            //put hitted object in the selectedCueTransform
            selectedCubeTransform = hit.collider.transform.parent.tag;
            CmdSendPosAndtrueToServer(selectedCubeTransform);
        }
    }

    [Command]
    void CmdSendPosAndtrueToServer(string name)
    {
        RpcSendPosAndtrueToServer(name);
    }

    [ClientRpc]
    void RpcSendPosAndtrueToServer(string name)
    {
        selectedQRCodeObj = GameObject.FindGameObjectWithTag(name);
        GetSelectedCubeTransformInfo(selectedQRCodeObj.transform);
        SetObjectAlignwithTransformPoint(selectedQRCodeObj.name);
        ChangeModelToAlignTransform(m_QRCubePos, m_QRCubeRot);
    }

    //Data message
    [Command]
    void CmdSendDataToServer()
    {
        dataCubeName = csvController.GetInstance().getString(1, 0);
        dataScript.dataText = dataCubeName;
    }

    void GetSelectedCubeTransformInfo(Transform parentTransform)
    {
        m_QRCubePos = parentTransform.position;
        m_QRCubeRot = parentTransform.rotation;
    }


    void SetObjectAlignwithTransformPoint(string transformName)
    {
        Debug.Log(selectedQRCodeObj.tag);
        ModelChilds = GameObject.FindGameObjectWithTag("ModelQRCodes");

        //worldPos = ModelChilds.transform.GetChild(0).position;
        worldPos = FindInChildren(ModelChilds, transformName).transform.position;
        worldRot = FindInChildren(ModelChilds, transformName).transform.rotation;

        //spawn a empty object align with QRCode and set as model's parent
        go = GameObject.Instantiate(TransformTools, Vector3.up, Quaternion.identity);
        go.transform.position = worldPos;
        go.transform.rotation = worldRot;
    }

    //Pass the ShowCaseRoom triggered QRCode object position and rotation to the empty object.
    //And rotating ModelToAlign object base on the empty object
    void ChangeModelToAlignTransform(Vector3 m_QRCubePos, Quaternion m_QRCubeRot)
    {
        ModelTransform = GameObject.FindGameObjectWithTag("ModelToAlign");
        ModelTransform.transform.SetParent(go.transform);
        go.transform.position = m_QRCubePos;
        go.transform.rotation = m_QRCubeRot;
        ModelTransform.transform.SetParent(null);
        Destroy(go);
    }

    public Transform FindInChildren(Transform transform, string name)
    {
        if (transform == null) return null;
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name == name) return child;
            Transform subChild = FindInChildren(child, name);
            if (subChild != null) return subChild;
        }
        return null;
    }

    public GameObject FindInChildren(GameObject gameObject, string name)
    {
        if (gameObject == null) return null;
        Transform transform = gameObject.transform;
        Transform child = FindInChildren(transform, name);
        return child != null ? child.gameObject : null;
    }
}
