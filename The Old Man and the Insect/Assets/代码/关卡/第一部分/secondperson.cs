using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondperson : MonoBehaviour
{
    public DialogueData dia;

    private bool canon = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (canon)
        {
            StartCoroutine(flow());
            canon = false;
        }
    }

    IEnumerator flow()
    {
        yield return new WaitForSeconds(0);
        DialogueManager.Instance.StartDialogue(dia,switchscene);
    }

    void switchscene()
    {
        LevelStateManager.Instance.SwitchState(LevelState.KnockingDoor);
    }
}
