using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//资源加载优先级枚举
public enum LoadResPriority 
{
    RES_HIGHT = 0,    //最高优先级
    RES_MIDDLE,     //一般优先级
    RES_SLOW,       //低优先级
    RES_NUM,        //优先级数量
}

//同步加载中间类结构
public class ResouceObj 
{
    //路径对应CRC
    public uint m_Crc = 0;
    //存ResouceItem
    public ResouceItem m_ResItem = null;
    //实例化出来的GameObject
    public GameObject m_CloneObj = null;
    //是否跳场景清除
    public bool m_bClear = true;
    //GameObject Guid
    public long m_Guid = 0;
    //是否已经放回对象池
    public bool m_Already = false;

    //-----------------------------------//
    //是否方法场景节点下面
    public bool m_SetSceneParent = false;
    //实例化资源加载完成回调
    public OnAsyncObjFinish m_DealFinish = null;
    //异步参数
    public object m_Param1, m_Param2, m_Param3 = null;

    //-------------------------------------------//
    //离线数据
    public OfflineData m_OfflineData = null;

    public void Reset()
    {
        m_Crc = 0;
        m_ResItem = null;
        m_CloneObj = null;
        m_bClear = true;
        m_Guid = 0;
        m_Already = false;
        m_SetSceneParent = false;
        m_DealFinish = null;
        m_Param1 = m_Param2 = m_Param3 = null;
        m_OfflineData = null;
    }
}


//异步加载中间类结构
public class AsyncLoadResParam 
{
    //回调列表
    public List<AsyncCallBack> m_CallBackList = new List<AsyncCallBack>();

    public uint m_Crc;
    public string m_Path;
    public bool m_Sprite = false;
    public LoadResPriority m_Priority = LoadResPriority.RES_SLOW;

    public void Reset()
    {
        m_CallBackList.Clear();
        m_Crc = 0;
        m_Path = "";
        m_Sprite = false;
        m_Priority = LoadResPriority.RES_SLOW;
    }
}

public class AsyncCallBack
{
    //加载完成的回调(针对ObjManager)
    public OnAsyncFinish m_DealFinish = null;
    //ObjectManager 对应的中间类
    public ResouceObj m_ResObj = null;

 //-----------------------------------------------//
    //加载完成的回调
    public OnAsyncObjFinish m_DealObjFinish = null;
    //回调参数
    public object m_Param1 = null, m_Param2 = null, m_Param3 = null;

    public void Reset()
    {
        m_DealObjFinish = null;
        m_DealFinish = null;
        m_Param1 = null;
        m_Param2 = null;
        m_Param3 = null;
        m_ResObj = null;
    }
}

// 资源加载完成回调  委托 （异步加载）
public delegate void OnAsyncObjFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null);

//实例化对象加载完成回调
public delegate void OnAsyncFinish(string path, ResouceObj resObj, object param1 = null, object param2 = null, object param3 = null);

public class ResourceManager : Singleton<ResourceManager>
{
    protected long m_Guid = 0;

    public bool m_LoadFormAssetBundle = false;

    //缓存使用的资源列表
    public Dictionary<uint, ResouceItem> AssetDic { get; set; } = new Dictionary<uint, ResouceItem>();

    //缓存引用计数为零的资源列表,达到缓存最大的时候释放这个列表里面最早没用的资源
    protected CMapList<ResouceItem> m_NoRefrenceAssetMapList = new CMapList<ResouceItem>();

    //异步加载   中间类，回调类的类对象池
    protected ClassObjectPool<AsyncLoadResParam> m_AsyncLoadResParamPool = new ClassObjectPool<AsyncLoadResParam>(50);
    protected ClassObjectPool<AsyncCallBack> m_AsyncCallBackPool = new ClassObjectPool<AsyncCallBack>(100);

