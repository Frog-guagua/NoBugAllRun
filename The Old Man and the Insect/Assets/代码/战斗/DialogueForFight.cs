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

    public void Speak(string dialogue, System.Action callback = null)
    {
        text.text = dialogue;
        onEnd = callback;
        backgrond.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(backgrond.GetComponent<RectTransform>());
    }


    public IEnumerator WaitForClose()
    {
        while (backgrond.activeSelf) yield return null;
    }
    public void OnClickDialogue()
    {
        backgrond.SetActive(false);
        onEnd?.Invoke();
    }
}
