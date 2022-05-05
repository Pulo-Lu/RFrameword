using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Object_1_UI : Window
{
    private Object_1_Panel m_Resour_1_Panel;

    public override void Awake(params object[] paralist)
    {
        m_Resour_1_Panel = GameObject.GetComponent<Object_1_Panel>();
        AddButtonListener(m_Resour_1_Panel.m_Button1, OnClickButton1);      
        AddButtonListener(m_Resour_1_Panel.m_Button2, OnClickButton2);  
        AddButtonListener(m_Resour_1_Panel.m_Button3, OnClickButton3);
        AddButtonListener(m_Resour_1_Panel.m_Button4, OnClickButton4);
        AddButtonListener(m_Resour_1_Panel.m_Button5, OnClickButton5);
        AddButtonListener(m_Resour_1_Panel.m_Button6, OnClickButton6);

        AddButtonListener(m_Resour_1_Panel.m_ExitButton, OnClickExit);

    }

    void OnClickButton1()
    {
        Debug.Log("点击了基础资源同步加载");
    } 
    
    void OnClickButton2()
    {
        Debug.Log("点击了基础资源卸载");
    } 

    void OnClickButton3()
    {
        Debug.Log("点击了基础资源异步加载");
    }   
    
    void OnClickButton4()
    {
        Debug.Log("点击了预加载");
    }   
    
    void OnClickButton5()
    {
        Debug.Log("点击了清空缓存");
    }   
    
    void OnClickButton6()
    {
        Debug.Log("点击了TODO");
    }

    void OnClickExit()
    {
        UIManager.Instance.ShowWnd(ConStr.RESOURPANEL);
        UIManager.Instance.CloseWnd(this);

    }
}
