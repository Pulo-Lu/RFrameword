using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameStart : MonoSingleton<GameStart>
{
    private GameObject m_obj;

    protected override void Awake()
    {
        base.Awake();
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
        GameMapManager.Instance.LoadScene(ConStr.MENUSCNEN);
    }

    void RegisterUI()
    {
        UIManager.Instance.Register<MenuUI>(ConStr.MENUPANEL);
        UIManager.Instance.Register<LoadingUI>(ConStr.LOADINGPANEL);
    }

    void LoadConfiger()
    {

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
    }
}