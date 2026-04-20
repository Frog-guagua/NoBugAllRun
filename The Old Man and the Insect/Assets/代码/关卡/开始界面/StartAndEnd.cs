using UnityEngine;
using UnityEngine.SceneManagement;

public class StartAndEnd : MonoBehaviour
{
     private string targetSceneName="HomeScene";
     public GameObject settingMenu;

     [SerializeField] AudioClip beginBGM;

     void Start()
     {
         AudioMgr.Instance.PlayBGM(beginBGM);
     }

    public void SwitchToScene()
    {
        AudioMgr.Instance.StopBGM();
       Transition.Instance.SwitchSceneWithFade(targetSceneName);
      
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    public void SettingMenuOn()
    {
        settingMenu.SetActive(true);
    }
    public void SettingMenuOff()
    {
        settingMenu.SetActive(false);
    }
}
