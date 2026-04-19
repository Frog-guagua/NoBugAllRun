using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hutong0 : MonoBehaviour
{


    public string str;
    public GameObject player;
    public Vector2 leftAndDown_DoorRange, rightAndUp_DoorRange;

    public Hint hint;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(gethint());
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > leftAndDown_DoorRange.x
               && player.transform.position.x < rightAndUp_DoorRange.x
               && player.transform.position.y > leftAndDown_DoorRange.y
               && player.transform.position.y < rightAndUp_DoorRange.y
              )
        {
            Transition.Instance.SwitchSceneWithFade("HuTong1");
        }
    }

    IEnumerator gethint()
    {
        yield return new WaitForSeconds(1);
        hint.ShowHint(str);
    }
}
