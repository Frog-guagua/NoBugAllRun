using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleResover : MonoBehaviour
{
    [Header("格子管理器引用")]
    public GridManager gridManager;

    [Header("动画参数")]
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float moveDuration = 0.5f;

    private Vector3 offset = Vector3.zero; // 如果你的格子位置有偏移，在这里设置
    public bool Nobug=false;
    /// <summary>
    /// 战斗结算主协程（攻击 + 死亡 + 胜负 + 后排前移）
    /// </summary>
    public IEnumerator BattleResolve()
    {
        // 1. 获取双方前排虫子（索引 4-7 为我方前排，8-11 为敌方前排）
        List<InsectData> myFrontBugs = new List<InsectData>();
        List<InsectData> enemyFrontBugs = new List<InsectData>();

        for (int i = 4; i <= 7; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null)
                myFrontBugs.Add(GridManager.Grids[i].bugOnGrid.GetComponent<InsectData>());
        }
        for (int i = 8; i <= 11; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null)
            {
                Debug.Log(GridManager.Grids[i].bugOnGrid.name); 
                enemyFrontBugs.Add(GridManager.Grids[i].bugOnGrid.GetComponent<InsectData>());
            }
                
        }
           // 在计算伤害之前，播放攻击动画
        List<Coroutine> animRoutines = new List<Coroutine>();
        foreach (var bug in myFrontBugs)
            if (bug != null) animRoutines.Add(StartCoroutine(AttackAnimation(bug.gameObject, new Vector3(0, 0.5f, 0), 0.2f)));
        foreach (var bug in enemyFrontBugs)
            if (bug != null) animRoutines.Add(StartCoroutine(AttackAnimation(bug.gameObject, new Vector3(0, -0.5f, 0), 0.2f)));
        foreach (var routine in animRoutines) yield return routine;
        // 2. 计算伤害（射线锁定对手，确保一对一）
        Dictionary<InsectData, int> damageMap = new Dictionary<InsectData, int>();
        float rayDistance = 10f;
        LayerMask targetLayer = LayerMask.GetMask("Bug");

// 我方攻击敌方（向上射线，起点偏移避免自碰）
        foreach (var myBug in myFrontBugs)
        {
            if (myBug == null) continue;
            Vector2 origin = (Vector2)myBug.transform.position + Vector2.up * 0.8f;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayDistance, targetLayer);
            Debug.DrawRay(origin, Vector2.up * rayDistance, Color.red, 1f);
            if (hit.collider != null && hit.collider.gameObject != myBug.gameObject)
            {
                InsectData enemy = hit.collider.GetComponent<InsectData>();
                if (enemy != null)
                {
                    damageMap[enemy] = damageMap.GetValueOrDefault(enemy) + myBug.insectAtk;
                    Debug.Log($"{myBug.name} 击中 {enemy.name}");
                }
            }
            else
            {
                Debug.Log($"{myBug.name} 未击中，起点 {origin}，射线向上 {rayDistance}，命中物体：{(hit.collider ? hit.collider.name : "无")}");
            }
        }

