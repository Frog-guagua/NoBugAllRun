using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 呱： 写在最前面 ——> 这个脚本挂载在遮幕上
//    哈基蛙你这家伙……又在执着于写 妙妙画面效果了吗
//    这个适用于想要实现 渐显/渐隐 取决你调用这其中的哪个函数咯hiahia

// 元神真好玩
// 我要玩洛克王国
// 怎么样能同时玩洛克王国和元神

// 考虑到遮罩ui时常使用image，这里加了一些重载，同时写了一个方法,切换场景直接调用即可。
public class Transition : MonoBehaviour
{
    public static Transition Instance { get; private set; }
    [SerializeField] private string persistentSceneNameOverride;
    private string persistentSceneName;
    private SpriteRenderer SR;
    private Image Img;
    private Coroutine switchRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SR = GetComponent<SpriteRenderer>();
        Img = GetComponent<Image>();
        persistentSceneName = !string.IsNullOrEmpty(persistentSceneNameOverride) ? persistentSceneNameOverride : 
            (Application.CanStreamedLevelBeLoaded("PersistantScene") ? "PersistantScene" : gameObject.scene.name);
        DontDestroyOnLoad(gameObject);
    }

    // 呱：这是渐显
    public void FadeIn(float duration)
    {
        StartCoroutine(Fade(0f, 1f, duration));
    }

    // 呱：这是渐隐
    public void FadeOut(float duration)
    {
        StartCoroutine(Fade(1f, 0f, duration));
    }

    public IEnumerator FadeIn(Image image, float duration)
    {
        yield return StartCoroutine(Fade(image, 0f, 1f, duration));
    }

    public IEnumerator FadeOut(Image image, float duration)
    {
        yield return StartCoroutine(Fade(image, 1f, 0f, duration));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        Color color = SR?.color ?? Img.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            color.a = alpha;
            if (SR != null)
            {
                SR.color = color;
            }
            else if (Img != null)
            {
                Img.color = color;
            }
            yield return null;
        }

        color.a = endAlpha;
        if (SR != null)
        {
            SR.color = color;
        }
        else if (Img != null)
        {
            Img.color = color;
        }
    }

    private IEnumerator Fade(Image image, float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        Color color = image.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            color.a = alpha;
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }

    /// <summary>
    /// 切换场景调用这个。传入目标场景的名字。这里默认场上只存在两个场景
    /// </summary>
    /// <param name="targetSceneName"></param>
    /// <param name="fadeInDuration"></param>
    /// <param name="fadeOutDuration"></param>
    public void SwitchSceneWithFade(string targetSceneName, float fadeInDuration = 0.6f, float fadeOutDuration = 0.6f)
    {   
       // AudioMgr.Instance.StopBGM();
        if (switchRoutine != null)
        {
            StopCoroutine(switchRoutine);
        }
        switchRoutine = StartCoroutine(SwitchSceneWithFadeRoutine(targetSceneName, fadeInDuration, fadeOutDuration));
    }

    private IEnumerator SwitchSceneWithFadeRoutine(string targetSceneName, float fadeInDuration, float fadeOutDuration)
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            yield break;
        }

        yield return StartCoroutine(FadeTo(1f, fadeInDuration));

        if (!SceneManager.GetSceneByName(targetSceneName).isLoaded)
        {
            var loadOp = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            yield return loadOp;
        }

        var loadedScene = SceneManager.GetSceneByName(targetSceneName);
        SceneManager.SetActiveScene(loadedScene);

        var scenesToUnload = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded && scene.name != persistentSceneName && scene.name != targetSceneName)
            {
                scenesToUnload.Add(scene);
            }
        }

        foreach (var scene in scenesToUnload)
        {
            yield return SceneManager.UnloadSceneAsync(scene);
        }

        yield return StartCoroutine(FadeTo(0f, fadeOutDuration));
        switchRoutine = null;
    }

    private IEnumerator FadeTo(float endAlpha, float duration)
    {
        if (SR != null)
        {
            yield return StartCoroutine(Fade(SR.color.a, endAlpha, duration));
        }
        else if (Img != null)
        {
            yield return StartCoroutine(Fade(Img, Img.color.a, endAlpha, duration));
        }
    }
}