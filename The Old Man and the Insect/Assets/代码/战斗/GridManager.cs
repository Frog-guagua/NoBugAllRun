using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



//呱： 写在最前面 这个脚本挂在 空物体“格子管理器”上面
public class GridManager : MonoBehaviour
{
    
    [Header("移动参数")]
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [SerializeField] List<GameObject> RivalFrontGrids = new List<GameObject>();
    [SerializeField] List<GameObject> MyFrontGrids = new List<GameObject>();
    private bool[] isOnGrid = new bool[16];
    [SerializeField] GameObject fightManager;
    public static int nowGridIndex;
    public float rayDistance = 5f;
    public LayerMask targetLayer;

    
    //呱：格子物体和 格子压缩包数组
    public GameObject[] grids = new GameObject[16];
    public static Grid[] Grids = new Grid[16];
    [SerializeField]  Vector3 enemyOffset;
    [SerializeField]  Vector3 myOffset;
 
    void Start()
    {
        //呱 ： 为格子数组赋固有属性的值
        for (int i = 0; i < grids.Length; i++)
        {
            Grids[i].gridObject = grids[i];
            Grids[i].index = i;
            if (i >= 0 && i < 8)
            {
                Grids[i].matchedPos = grids[i].transform.position + myOffset;
            }
            else
            {
                Grids[i].matchedPos = grids[i].transform.position + enemyOffset;
            }
           
            
            //呱：格子 初始默认为 没有虫虫在上面
            Grids[i].isOccupied = false;
            Grids[i].bugOnGrid = null;
            
            
            //呱：利用int舍弃精度转换 除以4 来确定行数 
            switch (i/ 4)
            {
                case 0:
                    Grids[i].gridType = E_GridType.MyBack;
                    break;
                case 1 :
                    Grids[i].gridType = E_GridType.MyFront;
                    break;
                case 2:
                    Grids[i].gridType = E_GridType.EnemyFront;
                    break;
                case 3 :
                    Grids[i].gridType = E_GridType.EnemyBack;
                    break;

            }

            //呱：判断是否是右边区域的虫子
            Grids[i].isRight = (i % 4 == 2 || i % 4 == 3);

        }
    }

    void Update() { }

    //呱：用来判断 放置虫子时  虫子在哪个格子上空
    public Grid? OnWhichGrid()
    {
        #region 点击逻辑处理

        Vector3 mousePos = 
            Camera.main.ScreenToWorldPoint
                (new Vector3(Input.mousePosition.x, Input.mousePosition.y, grids[0].transform.position.z));
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        #endregion
        
        for (int i = 0; i < Grids.Length; i++)
        {
            if (hit.collider == Grids[i].gridObject.GetComponent<Collider2D>())
            {
                //呱： 在格子上 
                isOnGrid[i] = true;
                
                //呱：索引
                nowGridIndex = i;
                
                return Grids[i];
            }
        }
        CageZoom.CageHasZoomed = false;
        return null;
    }

    public IEnumerator ExecuteBattle(Vector3 forwardOffset, Vector3 backwardOffset, float moveDuration)
    {
        
        yield return StartCoroutine(MoveHitObjectsWithReturnCoroutine(forwardOffset, backwardOffset, moveDuration));
        yield return StartCoroutine(ApplyDamageByIndex());
        DataBroker.actionValue = 2;
    }

    private IEnumerator MoveHitObjectsWithReturnCoroutine(Vector3 forwardOffset, Vector3 backwardOffset, float duration)
    {
       
        List<GameObject> forwardObjects = new List<GameObject>();
        List<GameObject> backwardObjects = new List<GameObject>();

        foreach (var grid in MyFrontGrids)
        {
            if (grid == null) continue;
            Vector2 origin = grid.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayDistance, targetLayer);
            if (hit.collider != null && !forwardObjects.Contains(hit.collider.gameObject))
                forwardObjects.Add(hit.collider.gameObject);
        }

