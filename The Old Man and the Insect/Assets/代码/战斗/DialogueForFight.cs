using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueForFight : MonoBehaviour
{
    [Tooltip("对话框背景")]
    [SerializeField] GameObject backgrond;     
    

    [Tooltip("文字")]
    [SerializeField] TextMeshProUGUI text;   

    private Action onEnd;

    void Start()
    {
        backgrond.SetActive(false);
    }

    public void Show(string msg, System.Action callback = null)
    {
        text.text = msg;
        onEnd = callback;
        backgrond.SetActive(true);
    }

    public void OnClickDialogue()
    {
        backgrond.SetActive(false);
        onEnd?.Invoke();
    }
}
