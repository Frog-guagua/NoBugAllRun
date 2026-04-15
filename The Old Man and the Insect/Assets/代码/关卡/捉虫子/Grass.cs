using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{   
    public Transform player;
    public float distance = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(player.position, transform.position) <= distance&&Input.GetMouseButton(0))
        {
            Transition.Instance.SwitchSceneWithFade("CatchBugSence");
        }
    }
}
