using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class table : MonoBehaviour
{
    public Transform player;
    public static bool tableCanInteract = false;
    public InsectDataSO a;
    public InsectDataSO b;
    public float dis = 2f;
    public bool active=false;

    private void OnMouseDown()
    {
        if (tableCanInteract && Vector2.Distance(player.position, transform.position) <= dis)
        {
            Debug.Log("你点到了神秘小桌子");
            SetCage();
            LevelStateManager.Instance.SwitchState(LevelState.guide1);
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
        CageUI.Instance.setAct();
        // 延迟添加，等格子生成完毕
        StartCoroutine(AddInsectAfterDelay());
    }

    IEnumerator AddInsectAfterDelay()
    {
        // 等一帧 + 一小段延迟，确保 CageUI 的格子已经生成
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.2f);

        CageManager.Instance.AddInsect(a);
        CageManager.Instance.AddInsect(b);
        CageManager.Instance.AddInsect(a);
    }

    
}