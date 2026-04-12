using UnityEngine;
using UnityEngine.UI;

public class TaskWindow : MonoBehaviour
{
    private static TaskWindow _instance;

    public Text titleText;
    public Text contentText;
    public Button closeButton;

    private TaskDataSO currentTask;

   
    private TaskWindow() { }


    public static TaskWindow Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("TaskWindow instance is not initialized yet.");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 保证实例不会被销毁
        }

        // 确保窗口初始时被隐藏
        gameObject.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseWindow);
        }
        
    }

    public void Show(TaskDataSO task)
    {
        if (task == null ) return; // 如果任务已完成或为空，则不显示

        currentTask = task;
        titleText.text = task.Title;
        contentText.text = task.Content;
        gameObject.SetActive(true); // 显示窗口
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false); // 隐藏窗口
    }

    public void CompleteTask()
    {
        if (currentTask != null)
        {
            
            CloseWindow(); // 完成后关闭窗口
        }
    }
}
