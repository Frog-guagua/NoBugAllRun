using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class Before2Mgr: MonoBehaviour
{   
    public float distance=4;
    public Transform player;
    public DialogueData dialogue;
    private static bool canContinue=true;
    public Button btn;
    public static bool canleave=false;
    public GameObject obj;
   public Animator anim;
   private bool canquit=false;
    public AudioClip clip;
   private static int getInCount = 1;
    // Start is called before the first frame update
    void Start()
    {   
        AudioMgr.Instance.PlayBGM(clip);
        Debug.Log(DataBroker.Instance.datasFromCage.Count);
        Bag.canOpenBag = true;
        if (getInCount == 1)
        {
            CageManager.Instance.showhi("靠近对话");
        }
        if (getInCount > 1)
        {   
            Debug.Log("对话与动画");
            if (DataBroker.WinGame2)
            {   
                startwin();
               // DialogueManager.Instance.StartDialogue(win);
            }
            else
            {
                if (DataBroker.reputation<0)
                {
                    CageManager.Instance.showhi("在李四爷一行人的嘲笑中，你不禁开始怀疑起了自己....");
                    canquit = true;
                }
                 // DialogueManager.Instance.StartDialogue(lose);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canquit == true&&Input.GetMouseButton(0))
        {   
            Debug.Log("退出游戏");
            CageManager.Instance.exitGame();
            canquit = false;
        }
    }
    
    public void startwin()
    {
        StartCoroutine(winflow());
    }
    IEnumerator winflow()
    {   
        anim.SetBool("canLeave",true);
        player.GetComponent<Rigidbody2D>().position = new Vector2(1.2f,-2f);
        
        PlayerMove.canMove = false;
        yield return new WaitForSeconds(4f);
        PlayerMove.canMove = true;
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
