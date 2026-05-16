using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event1Mgr : MonoBehaviour
{
    public DialogueData diadata;
    public DialogueData diadata_lastsentence;
    public GameObject btnFather;
    public GameObject hintobj;
    private Hint hint;
    public GameObject leave;
    public string desciption1="1";
    public string desciption2="2";
    public string desciption3="3";
    private bool canstart = true;
    private bool canChange=true;

    public DialogueData dataA;
    public DialogueData dataB;
    public DialogueData dataC;
    // Start is called before the first frame update
    private void Awake()
    {
        btnFather.SetActive(false);
    }

    void Start()
    {  
        DialogueManager.Instance.switchUI_tem();
        hint = hintobj.GetComponent<Hint>();
        diadata_lastsentence.dialogueList[0].canContinue=false;
        DataBroker.catchTime++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player"&&canstart)
        {
            StartCoroutine(Event1());
            canstart = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canChange)
        {
          PlayerMove.canMove=true;
        }
        else
        {
            PlayerMove.canMove=false;
        }
    }

    public IEnumerator Event1()
    {   
        canChange=false;
        yield return new WaitForSeconds(0.5f);
        DialogueManager.Instance.StartDialogue(diadata,StartChoose);
    }

    public void StartChoose()
    {   
        DialogueManager.Instance.StartDialogue(diadata_lastsentence);
        btnFather.SetActive(true);
        
    }
    public void button1()
    {
        DialogueManager.Instance.EndDialogue(null);
        DataBroker.actionValue += 1;
        DataBroker.experience += 2;
        Debug.Log(DataBroker.experience);
        Debug.Log(DataBroker.actionValue);
        DialogueManager.Instance.canContinue=true;
        btnFather.SetActive(false);
        hint.ShowHint(desciption1);
        
        diadata_lastsentence.dialogueList[0].canContinue=true;
        leave.SetActive(true);
       
        DialogueManager.Instance.StartDialogue(dataA,switchui);
        PlayerMove.canMove=false;
      //  DialogueManager.Instance.switchUI_1();
    }

    public void button2()
    {
        DialogueManager.Instance.EndDialogue(null);
        DataBroker.actionValue += 1;
        DataBroker.experience += 4;
        Debug.Log(DataBroker.experience);
        Debug.Log(DataBroker.actionValue);
        DialogueManager.Instance.canContinue=true;
        btnFather.SetActive(false);
        hint.ShowHint(desciption2);
      
        diadata_lastsentence.dialogueList[0].canContinue=true;
        leave.SetActive(true);
        
        DialogueManager.Instance.StartDialogue(dataB,switchui);
        PlayerMove.canMove=false;
      //  DialogueManager.Instance.switchUI_1();
        
    }

    public void button3()
    {
        DialogueManager.Instance.EndDialogue(null);
        DataBroker.actionValue += 1;
        DataBroker.reputation += 6;
        Debug.Log(DataBroker.experience);
        Debug.Log(DataBroker.reputation);
        DialogueManager.Instance.canContinue=true;
        btnFather.SetActive(false);
        hint.ShowHint(desciption3);
       
        diadata_lastsentence.dialogueList[0].canContinue=true;
        leave.SetActive(true);
        
        DialogueManager.Instance.StartDialogue(dataC,switchui);
        PlayerMove.canMove=false;
     //   DialogueManager.Instance.switchUI_1();
    }

    void switchui()
    {   
        canChange=true;
        DialogueManager.Instance.switchUI_1();
    }
}