    //Mono脚本
    protected MonoBehaviour m_Startmono;
    //正在异步加载的资源列表
    protected List<AsyncLoadResParam>[] m_LoadingAssetList = new List<AsyncLoadResParam>[(int)LoadResPriority.RES_NUM];
    //正在异步加载的Dic
    protected Dictionary<uint, AsyncLoadResParam> m_LoadingAssetDic = new Dictionary<uint, AsyncLoadResParam>();

    //最长连续卡着加载资源的时间，单位微秒
    private const long MAXLOADRESTIME = 200000;

    //最大缓存个数
    private const int MAXCACHECOUNT = 500;

    public void Init(MonoBehaviour mono)
    {
        for(int i=0; i < (int)LoadResPriority.RES_NUM; i++)
        {
            m_LoadingAssetList[i] = new List<AsyncLoadResParam>();
        }
        m_Startmono = mono;
        m_Startmono.StartCoroutine(AsyncLoadCor());
    }

    /// <summary>
    /// 创建唯一的 GUID
    /// </summary>
    /// <returns></returns>
    public long CreatGuid()
    {
        return m_Guid++;
    }

    /// <summary>
    /// 清空缓存
    /// </summary>
    public void ClearCache()
    {
        List<ResouceItem> tempList = new List<ResouceItem>();
        foreach(ResouceItem item in AssetDic.Values)
        {
            if (item.m_Clear)
            {
                tempList.Add(item);
            }
        }
        foreach (ResouceItem item in tempList)
        {
            DestoryResourceItem(item, true);
        }
        tempList.Clear();
    }

