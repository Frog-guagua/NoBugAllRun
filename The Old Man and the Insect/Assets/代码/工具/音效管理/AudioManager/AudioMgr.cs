// AudioMgr.cs
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    private static AudioMgr _instance;
    
    // BGM AudioSource
    public AudioSource bgmSource;
    // 音效 AudioSource
    public AudioSource sfxSource;

    /// <summary>
    /// 获取或创建单例实例。
    /// </summary>
    public static AudioMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioMgr>();
                if (_instance == null)
                {
                    GameObject audioObj = new GameObject("Audio Manager");
                    _instance = audioObj.AddComponent<AudioMgr>();
                    DontDestroyOnLoad(audioObj); // 切换场景时不要销毁
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        // 如果已经有实例存在，则销毁当前实例以避免重复
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 确保在不同场景间持久化
        }
    }
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource != null && clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
            bgmSource.loop = true;
        }
    }
    
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    //呱：加了一个循环音效功能
    public void LoopSFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.clip = clip;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }
    
    //呱：加入了停止音效的功能！
    public void StopSFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = null;
        }
    }
    
    
    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = Mathf.Clamp01(volume); // 限制音量在0-1之间
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(volume); // 限制音量在0-1之间
        }
    }
}
