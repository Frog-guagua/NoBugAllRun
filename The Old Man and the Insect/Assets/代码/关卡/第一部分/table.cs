using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class table : MonoBehaviour
{
    public Transform player;
    public  bool tableCanInteract = false;
    public InsectDataSO a;
    public InsectDataSO b;
    public float dis = 2f;
    public bool active=false;
    public Hint hint;
    public string str;
    private bool cageOn = false;
    private bool isset = false;
    public static bool tableact=false;
    public GameObject cage;
    void Update()
    {   
       
        if (Vector2.Distance(player.position, transform.position) <= dis)
        {
            if (cageOn==false&&tableact)
            {
                PlayerMove.canMove = false;
                hint.ShowHint(str);
                Debug.Log("放笼子");
                cage.SetActive(true);
                cageOn = true;
            }
            
            tableCanInteract = true;
            
        }
        else
        {
            tableCanInteract = false;
        }
    }
    private void OnMouseDown()
    {   
        
        
        if (tableCanInteract && Vector2.Distance(player.position, transform.position) <= dis&&cageOn)
        {   
            PlayerMove.canMove = false;
            Debug.Log("你点到了神秘小桌子");
            if (isset==false)
            {   
                
                isset = true;
                SetCage();
               
            }
            else
            {
                CageUI.Instance.setAct();
            }
           
            tableCanInteract = false;
            active = true;
        }

        if (active)
        {
           CageUI.Instance.setAct();
        }
    }

    public void SetCage()
    {
        
        // 延迟添加，等格子生成完毕
        StartCoroutine(AddInsectAfterDelay());
    }

    IEnumerator AddInsectAfterDelay()
    {
        // 等一帧 + 一小段延迟，确保 CageUI 的格子已经生成
      
        yield return new WaitForSeconds(0.6f);
        CageUI.Instance.setAct();
        CageManager.Instance.AddInsect(a);
        CageManager.Instance.AddInsect(b);
        CageManager.Instance.AddInsect(a);
        LevelStateManager.Instance.SwitchState(LevelState.guide1);
    }

    
}