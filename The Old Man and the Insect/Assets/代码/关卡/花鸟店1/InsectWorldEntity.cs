// InsectWorldEntity.cs
using UnityEngine;

public class InsectWorldEntity : MonoBehaviour
{
    [HideInInspector] public int cageSlotIndex = -1;

    private void Awake()
    {
        if (GetComponent<Collider>() == null && GetComponent<Collider2D>() == null)
            gameObject.AddComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {   
      if( anictrl.canget == false)return;
        if (cageSlotIndex < 0) return;
        InsectUpgradePanel.Instance?.Show(this);
       
    }

    public void UpgradeAtk()
    {
       

        InsectData cageData = CageManager.Instance.insectInCage[cageSlotIndex];
        InsectData myData = GetComponent<InsectData>();
        

        cageData.LetAtkUp();

        myData.insectAtk = cageData.insectAtk;
        myData.insectLevel = cageData.insectLevel;
    }

    public void UpgradeHp()
    {
       

        InsectData cageData = CageManager.Instance.insectInCage[cageSlotIndex];
        InsectData myData = GetComponent<InsectData>();
       

        cageData.LetHPUp();

        myData.insectHP = cageData.insectHP;
        myData.insectLevel = cageData.insectLevel;
    }
}