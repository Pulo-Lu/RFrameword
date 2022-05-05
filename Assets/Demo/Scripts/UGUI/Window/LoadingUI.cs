using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : Window
{
    private LoadingPanel m_MainPanel;
    private string m_SceneName;

    public override void Awake(params object[] paralist)
    {
        m_MainPanel = GameObject.GetComponent<LoadingPanel>();
        m_SceneName = (string)paralist[0];
    }

    public override void OnUpdate()
    {
        if (m_MainPanel == null)
            return;


        m_MainPanel.m_Slider.value = GameMapManager.LoadingProgress / 100.0f;
        m_MainPanel.m_Text.text = string.Format("{0}%", GameMapManager.LoadingProgress);

        if (GameMapManager.LoadingProgress >= 100)
        {
            LoadOtherScene();
        }
    }

    /// <summary>
    /// 加载对应场景第一个UI
    /// </summary>
    public void LoadOtherScene()
    {
        //根据场景名字打开对应场景第一个界面
        if(m_SceneName == ConStr.MENU0SCNEN)
        {
            UIManager.Instance.PopUpWnd(ConStr.MENUPANEL);
        }
        if (m_SceneName == ConStr.MENU1SCNEN)
        {
            UIManager.Instance.PopUpWnd(ConStr.MAIN1PANEL);

        }
        if (m_SceneName == ConStr.MENU2SCNEN)
        {
            UIManager.Instance.PopUpWnd(ConStr.MAIN2PANEL);
        }
        if (m_SceneName == ConStr.MENU3SCNEN)
        {
            UIManager.Instance.PopUpWnd(ConStr.MAIN3PANEL);
        }
        UIManager.Instance.CloseWnd(ConStr.LOADINGPANEL);
    }
}
