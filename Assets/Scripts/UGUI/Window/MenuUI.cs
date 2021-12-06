using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : Window
{
    private MenuPanel m_MainPanel;

    public override void Awake(params object[] paralist)
    {
        m_MainPanel = GameObject.GetComponent<MenuPanel>();
        AddButtonListener(m_MainPanel.m_StartButton, OnClickStart);  
        AddButtonListener(m_MainPanel.m_LoadButton, OnClickLoad);   
        AddButtonListener(m_MainPanel.m_ExitButton, OnClickExit);

        ResourceManager.Instance.AsyncLoadResource("Assets/GameData/UGUI/test1.png", OnLoadSpriteTest1, LoadResPriority.RES_MIDDLE, true);
        ResourceManager.Instance.AsyncLoadResource("Assets/GameData/UGUI/test3.png", OnLoadSpriteTest3, LoadResPriority.RES_HIGHT, true);

        ResourceManager.Instance.AsyncLoadResource("Assets/GameData/UGUI/test2.png", OnLoadSpriteTest2, LoadResPriority.RES_HIGHT, true);

        LoadMonsterData();


    }


    void LoadMonsterData()
    {
        MonsterData monsterData = ConfigManager.Instance.FindData<MonsterData>(CFG.TABLE_MONSTER);

        for(int i = 0; i < monsterData.AllMonster.Count; i++)
        {
            Debug.Log(string.Format("ID:{0} 名字:{1} 预置路径:{2} 等级:{3} 稀有度:{4} 高度:{5}", monsterData.AllMonster[i].Id, monsterData.AllMonster[i].Name,
                 monsterData.AllMonster[i].OutLook, monsterData.AllMonster[i].Level, monsterData.AllMonster[i].Rare, monsterData.AllMonster[i].Height));
        }
    }

    void OnLoadSpriteTest1(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
    {
        if (obj != null)
        {
            Sprite sp = obj as Sprite;
            m_MainPanel.m_Test1.sprite = sp;
            Debug.Log("图片一加载完成");
        }
    }
    
    void OnLoadSpriteTest2(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
    {
        if (obj != null)
        {
            Sprite sp = obj as Sprite;
            m_MainPanel.m_Test2.sprite = sp;
            Debug.Log("图片二加载完成");
        }
    }     
    
    void OnLoadSpriteTest3(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
    {
        if (obj != null)
        {
            Sprite sp = obj as Sprite;
            m_MainPanel.m_Test3.sprite = sp;
            Debug.Log("图片三加载完成");
        }
    }  


    public override void OnUpdate()
    {
    
    }

    void OnClickStart()
    {
        Debug.Log("点击了开始游戏");
    } 
    
    void OnClickLoad()
    {
        Debug.Log("点击了加载游戏");
    } 
    
    void OnClickExit()
    {
        Debug.Log("点击了推出游戏");
    }
}
