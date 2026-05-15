using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class Before2Mgr: MonoBehaviour
{   
    public float distance=3;
    public Transform player;
    public DialogueData dialogue;
    private static bool canContinue=true;
    public Button btn;
    public static bool canleave=false;
    public GameObject obj;
   
    

   private static int getInCount = 1;
    // Start is called before the first frame update
    void Start()
    {   
        Debug.Log(DataBroker.Instance.datasFromCage.Count);
        Bag.canOpenBag = true;
        if (getInCount > 1)
        {   
            Debug.Log("对话与动画");
            if (DataBroker.WinGame2)
            {
               // DialogueManager.Instance.StartDialogue(win);
            }
            else
            {
               // DialogueManager.Instance.StartDialogue(lose);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

  

    void OnMouseDown()
    {
        if (Vector2.Distance(player.position, transform.position) <= 
            distance&&canContinue&&canleave==false)
        {
            DialogueManager.Instance.StartDialogue(dialogue,setbtn); 
            canContinue = false;
        }
    }

    public void setbtn()
    {
        btn.gameObject.SetActive(true);
        obj.SetActive(true);
    }

    public void btnonclick()
    {
        FightFlowManager.OnGame2 = true;
        FightFlowManager.OnGame3 = false;
        btn.gameObject.SetActive(false);
        Bag.canOpenBag = false;
        canleave = true;
        AudioMgr.Instance.StopBGM();
        Transition.Instance.SwitchSceneWithFade("FightScene2");
        getInCount++;
        
      
    }
    
}