        foreach (var grid in RivalFrontGrids)
        {
            if (grid == null) continue;
            Vector2 origin = grid.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayDistance, targetLayer);
            if (hit.collider != null && !backwardObjects.Contains(hit.collider.gameObject))
                backwardObjects.Add(hit.collider.gameObject);
        }

        List<Coroutine> routines = new List<Coroutine>();
        foreach (var obj in forwardObjects)
            routines.Add(StartCoroutine(MoveRoundTrip(obj, obj.transform.position + forwardOffset, duration)));
        foreach (var obj in backwardObjects)
            routines.Add(StartCoroutine(MoveRoundTrip(obj, obj.transform.position + backwardOffset, duration)));

        foreach (var routine in routines) yield return routine;
    }

    private IEnumerator MoveRoundTrip(GameObject obj, Vector3 target, float duration)
    {
        Vector3 start = obj.transform.position;
        float elapsed = 0f;
        float halfDuration = duration / 2f;

        while (elapsed < halfDuration)
        {
            float t = moveCurve.Evaluate(elapsed / halfDuration);
            obj.transform.position = Vector3.Lerp(start, target, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = target;

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

private IEnumerator ApplyDamageByIndex()
{
   
    // 存储每只虫子将要受到的伤害（临时）
    Dictionary<InsectData, int> damageMap = new Dictionary<InsectData, int>();

    int count = Mathf.Min(MyFrontGrids.Count, RivalFrontGrids.Count);
    for (int i = 0; i < count; i++)
    {
        // 获取我方虫子
        Vector2 myOrigin = RivalFrontGrids[i].transform.position;
        RaycastHit2D myHit = Physics2D.Raycast(myOrigin, Vector2.up, rayDistance, targetLayer);
        InsectData myBug = myHit.collider != null ? myHit.collider.GetComponent<InsectData>() : null;

        // 获取敌方虫子
        Vector2 enemyOrigin = MyFrontGrids[i].transform.position;
        RaycastHit2D enemyHit = Physics2D.Raycast(enemyOrigin, Vector2.up, rayDistance, targetLayer);
        InsectData enemyBug = enemyHit.collider != null ? enemyHit.collider.GetComponent<InsectData>() : null;

        // 只有双方都存在时才互相攻击
        if (myBug != null && enemyBug != null)
        {
            // 我方攻击敌方
            if (!damageMap.ContainsKey(enemyBug)) damageMap[enemyBug] = 0;
            damageMap[enemyBug] += myBug.insectAtk;

            // 敌方攻击我方
            if (!damageMap.ContainsKey(myBug)) damageMap[myBug] = 0;
            damageMap[myBug] += enemyBug.insectAtk;

           
        }
      
    }

    // 统一应用伤害
    List<InsectData> allBugs = new List<InsectData>();
    foreach (var kvp in damageMap)
    {
        kvp.Key.insectHP -= kvp.Value;
        allBugs.Add(kvp.Key);
      
    }


    List<InsectData> allMyBugs = new List<InsectData>();
    List<InsectData> allEnemyBugs = new List<InsectData>();
    for (int i = 0; i < count; i++)
    {
        RaycastHit2D myHit = Physics2D.Raycast(RivalFrontGrids[i].transform.position, Vector2.up, rayDistance, targetLayer);
        InsectData myBug = myHit.collider != null ? myHit.collider.GetComponent<InsectData>() : null;
        if (myBug != null) allMyBugs.Add(myBug);

        RaycastHit2D enemyHit = Physics2D.Raycast(MyFrontGrids[i].transform.position, Vector2.up, rayDistance, targetLayer);
        InsectData enemyBug = enemyHit.collider != null ? enemyHit.collider.GetComponent<InsectData>() : null;
        if (enemyBug != null) allEnemyBugs.Add(enemyBug);
    }

    // 更新我方虫子 UI
    foreach (var bug in allMyBugs)
    {
        if (bug != null) UpdateBugUI(bug);
        
    }
    // 更新敌方虫子 UI
    foreach (var bug in allEnemyBugs)
    {
        if (bug != null) UpdateBugUI(bug);
    }

    // 处理死亡（摇晃消失）
    List<Coroutine> deathRoutines = new List<Coroutine>();
    foreach (var bug in allMyBugs)
    {
        if (bug.Name == "对手虫A左")
        {
            if (FightDataManager.ActionPoints == 1)
            {
                bug.insectHP = 0;
                bug.insectAtk = 1;
                deathRoutines.Add(StartCoroutine(ShakeAndFade(bug.gameObject)));
                UpdateBugUI(bug);
            }
            else
            {
                bug.insectHP = 1;
                bug.insectAtk = 1;
                UpdateBugUI(bug);
            }

        }
        
        if (bug.Name == "战斗虫A")
        {
            if (FightDataManager.ActionPoints == 1)
            {
                bug.insectHP = 2;
                bug.insectAtk = 2;
                bug.insectLevel = 2;
                UpdateBugUI(bug);
                yield break;
            }
            else
            {
                bug.insectHP = 1;
                bug.insectAtk = 1;
                UpdateBugUI(bug);
                yield break;
            }

        }
        Debug.Log($"{bug.Name} {bug.insectHP} {bug.insectAtk}");
        if (bug != null && bug.insectHP <= 0 && bug.gameObject.activeSelf)
            deathRoutines.Add(StartCoroutine(ShakeAndFade(bug.gameObject)));
    }
    
    
    foreach (var bug in allEnemyBugs)
    { 
        if (bug.Name == "战斗虫A")
        {
            if (FightDataManager.ActionPoints == 1)
            {
                bug.insectHP = 2;
                bug.insectAtk = 2;
                UpdateBugUI(bug);
                yield break;
            }
            else
            {
                bug.insectHP = 1;
                bug.insectAtk = 1;
                UpdateBugUI(bug);
                yield break;
            }

        }
        
        if (bug != null && bug.insectHP <= 0 && bug.gameObject.activeSelf&&bug.Name != "战斗虫A")
            deathRoutines.Add(StartCoroutine(ShakeAndFade(bug.gameObject)));
        
    }
    foreach (var routine in deathRoutines) yield return routine;
}

/// <summary>
/// 更新虫子头顶的 UI 文字（HP 和 ATK）
/// </summary>
private void UpdateBugUI(InsectData bug)
{
    // 尝试在虫子的子物体中查找 TextMeshProUGUI 组件
    TextMeshProUGUI tmp = bug.GetComponentInChildren<TextMeshProUGUI>();
    if (tmp != null)
    {
        tmp.text = $"{bug.insectHP}\n\n\n{bug.insectAtk}";
       
    }
    else
    {
        
    }
}

    private IEnumerator ShakeAndFade(GameObject obj)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Vector3 originalPos = obj.transform.position;
        float shakeDuration = 0.3f;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-0.3f, 0.3f);
            float offsetY = Random.Range(-0.3f, 0.3f);
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