using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event2 : MonoBehaviour
{
    // 单例实例
    private static Event2 _instance;

    public Hint hint;

    bool canChange;
    public string desciption1;
    public string desciption2;
    public string desciption3;
    public InsectDataSO DataSo;
    public Animator ani;

    public DialogueData data;
    // 获取单例实例的属性
    public static Event2 Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Event2>();
                if (_instance == null)
                {
                    Debug.LogError("An instance of Event2 is needed in the scene, but there is none.");
                }
            }
            return _instance;
        }
    }

    public GameObject btnFather;
    
    public DialogueData event2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && canChange)
        {
            Transition.Instance.SwitchSceneWithFade("FlowerShop");
        }
    }

    // 确保在场景中只有一个Event2实例
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
           
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {   
        data.dialogueList[0].canContinue=false;
        PlayerMove.canMove=false;
        ani.SetBool("canMove", true);
        StartCoroutine(flow());
        DialogueManager.Instance.switchUI_tem();
    }

    // Update is called once per frame
    
    public void button1()
    {
        DataBroker.actionValue += 2;
        CageManager.Instance.AddInsect(DataSo);
        Debug.Log(DataBroker.experience);
        Debug.Log(DataBroker.actionValue);
        DialogueManager.Instance.canContinue=true;
        btnFather.SetActive(false);
        hint.ShowHint(desciption1);
        canChange=true; 
        PlayerMove.canMove=true;
        DialogueManager.Instance.EndDialogue(null);
        DialogueManager.Instance.switchUI_1();
        data.dialogueList[0].canContinue=true;
        
    }

    public void button2()
    {
        DataBroker.actionValue += 1;
        DataBroker.reputation += 4;
        CageManager.Instance.AddInsect(DataSo);
        Debug.Log(DataBroker.experience);
        Debug.Log(DataBroker.actionValue);
        DialogueManager.Instance.canContinue=true;
        btnFather.SetActive(false);
        hint.ShowHint(desciption2);
        canChange=true;
        PlayerMove.canMove=true;
        DialogueManager.Instance.EndDialogue(null);
        DialogueManager.Instance.switchUI_1();
        data.dialogueList[0].canContinue=true;
    }

    public void button3()
    {
        DataBroker.actionValue += 1;
        CageManager.Instance.AddInsect(DataSo);
        Debug.Log(DataBroker.experience);
        Debug.Log(DataBroker.reputation);
        DialogueManager.Instance.canContinue=true;
        btnFather.SetActive(false);
        hint.ShowHint(desciption3);
        canChange=true;
        PlayerMove.canMove=true;
        DialogueManager.Instance.EndDialogue(null);
        DialogueManager.Instance.switchUI_1();
        data.dialogueList[0].canContinue=true;
    }

    IEnumerator flow()
    {
      
        yield return new WaitForSeconds(3.7f);
        ani.enabled=false;
        DialogueManager.Instance.StartDialogue(event2,setfather);
       
        
    }

    void setfather()
    {   
        DialogueManager.Instance.StartDialogue(data);
        btnFather.SetActive(true);
    }
}
