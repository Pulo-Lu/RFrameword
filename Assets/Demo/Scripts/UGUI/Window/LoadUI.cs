using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadUI : Window
{
    private LoadPanel m_LoadPanel;

    public override void Awake(params object[] paralist)
    {
        m_LoadPanel = GameObject.GetComponent<LoadPanel>();
        AddButtonListener(m_LoadPanel.m_Btn1, OnClickBtn1);      
        AddButtonListener(m_LoadPanel.m_Btn2, OnClickBtn2);  
        AddButtonListener(m_LoadPanel.m_Btn3, OnClickBtn3);   
        AddButtonListener(m_LoadPanel.m_ExitButton, OnClickExit);

    }

    void OnClickBtn1()
    {

        GameMapManager.Instance.LoadScene(ConStr.MENU1SCNEN);
        //UIManager.Instance.PopUpWnd(ConStr.MAIN1PANEL);
        UIManager.Instance.HideWnd(this);

    } 
    
    void OnClickBtn2()
    {
        GameMapManager.Instance.LoadScene(ConStr.MENU2SCNEN);
        //UIManager.Instance.PopUpWnd(ConStr.MAIN2PANEL);
        UIManager.Instance.HideWnd(this);
    } 

    void OnClickBtn3()
    {
        GameMapManager.Instance.LoadScene(ConStr.MENU3SCNEN);
        //UIManager.Instance.PopUpWnd(ConStr.MAIN3PANEL);
        UIManager.Instance.HideWnd(this);
    }

    void OnClickExit()
    {
        UIManager.Instance.ShowWnd(ConStr.MENUPANEL);
        UIManager.Instance.HideWnd(this);

    }
}