    /// <summary>
    /// 取消异步加载资源
    /// </summary>
    /// <returns></returns>
    public bool CancleLoad(ResouceObj resObj)
    {
        AsyncLoadResParam para = null;
        if(m_LoadingAssetDic.TryGetValue(resObj.m_Crc,out para) && m_LoadingAssetList[(int)para.m_Priority].Contains(para))
        {
            for(int i = para.m_CallBackList.Count; i >= 0; i--)
            {
                AsyncCallBack tempCallBack = para.m_CallBackList[i];
                if (tempCallBack != null && resObj == tempCallBack.m_ResObj) 
                {
                    tempCallBack.Reset();
                    m_AsyncCallBackPool.Recycle(tempCallBack);
                    para.m_CallBackList.Remove(tempCallBack);
                }
            }

            if(para.m_CallBackList.Count <= 0)
            {
                para.Reset();
                m_LoadingAssetList[(int)para.m_Priority].Remove(para);
                m_AsyncLoadResParamPool.Recycle(para);
                m_LoadingAssetDic.Remove(resObj.m_Crc);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 根据ResObj 增加引用计数
    /// </summary>
    /// <returns></returns>
    public int IncreaseResouceRef(ResouceObj resobj, int count = 1)
    {
        return resobj != null ? IncreaseResouceRef(resobj.m_Crc, count) : 0;
    }

    /// <summary>
    /// 根据path 增加引用计数
    /// </summary>
    /// <param name="crc"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int IncreaseResouceRef(uint crc = 0, int count = 1)
    {
        ResouceItem item = null;
        if (!AssetDic.TryGetValue(crc, out item) || item == null)  
            return 0;

        item.RefCount += count;
        item.m_LastUseTime = Time.realtimeSinceStartup;

        return item.RefCount;

    }

    /// <summary>
    /// 根据ResObj 减少引用计数
    /// </summary>
    /// <param name="resobj"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int DecreaseResouceRef(ResouceObj resobj,int count = 1)
    {
        return resobj != null ? DecreaseResouceRef(resobj.m_Crc, count) : 0;
    }

    /// <summary>
    /// 根据path路径 减少引用计数
    /// </summary>
    /// <param name="crc"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int DecreaseResouceRef(uint crc, int count = 1)
    {
        ResouceItem item = null;
        if (!AssetDic.TryGetValue(crc, out item) || item == null)
            return 0;

        item.RefCount -= count;

        return item.RefCount;
    }

    /// <summary>
    /// 预加载资源
    /// </summary>
    /// <param name="path"></param>
    public void PreloadRes(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        uint crc = Crc32.GetCrc32(path);
        ResouceItem item = GetCacheResourceItem(crc, 0);
        if (item != null)
        {
            return;
        }

        Object obj = null;

#if UNITY_EDITOR
        if (!m_LoadFormAssetBundle)
        {
            item = AssetBundleManager.Instance.FindResouceItem(crc);
            if (item != null && item.m_Obj != null)
            {
                obj = item.m_Obj as Object;
            }
            else
            {
                if (item == null)
                {
                    item = new ResouceItem();
                    item.m_Crc = crc;
                }
                obj = LoadAssetByEditor<Object>(path);
            }
        }
#endif

        if (obj == null)
        {
            item = AssetBundleManager.Instance.LoadResouceAssetBundle(crc);
            if (item != null && item.m_AssetBundle != null)
            {
                if (item.m_Obj != null)
                {
                    obj = item.m_Obj;
                }
                else
                {
                    obj = item.m_AssetBundle.LoadAsset<Object>(item.m_AssetName);
                }
            }
        }

        CacheResource(path, ref item, crc, obj);
        //跳场景不清空缓存
        item.m_Clear = false;
        ReleaseResrouce(obj, false);
    }

    /// <summary>
    /// 同步加载资源，针对给ObjectManager 的接口
    /// </summary>
    /// <param name="path"></param>
    /// <param name="resObj"></param>
    /// <returns></returns>
    public ResouceObj LoadResource(string path,ResouceObj resObj)
    {
        if (resObj == null)
        {
            return null;
        }
        uint crc = resObj.m_Crc == 0 ? Crc32.GetCrc32(path) : resObj.m_Crc;

        ResouceItem item = GetCacheResourceItem(crc);
        if(item != null)
        {
            resObj.m_ResItem = item;
            return resObj;
        }

        Object obj = null;
#if UNITY_EDITOR
        if (!m_LoadFormAssetBundle)
        {
            item = AssetBundleManager.Instance.FindResouceItem(crc);
            if (item != null && item.m_Obj != null) 
            {
                obj = item.m_Obj as Object;
            }
            else
            {
                if(item == null)
                {
                    item = new ResouceItem();
                    item.m_Crc = crc;
                }
                obj = LoadAssetByEditor<Object>(path);
            }
        }
#endif
        if(obj == null)
        {
            item = AssetBundleManager.Instance.LoadResouceAssetBundle(crc);
            if (item != null && item.m_AssetBundle != null)
            {
                if (item.m_Obj != null)
                {
                    obj = item.m_Obj as Object;
                }
                else
                {
                    obj = item.m_AssetBundle.LoadAsset<Object>(item.m_AssetName);
                }
            }
        }

        CacheResource(path, ref item, crc, obj);
        resObj.m_ResItem = item;
        item.m_Clear = resObj.m_bClear;

        return resObj;
    }


    /// <summary>
    /// 同步资源加载，外部直接调用，仅加载不需要实例化资源，例如Texture,音频等
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        uint crc = Crc32.GetCrc32(path);
        ResouceItem item = GetCacheResourceItem(crc);
        if (item != null)
        {
            return item.m_Obj as T;
        }

        T obj = null;

#if UNITY_EDITOR
        if (!m_LoadFormAssetBundle)
        {
            item = AssetBundleManager.Instance.FindResouceItem(crc);
            if (item != null && item.m_AssetBundle != null)
            {
                if (item.m_Obj != null)
                {
                    obj = (T)item.m_Obj;
                }
                else
                {
                    obj = item.m_AssetBundle.LoadAsset<T>(item.m_AssetName);
                }
            }
            else
            {
                if (item == null)
                {
                    item = new ResouceItem();
                    item.m_Crc = crc;
                }
                obj = LoadAssetByEditor<T>(path);
            }
        }
#endif

        if (obj == null)
        {
            item = AssetBundleManager.Instance.LoadResouceAssetBundle(crc);
            if (item != null && item.m_AssetBundle != null)
            {
                if(item.m_Obj != null)
                {
                    obj = item.m_Obj as T;
                }
                else 
                {
                    obj = item.m_AssetBundle.LoadAsset<T>(item.m_AssetName);
                }
            }
        }

        CacheResource(path,ref item, crc, obj);

        return obj;
    }

    /// <summary>
    /// 根据 ResouceObj 卸载资源
    /// </summary>
    /// <param name="resObj"></param>
    /// <param name="destoryObj">是否完全清除</param>
    /// <returns></returns>
    public bool ReleaseResrouce(ResouceObj resObj, bool destoryObj = false)
    {
        if(resObj == null) { return false; }

        ResouceItem item = null;
        if (!AssetDic.TryGetValue(resObj.m_Crc, out item) || null == item)
        {
            Debug.LogError("AssetDic里不存在该资源： " + resObj.m_CloneObj.name + " 可能释放了多次");
        }

        GameObject.Destroy(resObj.m_CloneObj);

        item.RefCount--;

        DestoryResourceItem(item, destoryObj);
        return true;
    }

    /// <summary>
    /// 不需要实例化的资源的卸载,根据对象
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="destoryObj">是否完全清除</param>
    /// <returns></returns>
    public bool ReleaseResrouce(Object obj, bool destoryObj = false)
    {
        if(obj == null)
        { 
            return false;
        }

        ResouceItem item = null;
        foreach(ResouceItem res in AssetDic.Values)
        {
            if(res.m_Guid == obj.GetInstanceID())
            {
                item = res;
            }
        }

        if(item == null)
        {
            Debug.LogError("AssetDic里不存在该资源： " + obj.name + " 可能释放了多次!");
            return false;
        }

        item.RefCount--;

        DestoryResourceItem(item, destoryObj);
        return true;
    }

    /// <summary>
    /// 不需要实例化的资源的卸载,根据路径
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destoryObj"></param>
    /// <returns></returns>
    public bool ReleaseResrouce(string path, bool destoryObj = false)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        uint crc = Crc32.GetCrc32(path);
        ResouceItem item = null;
        if(!AssetDic.TryGetValue(crc,out item) || null == item)
        {
            Debug.LogError("AssetDic里不存在该资源： " + path + " 可能释放了多次");
        }

        item.RefCount--;

        DestoryResourceItem(item, destoryObj);
        return true;
    }


    /// <summary>
    /// 缓存加载的资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="item"></param>
    /// <param name="crc"></param>
    /// <param name="obj"></param>
    /// <param name="addrefcount"></param>
    void CacheResource(string path, ref ResouceItem item, uint crc, Object obj, int addrefcount = 1)
    {
        //缓存太多，清除最早没有使用的资源
        WashOut();

        if (item == null)
        {
            Debug.LogError("ResouceLoad is null , path : " + path);
        }
        if(obj == null)
        {
            Debug.LogError("ResouceLoad Fail : " + path);
        }

        item.m_Obj = obj;
        item.m_Guid = obj.GetInstanceID();
        item.m_LastUseTime = Time.realtimeSinceStartup;
        item.RefCount += addrefcount;
        ResouceItem oldItem = null;
        if(AssetDic.TryGetValue(item.m_Crc,out oldItem))
        {
            AssetDic[item.m_Crc] = item;
        }
        else
        {
            AssetDic.Add(item.m_Crc, item);
        }
    }

    /// <summary>
    /// 缓存太多，清除最早没有使用的资源
    /// </summary>
    protected void WashOut()
    {
        //当大于缓存个数时，进行一半释放
        while (m_NoRefrenceAssetMapList.Size() >= MAXCACHECOUNT)
        {
            for (int i = 0; i < MAXCACHECOUNT / 2; i++)
            {
                ResouceItem item = m_NoRefrenceAssetMapList.Back();

                DestoryResourceItem(item, true);
            }

        }
    }

    /// <summary>
    /// 回收一个资源
    /// </summary>
    /// <param name="item"></param>
    /// <param name="destroyCache">是否缓存</param>
    protected void DestoryResourceItem(ResouceItem item, bool destroyCache = false)
    {
        if(item ==null || item.RefCount > 0)
        {
            return;
        }

        //缓存
        if (!destroyCache)
        {
            m_NoRefrenceAssetMapList.InsertToHead(item);
            return;
        }

        //不缓存
        if (!AssetDic.Remove(item.m_Crc))
        {
            return;
        }

        m_NoRefrenceAssetMapList.Remove(item);

        //释放assetbundle 的引用
        AssetBundleManager.Instance.ReleaseAsset(item);

        //清空资源对象的对象池
        ObjectManager.Instance.ClearPoolObject(item.m_Crc);

        if(item.m_Obj != null)
        {
#if UNITY_EDITOR
            Resources.UnloadUnusedAssets();
#endif
            item.m_Obj = null;

        }

    }

#if UNITY_EDITOR
    protected T LoadAssetByEditor<T>(string path) where T : UnityEngine.Object
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
    }
#endif

