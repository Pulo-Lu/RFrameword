using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameStart : MonoSingleton<GameStart>
{
    public bool LoadFormAssetBundle;

    GameObject Obj;

    protected override void Awake()
    {
        base.Awake();

        ResourceManager.Instance.m_LoadFormAssetBundle = LoadFormAssetBundle;

        GameObject.DontDestroyOnLoad(gameObject);
        AssetBundleManager.Instance.LoadAssetBundleConfig();
        ResourceManager.Instance.Init(this);
        ObjectManager.Instance.Init(transform.Find("RecylcePoolTrs"),transform.Find("SceneTrs"));
    }

    private void Start()
    {
        LoadConfiger();

        UIManager.Instance.Init(
            transform.Find("UIRoot") as RectTransform,
            transform.Find("UIRoot/WndRoot") as RectTransform,
            transform.Find("UIRoot/UICamera").GetComponent<Camera>(),
            transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>());
        
        RegisterUI();

        GameMapManager.Instance.Init(this);

        Obj = ObjectManager.Instance.InstantiateObject("Assets/GameData/Prefabs/Attack.prefab", true);
        ObjectManager.Instance.ReleaseObject(Obj);

        //加载场景
        GameMapManager.Instance.LoadScene(ConStr.MENU0SCNEN);
    }

    /// <summary>
    /// 注册UI窗口
    /// </summary>
    void RegisterUI()
    {
        UIManager.Instance.Register<MenuUI>(ConStr.MENUPANEL);

        UIManager.Instance.Register<LoadingUI>(ConStr.LOADINGPANEL);

        UIManager.Instance.Register<LoadUI>(ConStr.LOADPANEL);

        UIManager.Instance.Register<Main1UI>(ConStr.MAIN1PANEL);

        UIManager.Instance.Register<Main2UI>(ConStr.MAIN2PANEL);

        UIManager.Instance.Register<Main3UI>(ConStr.MAIN3PANEL);

        UIManager.Instance.Register<ResourUI>(ConStr.RESOURPANEL);


    }


    /// <summary>
    /// 加载配置表
    /// </summary>
    void LoadConfiger()
    {
        ConfigManager.Instance.LoadData<MonsterData>(CFG.TABLE_MONSTER);
        ConfigManager.Instance.LoadData<BuffData>(CFG.TABLE_BUFF);
    }

    private void Update()
    {
        UIManager.Instance.OnUpdate();

    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        ResourceManager.Instance.ClearCache();
        Resources.UnloadUnusedAssets();
        Debug.Log("清空编辑器缓存");
#endif
        Application.Quit();
    }
}
