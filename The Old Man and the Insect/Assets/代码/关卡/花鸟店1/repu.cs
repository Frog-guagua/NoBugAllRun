using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class repu : MonoBehaviour
{
    public TextMeshProUGUI retext;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        retext.text = DataBroker.reputation.ToString();
    }
}
