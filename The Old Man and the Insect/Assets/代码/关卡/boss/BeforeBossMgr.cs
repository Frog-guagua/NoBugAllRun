using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeBossMgr : MonoBehaviour
{
    public static BeforeBossMgr Instance { get; private set; }
    public static bool secondin=false;
    public DialogueData dialogueData;
    public GameObject btn;
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
                Debug.Log("何苦如此，仅仅为求一败？");
            }

            if (DataBroker.WinGame3 == false)
            {
                Debug.Log("无妨，我的结局，亦是你的宿命");
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