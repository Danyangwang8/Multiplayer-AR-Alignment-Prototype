using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DataScript : NetworkBehaviour
{
    public Text canvasdataText;
    public SelectQRCode selectQR;

    [SyncVar(hook = nameof(OnDataTextChanged))]
    public string dataText;

    void OnDataTextChanged(string oldStr, string newStr)
    {
        canvasdataText.text = dataText;
    }
}
