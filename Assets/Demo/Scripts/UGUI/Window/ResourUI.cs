using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourUI : Window
{
    private ResourPanel m_ResourPanel;

    public override void Awake(params object[] paralist)
    {
        m_ResourPanel = GameObject.GetComponent<ResourPanel>();
        AddButtonListener(m_ResourPanel.m_ABManagerButton, OnClickABManager);      
        AddButtonListener(m_ResourPanel.m_ResourceManagerButton, OnClickResourceManager);  
        AddButtonListener(m_ResourPanel.m_ObjectManagerButton, OnClickObjectManager);   
        AddButtonListener(m_ResourPanel.m_ExitButton, OnClickExit);

    }

    void OnClickABManager()
    {
        Debug.Log("点击了AssetsBundle资源管理");
    } 
    
    void OnClickResourceManager()
    {

    } 

    void OnClickObjectManager()
    {
        Debug.Log("点击了ObjectManager对象管理");

    }

    void OnClickExit()
    {


    }
}
