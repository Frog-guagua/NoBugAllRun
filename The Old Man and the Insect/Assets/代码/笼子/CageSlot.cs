using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CageSlot : MonoBehaviour
{
    public int slotID;
    private CageUI cageUI;
    public Button btn;
    public InsectData Data=new InsectData();
    private Image img;
   
    // Start is called before the first frame update
    void Start()
    {   
        img = GetComponent<Image>();
        cageUI = GetComponentInParent<CageUI>();
        btn = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Data.insectId != 0)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 255f);
            
        }
        else
        {
            
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
        }
        
    }
    public void onclick()
    {
        if (Data.insectId != 0)
        {


            CageManager.Instance.currentChosenData = CageManager.Instance.insectInCage[slotID];
            cageUI.slotOnClick(); //在专门管理ui的代码里处理相关逻辑
            if (Data.insectId != 0)
            {
                AudioMgr.Instance.PlaySFX(Id_To_Insect_Dic.IdToInsectDic[Data.insectId].insectSound);
            }
        }

    }

    public void refreshSlot()
    {   
        Data=CageManager.Instance.insectInCage[slotID];
        //这里更新ui等
        if(Data.insectImage!=null)
        {
            btn.image.sprite=Data.insectImage;
        }
    }
}