    /// <summary>
    /// 从资源池获取缓存的资源
    /// </summary>
    /// <param name="crc"></param>
    /// <param name="addrefcount"></param>
    /// <returns></returns>
    ResouceItem GetCacheResourceItem(uint crc, int addrefcount = 1)
    {
        ResouceItem item = null;
        if(AssetDic.TryGetValue(crc,out item))
        {
            if (item != null)
            {
                item.RefCount += addrefcount;
                item.m_LastUseTime = Time.realtimeSinceStartup;

                /*if(item.RefCount <= 1)
                {
                    m_NoRefrenceAssetMapList.Remove(item);
                }*/
            }
        }

        return item;
    }

    /// <summary>
    /// 异步加载资源，仅仅是不需要实例化资源，例如Texture,音频等
    /// </summary>
    public void AsyncLoadResource(string path, OnAsyncObjFinish dealFinish, LoadResPriority priority, bool isSprite = false, object param1 = null,
        object param2 = null, object param3 = null, uint crc = 0)
    {
        if(crc == 0)
        {
            crc = Crc32.GetCrc32(path);
        }
        ResouceItem item = GetCacheResourceItem(crc);
        if (item != null)
        {
            if (dealFinish != null)
            {
                dealFinish(path, item.m_Obj, param1, param2, param3);
            }
            return;
        }

