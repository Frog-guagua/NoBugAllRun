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
    

    // ========== 新增：往返移动 + 战斗结算 ==========
    /// <summary>
    /// 执行完整的战斗流程（移动、伤害、死亡、重置行动点）
    /// </summary>
    public IEnumerator ExecuteBattle(Vector3 forwardOffset, Vector3 backwardOffset, float moveDuration)
    {
        // 1. 执行往返移动，并等待完成
        yield return StartCoroutine(MoveHitObjectsWithReturnCoroutine(forwardOffset, backwardOffset, moveDuration));

        // 2. 移动完成后，进行伤害结算（按格子索引对应）
        yield return StartCoroutine(ApplyDamageByIndex());

        // 3. 重置行动点为 2（通过 DataBroker）
        DataBroker.actionValue = 2;
       
        

        
    }

    /// <summary>
    /// 移动协程（收集射线击中的物体并往返移动，返回后继续）
    /// </summary>
    private IEnumerator MoveHitObjectsWithReturnCoroutine(Vector3 forwardOffset, Vector3 backwardOffset, float duration)
    {
        List<GameObject> forwardObjects = new List<GameObject>(); // 被 MyFrontGrids 击中的物体（敌方）
        List<GameObject> backwardObjects = new List<GameObject>(); // 被 RivalFrontGrids 击中的物体（我方）

        // 从 MyFrontGrids 发射射线，收集敌方物体
        foreach (var grid in MyFrontGrids)
        {
            if (grid == null) continue;
            Vector2 origin = grid.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayDistance, targetLayer);
            if (hit.collider != null && !forwardObjects.Contains(hit.collider.gameObject))
                forwardObjects.Add(hit.collider.gameObject);
        }

        // 从 RivalFrontGrids 发射射线，收集我方物体
        foreach (var grid in RivalFrontGrids)
        {
            if (grid == null) continue;
            Vector2 origin = grid.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayDistance, targetLayer);
            if (hit.collider != null && !backwardObjects.Contains(hit.collider.gameObject))
                backwardObjects.Add(hit.collider.gameObject);
        }

        // 同时移动所有物体（往返移动）
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

    /// <summary>
    /// 根据格子索引对应进行伤害结算（假设 MyFrontGrids 和 RivalFrontGrids 顺序一一对应）
    /// </summary>
    private IEnumerator ApplyDamageByIndex()
    {
        int count = Mathf.Min(MyFrontGrids.Count, RivalFrontGrids.Count);
        List<InsectData> myBugs = new List<InsectData>();   // 我方虫子（从 RivalFrontGrids 射线击中）
        List<InsectData> enemyBugs = new List<InsectData>(); // 敌方虫子（从 MyFrontGrids 射线击中）

        // 重新获取一次射线击中的物体（确保与移动前一致）
        for (int i = 0; i < count; i++)
        {
            // 我方格子发射射线，获取我方虫子
            Vector2 myOrigin = RivalFrontGrids[i].transform.position;
            RaycastHit2D myHit = Physics2D.Raycast(myOrigin, Vector2.up, rayDistance, targetLayer);
            if (myHit.collider != null)
            {
                InsectData bug = myHit.collider.GetComponent<InsectData>();
                if (bug != null) myBugs.Add(bug);
            }

            // 敌方格子发射射线，获取敌方虫子
            Vector2 enemyOrigin = MyFrontGrids[i].transform.position;
            RaycastHit2D enemyHit = Physics2D.Raycast(enemyOrigin, Vector2.up, rayDistance, targetLayer);
            if (enemyHit.collider != null)
            {
                InsectData bug = enemyHit.collider.GetComponent<InsectData>();
                if (bug != null) enemyBugs.Add(bug);
            }
        }

        // 同时计算伤害并扣血（避免先手秒杀导致对方无法攻击）
        Dictionary<InsectData, int> damageMap = new Dictionary<InsectData, int>();

        // 我方攻击敌方：每个我方虫子攻击对应的敌方虫子（按索引）
        for (int i = 0; i < Mathf.Min(myBugs.Count, enemyBugs.Count); i++)
        {
            InsectData myBug = myBugs[i];
            InsectData enemyBug = enemyBugs[i];
            if (myBug != null && enemyBug != null)
            {
                int dmg = myBug.insectAtk;
                if (!damageMap.ContainsKey(enemyBug)) damageMap[enemyBug] = 0;
                damageMap[enemyBug] += dmg;
            }
        }

        // 敌方攻击我方：每个敌方虫子攻击对应的我方虫子
        for (int i = 0; i < Mathf.Min(enemyBugs.Count, myBugs.Count); i++)
        {
            InsectData enemyBug = enemyBugs[i];
            InsectData myBug = myBugs[i];
            if (enemyBug != null && myBug != null)
            {
                int dmg = enemyBug.insectAtk;
                if (!damageMap.ContainsKey(myBug)) damageMap[myBug] = 0;
                damageMap[myBug] += dmg;
            }
        }

        // 统一扣血
        foreach (var kvp in damageMap)
        {
            kvp.Key.insectHP -= kvp.Value;
            Debug.Log($"{kvp.Key.name} 受到 {kvp.Value} 点伤害，剩余 HP：{kvp.Key.insectHP}");
        }

        // 处理死亡：摇晃消失
        List<Coroutine> deathRoutines = new List<Coroutine>();
        foreach (var bug in myBugs)
        {
            if (bug != null && bug.insectHP <= 0)
                deathRoutines.Add(StartCoroutine(ShakeAndFade(bug.gameObject)));
        }
        foreach (var bug in enemyBugs)
        {
            if (bug != null && bug.insectHP <= 0)
                deathRoutines.Add(StartCoroutine(ShakeAndFade(bug.gameObject)));
        }
        // 等待所有死亡动画完成
        foreach (var routine in deathRoutines) yield return routine;
    }

    /// <summary>
    /// 摇晃后渐隐消失
    /// </summary>
    private IEnumerator ShakeAndFade(GameObject obj)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Vector3 originalPos = obj.transform.position;
        float shakeDuration = 0.3f;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-0.1f, 0.1f);
            float offsetY = Random.Range(-0.1f, 0.1f);
            obj.transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = originalPos;

        float fadeDuration = 0.5f;
        Color color = sr.color;
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            color.a = alpha;
            sr.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.SetActive(false);
    }
}