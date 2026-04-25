using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BugToCatch : InsectData
{

    public InsectDataSO data;
    public SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void Init(InsectDataSO soData = null)
    {
        if (soData != null)
        {
            data = soData;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (data == null)
        {
            Debug.LogError($"{nameof(BugToCatch)}({name}) 的 {nameof(data)} 为空，无法调用 {nameof(GetSoData)}。请检查预制体/列表引用是否真的绑定了 InsectDataSO。", this);
            return;
        }

        GetSoData(data);

        if (spriteRenderer != null && data.insectImage != null)
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
        if (CatchingManager.canCatch)
        {


            CatchingManager.Instance.startCatch(this.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
