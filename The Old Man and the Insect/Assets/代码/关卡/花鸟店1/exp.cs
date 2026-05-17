using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class exp : MonoBehaviour
{   
    public TextMeshProUGUI expText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        expText.text=DataBroker.experience.ToString();
    }
}
