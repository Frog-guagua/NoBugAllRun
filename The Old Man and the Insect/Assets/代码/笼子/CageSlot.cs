using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageSlot : MonoBehaviour
{
    public int slotID;
    private CageUI cageUI;

    public InsectData Data;
    // Start is called before the first frame update
    void Start()
    {
        cageUI = GetComponentInParent<CageUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onclick()
    {
        CageManager.Instance.currentChosenData=CageManager.Instance.insectInCage[slotID];
        cageUI.slotOnClick();//在专门管理ui的代码里处理相关逻辑
       
    }

    public void refreshSlot()
    {   
        Data=CageManager.Instance.insectInCage[slotID];
        //这里更新ui等
    }
}
