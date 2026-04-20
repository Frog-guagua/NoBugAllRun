using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//呱神 强
//呱神 强
//呱神 强
//呱神 强
//呱神 强
//呱神 强
//呱神 强
//呱神 强
//呱神 强
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
   public Vector2 hiddenPos ;//不同情况会有差别
    //我们直接手动填

   
   public Vector2 shownPos;

    [Header("位置参考(可选)")]
    [SerializeField] RectTransform hiddenPosTarget;
    [SerializeField] RectTransform shownPosTarget;

    private float duration = 1f;
    private RectTransform rectTransform;
    private bool isShowing = false;
    private Coroutine currentAnim = null;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        UpdatePositionsFromTargets();
        rectTransform.anchoredPosition = hiddenPos;
        gameObject.SetActive(false); 
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
        UpdatePositionsFromTargets();
        if (text != null) text.text = content;
        gameObject.SetActive(true);
        StartCoroutine(RefreshAndAnimate());
    }

    IEnumerator RefreshAndAnimate()
    {
        // 先让文字更新
        yield return null;
        // 强制刷新所有 Canvas 布局
        Canvas.ForceUpdateCanvases();
        // 再强制重建背景框的布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(backgrond.GetComponent<RectTransform>());
        // 再等一帧，确保布局稳定
        yield return null;
    
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(AnimateTo(shownPos, true));
    }
    // 呱：隐藏提示框
    public void HideHint()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(AnimateTo(hiddenPos, false));
    }
    public IEnumerator WaitForClose()
    {
        while (gameObject.activeSelf) yield return null;
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

    private void UpdatePositionsFromTargets()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        var parentRect = rectTransform.parent as RectTransform;
        if (parentRect == null) return;

        var canvas = GetComponentInParent<Canvas>();
        Camera cam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) cam = canvas.worldCamera;

        var hiddenPosIsExternal = hiddenPos.sqrMagnitude > 0.0001f;
        var shownPosIsExternal = shownPos.sqrMagnitude > 0.0001f;

        if (!hiddenPosIsExternal && hiddenPosTarget != null && TryGetAnchoredPositionInParent(parentRect, cam, hiddenPosTarget.position, out var hp))
            hiddenPos = hp;

        if (!shownPosIsExternal && shownPosTarget != null && TryGetAnchoredPositionInParent(parentRect, cam, shownPosTarget.position, out var sp))
            shownPos = sp;
    }

    private bool TryGetAnchoredPositionInParent(RectTransform parentRect, Camera cam, Vector3 worldPos, out Vector2 anchoredPos)
    {
        anchoredPos = default;
        var screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, cam, out var localPoint)) return false;

        var anchorRef = (rectTransform.anchorMin + rectTransform.anchorMax) * 0.5f;
        var parentSize = parentRect.rect.size;
        var parentPivot = parentRect.pivot;
        var anchorPosInParent = new Vector2(
            (anchorRef.x - parentPivot.x) * parentSize.x,
            (anchorRef.y - parentPivot.y) * parentSize.y
        );

        anchoredPos = localPoint - anchorPosInParent;
        return true;
    }
   
  
}
