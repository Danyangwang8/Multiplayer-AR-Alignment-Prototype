using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SelectQRCode : NetworkBehaviour
{
    enum QRCubeColor
    {
        Red,
        Black,
        Blue,
        Green
    }


    private Ray ray;
    private RaycastHit hit;
    [SerializeField]
    private Camera cam;
    private Transform m_transform;
    private Transform selectedCubeTransform;
    private Vector3 m_QRCubePos;
    private Quaternion m_QRCubeRot;

    private Transform ModelTransform;
    [SerializeField]
    private GameObject TransformTools;
    private Transform ModelTransformChild;
    private GameObject go;
    [SerializeField]
    private LayerMask mask;

    void Start()
    {
        if(cam == null)
        {
            Debug.LogError("SelectQRCode: No Camera referenced!");
            this.enabled = false;
        }
        m_transform = gameObject.GetComponent<Transform>();
        ModelTransform = GameObject.Find("ModelToAlign").GetComponent<Transform>();

    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Input.GetMouseButtonDown(0))
        {
            Selecting();
        }

    }

    void Selecting()
    {
        mask = LayerMask.GetMask("QRCode");
        if (Physics.Raycast(ray, out hit, mask) && hit.collider.transform.name == "Panel")
        {
            Debug.DrawLine(m_transform.position, hit.point, Color.red);
            CmdGetSelectedCubeTransformInfo();
        }
    }

    void CmdGetSelectedCubeTransformInfo()
    {
        selectedCubeTransform = hit.collider.transform.parent;
        m_QRCubePos = selectedCubeTransform.position;
        m_QRCubeRot = selectedCubeTransform.rotation;
        //Debug.Log($"pos: {m_QRCubePos}, Rot: {m_QRCubeRot}");
        SetObjectAlignwithTransformPoint(selectedCubeTransform);
        ChangeModelToAlignTransform(m_QRCubePos, m_QRCubeRot);

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
        go.transform.SetParent(ModelTransform);
        
    }

    //Pass the ShowCaseRoom triggered QRCode object position and rotation to the empty object.
    //And rotating ModelToAlign object base on the empty object
    void ChangeModelToAlignTransform(Vector3 m_QRCubePos, Quaternion m_QRCubeRot)
    {
        go.transform.SetParent(null);
        ModelTransform.SetParent(go.transform);

        go.transform.position = m_QRCubePos;
        go.transform.rotation = m_QRCubeRot;

        ModelTransform.SetParent(null);
        Destroy(go);
    }
}