        //判断是否在加载中
        AsyncLoadResParam para = null;
        if (!m_LoadingAssetDic.TryGetValue(crc, out para) || para == null)
        {
            para = m_AsyncLoadResParamPool.Spawn(true);
            para.m_Crc = crc;
            para.m_Path = path;
            para.m_Sprite = isSprite;
            para.m_Priority = priority;
            m_LoadingAssetDic.Add(crc, para);
            m_LoadingAssetList[(int)priority].Add(para);
        }

        //往回调列表里面加回调
        AsyncCallBack callBack = m_AsyncCallBackPool.Spawn(true);
        callBack.m_DealObjFinish = dealFinish;
        callBack.m_Param1 = param1;
        callBack.m_Param2 = param2;
        callBack.m_Param3 = param3;
        para.m_CallBackList.Add(callBack);
    }

    /// <summary>
    /// 针对ObjManager的异步加载接口
    /// </summary>
    /// <param name="path"></param>
    /// <param name="resObj"></param>
    /// <param name="dealFinish"></param>
    /// <param name="priority"></param>
    public void AsyncLoadResource(string path,ResouceObj resObj, OnAsyncFinish dealFinish, LoadResPriority priority)
    {
        ResouceItem item = GetCacheResourceItem(resObj.m_Crc);
        if (item != null)
        {
            resObj.m_ResItem = item;
            if (dealFinish != null)
            {
                dealFinish(path, resObj);
            }
            return;
        }
        //判断是否在加载中
        AsyncLoadResParam para = null;
        if (!m_LoadingAssetDic.TryGetValue(resObj.m_Crc, out para) || para == null)
        {
            para = m_AsyncLoadResParamPool.Spawn(true);
            para.m_Crc = resObj.m_Crc;
            para.m_Path = path;
            para.m_Priority = priority;
            m_LoadingAssetDic.Add(resObj.m_Crc, para);
            m_LoadingAssetList[(int)priority].Add(para);
        }

        //往回调列表里面加回调
        AsyncCallBack callBack = m_AsyncCallBackPool.Spawn(true);
        callBack.m_DealFinish = dealFinish;
        callBack.m_ResObj = resObj;
        para.m_CallBackList.Add(callBack);
    }


