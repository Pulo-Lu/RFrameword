using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OfflineDataEditor
{
    [MenuItem("Assets/生成离线数据")]
    public static void AssetCreatOfflineData()
    {
        GameObject[] objects = Selection.gameObjects;
        for(int i = 0; i < objects.Length; i++)
        {
            EditorUtility.DisplayCancelableProgressBar("添加离线数据", "正在修改：" + objects[i] + "......", 1.0f / objects.Length * i);
            CreatOfflineData(objects[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 创建离线数据脚本
    /// </summary>
    /// <param name="obj"></param>
    public static void CreatOfflineData(GameObject obj)
    {
        OfflineData offlineData = obj.GetComponent<OfflineData>();
        if(offlineData == null)
        {
            offlineData = obj.AddComponent<OfflineData>();
        }
        offlineData.BindData();
        
        //编辑器模式下保存Obj
        EditorUtility.SetDirty(obj);

        Debug.Log("修改了" + obj.name + " prefab!");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();

    }

    [MenuItem("Assets/生成UI离线数据")]
    public static void AssetCreatUIData()
    {
        GameObject[] objects = Selection.gameObjects;
        for (int i = 0; i < objects.Length; i++)
        {
            EditorUtility.DisplayCancelableProgressBar("添加UI离线数据", "正在修改：" + objects[i] + "......", 1.0f / objects.Length * i);
            CreatUIData(objects[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("离线数据/生成所有UI Prefab离线数据")]
    public static void AllCreatUIData()
    {
        //找到 m_AllPrefabPath 路径下的所有 Prefab, 返回的是GUID
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/GameData/Prefabs/UGUI" });

        for(int i = 0; i < allStr.Length; i++)
        {
            // GUIDToAssetPath ：根据GUID获取文件路径
            string PrefabPath = AssetDatabase.GUIDToAssetPath(allStr[i]);
            EditorUtility.DisplayCancelableProgressBar("添加UI离线数据", "正在扫描路径：" + PrefabPath + "......", 1.0f / allStr.Length * i);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (obj == null)
                continue;

            CreatUIData(obj);
        }
        Debug.Log("UI离线数据全部生成完毕！");
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 添加UI离线数据脚本
    /// </summary>
    /// <param name="obj"></param>
    public static void CreatUIData(GameObject obj)
    {
        obj.layer = LayerMask.NameToLayer("UI");

        UIOfflineData uiData = obj.GetComponent<UIOfflineData>();
        if (uiData == null)
        {
            uiData = obj.AddComponent<UIOfflineData>();
        }
        uiData.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改了" + obj.name + " UI prefab!");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }


    [MenuItem("Assets/生成特效离线数据")]
    public static void AssetCreatEffectData()
    {
        GameObject[] objects = Selection.gameObjects;
        for (int i = 0; i < objects.Length; i++)
        {
            EditorUtility.DisplayCancelableProgressBar("添加特效离线数据", "正在修改：" + objects[i] + "......", 1.0f / objects.Length * i);
            CreatEffectData(objects[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("离线数据/生成所有特效 Prefab离线数据")]
    public static void AllCreatEffectData()
    {
        //找到 m_AllPrefabPath 路径下的所有 Prefab, 返回的是GUID
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/GameData/Prefabs/Effect" });

        for (int i = 0; i < allStr.Length; i++)
        {
            // GUIDToAssetPath ：根据GUID获取文件路径
            string PrefabPath = AssetDatabase.GUIDToAssetPath(allStr[i]);
            EditorUtility.DisplayCancelableProgressBar("添加特效离线数据", "正在扫描路径：" + PrefabPath + "......", 1.0f / allStr.Length * i);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (obj == null)
                continue;

            CreatEffectData(obj);
        }
        Debug.Log("特效离线数据全部生成完毕！");
        EditorUtility.ClearProgressBar();
    }

    public static void CreatEffectData(GameObject obj)
    {
        EffectOfflineData effectData = obj.GetComponent<EffectOfflineData>();
        if (effectData == null)
        {
            effectData = obj.AddComponent<EffectOfflineData>();
        }

        effectData.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改了" + obj.name + " 特效 prefab!");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();

    }

}
