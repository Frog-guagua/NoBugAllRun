using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class part3Manager : MonoBehaviour
{   
    public float distance=3;
    public Transform player;
    public DialogueData dialogue;
    private static bool canContinue=true;
    public Button btn;
    public static bool canleave=false;

    public Vector2 leftAndDown_DoorRange;
    public Vector2 rightAndUp_DoorRange;
    private bool isswitch=false;

   private static int getInHutong1Count = 1;
    // Start is called before the first frame update
    void Start()
    {   
        Debug.Log(DataBroker.Instance.datasFromCage.Count);
        Bag.canOpenBag = true;
        if (getInHutong1Count > 1)
        {
            Debug.Log("对话与动画");
            Debug.Log(DataBroker.Instance.datasFromFight.Count);
            for (int i = 0; i < DataBroker.Instance.datasFromFight.Count; i++)
            {
                Debug.Log("当前种类"+DataBroker.Instance.datasFromFight[i].bugType);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > leftAndDown_DoorRange.x
            && player.transform.position.x < rightAndUp_DoorRange.x
            && player.transform.position.y > leftAndDown_DoorRange.y
            && player.transform.position.y < rightAndUp_DoorRange.y
            &&isswitch==false&&canleave==true)
        {
            Transition.Instance.SwitchSceneWithFade("Choose");
           
            isswitch = true;
        }
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
    }

    public void btnonclick()
    {
        FightFlowManager.OnGame1 = true;
        btn.gameObject.SetActive(false);
        Bag.canOpenBag = false;
        canleave = true;
        AudioMgr.Instance.StopBGM();
        Transition.Instance.SwitchSceneWithFade("Fight1Scene");
        getInHutong1Count++;
        
      
    }
    
}
