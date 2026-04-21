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
    [SerializeField]public GameObject background;     
    

    [Tooltip("文字")]
    [SerializeField] TextMeshProUGUI text;   

    private Action onEnd;

    void Start()
    {
        background.SetActive(false);
    }

    public void Speak(string dialogue, System.Action callback = null)
    {
        text.text = dialogue;
        onEnd = callback;
        background.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(background.GetComponent<RectTransform>());
    }


    public IEnumerator WaitForClose()
    {
        while (background.activeSelf) yield return null;
    }
    public void OnClickDialogue()
    {
        background.SetActive(false);
        onEnd?.Invoke();
    }
}
