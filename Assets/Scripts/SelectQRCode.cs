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
    public ModelTransformManager mm;

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

    public override void OnStartLocalPlayer()
    {
        mm.selectQR = this;
        if (cam == null)
        {
            Debug.LogError("SelectQRCode: No Camera referenced!");
            this.enabled = false;
        }
        m_transform = gameObject.GetComponent<Transform>();
        ModelTransform = GameObject.Find("ModelToAlign").GetComponent<Transform>();
    }

    private void Awake()
    {
        mm = FindObjectOfType<ModelTransformManager>();
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
            if(ModelTransform != null)
            {
                CmdChangeModelTransform(ModelTransform.position, ModelTransform.rotation);
            }
        }
    }


    void Selecting()
    {
        mask = LayerMask.GetMask("QRCode");
        if (Physics.Raycast(ray, out hit, mask) && hit.collider.transform.name == "Panel")
        {
            Debug.DrawLine(m_transform.position, hit.point, Color.red);
            GetSelectedCubeTransformInfo(hit.collider.transform.parent);
            SetObjectAlignwithTransformPoint(selectedCubeTransform);
            ChangeModelToAlignTransform(m_QRCubePos, m_QRCubeRot);
        }
    }

    void GetSelectedCubeTransformInfo(Transform parentTransform)
    {
        selectedCubeTransform = parentTransform;
        m_QRCubePos = selectedCubeTransform.position;
        m_QRCubeRot = selectedCubeTransform.rotation;
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

        ModelTransform.SetParent(null);
        Destroy(go);

        mm.current_Pos = ModelTransform.position;
        mm.current_Rot = ModelTransform.rotation;
    }

    [ClientRpc]
    public void CmdChangeModelTransform(Vector3 pos, Quaternion rot)
    {
        ModelTransform.position = pos;
        ModelTransform.rotation = rot;
    }
}
