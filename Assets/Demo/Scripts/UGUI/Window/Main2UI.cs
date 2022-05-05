using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main2UI : Window
{
    private Main2Panel m_Main2Panel;

    private AudioClip clip;

    List<GameObject> objects = new List<GameObject>();

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    public override void Awake(params object[] paralist)
    {
        m_Main2Panel = GameObject.GetComponent<Main2Panel>();
        AddButtonListener(m_Main2Panel.btn1, OnClickBtn1);
        AddButtonListener(m_Main2Panel.btn1_1, OnClickBtn1_1);
        AddButtonListener(m_Main2Panel.btn1_2, OnClickBtn1_2);
        AddButtonListener(m_Main2Panel.btn2, OnClickBtn2);   
        AddButtonListener(m_Main2Panel.btn3, OnClickBtn3);        
        AddButtonListener(m_Main2Panel.btn4, OnClickBtn4);  
        AddButtonListener(m_Main2Panel.btn5, OnClickBtn5);
        AddButtonListener(m_Main2Panel.btn6, OnClickBtn6);
        AddButtonListener(m_Main2Panel.Exit, OnClickExit);
    }

    void OnClickBtn1()
    {
        sw.Reset();
        sw.Start();
        ResourceManager.Instance.AsyncLoadResource("Assets/GameData/UGUI/test1.png", OnLoadTest1Finish, LoadResPriority.RES_HIGHT,true);
        ResourceManager.Instance.AsyncLoadResource("Assets/GameData/UGUI/test2.png", OnLoadTest2Finish, LoadResPriority.RES_SLOW, true);
        ResourceManager.Instance.AsyncLoadResource("Assets/GameData/UGUI/test3.png", OnLoadTest3Finish, LoadResPriority.RES_SLOW, true);
        ResourceManager.Instance.AsyncLoadResource("Assets/GameData/Sounds/senlin.mp3",OnLoadFinish,LoadResPriority.RES_MIDDLE);
        sw.Stop();
        Debug.Log("异步加载资源消耗时间： " + sw.ElapsedMilliseconds + " ms");
     
    }

    private void OnLoadTest1Finish(string path, UnityEngine.Object obj, object param1, object param2, object param3)
    {
        if (obj != null)
        {
            Sprite sp = obj as Sprite;
            m_Main2Panel.Image1.sprite = sp;
            Debug.Log("图片一加载出来了");
        }
    }

    private void OnLoadTest2Finish(string path, UnityEngine.Object obj, object param1, object param2, object param3)
    {
        if (obj != null)
        {
            Sprite sp = obj as Sprite;
            m_Main2Panel.Image2.sprite = sp;
            Debug.Log("图片二加载出来了");
        }
    }

    private void OnLoadTest3Finish(string path, UnityEngine.Object obj, object param1, object param2, object param3)
    {
        if (obj != null)
        {
            Sprite sp = obj as Sprite;
            m_Main2Panel.Image3.sprite = sp;
            Debug.Log("图片三加载出来了");
        }
    }

    private void OnLoadFinish(string path, UnityEngine.Object obj, object param1, object param2, object param3)
    {
        if (obj != null)
        {
            clip = obj as AudioClip;
            m_Main2Panel.m_Audio.clip = clip;
            m_Main2Panel.m_Audio.Play();
            Debug.Log("声音加载出来了");
        }
    }


    void OnClickBtn1_1()
    {
        m_Main2Panel.m_Audio.Stop();
        m_Main2Panel.m_Audio.clip = null;
        ResourceManager.Instance.ReleaseResrouce(clip, true);
        clip = null;
        
        ResourceManager.Instance.ReleaseResrouce(m_Main2Panel.Image1.sprite, true);
        m_Main2Panel.Image1.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(m_Main2Panel.Image2.sprite, true);
        m_Main2Panel.Image2.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(m_Main2Panel.Image3.sprite, true);
        m_Main2Panel.Image3.sprite = null;
    }

    void OnClickBtn1_2()
    {
        m_Main2Panel.m_Audio.Stop();
        m_Main2Panel.m_Audio.clip = null;
        ResourceManager.Instance.ReleaseResrouce(clip);
        clip = null;
        ResourceManager.Instance.ReleaseResrouce(m_Main2Panel.Image1.sprite);
        m_Main2Panel.Image1.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(m_Main2Panel.Image2.sprite);
        m_Main2Panel.Image2.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(m_Main2Panel.Image3.sprite);
        m_Main2Panel.Image3.sprite = null;
    }

    void OnClickBtn2()
    {
        sw.Reset();
        sw.Start();
        ObjectManager.Instance.InstantiateObjectAsync("Assets/GameData/Prefabs/Attack.prefab", OnObjLoadFinish,LoadResPriority.RES_MIDDLE, true);
		ObjectManager.Instance.InstantiateObjectAsync("Assets/GameData/Prefabs/Attack.prefab", OnObjLoadFinish,LoadResPriority.RES_MIDDLE, true);
		ObjectManager.Instance.InstantiateObjectAsync("Assets/GameData/Prefabs/Attack.prefab", OnObjLoadFinish,LoadResPriority.RES_MIDDLE, true);
        sw.Stop();
        Debug.Log("异步加载对象消耗时间： " + sw.ElapsedMilliseconds + " ms");
    }

    private void OnObjLoadFinish(string path, UnityEngine.Object obj, object param1, object param2, object param3)
    {
        GameObject Obj = obj as GameObject;
        objects.Add(Obj);
    }



    void OnClickBtn3()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject obj = objects[i];
            ObjectManager.Instance.ReleaseObject(obj);
            obj = null;
        }
        objects.Clear();
    }

    void OnClickBtn4()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject obj = objects[i];
            ObjectManager.Instance.ReleaseObject(obj, 0, true);
            obj = null;
        }
        objects.Clear();

    }

    void OnClickBtn5()
    {
        GameMapManager.Instance.LoadScene("Test");
    }

    void OnClickBtn6()
    {
        sw.Reset();
        sw.Start();
        ObjectManager.Instance.InstantiateObject("Assets/GameData/Prefabs/Attack.prefab", false);
        sw.Stop();
        Debug.Log("异步加载对象消耗时间： " + sw.ElapsedMilliseconds + " ms");
    }

    void OnClickExit()
    {
        UIManager.Instance.CloseWnd(this, true);
        //加载场景
        GameMapManager.Instance.LoadScene(ConStr.MENU0SCNEN);

    }

}
