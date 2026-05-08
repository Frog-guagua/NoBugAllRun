using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeBossMgr : MonoBehaviour
{
    public static BeforeBossMgr Instance { get; private set; }
    public static bool secondin=false;
    public DialogueData dialogueData;
    public GameObject btn;
    public DialogueData win;
    public DialogueData lose;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
       
    }

    void Start()
    {   if(secondin==false)
        StartCoroutine(flow());
        if (secondin == true)
        {
            if (DataBroker.WinGame3 == true)
            {
                DialogueManager.Instance.StartDialogue(win);
            }

            if (DataBroker.WinGame3 == false)
            {
                DialogueManager.Instance.StartDialogue(lose);
            }
        }
    }

    IEnumerator flow()
    {
        yield return new WaitForSeconds(1.5f);
        DialogueManager.Instance.StartDialogue(dialogueData, startFight);
    }

    void startFight()
    {
        FightFlowManager.OnGame3 = true;
        FightFlowManager.OnGame2=false;
        btn.SetActive(true);
    }

    public void OnClick()
    {   
        secondin = true;
      Transition.Instance.SwitchSceneWithFade("FightScene3");
      btn.SetActive(false);
    }
}