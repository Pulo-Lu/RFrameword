using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameMapManager : Singleton<GameMapManager>
{
    //加载场景完成回调
    public Action LoadSceneOverCallBack;

    //加载场景开始回调
    public Action LoadSceneEnterCallBack;

    //当前场景名
    public string CurrentMapName { get; set; }

    //当前场景是否完成
    public bool AlreadyLoadScene { get; set; }

    //切换场景的进度条
    public static int LoadingProgress = 0;

    public MonoBehaviour m_Mono;

    /// <summary>
    /// 场景管理初始化
    /// </summary>
    /// <param name="mono"></param>
    public void Init(MonoBehaviour mono)
    {
        m_Mono = mono;
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="name">场景名</param>
    public void LoadScene(string name)
    {
        LoadingProgress = 0;
        m_Mono.StartCoroutine(LoadSceneAsync(name));
        UIManager.Instance.PopUpWnd(ConStr.LOADINGPANEL, true, name);
    } 

    /// <summary>
    /// 设置场景环境
    /// </summary>
    /// <param name="name"></param>
    void SetSceneSetting(string name)
    {
        //设置各种场景环境，可以根据配置表来 

        //TODO
    }

    IEnumerator LoadSceneAsync(string name)
    {
        if (LoadSceneEnterCallBack != null)
        {
            LoadSceneEnterCallBack();
        }

        ClearCache();
        AlreadyLoadScene = false;
        //加载空场景
        AsyncOperation unLoadScene = SceneManager.LoadSceneAsync(ConStr.MEPTYSCNEN, LoadSceneMode.Single);

        while(unLoadScene != null && !unLoadScene.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        LoadingProgress = 0;

        int targetProgress = 0;
        //加载要加载的场景
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(name);

        if(asyncScene!=null && !asyncScene.isDone)
        {
            //设置场景可见（false）
            asyncScene.allowSceneActivation = false;

            //异步加载小于90%
            while (asyncScene.progress < 0.9f)
            {
                //异步加载进度条
                targetProgress = (int)asyncScene.progress * 100;
                yield return new WaitForEndOfFrame();
                //平滑过渡
                while (LoadingProgress < targetProgress)
                {
                    ++LoadingProgress;
                    yield return new WaitForEndOfFrame();
                }
            }

            CurrentMapName = name;
            SetSceneSetting(name);

            //自行加载剩余10%
            targetProgress = 100;
            while (LoadingProgress < targetProgress - 2) 
            {
                ++LoadingProgress;
                yield return new WaitForEndOfFrame();
            }
            LoadingProgress = 100;

            //设置场景可见（true） 允许场景显示
            asyncScene.allowSceneActivation = true;
            AlreadyLoadScene = true;

            if (LoadSceneOverCallBack != null)
            {
                LoadSceneEnterCallBack();
            }
        }
    }

    /// <summary>
    /// 跳场景需要清除的东西
    /// </summary>
    private void ClearCache()
    {
        ObjectManager.Instance.ClearCache();
        ResourceManager.Instance.ClearCache();
        Debug.Log("清空缓存");
    }
}
