using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;//单例，挂在永久的空场景上，随时调用

    [Header("Dialogue Data")]
   public DialogueData currentDialogueData;//对话数据的SO文件

    [Header("UI")]//对话UI面板
    public TMP_Text speakerNameText;
    public Image speakerSpriteImg;
    public TMP_Text contentText;
    public GameObject dialogueUI;

    [Header("Typewriter")]//打字机效果
    public float typeWriterSpeed = 0.05f;

    private Coroutine typeWriterCoroutine;//打字机协程
    private bool isSentenceFinish;
    private bool isDialoguePlaying;
    private int currentSentenceIndex;//这个变量记录当前说到第几句话了
    private Action currentact;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (dialogueUI != null) dialogueUI.SetActive(false);

        typeWriterCoroutine = null;
        isSentenceFinish = false;
        isDialoguePlaying = false;
        currentSentenceIndex = 0;
    }

    public void StartDialogue(DialogueData targetDialogueData,Action endact=null)
        //可以在任何地方启动对话
        //传入目的对话的SO文件即可
    //endact是结束对话时触发的方法，没有就传个空的
    {
        if (isDialoguePlaying) return;//确保只有一段对话在执行
        currentDialogueData = targetDialogueData;
        isDialoguePlaying = true;//进入对话状态
        currentSentenceIndex = 0;

        PlayerMove.canMove = false;
        
        dialogueUI.SetActive(true);
        currentact = endact;
        PlayCurrentSentence();
    }

    private void PlayCurrentSentence()
    {
        if (currentSentenceIndex >= currentDialogueData.dialogueList.Count)
        {
            EndDialogue(currentact);
            return;//避免超出范围，及时结束对话
        }

        isSentenceFinish = false;

        if (typeWriterCoroutine != null) StopCoroutine(typeWriterCoroutine);
        contentText.text = "";

        var sentence = currentDialogueData.dialogueList[currentSentenceIndex];
        //取出记录在list里的结构体数据
       

        speakerNameText.text = sentence.speakerName;
        speakerSpriteImg.sprite = sentence.speakerSprite;

        if (sentence.Shake)
        {
            CamaraShake.ShakeCameraInDialogue(0.5f,0.5f);
        }
        typeWriterCoroutine = StartCoroutine(TypeWriterCoroutine(sentence.content));
    }

    private IEnumerator TypeWriterCoroutine(string text)//打字机效果
    {
        for (int i = 0; i < text.Length; i++)
        {
            contentText.text += text[i];
            yield return new WaitForSeconds(typeWriterSpeed);
        }

        isSentenceFinish = true;
    }

    private void Update()
    {
        if (isDialoguePlaying && isSentenceFinish && Input.GetMouseButtonDown(0))
        {
            NextSentence();//鼠标点击切换下一句
        }
    }

    private void NextSentence()
    {
        currentSentenceIndex++;
        PlayCurrentSentence();
    }

    private void EndDialogue(Action endAct)//关闭面板,执行结束对话操作
    {
        isDialoguePlaying = false;
        isSentenceFinish = false;
        currentSentenceIndex = 0;

        dialogueUI.SetActive(false);
        
        PlayerMove.canMove = true;
            
        if (typeWriterCoroutine != null) StopCoroutine(typeWriterCoroutine);
        typeWriterCoroutine = null;
        endAct?.Invoke();
        currentact = null;
    }
    
}