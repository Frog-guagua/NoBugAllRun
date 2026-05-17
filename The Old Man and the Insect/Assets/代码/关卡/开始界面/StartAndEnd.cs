using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartAndEnd : MonoBehaviour
{
     private string targetSceneName="HomeScene";
     public GameObject settingMenu;
     
     [SerializeField] AudioClip beginBGM;
     [SerializeField] AudioClip clickSFX;
     private bool haveAnim = false;

     private Image img;

     void Update()
     {
         if (haveAnim) return;
         if (MoveAnime.FinishAnim)
         {
             StartCoroutine(FadeInRoutine());
             haveAnim = true;
         }
         
     }
     
     void Start()
     {
         img = GetComponent<Image>();
         Color newColor = img.color;
         newColor.a = 0f; 
         img.color = newColor;
         AudioMgr.Instance.PlayBGM(beginBGM);
     }
 
    public void SwitchToScene()
    {
        AudioMgr.Instance.PlaySFX(clickSFX);
        AudioMgr.Instance.StopBGM();
       Transition.Instance.SwitchSceneWithFade(targetSceneName);
      
    }

    public void QuitGame()
    {
        AudioMgr.Instance.PlaySFX(clickSFX);
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    public void SettingMenuOn()
    {
        AudioMgr.Instance.PlaySFX(clickSFX);
        settingMenu.SetActive(true);
    }
    public void SettingMenuOff()
    {
        AudioMgr.Instance.PlaySFX(clickSFX);
        settingMenu.SetActive(false);
    }

    IEnumerator FadeInRoutine()
    {
        float duration = 1f;   // 渐显时长
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;  // 0 → 1
            Color c = img.color;
            c.a = Mathf.Lerp(0f, 1f, t);
            img.color = c;
            yield return null;
        }

        // 确保最终完全不透明
        Color final = img.color;
        final.a = 1f;
        img.color = final;
    }
}
