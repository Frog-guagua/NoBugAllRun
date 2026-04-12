using UnityEngine;

[CreateAssetMenu(fileName = "New Task", menuName = "Tasks/Task Data", order = 51)]
public class TaskDataSO : ScriptableObject
{
    public string Title;
    public string Content;
    
}