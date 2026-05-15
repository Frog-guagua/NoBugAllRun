using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{   
    public Transform player;
    public float distance ;
    public GameObject wow;
    // Start is called before the first frame update
    void Start()
    {
        DataBroker.Instance.canBeginCatch=!DataBroker.Instance.canBeginCatch;
        DataBroker.Instance.canEndCatch = !DataBroker.Instance.canEndCatch;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(player.position, transform.position) <= distance&&
            Input.GetMouseButton(0)&&DataBroker.Instance.canBeginCatch)
        {
            Transition.Instance.SwitchSceneWithFade("CatchBugSence");
        }
        wow.SetActive(DataBroker.Instance.canBeginCatch);
    }
}
