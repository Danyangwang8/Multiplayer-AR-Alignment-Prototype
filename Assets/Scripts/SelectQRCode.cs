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

    private Transform selectedCubeTransform;
    private Vector3 m_QRCubePos;
    private Quaternion m_QRCubeRot;

    private ModelTransformManager mm;

    private Transform ModelTransform;


    [SerializeField]
    private GameObject TransformTools;
    private Transform ModelTransformChild;
    private GameObject go;
    [SerializeField]
    private LayerMask mask;
    private bool isSelected = false;

    public override void OnStartLocalPlayer()
    {
        if (cam == null)
        {
            Debug.LogError("SelectQRCode: No Camera referenced!");
            this.enabled = false;
        }
        m_transform = gameObject.GetComponent<Transform>();
        ModelTransform = GameObject.Find("ModelToAlign").GetComponent<Transform>();

    }

    void Update()
    {
        mm.selectQR = this;
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
            GetSelectedCubeTransformInfo(selectedCubeTransform);
            SetObjectAlignwithTransformPoint(selectedCubeTransform);
            ChangeModelToAlignTransform(m_QRCubePos, m_QRCubeRot);
            isSelected = false;
        }
    }
    private void Awake()
    {
        mm = FindObjectOfType<ModelTransformManager>();
    }

    void Selecting()
    {
        mask = LayerMask.GetMask("QRCode");
        if (Physics.Raycast(ray, out hit, mask) && hit.collider.transform.name == "Panel")
        {
            Debug.DrawLine(m_transform.position, hit.point, Color.red);
            //put hitted object in the selectedCueTransform
            selectedCubeTransform = hit.collider.transform.parent;
            isSelected = true;
            CmdSendPosAndtrueToServer();
        }
    }

    [Command]
    void CmdSendPosAndtrueToServer()
    {
        isSelected = true;
        selectedCubeTransform = hit.collider.transform.parent;
        Debug.Log("Send Pos to server");
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

        mm.current_Pos = ModelTransform.position;
        mm.current_Rot = ModelTransform.rotation;
    }
}
