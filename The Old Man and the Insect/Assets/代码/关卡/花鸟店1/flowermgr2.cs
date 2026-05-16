using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flowermgr2 : MonoBehaviour
{
    public static int getinTime = 1;
    bool canchange = true;
    public DialogueData data1;
    public DialogueData data2;
    public AudioClip Clip;
    public DialogueData data3;
    public DialogueData data4;
    // Start is called before the first frame update
    public Hint hint;
    public GameObject panel;
    public string description1;
    public string description2;
    public string description3;
    bool firststartflow=false;
    bool firststartflow2=false;
    bool firststartflow3=false;
    void Start()
    {   
        AudioMgr.Instance.PlaySFX(Clip);
        canchange = true;
        if (getinTime == 1)
        {
            DialogueManager.Instance.StartDialogue(data1);
           firststartflow = true;
        }
        else
        {
            DialogueManager.Instance.StartDialogue(data2);
        }
    }

   

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&canchange)
        {
            if (DataBroker.experience >0)
            {
                panel.SetActive(true);
            }
            else
            {
                startdialogue();
            }
        }

        if (firststartflow&&Input.GetMouseButtonDown(0))
        {
            firststartflow = false;
            StartCoroutine(flow1());
        }
        if (firststartflow2&&Input.GetMouseButtonDown(0))
        { 
            firststartflow2 = false;
            StartCoroutine(flow2());
        }

        if (firststartflow3 && Input.GetMouseButtonDown(0))
        {   
            
            firststartflow3 = false;
            StartCoroutine(flow3());
        }
    }

    IEnumerator flow1()
    {
        yield return new WaitForSeconds(1);
        hint.ShowHint(description1);
        firststartflow2 = true;
    }
    IEnumerator flow2()
    {
        yield return new WaitForSeconds(1);
        hint.ShowHint(description2);
        firststartflow3 = true;
    }

    IEnumerator flow3()
    {
        yield return new WaitForSeconds(1);
        hint.ShowHint(description3);
    }
    public void startdialogue()
    {   
        canchange = false;
        if (getinTime == 1)
        {
            DialogueManager.Instance.StartDialogue(data3,switchscene);
           
        }
        else
        {
            DialogueManager.Instance.StartDialogue(data4,switchscene);
        }
    }
    void switchscene()
    {   
       
        if (getinTime == 1)
        {   
            getinTime++;
            Transition.Instance.SwitchSceneWithFade("BeforeFight2");
        }
        else
        {
            Transition.Instance.SwitchSceneWithFade("BeforeBoss");
        }
    }
}
