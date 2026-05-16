using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flowermgr2 : MonoBehaviour
{
    public static int getinTime = 1;

    public DialogueData data1;
    public DialogueData data2;
    // Start is called before the first frame update
    public Hint hint;
    void Start()
    {   
        
        if (getinTime == 1)
        {
            DialogueManager.Instance.StartDialogue(data1);
           
        }
        else
        {
            DialogueManager.Instance.StartDialogue(data2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
        }
    }
}
