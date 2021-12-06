using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class BundleEditor
{
    //[MenuItem("Tools/打包")]
    /// <summary>
    /// 默认方法打AB包
    /// </summary>
    /*public static void Build()
    {
        //  BuildAssetBundles 参数
        //  第一参数为将资源打包到的目标路径
        //  第二参数为 Bundle 的压缩格式
        //  第三参数为 打包的平台

        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression,
            EditorUserBuildSettings.activeBuildTarget);

        //编辑器刷新
        AssetDatabase.Refresh();
    }*/

    //AB包位置
    private static string m_BundleTargetPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString();

    private static string ABCONFIGPATH = "Assets/RealFram.Editor/Editor/Resource/ABConfig.asset";

    private static string ABBYTEPATH = "Assets/GameData/Data/ABData/AssetBundleConfig.bytes";


    //Key是ab包名，value是路径，是所有文件夹ab包dic
    private static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();

    //过滤的List  
    private static List<string> m_AllFileAB = new List<string>();

    //单个prefab的AB包
    private static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();

    //储存所有有效路径
    private static List<string> m_ConfigFil = new List<string>();


    /// <summary>
    /// 基于Assets序列化生成编辑器打包配置表 方法打AB包
    /// </summary>
    [MenuItem("Tools/打包")]
    public static void Build()
    {
        //打包前将Xml转成二进制
        DataEditor.AllXmlToBinary();

        m_ConfigFil.Clear();
        m_AllFileAB.Clear();
        m_AllFileDir.Clear();      
        m_AllPrefabDir.Clear(); //每次运行都要清除
        //加载配置表
        ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);

        //根据文件夹打包
        foreach (ABConfig.FileDirABName fileDir in abConfig.m_AllFileDirAB)
        {
            if (m_AllFileDir.ContainsKey(fileDir.ABName))
            {
                Debug.LogError("AB包配置名字重复，请检查！");
            }
            else
            {
                m_AllFileDir.Add(fileDir.ABName, fileDir.Path);
                m_AllFileAB.Add(fileDir.Path);
                m_ConfigFil.Add(fileDir.Path);
            }
        }

        //根据文件打包
        //找到 m_AllPrefabPath 路径下的所有 Prefab, 返回的是GUID
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", abConfig.m_AllPrefabPath.ToArray());

        for (int i = 0; i < allStr.Length; i++) 
        {
            // GUIDToAssetPath ：根据GUID获取文件路径
            string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
            //进度条
            EditorUtility.DisplayCancelableProgressBar("查找Prefab", "Prefab:" + path, i * 1.0f / allStr.Length);
            m_ConfigFil.Add(path);

            if (!ContainAllFileAB(path))
            {
                //加载资源
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                //获取依赖关系
                string[] allDepend = AssetDatabase.GetDependencies(path);
                //临时的List
                List<string> allDependPath = new List<string>();

                for(int j = 0; j < allDepend.Length; j++)
                {
                    //Debug.Log(allDepend[j]);
                    //如果 allDepend[j] 不在AB包里，且不是C#脚本,则加入AB包
                    if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))
                    {
                        //加入到过滤的List 
                        m_AllFileAB.Add(allDepend[j]);
                        //加入到临时的List 目的：把 Prefab 的相关项打包到一个AB包里
                        allDependPath.Add(allDepend[j]);
                    }
                }
                if (m_AllPrefabDir.ContainsKey(obj.name))
                {
                    Debug.LogError("存在相同名字的Prefab！" + obj.name);
                }
                else
                {
                    //把 Prefab 的相关项打包到一个AB包里，名字由Prefab决定
                    m_AllPrefabDir.Add(obj.name, allDependPath);
                }
            }
        }

        //设置所有文件夹AB包名
        foreach (string name in m_AllFileDir.Keys)
        {
            SetABName(name, m_AllFileDir[name]);
        }
        //设置所有文件AB包名
        foreach (string name in m_AllPrefabDir.Keys)
        {
            SetABName(name, m_AllPrefabDir[name]);
        }

        //打包AB包
        BunildAssetBundle();

        //清除AB包名
        string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
        for(int i = 0; i  < oldABNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
            EditorUtility.DisplayCancelableProgressBar("清除AB包名", "名字：" + oldABNames[i], i * 1.0f / oldABNames.Length);
        }

        //刷新编辑器
        AssetDatabase.Refresh();
        //清除进度条 
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 设置AB包
    /// </summary>
    /// <param name="name">AB包名</param>
    /// <param name="path">文件路径</param>
    static void SetABName(string name,string path)
    {
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if(assetImporter == null)
        {
            Debug.LogError("不存在此路径文件：" + path);
        }
        else
        {
            assetImporter.assetBundleName = name;
        }
    }

    /// <summary>
    /// 设置AB包(重载)
    /// </summary>
    /// <param name="name">AB包名</param>
    /// <param name="paths">文件夹路径</param>
    static void SetABName(string name,List<string> paths)
    {
        for(int i  = 0;i< paths.Count; i++)
        {
            SetABName(name, paths[i]);
        }
    }


    // 打包AB包
    static void BunildAssetBundle()
    {
        string[] allBundles = AssetDatabase.GetAllAssetBundleNames();

        //Key是全路径，value是ab包名字，
        Dictionary<string, string> resPathDic = new Dictionary<string, string>();

        for (int i = 0; i < allBundles.Length; i++) 
        {
            //根据名字获取路径
            string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundles[i]);
            for (int j = 0; j < allBundlePath.Length; j++)
            {
                if (allBundlePath[j].EndsWith(".cs"))
                {
                    continue;
                }
                Debug.Log("此AB包： " + allBundles[i] + " 下面包含的资源文件路径： " + allBundlePath[j]);
                if (ValidPath(allBundlePath[j]))
                {
                    resPathDic.Add(allBundlePath[j], allBundles[i]);
                }
            }
        }

        if (!Directory.Exists(m_BundleTargetPath))
        {
            Directory.CreateDirectory(m_BundleTargetPath);
        }

        //删除无用AB包
        DeleteAb();

        //生成自定义配置表
        WriteData(resPathDic);

        //生成AB包
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(m_BundleTargetPath, BuildAssetBundleOptions.ChunkBasedCompression,
            EditorUserBuildSettings.activeBuildTarget);

        if(manifest == null)
        {
            Debug.LogError("AssetBunle 打包失败！");
        }
        else
        {
            Debug.Log("AssetBunle 打包成功！");
        }
    }

    static void WriteData(Dictionary<string, string> resPathDic)
    {
        AssetBundleConfig config = new AssetBundleConfig();
        config.ABList = new List<ABBase>();
        foreach (string path in resPathDic.Keys)
        {
            ABBase abBase = new ABBase();
            abBase.Path = path;
            abBase.Crc = Crc32.GetCrc32(path);
            abBase.ABName = resPathDic[path];
            abBase.AssetName = path.Remove(0, path.LastIndexOf("/") + 1);

            abBase.ABDependce = new List<string>();
            string[] resDependce = AssetDatabase.GetDependencies(path);
            for (int i = 0; i < resDependce.Length; i++)
            {
                string tempPath = resDependce[i];
                if (tempPath == path || path.EndsWith(".cs"))
                    continue;

                string abName = "";
                if (resPathDic.TryGetValue(tempPath, out abName))
                {
                    if (abName == resPathDic[path])
                        continue;

                    if (!abBase.ABDependce.Contains(abName))
                    {
                        abBase.ABDependce.Add(abName);
                    }
                }
            }
            config.ABList.Add(abBase);
        }

        //写入XML
        string xmlPath = Application.dataPath + "/AssetbundleConfig.xml";
        if (File.Exists(xmlPath)) File.Delete(xmlPath);
        FileStream fileStream = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
        XmlSerializer xs = new XmlSerializer(config.GetType());
        xs.Serialize(sw, config);
        sw.Close();
        fileStream.Close();

        //写入二进制
        foreach (ABBase abBase in config.ABList)
        {
            abBase.Path = "";
        }
 
        FileStream fs = new FileStream(ABBYTEPATH, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        fs.Seek(0, SeekOrigin.Begin);
        fs.SetLength(0);
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(fs, config);
        fs.Close();

        AssetDatabase.Refresh();
        SetABName("assetbundleconfig", ABBYTEPATH);
    
    }

    // 删除无用AB包（改名或者没有使用）
    static void DeleteAb()
    {
        //获取所有AB包的名字
        string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames();
        //获取指定目录
        DirectoryInfo direction = new DirectoryInfo(m_BundleTargetPath);
        //从文件夹获取所有文件
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

        for(int i = 0; i < files.Length; i++)
        {
            if (ContainABName(files[i].Name, allBundlesName) || files[i].Name.EndsWith(".meta")|| files[i].Name.EndsWith(".manifest")
                || files[i].Name.EndsWith("assetbundleconfig"))
            {
                continue;
            }
            else
            {
                Debug.Log("此AB包已经被删除或者改名： " + files[i].Name);
                //如果存在，删除
                if (File.Exists(files[i].FullName))
                {
                    File.Delete(files[i].FullName);
                }
            }
        }
    }

    /// <summary>
    /// 遍历文件夹里的文件名与设置的所有AB包进行判断
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strs"></param>
    /// <returns></returns>
    static bool ContainABName(string name,string[] strs)
    {
        for(int i = 0; i < strs.Length; i++)
        {
            if (name == strs[i]) 
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 是否包含在已经有的AB包里，来做AB包冗余剔除
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool ContainAllFileAB(string path)
    {
        for(int i = 0; i < m_AllFileAB.Count; i++)
        {
            if (path == m_AllFileAB[i] || (path.Contains(m_AllFileAB[i]) && (path.Replace(m_AllFileAB[i], "")[0] == '/'))) 
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 是否有效路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool ValidPath(string path)
    {
        for(int i = 0;i< m_ConfigFil.Count; i++)
        {
            if (path.Contains(m_ConfigFil[i]))
            {
                return true;
            }
        }
        return false;
    }
}
