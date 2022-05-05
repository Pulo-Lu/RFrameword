using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class BuildTool
{
    private static string m_AppName = PlayerSettings.productName; //RealConfig.GetRealFram().m_AppName;

    public static string m_AndroidPath = Application.dataPath + "/../BuildTarget/Android/";
    public static string m_IOSPath = Application.dataPath + "/../BuildTarget/IOS/";
    public static string m_WindowsPath = Application.dataPath + "/../BuildTarget/Windows/";

    [MenuItem("Build/标准包")]
    public static void Build()
    {
        //打ab包
        BundleEditor.Build();

        //生成可执行程序
        string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
        Copy(abPath, Application.streamingAssetsPath);
        string savePath = "";

        if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            savePath = m_AndroidPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now) + ".apk";
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            savePath = m_IOSPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now);
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
            EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
        {
            savePath = m_WindowsPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}/{1}.exe",
                DateTime.Now, m_AppName);
        }

        BuildPipeline.BuildPlayer(FindEnableEditorScene(), savePath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);

        DeleteDir(Application.streamingAssetsPath);
    }

    /// <summary>
    /// 获取已经打开的场景
    /// </summary>
    /// <returns></returns>
    private static string[] FindEnableEditorScene()
    {
        List<string> editorScene = new List<string>();

        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
                continue;
            editorScene.Add(scene.path);
        }
        return editorScene.ToArray();
    }

    [MenuItem("Build/将AB包拷贝到 StreamingAssetsPath 目录")]
    public static void CopyAB()
    {
        string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
        Copy(abPath, Application.streamingAssetsPath);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 拷贝AB包
    /// </summary>
    /// <param name="srcPath"></param>
    /// <param name="targetPath"></param>
    private static void Copy(string srcPath, string targetPath)
    {
        try
        {
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            //原路径
            string scrdir = Path.Combine(targetPath, Path.GetFileName(srcPath));

            if (Directory.Exists(srcPath)) 
            {
                scrdir += Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(scrdir))
            {
                Directory.CreateDirectory(scrdir);
            }

            string[] files = Directory.GetFileSystemEntries(srcPath);

            foreach( string file in files)
            {
                if (Directory.Exists(file))
                {
                    Copy(file, scrdir);
                }
                else
                {
                    File.Copy(file, scrdir + Path.GetFileName(file), true);
                }
            }
        }
        catch
        {
            Debug.LogError("无法复制： " + srcPath + " 到" + targetPath);
        }
    }

    public static void DeleteDir(string srcPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
            foreach(FileSystemInfo info in fileInfo)
            {
                if(info is DirectoryInfo)
                {
                    DirectoryInfo subdir = new DirectoryInfo(info.FullName);
                    subdir.Delete(true);
                }
                else
                {
                    File.Delete(info.FullName);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
