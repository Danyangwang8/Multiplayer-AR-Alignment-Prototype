using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadData : MonoBehaviour
{
    void Start()
    {
        TextAsset metaData = Resources.Load<TextAsset>("Metadata");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
