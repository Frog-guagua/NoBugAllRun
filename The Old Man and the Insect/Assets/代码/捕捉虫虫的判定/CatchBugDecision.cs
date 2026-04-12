using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CatchBugDecision : MonoBehaviour
{
    #region 移动区域

    //[移动区域] ：用来获取人和虫子移动的 限定长度 
    [Header("MovingZone")]
    [SerializeField] GameObject MovingZone;
    private SpriteRenderer  MovingZone_SR;
    private float limitedLength = 0f;
    private float MovingZoneLeftPosX = 0;
    private float MovingZoneRightPosX = 0;

    #endregion

    #region 虫虫判定区域

    [Header("BugZone")]
    //[虫虫判定区域] ： 根据其判定区域的长度与位置 用作捕获虫虫的判定 
    [SerializeField]  GameObject BugZone;
    [SerializeField] float stayTime ;
    private SpriteRenderer BugZone_SR;
    private float BugZoneLength = 0f;
    private Vector2 BugZonePos;
    private bool isMoving = false;
    
    

    #endregion

    #region 虫虫本身
    [Header("Bug")]
    //[虫虫本身]： 获取其挂载的虫虫图片 做出动效 挂载为虫虫判定区域的子物体
    [SerializeField] GameObject Bug;
    private SpriteRenderer Bug_SR;
    [SerializeField] AnimationCurve BugAnimationCurve; 

    #endregion

    #region 手判定区域 
    [Header("HandZone")]
    //[手判定区域] ： 根据其判定区域的长度与位置 用作捕获虫虫的判定 
    [SerializeField] GameObject HandZone;
    [SerializeField] float HandMoveForce = 0f; //手的移动驱动力
    private SpriteRenderer HandZone_SR;
    private Rigidbody2D HandZone_RB;
    private float HandZoneLength = 0f;
    private Vector2 HandZonePos;
    #endregion
    
    #region 恒定y值

    //[恒定y值]
    private float Y ;

    #endregion

    #region 进度条
    
    //[进度条] ： 用来判定是否成功抓获虫子
    [Header("ProgressBar")]
    [SerializeField] GameObject progressBar;
    [SerializeField]  float IncreaseSpeed = 0;
    [SerializeField]  float ReduceSpeed = 0;
    [SerializeField] float StandardCatchTime;
    SpriteRenderer progressBar_SR;
    private float ProgressBarOriginLength;
    private float ProgressBarNowLength;
    private float NowCatchTime;
    private float fullScaleX;
    
    #endregion
  
  
    void Start()
    {
        #region 获取必要数据

        //呱：一开始先 获得左右端点的x坐标
        MovingZone_SR = MovingZone.GetComponent<SpriteRenderer>();
        limitedLength = MovingZone_SR.bounds.size.x;
        MovingZoneLeftPosX = MovingZone_SR.bounds.center.x - limitedLength/2 + 3;
        MovingZoneRightPosX = MovingZone_SR.bounds.center.x + limitedLength/2-1;
       
        //呱：获取虫虫判定区域的长度 和刚体 初始坐标
        BugZone_SR = BugZone.GetComponent<SpriteRenderer>();
        BugZoneLength = BugZone_SR.bounds.size.x;
        
        //呱：获取虫虫的图片 方便后期做动效
        Bug_SR = Bug.GetComponent<SpriteRenderer>();
        
        //呱：获取手判定区域的长度 和刚体 初始坐标
        HandZone_SR = HandZone.GetComponent<SpriteRenderer>();
        HandZoneLength = HandZone_SR.bounds.size.x;
        HandZone_RB = HandZone.GetComponent<Rigidbody2D>();
        HandZonePos = HandZone_RB.transform.position;
        
        //呱：初始化 进度条的图片 获取初始进度条长度
        progressBar_SR = progressBar.GetComponent<SpriteRenderer>();
        ProgressBarOriginLength = progressBar_SR.bounds.size.x;
        fullScaleX = progressBar.transform.localScale.x;
        #endregion
        
        #region 设定初始位置

        //呱：设定虫虫初始位置 随机
        Y = BugZonePos.y;
        BugZonePos =
            new Vector2
                (Random.Range(MovingZoneLeftPosX+BugZoneLength,MovingZoneRightPosX-BugZoneLength),Y);
 
        //呱：设定手初始位置 固定
        HandZonePos = 
            new Vector2
                (MovingZone_SR.bounds.center.x,HandZonePos.y);
        
        #endregion
        
    }

    
    void Update()
    {
        GetPosition();
        BugMove();
        HandMove();
        CatchTimeManage();
        ProgressBarManage();
        BugAnimation();
    }

    //呱： 虫虫の等待
    IEnumerator BugStay()
    {
        yield return  new WaitForSeconds(stayTime);
        isMoving = false;
    }
    
    //呱：虫虫移动目的地函数
    void BugMove()
    {
        if (isMoving) return;
        isMoving = true;
        float targetX = BugZonePos.x; 
        
        
        if (BugIsCaught())
        {
            //呱：想的是用这样的方法 判断虫虫在手的 左边还是右边
            if (BugZonePos.x - HandZonePos.x < 0)
            {
                //呱：虫虫在手的左边 所以虫虫要往左逃
                float goalX = Random.Range(MovingZoneLeftPosX, BugZonePos.x);
                targetX = goalX;
                
            }
            else
            {
                //呱：虫虫在手的右边 所以虫虫要往右逃
                float goalX = Random.Range(BugZonePos.x, MovingZoneRightPosX);
                targetX = goalX;
            }
        }
        else
        {
            //呱：让虫虫站在原地 等一下 手
            float newX = Random.Range(MovingZoneLeftPosX, MovingZoneRightPosX);
            targetX = newX;
        }

       
        StartCoroutine(MoveToTarget(targetX));
    }

    //呱：虫虫移动实现协程
    IEnumerator MoveToTarget(float targetX)
    {
        Vector2 startPos = BugZone.transform.position;
        float startX = startPos.x;
        float duration = 0.3f; 
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float newX = Mathf.Lerp(startX, targetX, t / duration);
            BugZone.transform.position = new Vector2(newX, Y);
            yield return null;
        }
        
        BugZone.transform.position = new Vector2(targetX, Y);

        
        StartCoroutine(BugStay());
    }
    
    //呱：手移动实现函数
    void HandMove()
    {
        float newX = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
             newX = HandZonePos.x - HandMoveForce*Time.deltaTime;
             if (newX < MovingZoneLeftPosX) newX = MovingZoneLeftPosX;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
             newX = HandZonePos.x + HandMoveForce*Time.deltaTime;
             if (newX > MovingZoneRightPosX) newX = MovingZoneRightPosX;
        }

        HandZone.transform.position = 
            new Vector2(Mathf.Lerp(HandZonePos.x,newX,0.1f), Y);
    }
    
    //呱：管理进度条的尺寸 和 显示
    void ProgressBarManage()
    {
        float progress = NowCatchTime / StandardCatchTime;
        Vector3 scale = progressBar.transform.localScale;
        scale.x = progress*fullScaleX;  
        progressBar.transform.localScale = scale;
    }
    
    //呱：累计时间的函数 用来判定是否抓捕成功
    void CatchTimeManage()
    {
        if (BugIsCaught())
        {
            NowCatchTime += Time.deltaTime * IncreaseSpeed;
            if (NowCatchTime >= StandardCatchTime)
            {
                Vector3 scale = progressBar.transform.localScale;
                scale.x = fullScaleX;
                progressBar.transform.localScale = scale;
                //呱：这样就结束了
                enabled = false;
            }
        }
        else
        {
            NowCatchTime -= Time.deltaTime * ReduceSpeed;
            if (NowCatchTime < 0) NowCatchTime = 0;
        }

    }
    
    //呱：判断虫虫是否被抓住
    bool BugIsCaught()
    {
        float handHalf = HandZoneLength / 2f;
        float bugHalf = BugZoneLength / 2f;
        float distance = Mathf.Abs(HandZonePos.x - BugZonePos.x);
        
        
        return distance < handHalf + bugHalf;
    }
    
    //呱：获取虫虫判定区域 和 手判定区域 的坐标
    void GetPosition()
    {
        HandZonePos = HandZone.transform.position;
        BugZonePos = BugZone.transform.position;
    }
    
    //呱：给虫虫做动效的函数
    void BugAnimation()
    {
        float time = Time.time * 5f;   
        float curveValue = BugAnimationCurve.Evaluate(time % 1f);

        
        float yOffset = Mathf.Abs(Mathf.Sin(time * Mathf.PI * 2f)) * 0.1f;  // 快速弹起，稍慢落下
        Bug.transform.localPosition = new Vector3(0, yOffset, 0);

        
        float shake = Mathf.Sin(time * 30f) * 0.02f;
        float angle = shake * (1f + curveValue * 2f);
        Bug.transform.localRotation = Quaternion.Euler(0, 0, angle);
        
      
    }
}