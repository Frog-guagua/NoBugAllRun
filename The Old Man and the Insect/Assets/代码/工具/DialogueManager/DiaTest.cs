using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaTest : MonoBehaviour
{   public DialogueData data;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DialogueTest()
    {
        DialogueManager.Instance.StartDialogue(data,a);
    }

    public void a()
    {
        print("y");
    }
}
