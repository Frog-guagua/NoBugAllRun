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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (Vector2.Distance(player.position, transform.position) <= distance&&canContinue)
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
        Transition.Instance.SwitchSceneWithFade("Fight1Scene");
    }
    
}
