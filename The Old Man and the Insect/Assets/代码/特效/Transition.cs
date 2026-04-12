using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//呱： 写在最前面 ——> 这个脚本挂载在遮幕上
//    哈基蛙你这家伙……又在执着于写 妙妙画面效果了吗
//    这个适用于想要实现 渐显/渐隐 取决你调用这其中的哪个函数咯hiahia



//元神真好玩
//我要玩洛克王国
//怎么样能同时玩洛克王国和元神

//考虑到遮罩ui时常使用image，这里加了一些重载，同时写了一个方法,切换场景直接调用即可。
public class Transition : MonoBehaviour
{
    public static Transition Instance { get; private set; }
    private string persistentSceneName;
    private SpriteRenderer SR;
    private Image Img;


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
        persistentSceneName = gameObject.scene.name;
        DontDestroyOnLoad(gameObject);
    }

   //呱：这是渐显
    public void FadeIn(float duration)
    {
        if (SR != null)
        {
            StartCoroutine(Fade(0f, 1f, duration));
            return;
        }

        if (Img != null)
        {
            StartCoroutine(Fade(Img, 0f, 1f, duration));
        }
    }
    
    //呱：这是渐隐
    public void FadeOut(float duration)
    {
        if (SR != null)
        {
            StartCoroutine(Fade(1f, 0f, duration));
            return;
        }

        if (Img != null)
        {
            StartCoroutine(Fade(Img, 1f, 0f, duration));
        }
    }

    public void FadeIn(Image image, float duration)
    {
        StartCoroutine(Fade(image, 0f, 1f, duration));
    }

    public void FadeOut(Image image, float duration)
    {
        StartCoroutine(Fade(image, 1f, 0f, duration));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        Color color = SR.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            color.a = alpha;
            SR.color = color;
            yield return null;
        }

        color.a = endAlpha;
        SR.color = color;
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
/// <summary>
/// 切换场景调用这个。传入目标场景的名字。这里默认场上只存在两个场景
/// </summary>
/// <param name="targetSceneName"></param>
/// <param name="fadeInDuration"></param>
/// <param name="fadeOutDuration"></param>
    public void SwitchSceneWithFade
        (string targetSceneName, float fadeInDuration = 0.35f, float fadeOutDuration = 0.35f)
    {
        StartCoroutine(SwitchSceneWithFadeRoutine(targetSceneName, fadeInDuration, fadeOutDuration));
    }

    private IEnumerator SwitchSceneWithFadeRoutine
        (string targetSceneName, float fadeInDuration, float fadeOutDuration)
    {
        Scene oldScene = SceneManager.GetSceneAt(0);
        if (oldScene.name == persistentSceneName && SceneManager.sceneCount > 1)
        {
            oldScene = SceneManager.GetSceneAt(1);
        }//d老师真厉害轻而易举就帮我拿到了要卸载的场景
        //默认场上只有两个场景

        yield return StartCoroutine(FadeTo(1f, fadeInDuration));

        if (!SceneManager.GetSceneByName(targetSceneName).isLoaded)
        {
            var loadOp = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            yield return loadOp;
        }

        var loadedScene = SceneManager.GetSceneByName(targetSceneName);
        SceneManager.SetActiveScene(loadedScene);//它的意思就是： 目标场景加载完后，把“当前主场景”切到目标场景 ，这样后续逻辑（生成物体、运行关卡等）都以目标场景为准
        //老师太强了
        //严肃学习
        if (oldScene.isLoaded && oldScene.name != persistentSceneName && oldScene.name != targetSceneName)
        {
            yield return SceneManager.UnloadSceneAsync(oldScene);
        }

        yield return StartCoroutine(FadeTo(0f, fadeOutDuration));
    }
}
