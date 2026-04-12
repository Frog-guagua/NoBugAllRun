using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using LitJson;
using System.IO;
//这是一个json持久化保存数据的小框架
//存：SaveDataWithJson（要保存数据，起个名子。）
//取：LoadDataWithJson<T>（你起的名字）
public enum JsonType
{
    JsonUtility,
    LitJson
    //没导入litjson库就用jsonutility
}
public class JsonManager 
{   //单例直接用
    private static JsonManager instance=new JsonManager();
    public static JsonManager Instance => instance;
    private JsonManager(){}

    public void SaveDataWithJson(Object data, string fileName, JsonType type = JsonType.LitJson)//
    //litjson可以直接存字典。
    {
        string path=Application.persistentDataPath+"/"+fileName+".json";
        string jsonStr = "";
        switch (type)
        {
            case JsonType.LitJson:
                jsonStr=JsonMapper.ToJson(data);
                break;
            case JsonType.JsonUtility:
              
                jsonStr=JsonUtility.ToJson(data);
                break;
        }
        File.WriteAllText(path,jsonStr);
    }

    public T LoadDataWithJson<T>(string fileName, JsonType type = JsonType.LitJson)
        where T:new()//泛型约束，下面要new这里保证他有无参构造
    {
        string path=Application.streamingAssetsPath+"/"+fileName+".json";//默认文件夹
        if (!File.Exists(path))//没东西就换
        {
            path=Application.persistentDataPath+"/"+fileName+".json";
        }

        if (!File.Exists(path))
        {
            return new T();//啥也没有就返回默认
        }
        string jsonStr=File.ReadAllText(path);
        T data=default(T);
        switch (type)
        {
            case JsonType.JsonUtility:
           data=JsonUtility.FromJson<T>(jsonStr);
                break;
            case JsonType.LitJson:
                data=JsonMapper.ToObject<T>(jsonStr);
                break;
        }
        return data;
    }
}