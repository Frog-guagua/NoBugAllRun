using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    //呱 ：  现在是数学time！！！！ 函数 曲线 都端上来吧
    //      其实是因为按曲线的数值变化 来做移动会更有活人感
    [Header("妙妙效果曲线")]
    [SerializeField] AnimationCurve curve;
    
    [Header("UI")]
    [SerializeField] TextMeshProUGUI text;
    //呱：暂时还没研究出来
    //[SerializeField] Image image;
    [SerializeField] GameObject backgrond; 
    
    [Header("提示框位置")]
    //呱：对不住了我直接手打坐标 这个是左上角
    private Vector2 hiddenPos = new Vector2(-501f,368f);

    [SerializeField] private Vector2 shownPos;

    private float duration = 1.5f;
    private RectTransform rectTransform;
    private bool isShowing = false;
    private Coroutine currentAnim = null;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        rectTransform.anchoredPosition = hiddenPos;
        gameObject.SetActive(false); 
        
        hiddenPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isShowing)
        {
            HideHint();
        }
    }

    // 呱：显示提示框  里面可以自定义文字（ds老师教我的）
    public void ShowHint(string content)
    {
        if (text != null) text.text = content;
        gameObject.SetActive(true);
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(AnimateTo(shownPos, true));
    }

    // 呱：隐藏提示框
    public void HideHint()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(AnimateTo(hiddenPos, false));
    }

    IEnumerator AnimateTo(Vector2 targetPos, bool setShowingFlag)
    {
        isShowing = setShowingFlag; 
        Vector2 startPos = rectTransform.anchoredPosition;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float curveValue = curve.Evaluate(t);
            Vector2 newPos = Vector2.Lerp(startPos, targetPos, curveValue);
            rectTransform.anchoredPosition = newPos;
            time += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
        currentAnim = null;

        
        if (!setShowingFlag)
        {
            gameObject.SetActive(false);
        }
    }
  
}
