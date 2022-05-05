using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main1UI : Window
{
    private Main1Panel m_Main1Panel;

    private AudioClip clip;

    List<GameObject> objects = new List<GameObject>();

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    public override void Awake(params object[] paralist)
    {
        m_Main1Panel = GameObject.GetComponent<Main1Panel>();
        AddButtonListener(m_Main1Panel.btn1, OnClickBtn1);
        AddButtonListener(m_Main1Panel.btn1_1, OnClickBtn1_1);
        AddButtonListener(m_Main1Panel.btn1_2, OnClickBtn1_2);
        AddButtonListener(m_Main1Panel.btn2, OnClickBtn2);   
        AddButtonListener(m_Main1Panel.btn3, OnClickBtn3);        
        AddButtonListener(m_Main1Panel.btn4, OnClickBtn4);  
        AddButtonListener(m_Main1Panel.btn5, OnClickBtn5);
        AddButtonListener(m_Main1Panel.btn6, OnClickBtn6);
        AddButtonListener(m_Main1Panel.Exit, OnClickExit);

       
    }

    void OnClickBtn1()
    {
        sw.Reset();
        sw.Start();
        clip = ResourceManager.Instance.LoadResource<AudioClip>("Assets/GameData/Sounds/senlin.mp3");

        m_Main1Panel.Image1.sprite =  ResourceManager.Instance.LoadResource<Sprite>("Assets/GameData/UGUI/test1.png");
        m_Main1Panel.Image2.sprite =  ResourceManager.Instance.LoadResource<Sprite>("Assets/GameData/UGUI/test2.png");
        m_Main1Panel.Image3.sprite =  ResourceManager.Instance.LoadResource<Sprite>("Assets/GameData/UGUI/test3.png");

        sw.Stop();
        Debug.Log("同步加载资源消耗时间： " + sw.ElapsedMilliseconds + " ms");
        m_Main1Panel.m_Audio.clip = clip;
        m_Main1Panel.m_Audio.Play();
    }

    void OnClickBtn1_1()
    {
        m_Main1Panel.m_Audio.Stop();
        m_Main1Panel.m_Audio.clip = null;
        ResourceManager.Instance.ReleaseResrouce(clip, true);
        clip = null;

        ResourceManager.Instance.ReleaseResrouce(m_Main1Panel.Image1.sprite, true);
        m_Main1Panel.Image1.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(m_Main1Panel.Image2.sprite, true);
        m_Main1Panel.Image2.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(m_Main1Panel.Image3.sprite, true);
        m_Main1Panel.Image3.sprite = null;

    }

    void OnClickBtn1_2()
    {
        m_Main1Panel.m_Audio.Stop();
        m_Main1Panel.m_Audio.clip = null;
        ResourceManager.Instance.ReleaseResrouce(clip);
        clip = null;


        ResourceManager.Instance.ReleaseResrouce(m_Main1Panel.Image1.sprite);
        m_Main1Panel.Image1.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(m_Main1Panel.Image2.sprite);
        m_Main1Panel.Image2.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(m_Main1Panel.Image3.sprite);
        m_Main1Panel.Image3.sprite = null;
    }

    void OnClickBtn2()
    {
        sw.Reset();
        sw.Start();
        GameObject obj = ObjectManager.Instance.InstantiateObject("Assets/GameData/Prefabs/Attack.prefab", true);
        sw.Stop();
        Debug.Log("同步加载对象消耗时间： " + sw.ElapsedMilliseconds + " ms");
        objects.Add(obj);

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
        GameMapManager.Instance.LoadScene(ConStr.MENU0SCNEN);
    }
    
    void OnClickBtn6()
    {
        sw.Reset();
        sw.Start();
        GameObject obj = ObjectManager.Instance.InstantiateObject("Assets/GameData/Prefabs/Attack.prefab", true,false);
        sw.Stop();
        ObjectManager.Instance.ReleaseObject(obj);
        obj = null;
      
        Debug.Log("同步加载对象消耗时间： " + sw.ElapsedMilliseconds + " ms");
    }

    void OnClickExit()
    {
        //UIManager.Instance.ShowWnd(ConStr.LOADPANEL);
        UIManager.Instance.HideWnd(this);
        //加载场景
        GameMapManager.Instance.LoadScene(ConStr.MENU0SCNEN);

    }


}
