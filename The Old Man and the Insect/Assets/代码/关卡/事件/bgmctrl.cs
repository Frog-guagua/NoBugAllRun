using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgmctrl : MonoBehaviour
{
    public AudioClip bgm;
    // Start is called before the first frame update
    void Start()
    {
        AudioMgr.Instance.StopBGM();
        AudioMgr.Instance.PlayBGM(bgm);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
