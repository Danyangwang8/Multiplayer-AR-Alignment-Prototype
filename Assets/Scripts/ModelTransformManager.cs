using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelTransformManager : MonoBehaviour
{
    private Transform ModelTransform;
    private Transform selectedCubeTransform;
    private Vector3 m_QRCubePos;
    private Quaternion m_QRCubeRot;
    [SerializeField]
    private GameObject TransformTools;
    private Transform ModelTransformChild;
    private GameObject go;

    private SelectQRCode selectQR;

    private void Start()
    {
        ModelTransform = GameObject.Find("ModelToAlign").GetComponent<Transform>();
        selectQR = FindObjectOfType<SelectQRCode>();
    }

    private void Update()
    {
        if(selectQR.isSelected)
        {
            //selectedCubeTransform = selectQR.selectedCube;
            GetSelectedCubeTransformInfo(selectedCubeTransform);
            SetObjectAlignwithTransformPoint(selectedCubeTransform);
            ChangeModelToAlignTransform(m_QRCubePos, m_QRCubeRot);
        }
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
