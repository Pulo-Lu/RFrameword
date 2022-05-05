using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main3UI : Window
{
    private Main3Panel m_Main3Panel;

    private AudioClip clip;

    private GameObject obj;

    List<GameObject> objects = new List<GameObject>();

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    Texture2D txture1;
    Texture2D txture2;
    Texture2D txture3;

    public override void Awake(params object[] paralist)
    {
        m_Main3Panel = GameObject.GetComponent<Main3Panel>();
        AddButtonListener(m_Main3Panel.btn1, OnClickBtn1);
        AddButtonListener(m_Main3Panel.btn1_1, OnClickBtn1_1);
        AddButtonListener(m_Main3Panel.btn1_2, OnClickBtn1_2);
        AddButtonListener(m_Main3Panel.btn2, OnClickBtn2);   
        AddButtonListener(m_Main3Panel.btn3, OnClickBtn3);        
        AddButtonListener(m_Main3Panel.btn4, OnClickBtn4);  
        AddButtonListener(m_Main3Panel.btn5, OnClickBtn5);
        AddButtonListener(m_Main3Panel.btn6, OnClickBtn6);
        AddButtonListener(m_Main3Panel.Exit, OnClickExit);

        ResourceManager.Instance.PreloadRes("Assets/GameData/Sounds/senlin.mp3");
        ResourceManager.Instance.PreloadRes("Assets/GameData/UGUI/test1.png");
        ResourceManager.Instance.PreloadRes("Assets/GameData/UGUI/test2.png");
        ResourceManager.Instance.PreloadRes("Assets/GameData/UGUI/test3.png");
        ObjectManager.Instance.PreloadGameObjdect("Assets/GameData/Prefabs/Attack.prefab", 20,true);
    }

    void OnClickBtn1()
    {

        sw.Reset();
        sw.Start();

        clip = ResourceManager.Instance.LoadResource<AudioClip>("Assets/GameData/Sounds/senlin.mp3");

        txture1 = ResourceManager.Instance.LoadResource<Texture2D>("Assets/GameData/UGUI/test1.png");
        m_Main3Panel.Image1.sprite = Sprite.Create(txture1, new Rect(0, 0, txture1.width, txture1.height), new Vector2(0.5f, 0.5f));

        txture2 = ResourceManager.Instance.LoadResource<Texture2D>("Assets/GameData/UGUI/test2.png");
        m_Main3Panel.Image2.sprite = Sprite.Create(txture2, new Rect(0, 0, txture2.width, txture2.height), new Vector2(0.5f, 0.5f));

        txture3 = ResourceManager.Instance.LoadResource<Texture2D>("Assets/GameData/UGUI/test3.png");
        m_Main3Panel.Image3.sprite = Sprite.Create(txture3, new Rect(0, 0, txture3.width, txture3.height), new Vector2(0.5f, 0.5f));

        sw.Stop();
        Debug.Log("同步加载资源消耗时间： " + sw.ElapsedMilliseconds + " ms");
        m_Main3Panel.m_Audio.clip = clip;
        m_Main3Panel.m_Audio.Play();
    }

    void OnClickBtn1_1()
    {
        m_Main3Panel.m_Audio.Stop();
        m_Main3Panel.m_Audio.clip = null;
        ResourceManager.Instance.ReleaseResrouce(clip, true);
        clip = null;

        ResourceManager.Instance.ReleaseResrouce(txture1, true);
        txture1 = null;
        m_Main3Panel.Image1.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(txture2, true);
        txture2 = null;
        m_Main3Panel.Image2.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(txture3, true);
        txture3 = null;
        m_Main3Panel.Image3.sprite = null;
    }

    void OnClickBtn1_2()
    {
        m_Main3Panel.m_Audio.Stop();
        m_Main3Panel.m_Audio.clip = null;
        ResourceManager.Instance.ReleaseResrouce(clip);
        clip = null;

        ResourceManager.Instance.ReleaseResrouce(txture1);
        txture1 = null;
        m_Main3Panel.Image1.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(txture2);
        txture2 = null;
        m_Main3Panel.Image2.sprite = null;
        ResourceManager.Instance.ReleaseResrouce(txture3);
        txture3 = null;
        m_Main3Panel.Image3.sprite = null;
    }

    void OnClickBtn2()
    {
        sw.Reset();
        sw.Start();
        GameObject obj = ObjectManager.Instance.InstantiateObject("Assets/GameData/Prefabs/Attack.prefab", true, false);
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
        GameMapManager.Instance.LoadScene("Test");
    }

    void OnClickBtn6()
    {

    }

    void OnClickExit()
    {
        UIManager.Instance.CloseWnd(this, true);
        //加载场景
        GameMapManager.Instance.LoadScene(ConStr.MENU0SCNEN);

    }
}
