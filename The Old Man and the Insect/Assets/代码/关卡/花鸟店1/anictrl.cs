using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anictrl : MonoBehaviour
{   public Animator anim;
    public static bool canget=false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < -3)
        {
            canget = true;
            anim.SetBool("movein", true);
            
        }
        else
        {
            canget = false;
            anim.SetBool("movein", false);
        }
    }

   
}
