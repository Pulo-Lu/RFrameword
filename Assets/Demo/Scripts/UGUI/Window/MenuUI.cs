using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : Window
{
    private MenuPanel m_MainPanel;

    List<GameObject> objects = new List<GameObject>();


    public override void Awake(params object[] paralist)
    {
        m_MainPanel = GameObject.GetComponent<MenuPanel>();
        AddButtonListener(m_MainPanel.m_StartButton, OnClickStart);
        AddButtonListener(m_MainPanel.m_LoadButton, OnClickLoad); 
        AddButtonListener(m_MainPanel.m_ExitButton, OnClickExit);

        //LoadMonsterData();


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

    public override void OnUpdate()
    {

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    //ResourceManager.Instance.ReleaseResrouce(m_MainPanel.m_Audio.clip);
        //    //m_MainPanel.m_Audio.clip = null;
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        obj = GameObject.Instantiate(Resources.Load("Prefabs/Attack") as GameObject);
        //        objects.Add(obj);
        //    }
        //}  
        
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        Object.Destroy(objects[i]);
        //        objects[i] = null;
        //    }
        //    objects.Clear();
        //}


        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    ObjectManager.Instance.ReleaseObject(obj, 0,true);
        //    obj = null;
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    for(int i = 0; i < 1000; i++)
        //    {
        //        obj = ObjectManager.Instance.InstantiateObject("Assets/GameData/Prefabs/Attack.prefab", true);
        //        objects.Add(obj);
        //    }

        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    //ObjectManager.Instance.ReleaseObject(obj);
        //    for(int i = 0; i < objects.Count; i++)
        //    {
        //        ObjectManager.Instance.ReleaseObject(objects[i]);
        //        objects[i] = null;
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    //ObjectManager.Instance.ReleaseObject(obj);
        //    for (int i = 0; i < objects.Count; i++)
        //    {
        //        ObjectManager.Instance.ReleaseObject(objects[i],0,true);
        //        objects[i] = null;
        //    }
        //    objects.Clear();
        //}
    }

    void OnClickStart()
    {
        Debug.Log("点击了进入框架");
        UIManager.Instance.PopUpWnd(ConStr.LOADPANEL);
        UIManager.Instance.HideWnd(this);
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
