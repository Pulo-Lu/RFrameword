using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestEditor
{
    private static Sprite t;

    [MenuItem("Tools/测试加载")]
    public static void TestLoad()
    {
        t = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/GameData/UGUI/test1.png");
    }

    [MenuItem("Tools/测试卸载")]
    public static void TestUnLoad()
    {
        Resources.UnloadAsset(t);
        t = null;
    }
}
