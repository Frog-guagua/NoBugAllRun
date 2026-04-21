using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leavearea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (FlowerManager.Instance.canleave)
        {   
            FlowerManager.Instance.canstartflow=true;
            FlowerManager.Instance.SecondIn = true;
            FlowerManager.Instance.canleave = false;
            //Transition.Instance.SwitchSceneWithFade();
        }
    }
}
