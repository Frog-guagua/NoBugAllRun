using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class table : MonoBehaviour
{
    public Transform player;
    public static bool tableCanInteract = false;
    
    public float dis = 2f;
    private void OnMouseDown()
    {
        if (tableCanInteract&&Vector2.Distance(player.position,transform.position)<=dis)
        {
            Debug.Log("你点到了神秘小桌子");
            LevelStateManager.Instance.SwitchState(LevelState.leavinghouse);
            //执行放笼子动画
            SetCage();
        }


    }

    public void SetCage()//这个方法应该会在关键帧里调用，先写着
    {
       CageUI.Instance.setAct();
    }
}