    /// <summary>
    /// 异步加载
    /// </summary>
    /// <returns></returns>
    IEnumerator AsyncLoadCor()
    {
        List<AsyncCallBack> callBackList = null;
        //上一次yeil的时间
        long LastYieledTime = System.DateTime.Now.Ticks;
        while (true)
        {
            bool haveYield = false;
            for(int i = 0;i< (int)LoadResPriority.RES_NUM; i++)
            {
                if (m_LoadingAssetList[(int)LoadResPriority.RES_HIGHT].Count > 0)
                {
                    i = (int)LoadResPriority.RES_HIGHT;
                }
                else if(m_LoadingAssetList[(int)LoadResPriority.RES_MIDDLE].Count > 0)
                {
                    i = (int)LoadResPriority.RES_MIDDLE;
                }

                //正在加载的列表 
                List<AsyncLoadResParam> loadingList = m_LoadingAssetList[i];
                if (loadingList.Count <= 0)
                    continue;

                AsyncLoadResParam loadingItem = loadingList[0];
                loadingList.RemoveAt(0);
                callBackList = loadingItem.m_CallBackList;

                Object obj = null;
                ResouceItem item = null;

#if UNITY_EDITOR
                if (!m_LoadFormAssetBundle)
                {
                    if (loadingItem.m_Sprite)
                    {
                        obj = LoadAssetByEditor<Sprite>(loadingItem.m_Path);
                    }
                    else
                    {
                        obj = LoadAssetByEditor<Object>(loadingItem.m_Path);
                    }
                    //模拟异步加载
                    yield return new WaitForSeconds(0.5f);

                    item = AssetBundleManager.Instance.FindResouceItem(loadingItem.m_Crc);

                    if (item == null)
                    {

                        item = new ResouceItem();
                        item.m_Crc = loadingItem.m_Crc;
                    }

                }
#endif
                if(obj == null)
                {
                    item = AssetBundleManager.Instance.LoadResouceAssetBundle(loadingItem.m_Crc);
                    if (item != null && item.m_AssetBundle != null)
                    {
                        //AssetBundle的异步加载
                        AssetBundleRequest abRequest = null;
                        if (loadingItem.m_Sprite)
                        {
                            abRequest = item.m_AssetBundle.LoadAssetAsync<Sprite>(item.m_AssetName);
                        }
                        else
                        {
                            abRequest = item.m_AssetBundle.LoadAssetAsync(item.m_AssetName);
                        }
                        
                        yield return abRequest;
                        if (abRequest.isDone)
                        {
                            obj = abRequest.asset;
                        }
                        LastYieledTime = System.DateTime.Now.Ticks; 
                    }
                }

                //缓存资源
                CacheResource(loadingItem.m_Path, ref item, loadingItem.m_Crc, obj, callBackList.Count);

                //回调
                for(int j = 0; j < callBackList.Count; j++)
                {
                    AsyncCallBack callBack = callBackList[j];

                    if (callBack != null && callBack.m_DealFinish != null && callBack.m_ResObj != null) 
                    {
                        ResouceObj tempResObj = callBack.m_ResObj;
                        tempResObj.m_ResItem = item;
                        callBack.m_DealFinish(loadingItem.m_Path, tempResObj, tempResObj.m_Param1, tempResObj.m_Param2, tempResObj.m_Param3);
                        callBack.m_DealFinish = null;
                        tempResObj = null;
                    }

                    if (callBack != null && callBack.m_DealObjFinish != null) 
                    {
                        callBack.m_DealObjFinish(loadingItem.m_Path, obj, callBack.m_Param1, callBack.m_Param2, callBack.m_Param3);
                        callBack.m_DealObjFinish = null;
                    }

                    callBack.Reset();
                    m_AsyncCallBackPool.Recycle(callBack);
                }
                obj = null;
                callBackList.Clear();
                m_LoadingAssetDic.Remove(loadingItem.m_Crc);

                loadingItem.Reset();
                m_AsyncLoadResParamPool.Recycle(loadingItem);

                if (System.DateTime.Now.Ticks - LastYieledTime > MAXLOADRESTIME)
                {
                    yield return null;
                    LastYieledTime = System.DateTime.Now.Ticks;
                    haveYield = true;
                }
            }

            if(!haveYield || System.DateTime.Now.Ticks - LastYieledTime > MAXLOADRESTIME)
            {
                LastYieledTime = System.DateTime.Now.Ticks;
                yield return null;
            }
        }
    }
}

