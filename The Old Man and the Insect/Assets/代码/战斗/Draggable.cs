using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//呱：写在最前面 ——> 这个脚本名字我觉得我起的很好啊hhhh
//   就是 “可以被拖动的”  那么给你想要被拖动的 物体 挂上这个脚本吧！
public class Draggable : MonoBehaviour
{
    #region 向外传值

    //呱 ： 想要传值 因为只有Draggable才有 机会去得到 抓着的虫子的类型
    //     用静态 是想所有虫虫都挂载 这个脚本的话 静态类能保证唯一性 
    public static E_BugType nowBugType; //呱： 储存目前被拖拽的虫虫的类型
    public static int nowGridIndex;//呱： 记录 虫子在哪个格子上被松开了 方便传值进行判定
    public static GameObject nowBug;

    #endregion
    
    #region 需要优化

    //呱：后续美工优化掉吧 这个笼子拆分图层没法做啊
    [SerializeField]  GameObject cageButton;

    #endregion
    
    #region 获取挂载脚本

    //呱：在这里获取游戏物体 是为了获取上面挂载的脚本
    [SerializeField]  GameObject cage; //呱：获取 CageZoom 脚本 
    [SerializeField]  GameObject gridManager;//呱： 获取 GridManager 脚本
    private RoundManager fightManager;
    private FollowCage followCage;
    #endregion

    #region 拖拽相关参数

    private bool isDragging;
    private Vector3 dragOffset;


    #endregion
    
    
    
    void Start()
    {
        
        GetComponent<Collider2D>().enabled = true;
        followCage = GetComponent<FollowCage>();
        fightManager = GetComponent<RoundManager>();
    }
    
    void Update()
    {
        
        // 呱：按下左键 尝试开始拖拽（只有笼子放大时才允许拖拽）
        if (Input.GetMouseButtonDown(0) && CageZoom.CageHasZoomed)
        {
            TryStartDrag();
        }
        // 呱：按住左键且正在拖拽中 跟随鼠标
        else if (Input.GetMouseButton(0) && isDragging)
        {
         
            FollowMouse();
        }
        // 呱：松开左键 结束拖拽
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            
            #region 禁用碰撞体 （为了防止后面检测格子的时候干扰 / 短暂禁用了 虫子和笼子的 碰撞体）

            Collider2D bugCollider = GetComponent<Collider2D>();
            bugCollider.enabled = false;
            Collider2D cageButtonCollider =cageButton.GetComponent<Collider2D>();
            cageButtonCollider.enabled = false;
            
            #endregion
            
            //呱：调用这个函数的作用 是用来获取 放置虫子的格子 是哪个
            nowGridIndex = gridManager.GetComponent<GridManager>().OnWhichGrid();
            
            #region 恢复碰撞体  ： 在检测格子完毕以后就 恢复碰撞体

            bugCollider.enabled = true;
            cageButtonCollider.enabled = true;

            #endregion

            //呱：这个是为了记录 放置虫子的类型 
            nowBugType = GetComponent<BugInfomations>().bugType;
           
        
           
            StopDrag();
        }

      
        
    }

    #region 拖拽需要的函数

    void TryStartDrag()
    {
        
        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        // 临时禁用笼子按钮碰撞体，避免干扰射线
        Collider2D cageButtonCollider = cageButton.GetComponent<Collider2D>();
        if (cageButtonCollider != null) cageButtonCollider.enabled = false;
        
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
      
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
           
            nowBug = hit.collider.gameObject;
            isDragging = true;
            followCage.enabled = false;
           
            
            dragOffset = transform.position - mousePos;   

       
            // 拖拽时强制笼子缩小回原位
            cage.GetComponent<CageZoom>().OriginPos();
           
            BugInfomations bugInfo = GetComponent<BugInfomations>();
           
            DifBugDoDifThings(bugInfo.bugType);
            
        }
        else
        {
            // 没点中自己，也要恢复碰撞体
            if (cageButtonCollider != null) cageButtonCollider.enabled = true;
        }
    }

    void FollowMouse()
    {
       
        Vector3 mousePos = 
            Camera.main.ScreenToWorldPoint
                (new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        transform.position = mousePos + dragOffset;
    }

    void StopDrag()
    {
      
        isDragging = false;
        if (followCage != null) followCage.enabled = true;
       
    }

    #endregion
   
    
    void DifBugDoDifThings(E_BugType bugType)
    {
        switch (bugType)
        {
            case E_BugType.A:
                
                break;
            case E_BugType.B:
                
                break;
            case E_BugType.C:
                break;
            case E_BugType.D:
                break;
            case E_BugType.E:
                break;
            case E_BugType.F:
                break;
        }
    }
    
}