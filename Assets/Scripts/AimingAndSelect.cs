using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AimingAndSelect : NetworkBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private Transform m_transform;

    public override void OnStartLocalPlayer()
    {
        m_transform = gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(m_transform.position, hit.point, Color.red);
        }
    }
}
