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
    private bool canContinue=true;
    public Button btn;
    public static bool canleave=false;

    public Vector2 leftAndDown_DoorRange;
    public Vector2 rightAndUp_DoorRange;
    // Start is called before the first frame update
    void Start()
    {
        Bag.canOpenBag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > leftAndDown_DoorRange.x
            && player.transform.position.x < rightAndUp_DoorRange.x
            && player.transform.position.y > leftAndDown_DoorRange.y
            && player.transform.position.y < rightAndUp_DoorRange.y
            && canleave)
        {
            Transition.Instance.SwitchSceneWithFade("Choose");
        }
    }

  

    void OnMouseDown()
    {
        if (Vector2.Distance(player.position, transform.position) <= distance&&canContinue&&canleave==false)
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
        Transition.Instance.SwitchSceneWithFade("Fight1Scene");
        
        Bag.canOpenBag = false;
    }
    
}
