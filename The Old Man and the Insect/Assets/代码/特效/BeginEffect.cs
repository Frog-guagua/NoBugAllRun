using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginEffect : MonoBehaviour
{
   
    public List<GameObject> objects = new List<GameObject>();

    [Header("渐显/渐隐时长")]
    public float fadeDuration = 0.5f;

    [Header("停留时间")]
    public float stayDuration = 1f;

    [Header("要切换的下一个场景名称")]
    public string nextSceneName = "NextScene";

    void Start()
    {
 
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;
            SetObjectAlpha(obj, 0f);
            obj.SetActive(true);   
        }
        StartCoroutine(PlaySequence());
        
       
    }

    IEnumerator PlaySequence()
    {
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            // 渐显
            yield return StartCoroutine(FadeObject(obj, 0f, 1f, fadeDuration));

            // 停留
            yield return new WaitForSeconds(stayDuration);

            // 渐隐
            yield return StartCoroutine(FadeObject(obj, 1f, 0f, fadeDuration));
        }

       
        Transition.Instance.SwitchSceneWithFade(nextSceneName);
    }

    IEnumerator FadeObject(GameObject obj, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        SetObjectAlpha(obj, startAlpha);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
            SetObjectAlpha(obj, newAlpha);
            yield return null;
        }

        SetObjectAlpha(obj, endAlpha);
    }


    void SetObjectAlpha(GameObject obj, float alpha)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
       
        Material mat = renderer.material;
    
    
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
        
      
    }
}