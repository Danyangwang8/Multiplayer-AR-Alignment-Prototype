using System.Collections;
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

    private Transform ModelTransform;
    [SerializeField]
    private GameObject TransformTools;
    private Transform ModelTransformChild;
    private GameObject go;
    [SerializeField]
    private LayerMask mask;
    public bool isSelected = false;

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
        ModelTransform = GameObject.Find("ModelToAlign").GetComponent<Transform>();
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
            Debug.Log("isSelected: " + isSelected);
        }
        if(isSelected)
        {

            
            isSelected = false;
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
           // selectedQRCodeObj = GameObject.FindGameObjectWithTag(selectedCubeTransform);
            isSelected = true;
            CmdSendPosAndtrueToServer(selectedCubeTransform);
        }
    }

    [Command]
    void CmdSendPosAndtrueToServer(string name)
    {
        selectedQRCodeObj = GameObject.FindGameObjectWithTag(name);
        isSelected = true;
        GetSelectedCubeTransformInfo(selectedQRCodeObj.transform);
        SetObjectAlignwithTransformPoint(selectedQRCodeObj.transform);
        ChangeModelToAlignTransform(m_QRCubePos, m_QRCubeRot);
        Debug.Log(selectedQRCodeObj.name);
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


    void SetObjectAlignwithTransformPoint(Transform transformName)
    {
        Transform ModelChilds = ModelTransform.Find("QRcodes");
        ModelTransformChild = ModelChilds.transform.Find(transformName.name);

        Vector3 worldPos = ModelTransformChild.position;
        Quaternion worldRot = ModelTransformChild.rotation;

        //spawn a empty object align with QRCode and set as model's parent
        go = GameObject.Instantiate(TransformTools, Vector3.up, Quaternion.identity);

        go.transform.position = worldPos;
        go.transform.rotation = worldRot;
    }

    //Pass the ShowCaseRoom triggered QRCode object position and rotation to the empty object.
    //And rotating ModelToAlign object base on the empty object
    void ChangeModelToAlignTransform(Vector3 m_QRCubePos, Quaternion m_QRCubeRot)
    {
        ModelTransform.SetParent(go.transform);

        go.transform.position = m_QRCubePos;
        go.transform.rotation = m_QRCubeRot;
        Debug.Log("Moving Cube");
        ModelTransform.SetParent(null);
        Destroy(go);
    }
}