// 敌方攻击我方（向下射线，起点偏移避免自碰）
        foreach (var enemyBug in enemyFrontBugs)
        {
            if (enemyBug == null) continue;
            Vector2 origin = (Vector2)enemyBug.transform.position + Vector2.down * 0.8f;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayDistance, targetLayer);
            Debug.DrawRay(origin, Vector2.down * rayDistance, Color.blue, 1f);
            if (hit.collider != null && hit.collider.gameObject != enemyBug.gameObject)
            {
                InsectData myBug = hit.collider.GetComponent<InsectData>();
                if (myBug != null)
                {
                    damageMap[myBug] = damageMap.GetValueOrDefault(myBug) + enemyBug.insectAtk;
                    Debug.Log($"{enemyBug.name} 击中 {myBug.name}");
                }
            }
            else
            {
                Debug.Log($"{enemyBug.name} 未击中，起点 {origin}，射线向下 {rayDistance}，命中物体：{(hit.collider ? hit.collider.name : "无")}");
            }
        }
        // 3. 统一扣血
        foreach (var kvp in damageMap)
        {
            kvp.Key.insectHP -= kvp.Value;
            Debug.Log($"{kvp.Key.name} 受到 {kvp.Value} 伤害，剩余 HP：{kvp.Key.insectHP}");
        }

        // 4. 刷新所有虫子头顶的 UI
        foreach (var bug in myFrontBugs) UpdateBugUI(bug);
        foreach (var bug in enemyFrontBugs) UpdateBugUI(bug);

        // 5. 处理死亡（摇晃消失）
        List<Coroutine> deathRoutines = new List<Coroutine>();
        for (int i = 4; i <= 7; i++)
        {
            GameObject bugObj = GridManager.Grids[i].bugOnGrid;
            if (bugObj != null)
            {
                
                InsectData bug = bugObj.GetComponent<InsectData>();
                if (bug != null && bug.insectHP <= 0)
                {
                    deathRoutines.Add(StartCoroutine(ShakeAndFade(bugObj)));
                    GridManager.Grids[i].bugOnGrid = null;
                    GridManager.Grids[i].isOccupied = false;
                }
                    
            }
        }
        for (int i = 8; i <= 11; i++)
        {
            GameObject bugObj = GridManager.Grids[i].bugOnGrid;
            if (bugObj != null)
            {
                InsectData bug = bugObj.GetComponent<InsectData>();
                if (bug != null && bug.insectHP <= 0)
                {
                    deathRoutines.Add(StartCoroutine(ShakeAndFade(bugObj)));
                    GridManager.Grids[i].bugOnGrid = null;
                    GridManager.Grids[i].isOccupied = false;
                }
                  
            }
        }
        foreach (var routine in deathRoutines) yield return routine;

        // 6. 胜负判定（检查所有格子 0-7 和 8-15）
        bool hasMyBug = false;
        bool hasEnemyBug = false;
        for (int i = 0; i <= 7; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null) { hasMyBug = true; break; }
        }
        for (int i = 8; i <= 15; i++)
        {
            if (GridManager.Grids[i].bugOnGrid != null) { hasEnemyBug = true; break; }
        }

        if (!hasMyBug)
        {
            Debug.Log("玩家失败！");
            Nobug = true;
            DataBroker.WinGame2 = false;
            yield break;
        }
        if (!hasEnemyBug)
        {
            Debug.Log("玩家胜利！");
            Nobug = true;
            DataBroker.WinGame2 = true;
            yield break;
        }

        // 7. 没有结束 → 后排前移（我方后排虫子向前排移动）
        for (int i = 0; i <= 3; i++)  // 后排索引 0-3
        {
            GameObject backBug = GridManager.Grids[i].bugOnGrid;
            if (backBug == null) continue;

            int frontIndex = i + 4;
            if (frontIndex < GridManager.Grids.Length && GridManager.Grids[frontIndex].bugOnGrid == null)
            {
                // 移动虫子到前排
                yield return StartCoroutine(MoveBug(backBug, GridManager.Grids[frontIndex].matchedPos + offset));
                // 更新格子占用
                GridManager.Grids[i].bugOnGrid = null;
                GridManager.Grids[i].isOccupied = false;
                GridManager.Grids[frontIndex].bugOnGrid = backBug;
                GridManager.Grids[frontIndex].isOccupied = true;
            }
        }
    }
    /// <summary>
    /// 让虫子向前冲一下再回来（攻击动画）
    /// </summary>
    /// <param name="bug">虫子物体</param>
    /// <param name="forwardOffset">向前移动的偏移量（例如 new Vector3(0, 0.5f, 0)）</param>
    /// <param name="duration">单程时间（总时间 = 2 * duration）</param>
    private IEnumerator AttackAnimation(GameObject bug, Vector3 forwardOffset, float duration)
    {
        // 临时禁用 FollowCage 避免干扰
        FollowCage follow = bug.GetComponent<FollowCage>();
        if (follow != null) follow.enabled = false;

        Vector3 startPos = bug.transform.position;
        Vector3 targetPos = startPos + forwardOffset;

        // 向前移动
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = moveCurve.Evaluate(elapsed / duration);
            bug.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        bug.transform.position = targetPos;

        // 向后返回
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = moveCurve.Evaluate(elapsed / duration);
            bug.transform.position = Vector3.Lerp(targetPos, startPos, t);
            yield return null;
        }
        bug.transform.position = startPos;

        if (follow != null) follow.enabled = true;
    }
    // 移动虫子的协程（平滑移动）
    private IEnumerator MoveBug(GameObject bug, Vector3 targetPos)
    {
        // 临时禁用 FollowCage 避免干扰
        FollowCage follow = bug.GetComponent<FollowCage>();
        if (follow != null) follow.enabled = false;

        Vector3 startPos = bug.transform.position;
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = moveCurve.Evaluate(elapsed / moveDuration);
            bug.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        bug.transform.position = targetPos;

        if (follow != null) follow.enabled = true;
    }

    // 摇晃消失（复用你的逻辑）
    private IEnumerator ShakeAndFade(GameObject obj)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Vector3 originalPos = obj.transform.position;
        float elapsed = 0f;
        float shakeDur = 0.3f;
        while (elapsed < shakeDur)
        {
            float offsetX = Random.Range(-0.3f, 0.3f);
            float offsetY = Random.Range(-0.3f, 0.3f);
            obj.transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = originalPos;

        Color color = sr.color;
        elapsed = 0f;
        float fadeDur = 0.5f;
        while (elapsed < fadeDur)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDur);
            color.a = alpha;
            sr.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.SetActive(false);
    }

    // 更新虫子头顶文字（复用你的方法）
    private void UpdateBugUI(InsectData bug)
    {
        if (bug == null) return;
        TextMeshProUGUI tmp = bug.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = $"{bug.insectHP}\n\n\n{bug.insectAtk}";
    }
}