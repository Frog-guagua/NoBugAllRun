using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum LevelState
{
    OnEnterGame,
    KnockingDoor,
    openDoorAnm,
    dialogue,
    havingCage,
    guide1,
    guide2,
    guide3,
    leavinghouse,
    switchscene,
    secondin
}

public class LevelStateManager : MonoBehaviour
{
    #region 单例
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

    public static LevelState currentState;
    private static LevelState lastState;

    public AudioClip bgm;
    public GameObject player;

    [Header("音效")]
    public AudioClip KnockingSound;
    public AudioClip birdsound;

    [Header("敲门震动间隔")]
    public float shakeDelay = 1f;

    [Header("对话")]
    public DialogueData dia1;
    public DialogueData dia2;

    [Header("门检测区域范围")]
    public Vector2 leftAndDown_DoorRange;
    public Vector2 rightAndUp_DoorRange;

    [Header("任务提示")]
    public TaskDataSO task1;
    public GameObject door;
    private ObjectShake doorshake;
    private bool afterKnock = false;

    private bool canClickGuide = false;
    private bool isSwitching = false; // 防止重复触发
    private bool hasClicked = false;  // 确保只点一次

    private Hint hint;

    [Header("弹窗1")]
    public GameObject hintobj;
    public string GetCageStr = "getcage";

    [Header("弹窗2")]
    public GameObject hintobj2;
    public string guide1;
    public string guide2;
    public string guide3;
    
    
    private Hint hint2;
    public GameObject liu;
    public static bool canquit = false;
    [Header("二次进入")] 
    public DialogueData beforeopendoor2;

    public GameObject wang;
    public DialogueData afteropoen2;
    public GameObject liudaye;
    public static bool secondin = false;
    void Start()
    {   
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        door.transform.rotation = Quaternion.Euler(0, 0, 0);
        hint = hintobj.GetComponent<Hint>();
        doorshake = door.GetComponent<ObjectShake>();
        hint2 = hintobj2.GetComponent<Hint>();
        
       
        AudioMgr.Instance.PlayBGM(bgm);
        AudioMgr.Instance.PlaySFX(birdsound);
        if (secondin == false)
        {

            currentState = LevelState.OnEnterGame;
            lastState = LevelState.OnEnterGame;

            StartCoroutine(DelayToSwitchState(LevelState.KnockingDoor, 2.5f));
        }

        if (secondin)
        {   currentState = LevelState.secondin;
            lastState = LevelState.secondin;
            liudaye.SetActive(true);
            secondin=true;
            afterKnock = false;
            liu.SetActive(false);
            wang.SetActive(true);
            
        }
    }

    void Update()
    {
        if (currentState != lastState)
        {
            switch (currentState)
            {
                case LevelState.KnockingDoor:
                    StartCoroutine(KnockingDoorState());
                    StartCoroutine(KnockingShake());
                    break;

                case LevelState.dialogue:
                    if (secondin == false)
                    {
                        DialogueManager.Instance.StartDialogue(dia2, diaEnd);
                    }
                    else
                    {
                        DialogueManager.Instance.StartDialogue(afteropoen2);
                    }

                    break;

                case LevelState.openDoorAnm:
                    StartCoroutine(openAnim());
                    break;

                case LevelState.havingCage:
                    hint.ShowHint(GetCageStr);
                    table.tableact = true;
                    break;

                case LevelState.guide1:
                    hint2.ShowHint(guide1);
                    Debug.Log("进入 guide1");
                    canClickGuide = false;
                    isSwitching = false;
                    hasClicked = false; // 重置点击标记
                    StartCoroutine(EnableGuideClickAfterDelay(0.5f));
                    break;

                case LevelState.guide2:
                    hint2.ShowHint(guide2);
                    Debug.Log("进入 guide2");
                    canquit = true;
                    break;
                case LevelState.guide3:
                    hint2.ShowHint(guide3);
                    break;

                case LevelState.leavinghouse:
                    break;
                case LevelState.switchscene :
                    secondin = true;
                    Transition.Instance.SwitchSceneWithFade("HuTong0");
                    
                  
                    break;
            }

            lastState = currentState;
        }

        switch (currentState)
        {
            case LevelState.KnockingDoor:
                if (player.transform.position.x > leftAndDown_DoorRange.x
                    && player.transform.position.x < rightAndUp_DoorRange.x
                    && player.transform.position.y > leftAndDown_DoorRange.y
                    && player.transform.position.y < rightAndUp_DoorRange.y
                    && afterKnock)
                {
                    PlayerMove.canMove = false;
                  
                    SwitchState(LevelState.openDoorAnm);
                }
                break;
            case LevelState.leavinghouse:
                if (player.transform.position.x > leftAndDown_DoorRange.x
                    && player.transform.position.x < rightAndUp_DoorRange.x
                    && player.transform.position.y > leftAndDown_DoorRange.y
                    && player.transform.position.y < rightAndUp_DoorRange.y
                    && afterKnock)
                {
                    Bag.canOpenBag = true;
                    door.transform.rotation = Quaternion.Euler(0, 80, 0);
                    SwitchState(LevelState.switchscene);
                    Debug.Log("leave");
                }
                break;

            case LevelState.guide1:
                // 只响应第一次点击，不会重复触发
                if (canClickGuide && !hasClicked && !isSwitching && Input.GetMouseButtonDown(0))
                {
                    hasClicked = true;
                    StartCoroutine(SwitchGuide2AfterDelay(1f));
                }
                break;
            case LevelState.guide2:
                if (Input.GetKey(KeyCode.Escape))
                {
                    SwitchState(LevelState.guide3);
                }
                break;
            case LevelState.guide3:
                if (Input.GetMouseButton(0))
                {
                    SwitchState(LevelState.leavinghouse);
                }
                break;
        }
    }

    IEnumerator EnableGuideClickAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canClickGuide = true;
    }

    // 延迟1秒切换到guide2
    IEnumerator SwitchGuide2AfterDelay(float delay)
    {
        isSwitching = true;
        yield return new WaitForSeconds(delay);
        SwitchState(LevelState.guide2);
        isSwitching = false;
    }

    public void SwitchState(LevelState newState)
    {
        currentState = newState;
    }

    IEnumerator DelayToSwitchState(LevelState newState, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SwitchState(newState);
    }

    IEnumerator KnockingDoorState()
    {
        AudioMgr.Instance.PlaySFX(KnockingSound);
        yield return new WaitForSeconds(Delay_Before_Knocking);

        if (secondin==false)
        {
            DialogueManager.Instance.StartDialogue(dia1);
        }
        else
        {
            DialogueManager.Instance.StartDialogue(beforeopendoor2);
        }
        yield return new WaitForSeconds(1f);
        afterKnock = true;
    }

    IEnumerator KnockingShake()
    {
        doorshake.DoorShake();
        yield return new WaitForSeconds(shakeDelay);
        doorshake.DoorShake();
    }

    IEnumerator openAnim()
    {
        yield return new WaitForSeconds(0.3f);
        door.transform.rotation = Quaternion.Euler(0, 80, 0);
        yield return new WaitForSeconds(Delay_Before_dia);
       
        SwitchState(LevelState.dialogue);
        
    }

    void diaEnd()
    {
        print("获得笼子");
        SwitchState(LevelState.havingCage);
        door.transform.rotation = Quaternion.Euler(0, 0, 0);
        liu.SetActive(false);
    }
}