/// <summary>
/// 双向链表结构节点
/// </summary>
/// <typeparam name="T"></typeparam>
public class DoubleLinkedListNode<T> where T:class, new() 
{
    //前一个节点
    public DoubleLinkedListNode<T> prev = null;
    //后一个节点
    public DoubleLinkedListNode<T> next = null;
    //当前节点
    public T t = null;
}

//双向链表结构
public class DoubleLinkedList<T> where T:class, new()
{
    //表头
    public DoubleLinkedListNode<T> Head = null;
    //表尾
    public DoubleLinkedListNode<T> Tail = null;

    //双向链表结构类对象池
    protected ClassObjectPool<DoubleLinkedListNode<T>> m_DoubleLinkNodePool = ObjectManager.Instance.GetOrCreatClassPool<DoubleLinkedListNode<T>>(500);

    //个数
    protected int m_Count = 0;
    public int Count
    {
        get
        {
            return m_Count;
        }
    }

    /// <summary>
    /// 添加一个节点到头部
    /// </summary>
    /// <param name="t"></param>
    public DoubleLinkedListNode<T> AddToHeader(T t)
    {
        DoubleLinkedListNode<T> pList = m_DoubleLinkNodePool.Spawn(true);
        pList.next = null;
        pList.prev = null;
        pList.t = t;
        return AddToHeader(pList);
    }

    /// <summary>
    /// 添加一个节点到头部
    /// </summary>
    /// <param name="pNode"></param>
    public DoubleLinkedListNode<T> AddToHeader(DoubleLinkedListNode<T> pNode)
    {
        if(pNode == null) { return null; }

        pNode.prev = null;

        if(Head == null)
        {
            Head = Tail = pNode;
        }
        else
        {
            pNode.next = Head;
            Head.prev = pNode;
            Head = pNode;
        }
        m_Count++;
        return Head;
    }

    /// <summary>
    /// 添加一个节点到尾部
    /// </summary>
    /// <param name="t"></param>
    public DoubleLinkedListNode<T> AddToTail(T t)
    {
        DoubleLinkedListNode<T> pList = m_DoubleLinkNodePool.Spawn(true);
        pList.next = null;
        pList.next = null;
        pList.t = t;
        return AddToTail(pList);
    }

    /// <summary>
    /// 添加一个节点到尾部
    /// </summary>
    /// <param name="pNode"></param>
    public DoubleLinkedListNode<T> AddToTail(DoubleLinkedListNode<T> pNode)
    {
        if (pNode == null) { return null; }

        pNode.next = null;

        if (Tail == null)
        {
            Head = Tail = pNode;
        }
        else
        {
            pNode.prev = Tail;
            Tail.next = pNode;
            Tail = pNode;
        }
        m_Count++;
        return Tail;
    }

