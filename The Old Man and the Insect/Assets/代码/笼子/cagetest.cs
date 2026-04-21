using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cagetest : MonoBehaviour
{   
    public InsectDataSO insectData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick()
    {   
        CageUI.Instance.setAct();
        CageManager.Instance.AddInsect(insectData);
        print("test");
        DataBroker.experience++;
        
    }
}
