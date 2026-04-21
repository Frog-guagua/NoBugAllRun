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
    
    public string desciption1="1";
    public string desciption2="2";
    public string desciption3="3";

    private bool canChange=false;
    // Start is called before the first frame update
    private void Awake()
    {
        btnFather.SetActive(false);
    }

    void Start()
    {   
        hint = hintobj.GetComponent<Hint>();
        StartCoroutine(Event1());
    }

    // Update is called once per frame
    void Update()
    {
        if (canChange && Input.GetMouseButton(0))
        {
            Debug.Log("切换下一关");
            canChange=false;
        }
    }

    public IEnumerator Event1()
    {
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
        DataBroker.actionValue += 1;
        DataBroker.experience += 2;
        Debug.Log(DataBroker.experience);
        Debug.Log(DataBroker.actionValue);
        DialogueManager.Instance.canContinue=true;
        btnFather.SetActive(false);
        hint.ShowHint(desciption1);
        canChange=true;
        
    }

    public void button2()
    {
        DataBroker.actionValue += 1;
        DataBroker.experience += 4;
        Debug.Log(DataBroker.experience);
        Debug.Log(DataBroker.actionValue);
        DialogueManager.Instance.canContinue=true;
        btnFather.SetActive(false);
        hint.ShowHint(desciption2);
        canChange=true;
    }

    public void button3()
    {
        DataBroker.actionValue += 1;
        DataBroker.reputation += 6;
        Debug.Log(DataBroker.experience);
        Debug.Log(DataBroker.reputation);
        DialogueManager.Instance.canContinue=true;
        btnFather.SetActive(false);
        hint.ShowHint(desciption3);
        canChange=true;
    }
}
