using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//呱： 写在最前面 这个脚本挂在 空物体“格子管理器”上面
public class GridManager : MonoBehaviour
{
    public GameObject[] grids = new GameObject[16];
    [Header("移动参数")]
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [SerializeField] List<GameObject> RivalFrontGrids = new List<GameObject>();
    [SerializeField] List<GameObject> MyFrontGrids = new List<GameObject>();
    private bool[] isOnGrid = new bool[16];
    [SerializeField]GameObject fightManager;
    public static int nowGridIndex;
    public float rayDistance = 5f;
    public LayerMask targetLayer;   // 指定射线检测的层（可选）

    void Start()
    {
     
    
    }

    void Update()
    {
        
    }

   
    //呱：用来判断 放置虫子时  虫子在哪个格子上空
    public int OnWhichGrid()
    {

        #region 射线检测前置

        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint
                (new Vector3(Input.mousePosition.x, Input.mousePosition.y, grids[0].transform.position.z));
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        #endregion

        #region Debug ：已解决 我受不了了 死射线你到底打中了什么 ……

           //呱：Debug[已解决 ：我受不了了 死射线你到底打中了什么 ……]
           //Debug.Log("击中物体：" + (hit.collider ? hit.collider.name : "无"));
           
        #endregion
        
        for (int i = 0; i < grids.Length; i++)
        { 
            if (hit.collider == grids[i].GetComponent<Collider2D>())
            {
                isOnGrid[i] = true;
                nowGridIndex = i;
                return nowGridIndex;
            }
        }

        CageZoom.CageHasZoomed = false;
        return -1;
    }

    
  

  
    public void MoveHitObjectsByRay(Vector3 moveOffset, float duration)
    {
        StartCoroutine(MoveHitObjectsCoroutine(moveOffset, duration));
    }

    private IEnumerator MoveHitObjectsCoroutine(Vector3 moveOffset, float duration)
    {
        List<GameObject> hitObjects = new List<GameObject>();
        List<GameObject> allGrids = new List<GameObject>();
        allGrids.AddRange(RivalFrontGrids);
        allGrids.AddRange(MyFrontGrids);

        // 每个格子向上发射射线，收集被击中的物体（去重）
        foreach (var grid in allGrids)
        {
            if (grid == null) continue;
            Vector2 origin = grid.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayDistance, targetLayer);
            if (hit.collider != null && !hitObjects.Contains(hit.collider.gameObject))
            {
                hitObjects.Add(hit.collider.gameObject);
            }
        }

        // 同时移动所有被击中的物体
        List<Coroutine> routines = new List<Coroutine>();
        foreach (var obj in hitObjects)
        {
            if (obj == null) continue;
            Vector3 target = obj.transform.position + moveOffset;
            routines.Add(StartCoroutine(MoveSingle(obj, target, duration)));
        }
        foreach (var routine in routines) yield return routine;
    }

    private IEnumerator MoveSingle(GameObject obj, Vector3 target, float duration)
    {
        Vector3 start = obj.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = moveCurve.Evaluate(elapsed / duration);
            obj.transform.position = Vector3.Lerp(start, target, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = target;
    }
    

public void MoveHitObjectsWithReturn(Vector3 forwardOffset, Vector3 backwardOffset, float duration)
{
    StartCoroutine(MoveHitObjectsWithReturnCoroutine(forwardOffset, backwardOffset, duration));
}

private IEnumerator MoveHitObjectsWithReturnCoroutine(Vector3 forwardOffset, Vector3 backwardOffset, float duration)
{
    List<GameObject> forwardObjects = new List<GameObject>(); // 我方击中的物体（向前）
    List<GameObject> backwardObjects = new List<GameObject>(); // 敌方击中的物体（向后）

    // 1. 从 MyFrontGrids 发射射线，收集向前移动的物体
    foreach (var grid in MyFrontGrids)
    {
        if (grid == null) continue;
        Vector2 origin = grid.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayDistance, targetLayer);
        if (hit.collider != null && !forwardObjects.Contains(hit.collider.gameObject))
            forwardObjects.Add(hit.collider.gameObject);
    }

    // 2. 从 RivalFrontGrids 发射射线，收集向后移动的物体
    foreach (var grid in RivalFrontGrids)
    {
        if (grid == null) continue;
        Vector2 origin = grid.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayDistance, targetLayer);
        if (hit.collider != null && !backwardObjects.Contains(hit.collider.gameObject))
            backwardObjects.Add(hit.collider.gameObject);
    }

    // 3. 同时启动所有物体的往返移动
    List<Coroutine> routines = new List<Coroutine>();
    foreach (var obj in forwardObjects)
    {
        if (obj == null) continue;
        routines.Add(StartCoroutine(MoveRoundTrip(obj, obj.transform.position + forwardOffset, duration)));
    }
    foreach (var obj in backwardObjects)
    {
        if (obj == null) continue;
        routines.Add(StartCoroutine(MoveRoundTrip(obj, obj.transform.position + backwardOffset, duration)));
    }

    // 等待所有移动完成
    foreach (var routine in routines) yield return routine;
}

/// <summary>
/// 让物体移动到目标点再返回原点，使用曲线（0→1→0）
/// </summary>
private IEnumerator MoveRoundTrip(GameObject obj, Vector3 target, float duration)
{
    Vector3 start = obj.transform.position;
    float elapsed = 0f;
    float halfDuration = duration / 2f;

    // 前半段：移动到目标点
    while (elapsed < halfDuration)
    {
        float t = moveCurve.Evaluate(elapsed / halfDuration);
        obj.transform.position = Vector3.Lerp(start, target, t);
        elapsed += Time.deltaTime;
        yield return null;
    }
    obj.transform.position = target;

    // 后半段：移回原点
    elapsed = 0f;
    while (elapsed < halfDuration)
    {
        float t = moveCurve.Evaluate(elapsed / halfDuration);
        obj.transform.position = Vector3.Lerp(target, start, t);
        elapsed += Time.deltaTime;
        yield return null;
    }
    obj.transform.position = start;
}
}