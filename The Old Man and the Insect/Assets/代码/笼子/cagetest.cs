using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cagetest : MonoBehaviour
{   
    public InsectDataSO insectData;
    // Start is called before the first frame update
    void Start()
    {
     StartCoroutine(test());
   LevelStateManager.canquit=true;
    // LevelStateManager.secondin=true;
    
  //  LevelStateManager.secondin=true;
  
    }

    IEnumerator test()
    {   
        
     //   DataBroker.experience = 99;
     //   DataBroker.reputation = 10;
        CageUI.Instance.setAct();
        yield return new WaitForSeconds(0.4f);
        CageUI.Instance.setInactive();
      //  CageManager.Instance.AddInsect(insectData);
       // CageManager.Instance.AddInsect(insectData);
       
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick()
    {   
        CageUI.Instance.setAct();
        
        print("test");
      //  DataBroker.experience++;
     //   for (int i = 0; i < DataBroker.Instance.datasFromFight.Count; i++)
     //   {
     //       Debug.Log(DataBroker.Instance.datasFromFight[i].insectLevel);
     //   }
     // CageManager.Instance.AddInsect(insectData);
    }
}
