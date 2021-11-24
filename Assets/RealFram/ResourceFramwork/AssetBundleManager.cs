using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public class AssetBundleManager: Singleton<AssetBundleManager>
{
    //资源关系依赖配置表，可以根据Crc来找到对应的资源块
    protected Dictionary<uint, ResouceItem> m_ResouceItemDic = new Dictionary<uint, ResouceItem>();
    //存储已加载的AB包，Key为crc
    protected Dictionary<uint, AssetBundleItem> m_AssetBundleItemDic = new Dictionary<uint, AssetBundleItem>();
    //AssetBundleItem 类对象池
    protected ClassObjectPool<AssetBundleItem> m_AssetBundleItemPool = ObjectManager.Instance.GetOrCreatClassPool<AssetBundleItem>(500);

    /// <summary>
    /// 加载AB配置表
    /// </summary>
    /// <returns></returns>
    public bool LoadAssetBundleConfig()
    {
        m_ResouceItemDic.Clear();
        string configPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/assetbundleconfig";
        AssetBundle configAB = AssetBundle.LoadFromFile(configPath);
        TextAsset textAsset = configAB.LoadAsset<TextAsset>("assetbundleconfig");

        if(textAsset == null)
        {
            Debug.LogError("AssetBundleConfig 不存在！");
            return false;
        }

        MemoryStream stream = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        AssetBundleConfig config = (AssetBundleConfig)bf.Deserialize(stream);
        stream.Close();

        for(int i = 0; i < config.ABList.Count; i++)
        {
            ABBase abBase = config.ABList[i];
            ResouceItem item = new ResouceItem();
            item.m_Crc = abBase.Crc;
            item.m_AssetName = abBase.AssetName;
            item.m_ABName = abBase.ABName;
            item.m_DependAssetBundle = abBase.ABDependce; 

            if (m_ResouceItemDic.ContainsKey(item.m_Crc))
            {
                Debug.LogError("重复的CRC资源名：" + item.m_AssetName + " ab包名：" + item.m_ABName);
            }
            else
            {
                m_ResouceItemDic.Add(item.m_Crc, item);
            }
        }

        return true;
    }

    /// <summary>
    /// 根据路径的CRC加载中间类ResouceItem  
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    public ResouceItem LoadResouceAssetBundle(uint crc)
    {
        ResouceItem item = null;

        if(!m_ResouceItemDic.TryGetValue(crc,out item) || item == null)
        {
            Debug.LogError(string.Format("LoadResouceAssetBundle error: can not find crc {0} in AssewtBundleConfig", crc.ToString()));
            return item;
        }

        if(item.m_AssetBundle != null)
        {
            return item;
        }

        item.m_AssetBundle = LoadAssetBundle(item.m_ABName);

        if (item.m_DependAssetBundle != null) 
        {
            for(int i = 0; i < item.m_DependAssetBundle.Count; i++)
            {
                LoadAssetBundle(item.m_DependAssetBundle[i]);
            }
        }

        return item;
    }

    /// <summary>
    /// 根据路径加载单个 AssetBundle
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private AssetBundle LoadAssetBundle(string name)
    {
        AssetBundleItem item = null;
        uint crc = Crc32.GetCrc32(name);

        if(!m_AssetBundleItemDic.TryGetValue(crc,out item))
        {
            AssetBundle assetBundle = null;
            string fullPath = Application.streamingAssetsPath + "/" + name;
            if (File.Exists(fullPath))
            {
                assetBundle = AssetBundle.LoadFromFile(fullPath);
            }

            if (assetBundle == null)
            {
                Debug.LogError("Load AssetBundle Error: " + fullPath);
            }

            item = m_AssetBundleItemPool.Spawn(true);
            item.assetBundle = assetBundle;
            item.RefCount++;
            m_AssetBundleItemDic.Add(crc, item);
        }
        else
        {
            item.RefCount++;
        }

        return item.assetBundle;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="item"></param>
    public void ReleaseAsset(ResouceItem item)
    {
        if (item == null)
        {
            return;
        }

        if(item.m_DependAssetBundle !=null && item.m_DependAssetBundle.Count > 0)
        {
            for(int i = 0; i < item.m_DependAssetBundle.Count; i++)
            {
                UnLoadAssetBundle(item.m_DependAssetBundle[i]);
            }
            
        }
        UnLoadAssetBundle(item.m_ABName);
        
    }

    /// <summary>
    /// 根据路径卸载 AssetBundle
    /// </summary>
    /// <param name="name"></param>
    private void UnLoadAssetBundle(string name)
    {
        AssetBundleItem item = null;
        uint crc = Crc32.GetCrc32(name);

        if(m_AssetBundleItemDic.TryGetValue(crc,out item) && item != null)
        {
            item.RefCount--;
            if (item.RefCount <= 0 && item.assetBundle != null)
            {
                item.assetBundle.Unload(true);
                item.Rest();
                m_AssetBundleItemPool.Recycle(item);
                m_AssetBundleItemDic.Remove(crc);
            }
        }
    }

    /// <summary>
    /// 根据crc查找 ResouceItem
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    public ResouceItem FindResouceItem(uint crc)
    {
        ResouceItem item = null;
        m_ResouceItemDic.TryGetValue(crc, out item);
        return item;
    }
}



public class AssetBundleItem 
{
    public AssetBundle assetBundle = null;
    public int RefCount;

    public void Rest()
    {
        assetBundle = null;
        RefCount = 0;
    }
}


public class ResouceItem
{
    //资源路径的CRC
    public uint m_Crc = 0;
    //该资源的文件名
    public string m_AssetName = string.Empty;
    //该资源所在的Asset Bundle
    public string m_ABName = string.Empty;
    //该资源所依赖的AssetBundle
    public List<string> m_DependAssetBundle = null;
    //该资源加载完的AB包
    public AssetBundle m_AssetBundle = null;

    //-------------------------------------------------------------------//

    //资源对象
    public Object m_Obj = null;
    //资源唯一标识
    public int m_Guid = 0;
    //资源最后使用的时间
    public float m_LastUseTime = 0.0f;
    //是否跳场景清掉
    public bool m_Clear = true;
    //引用计数
    protected int m_RefCount = 0;
    public int RefCount 
    {
        get { return m_RefCount; }
        set
        {
            m_RefCount = value;
            if (m_RefCount < 0)
            {
                Debug.LogError("refCount < 0" + m_RefCount + " ," + (m_Obj != null ? m_Obj.name : "name is null"));
            }
        }
    }
}
