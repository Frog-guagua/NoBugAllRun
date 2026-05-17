using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fight1 : MonoBehaviour
{
    [Header("战斗虫身上的数据")]
    [SerializeField] List<InsectData> myData = new List<InsectData>();
    
    [SerializeField] List<GameObject> myFightBug= new List<GameObject>();
    [SerializeField] List<GameObject> enemyFightBug= new List<GameObject>();
    
    [SerializeField] List<TextMeshProUGUI> myFightBugDatas = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> enemyBugDatas = new List<TextMeshProUGUI>();

    [SerializeField] static GameObject bug;
    [SerializeField]  Sprite Level2;
 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public IEnumerator Round(int i)
    {

        if(i!= 1 && i!=2) yield break;
        if (i == 1)
        {
            StartCoroutine(InsectAnimationUtility.BattleMoveCoroutine(myFightBug[0], 0, 0.5f, 0.5f));
            StartCoroutine(InsectAnimationUtility.BattleMoveCoroutine(myFightBug[2], 0, 0.5f, 0.5f));
            StartCoroutine(InsectAnimationUtility.BattleMoveCoroutine(enemyFightBug[0], 1, 0.5f, 0.5f));
            yield return InsectAnimationUtility.BattleMoveCoroutine(enemyFightBug[1], 1, 0.5f, 0.5f);
            myFightBugDatas[0].text = "1\n\n1";
            myFightBugDatas[2].text = "1\n\n2";
            enemyBugDatas[0].text = "1\n\n1";
            enemyBugDatas[1].text = "0\n\n1";
            StartCoroutine(InsectAnimationUtility.DeathAnimationCoroutine(enemyFightBug[1]));

            myData[0].insectAtk = 1;
            myData[0].insectHP = 1;
            myData[2].insectAtk = 2;
            myData[2].insectHP = 1;

        }
        else if (i == 2)
        {
            myFightBug[0].GetComponent<SpriteRenderer>().sprite = Level2;
            StartCoroutine(InsectAnimationUtility.BattleMoveCoroutine(myFightBug[0], 0, 0.5f, 0.5f));
            StartCoroutine(InsectAnimationUtility.BattleMoveCoroutine(myFightBug[2], 0, 0.5f, 0.5f));
            StartCoroutine(InsectAnimationUtility.BattleMoveCoroutine(enemyFightBug[0], 1, 0.5f, 0.5f));
            yield return InsectAnimationUtility.BattleMoveCoroutine(enemyFightBug[2], 1, 0.5f, 0.5f);
            myFightBugDatas[0].text = "2\n\n2";
            myFightBugDatas[2].text = "0\n\n2";
            enemyBugDatas[0].text = "0\n\n1";
            enemyBugDatas[2].text = "0\n\n2";
            StartCoroutine(InsectAnimationUtility.DeathAnimationCoroutine(enemyFightBug[0]));
            StartCoroutine(InsectAnimationUtility.DeathAnimationCoroutine(enemyFightBug[2]));
            StartCoroutine(InsectAnimationUtility.DeathAnimationCoroutine(myFightBug[2]));
            
            myData[0].insectAtk = 2;
            myData[0].insectHP = 2;
            myData[0].insectLevel = 2;
            myData[0].bugType = E_BugType.A;
            myData[2].isDied = true;
            myData[2].insectAtk = 2;
            myData[2].insectHP = 2;
            myData[2].bugType = E_BugType.B;
            List<InsectData> datas = new List<InsectData>();
            datas.Add(myData[0]);
            datas.Add(myData[2]);
            DataBroker.Instance.give_datasFromFight(datas);
            
            
        }
    }
    
    
    
    
    public void ChangeLevelUpSprite(string levelUpSpriteName)
    {
        GameObject nowBug = myFightBug[0];
        SpriteRenderer sp = nowBug.GetComponent<SpriteRenderer>();
        Color color = Color.cyan;
        
        switch (levelUpSpriteName)
        {
            case "A":
                sp.sprite = Level2;
                break;
            case "B":
                // sp.sprite = levelUpSprite[1];
                sp.color = color;
                break;
            case "C":
                //sp.sprite = levelUpSprite[3];
                sp.color = color;
                break;
            case "D":
                // sp.sprite = levelUpSprite[4];
                sp.color = color;
                break;
            case "E":
                // sp.sprite = levelUpSprite[5];
                sp.color = color;
                break;
            case "F":
                //  sp.sprite = levelUpSprite[6];
                sp.color = color;
                break;
        }
    }
}





public static class InsectAnimationUtility
{
    /// <summary>
    /// 战斗动画：物体前后移动一次（先向指定方向移动，再返回原点）
    /// </summary>
    /// <param name="obj">要移动的物体</param>
    /// <param name="direction">移动方向：1 表示向 Y 负方向，0 表示向 Y 正方向</param>
    /// <param name="offsetDistance">移动距离（世界单位）</param>
    /// <param name="duration">单程移动时长（总动画时长 = duration * 2）</param>
    /// <param name="curve">移动曲线（可选，默认使用线性）</param>
    /// <returns>协程</returns>
    public static IEnumerator BattleMoveCoroutine(GameObject obj, int direction, float offsetDistance = 0.5f, float duration = 0.2f, AnimationCurve curve = null)
    {
        if (obj == null) yield break;

        Vector3 startPos = obj.transform.position;
        Vector3 targetPos = startPos;
        if (direction == 1)
            targetPos.y -= offsetDistance;   // 向 Y 负方向
        else
            targetPos.y += offsetDistance;   // 向 Y 正方向

        // 如果没有提供曲线，使用默认线性
        if (curve == null)
            curve = AnimationCurve.Linear(0, 0, 1, 1);

        float elapsed = 0f;
        float halfDuration = duration / 2f;

        // 向前移动
        while (elapsed < halfDuration)
        {
            float t = curve.Evaluate(elapsed / halfDuration);
            obj.transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = targetPos;

        elapsed = 0f;
        // 返回原点
        while (elapsed < halfDuration)
        {
            float t = curve.Evaluate(elapsed / halfDuration);
            obj.transform.position = Vector3.Lerp(targetPos, startPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = startPos;
    }

    /// <summary>
    /// 死亡动画：摇晃 + 淡出，最后禁用物体
    /// </summary>
    /// <param name="obj">要执行死亡动画的物体（必须有 SpriteRenderer）</param>
    /// <param name="shakeStrength">摇晃强度（随机偏移范围）</param>
    /// <param name="shakeDuration">摇晃持续时间</param>
    /// <param name="fadeDuration">淡出持续时间</param>
    /// <returns>协程</returns>
    public static IEnumerator DeathAnimationCoroutine(GameObject obj, float shakeStrength = 0.3f, float shakeDuration = 0.3f, float fadeDuration = 0.5f)
    {
        if (obj == null) yield break;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Vector3 originalPos = obj.transform.position;
        float elapsed = 0f;

        // 摇晃阶段
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-shakeStrength, shakeStrength);
            float offsetY = Random.Range(-shakeStrength, shakeStrength);
            obj.transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = originalPos;

        // 淡出阶段
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

        // 最终禁用物体
        obj.SetActive(false);
    }
}