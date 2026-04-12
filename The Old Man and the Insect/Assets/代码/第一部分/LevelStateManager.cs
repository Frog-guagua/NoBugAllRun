using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;


//ok啊也是偷懒直接写一个伪状态机
public enum LevelState
{
    OnEnterGame,
    KnockingDoor,
    openDoorAnm,
    dialogue,
    havingCage,
    leavinghouse
}

public class LevelStateManager : MonoBehaviour
{
    #region 单例

    

   
    private LevelStateManager() {}

   
    private static LevelStateManager _instance;

    
    public static LevelStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelStateManager>();
                if (_instance == null)
                {
                  
                    var singletonObject = new GameObject("LevelStateManagerSingleton");
                    _instance = singletonObject.AddComponent<LevelStateManager>();
                }
            }
            return _instance;
        }
    }
    #endregion
    [Header("敲门后延迟播放对话时间")]
    public float Delay_Before_Knocking = 2f;
    [Header("开始播放开门动画到开启对话的延迟时间")]
    public float Delay_Before_dia = 2f;
    private LevelState currentState;
    private LevelState lastState;

        
    public AudioClip bgm;
       
    public GameObject player;
    
    [Header("音效")]
    public AudioClip KnockingSound;
    public AudioClip birdsound;
    
    [Header("敲门震动间隔")]
    public float shakeDelay=1f;
    
    [Header("对话")]
    public DialogueData dia1;
    public DialogueData dia2;
    
    [Header("门检测区域范围")]
    public Vector2 leftAndDown_DoorRange;
    public Vector2 rightAndUp_DoorRange;
    
    [Header("任务提示")] public TaskDataSO task1;
    public GameObject door;
    private ObjectShake doorshake;
    private bool afterKnock = false;
    
    void Start()
    {
        doorshake = door.GetComponent<ObjectShake>();
        
        if (_instance != this && _instance != null)
        {   
            
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
         
        }
        AudioMgr.Instance.PlayBGM(bgm);
      
        currentState = LevelState.OnEnterGame;
        lastState = LevelState.OnEnterGame;
       //待实现：播放音效，等写了音效管理系统
       AudioMgr.Instance.PlaySFX(birdsound);
       
       StartCoroutine(DelayToSwitchState(LevelState.KnockingDoor, 4f));
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != lastState)//进入新状态执行一次
        {
            switch (currentState)
            {   
               case LevelState.KnockingDoor:
                   StartCoroutine(KnockingDoorState());
                   StartCoroutine(KnockingShake());
                   break;
               case LevelState.dialogue:
                   DialogueManager.Instance.StartDialogue(dia2,diaEnd);
                   break;
               case LevelState.openDoorAnm:
                   StartCoroutine(openAnim());
                   break;
               case LevelState.havingCage://此处逻辑待定，不知道美术资源是直接给个提笼子的老爷爷还是说给个笼子
                   TaskWindow.Instance.Show(task1);
                   table.tableCanInteract = true;
                   //桌子逻辑，点击桌子放笼子
                   break;
               case LevelState.leavinghouse:
                   TaskWindow.Instance.CompleteTask();
                   //离开房子
                   break;
               
            }
            lastState = currentState;
        }
        
        
        switch (currentState)
        {
            case LevelState.KnockingDoor://目前设定为玩家移动到门区域然后就开门,使用一个最愚蠢的坐标判定
                if (player.transform.position.x > leftAndDown_DoorRange.x
                    && player.transform.position.x < rightAndUp_DoorRange.x
                    && player.transform.position.y > leftAndDown_DoorRange.y &&
                    player.transform.position.y < rightAndUp_DoorRange.y
                    &&afterKnock)

                {
                  
                    PlayerMove.canMove = false;
                    SwitchState(LevelState.openDoorAnm);
                }
               

             
                break;
        }
    }

    // 每个状态的具体逻辑处理
   
    // 切换状态
    public void SwitchState(LevelState newState)
    {
        currentState = newState;
    }
    
    IEnumerator DelayToSwitchState(LevelState newState,float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SwitchState(newState);
    }

    IEnumerator KnockingDoorState()
    {   
       
        AudioMgr.Instance.PlaySFX(KnockingSound);
       
        yield return new WaitForSeconds(Delay_Before_Knocking);
        
        DialogueManager.Instance.StartDialogue(dia1);
        yield return new WaitForSeconds(1f);
        afterKnock = true;
    }

    IEnumerator KnockingShake()
    { doorshake.DoorShake();
        Debug.Log("调用shake");
        yield return new WaitForSeconds(shakeDelay);
        doorshake.DoorShake();
        
        
        
    }
    IEnumerator openAnim()
    {
        print("放开门动画");
        yield return new WaitForSeconds(Delay_Before_dia);
        SwitchState(LevelState.dialogue);
    }
    void diaEnd()
    {   
        print("获得笼子");
        //获得笼子
        SwitchState(LevelState.havingCage);
    }
}
