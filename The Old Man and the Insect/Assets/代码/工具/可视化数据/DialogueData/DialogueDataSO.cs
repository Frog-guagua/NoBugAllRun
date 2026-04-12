using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/新建对话数据", fileName = "NewDialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public struct DialogueSentence
    {
        public string speakerName;
        public Sprite speakerSprite;
        public string content;
        public bool Shake;
    }

    public List<DialogueSentence> dialogueList = new List<DialogueSentence>();
    //list中，每个结构体实例存入单句对话信息，把整段对话按顺序写进一个列表。
}