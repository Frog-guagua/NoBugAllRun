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
    public GameObject obj;
    public Vector2 leftAndDown_DoorRange;
    public Vector2 rightAndUp_DoorRange;
    private bool isswitch=false;
    public DialogueData Data;
    public Hint hint;
   private static int getInHutong1Count = 1;
   public Animator anim;
    // Start is called before the first frame update
    void Start()
    {   
        Debug.Log(DataBroker.Instance.datasFromCage.Count);
        Bag.canOpenBag = true;
        if (getInHutong1Count == 1)
        {
            StartCoroutine(flow());
        }
        if (getInHutong1Count > 1)
        {
            Debug.Log("对话与动画，记得加引导");
            Debug.Log(DataBroker.Instance.datasFromFight.Count);
            DialogueManager.Instance.StartDialogue(Data,startflow2);
            player.transform.position = new Vector2(1.13f,-0.04f);
            
        }
    }

    IEnumerator flow()
    {
        yield return new WaitForSeconds(0.7f);
        hint.ShowHint("靠近对话（点击）");
    }

    public void startflow2()
    {
        StartCoroutine(flow2());
    }
    IEnumerator flow2()
    {
        anim.SetBool("canLeave",true);
        PlayerMove.canMove = false;
        yield return new WaitForSeconds(2f);
        PlayerMove.canMove = true;
       hint.ShowHint("前往上方胡同");
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
        obj.SetActive(true);
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
