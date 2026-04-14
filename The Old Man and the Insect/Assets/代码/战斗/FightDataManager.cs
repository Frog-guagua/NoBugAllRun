using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FightDataManager : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> tagDatas = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> datas = new List<TextMeshProUGUI>();
    [SerializeField] List<InsectData> bugs = DataBroker.Instance.datasFromCage;
    
    void Start()
    {
        int i = 0;
        foreach (InsectData bug in bugs)
        {
           
            
            
            if (bug.insectHP == 0)
            {
                bug.gameObject.SetActive(false);
            }

            tagDatas[i].text = $"{bug.insectHP}    {bug.insectAtk}";
            datas[i].text = $"{bug.insectHP}\n\n\n{bug.insectAtk}";
            
            i++;
        }
    }

    
    void Update()
    {
        
    }
}
