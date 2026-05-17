using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbacusAnim : MonoBehaviour
{
    
    [Header("算盘珠")]
    [SerializeField] Transform ball;

    [Header("音效")]
    [SerializeField] AudioClip endSound;
    
    [Header("起点和终点")]
    [SerializeField] Vector3 startPoint;
    [SerializeField] Vector3 endPoint;

    [Header("珠子移速")]
    [SerializeField] float speed = 5f;

    public static bool Finsined = false;
    private bool isMoving = false;
    private int clickCount = 0;

    //呱：这个是鼠标点击【OnMouseDown】时候进行的判断 这是一个检测点击的【回调函数】
    void OnMouseDown()
    {
        //呱： 这是为了 防止 在珠子移动的时候 又重复点击触发逻辑
        if (isMoving) return;

        #region 第一套逻辑

        //呱 ：记录点击次数 + 判断点击次数为奇数还是偶数
        /*clickCount++;
        bool toEnd = (clickCount % 2 == 1); 
        
        //呱：根据判断 决定目标地点
        Vector3 target = toEnd ? endPoint : startPoint;*/

        #endregion
        
        #region 第二套逻辑
        
        //呱： 记录点击的时候的位置
        Vector3 origionPos = ball.transform.position;
        Vector3 targetPos = origionPos == startPoint? endPoint:startPoint;
        

        #endregion
       
        StartCoroutine(MoveTo(targetPos));
    }

    
    //呱： 移动协程 有关时间的请写协程
    IEnumerator MoveTo(Vector3 target)
    {
        isMoving = true;
        AudioMgr.Instance.PlaySFX(endSound);
        
        while (Vector3.Distance(ball.position, target) > 0.01f)
        {
            ball.position = Vector3.MoveTowards(ball.position, target, speed * Time.deltaTime);
            yield return null;
        }
        
        ball.position = target;
        isMoving = false;
        Finsined = true;
    }

    
    
}
