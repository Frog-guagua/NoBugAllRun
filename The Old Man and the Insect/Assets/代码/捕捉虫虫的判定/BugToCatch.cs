using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BugToCatch : InsectData
{

    public InsectDataSO data;
    public SpriteRenderer spriteRenderer;
    
    
    
    // Start is called before the first frame update
    void Start()
    {   GetSoData(data);
        spriteRenderer=this.GetComponent<SpriteRenderer>();
        if (data.insectImage != null)
        {
            spriteRenderer.sprite = data.insectImage;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {   
        CatchingManager.Instance.startCatch(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
