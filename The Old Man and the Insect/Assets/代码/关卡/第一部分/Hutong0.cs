using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hutong0 : MonoBehaviour
{

    public AudioClip clip;
    public string str;
    public GameObject player;
    public Vector2 leftAndDown_DoorRange, rightAndUp_DoorRange;
    private bool isswitch = false;

    public Hint hint;

    // Start is called before the first frame update
    void Start()
    {
        
      //  AudioMgr.Instance.PlayBGM(clip);
        StartCoroutine(flow());
    }

    IEnumerator flow()
    {
        yield return new WaitForSeconds(0.7f);
        hint.ShowHint("前进");
    }
    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > leftAndDown_DoorRange.x
            && player.transform.position.x < rightAndUp_DoorRange.x
            && player.transform.position.y > leftAndDown_DoorRange.y
            && player.transform.position.y < rightAndUp_DoorRange.y
            && isswitch == false)
        {
            Transition.Instance.SwitchSceneWithFade("HuTong1");
            isswitch = true;
        }
    }
}
    
