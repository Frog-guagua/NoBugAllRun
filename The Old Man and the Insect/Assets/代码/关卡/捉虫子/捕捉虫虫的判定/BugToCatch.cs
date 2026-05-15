using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BugToCatch : InsectData
{
    public bool cancatch = true;
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
            Debug.LogError($"{nameof(BugToCatch)}({Name}) 的 {nameof(data)} 为空，无法调用 {nameof(GetSoData)}。请检查预制体/列表引用是否真的绑定了 InsectDataSO。", this);
            return;
        }

        GetSoData(data);

        if (spriteRenderer != null && data.insectImage != null)
        {
            spriteRenderer.sprite = data.insectImage;
        }
    }

    void Update()
    {
       
    }

    public void OnMouseDown()
    {
        if (CatchingManager.canCatch && cancatch)
        {
            // === 新增：点击时置顶 ===
            spriteRenderer.sortingOrder = 200;
            // =====================
            
            CatchingManager.Instance.callNocatch();
            StartCoroutine(catchflow());
        }
    }

    void canNotcatch()
    {
        cancatch = false;
    }

    void canCatch()
    {
        cancatch = true;
    }

    private void OnEnable()
    {
        CatchingManager.cancatch += canCatch;
        CatchingManager.nocatch += canNotcatch;
    }

    void OnDisable()
    {
        CatchingManager.cancatch -= canCatch;
        CatchingManager.nocatch -= canNotcatch;
    }

    IEnumerator catchflow()
    {
        yield return new WaitForEndOfFrame();
        this.transform.SetParent(null, true);
        grassController.Instance.MovetoLeft();
        yield return new WaitForSeconds(2f);
       
        Vector3 targetPos = new Vector3(Random.Range(-6f, 6f), 0f, transform.position.z);
        float moveSpeed = 8f;

        // === 修复：用独立标志记录是否到达，避免和 cancatch 字段混淆 ===
        bool reachedTarget = false;

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        reachedTarget = true;
        transform.position = targetPos;
        // ============================================================

        // === 修复：判断到达标志，而不是 cancatch 字段 ===
        // 因为点击时 callNocatch() 已经把 cancatch 设成 false 了，这里用它判断永远进不去
        if (reachedTarget)
        {   yield return new WaitForSeconds(0.6f);
            CatchingManager.Instance.startCatch(this.gameObject);
            
        }
        // =============================================
    }
}