    /// <summary>
    /// 移除某个节点
    /// </summary>
    /// <param name="pNode"></param>
    public void RemoveNode(DoubleLinkedListNode<T> pNode)
    {
        if(pNode == null) { return; }

        if(pNode == Head)
            Head = pNode.next;

        if(pNode == Tail)
            Tail = pNode.prev;

        if (pNode.prev != null)
            pNode.prev.next = pNode.next;

        if (pNode.next != null)
            pNode.next.prev = pNode.prev;

        pNode.prev = pNode.next = null;
        pNode.t = null;
        m_DoubleLinkNodePool.Recycle(pNode);
        m_Count--;
    }

    /// <summary>
    /// 把某个节点移动到头部
    /// </summary>
    /// <param name="pNode"></param>
    public void MoveToHead(DoubleLinkedListNode<T> pNode)
    {
        if (pNode == null || pNode == Head)
            return;

        if (pNode.prev == null && pNode.next == null)
            return;

        if (pNode == Tail)
            Tail = pNode.prev;

        if (pNode.prev != null)
            pNode.prev.next = pNode.next; 
        
        if (pNode.next != null)
            pNode.next.prev = pNode.prev;

        pNode.prev = null;
        pNode.next = Head;
        Head.prev = pNode;
        Head = pNode;

        //链表元素只有一两个的情况
        if(Tail == null)
        {
            Tail = Head;
        }
    }
}

public class CMapList<T> where T:class, new()
{
    DoubleLinkedList<T> m_DLink = new DoubleLinkedList<T>();
    Dictionary<T, DoubleLinkedListNode<T>> m_FindMap = new Dictionary<T, DoubleLinkedListNode<T>>();

    ~CMapList()
    {
        Clear();
    }

    /// <summary>
    /// 清空列表
    /// </summary>
    public void Clear()
    {
        while (m_DLink.Tail != null)
        {
            Remove(m_DLink.Tail.t);
        }
    }

    /// <summary>
    /// 插入一个节点到表头
    /// </summary>
    /// <param name="t"></param>
    public void InsertToHead(T t)
    {
        DoubleLinkedListNode<T> node = null;
        if (m_FindMap.TryGetValue(t, out node) && node != null)
        {
            m_DLink.AddToHeader(node);
            return;
        }
        m_DLink.AddToHeader(t);
        m_FindMap.Add(t, m_DLink.Head);
    }

    /// <summary>
    /// 从表尾弹出（移除）一个节点
    /// </summary>
    public void Pop()
    {
        if(m_DLink.Tail != null)
        {
            Remove(m_DLink.Tail.t);
        }
    }

    /// <summary>
    /// 删除某节点
    /// </summary>
    /// <param name="t"></param>
    public void Remove(T t)
    {
        DoubleLinkedListNode<T> node = null;
        if (!m_FindMap.TryGetValue(t, out node) || node == null)
            return;

        m_DLink.RemoveNode(node);
        m_FindMap.Remove(t);
    }

    /// <summary>
    /// 获取尾部节点
    /// </summary>
    public T Back()
    {
        return m_DLink.Tail == null ? null : m_DLink.Tail.t;
    }

    /// <summary>
    /// 返回节点个数
    /// </summary>
    public int Size()
    {
        return m_FindMap.Count;
    }

    /// <summary>
    /// 查找是否存在该节点
    /// </summary>
    /// <param name="t"></param>
    public bool Find(T t)
    {
        DoubleLinkedListNode<T> node = null;
        if(!m_FindMap.TryGetValue(t,out node) || node == null)
            return false;

        return true;
    }

    /// <summary>
    /// 刷新某个节点，把节点移动到头部
    /// </summary>
    /// <param name="t"></param>
    public bool Reflash(T t)
    {
        DoubleLinkedListNode<T> node = null;
        if (!m_FindMap.TryGetValue(t, out node) || node == null)
            return false;

        m_DLink.MoveToHead(node);
        return true;
    }
}
