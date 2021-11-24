using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class ResourceTest : MonoBehaviour
{
    void Start()
    {
        //SerilizeTest();
        //DeSerilizerTest();

        //BinarySerTest();
        //BinaryDeSerTest();

        //ReadTestAssets();

        TestLoadAB();
    }

    void TestLoadAB()
    {
        /*AssetBundle configAB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/assetbundleconfig");
        TextAsset textAsset = configAB.LoadAsset<TextAsset>("AssetBundleConfig");
        MemoryStream stream = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        AssetBundleConfig testSerilize = (AssetBundleConfig)bf.Deserialize(stream);
        stream.Close();
        string path = "Assets/GameData/Prefabs/Attack.prefab";
        uint crc = Crc32.GetCrc32(path);
        ABBase abBase = null;
        for(int i = 0; i < testSerilize.ABList.Count; i++)
        {
            if(testSerilize.ABList[i].Crc == crc)
            {
                abBase = testSerilize.ABList[i];
            }
        }

        for(int i = 0; i < abBase.ABDependce.Count; i++)
        {
            AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abBase.ABDependce[i]);
        }
        AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abBase.ABName);
        GameObject obj = GameObject.Instantiate(assetBundle.LoadAsset<GameObject>(abBase.AssetName));*/
    }

    //Asset 序列化读取
    /*void ReadTestAssets()
    {
        AssetsSerilize assets = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetsSerilize>("Assets/Scripts/TestAssets.asset");
        Debug.Log(assets.Id);   
        Debug.Log(assets.Name);
        foreach (string a in assets.TestList)
        {
            Debug.Log(a);
        }
    }*/


    //写
    void SerilizeTest()
    {
        TestSerilize testSerilize = new TestSerilize();
        testSerilize.Id =  1;
        testSerilize.Name = "测试";
        testSerilize.List = new List<int>();
        testSerilize.List.Add(2);
        testSerilize.List.Add(3); 
        XmlSerilize(testSerilize);
    }

    //读
    void DeSerilizerTest()
    {
        TestSerilize testSerilize = XmlDeSerilize();
        Debug.Log(testSerilize.Id + "   " + testSerilize.Name);
        foreach (int a in testSerilize.List)
        {
            Debug.Log(a);
        }
    }

    //xml序列化  命名空间：System.Xml.Serialization;
    void XmlSerilize(TestSerilize testSerilize)
    {
        //打开文件流（文件地址，文件格式，文件权限）
        FileStream fileStream = new FileStream(Application.dataPath + "/test.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        //创建写入流（编码）
        StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
        //创建Xml序列化（Type类型）
        XmlSerializer xml = new XmlSerializer(testSerilize.GetType());
        //xml序列化（写入流，需要序列化的类）
        xml.Serialize(sw, testSerilize);
        //关闭
        sw.Close();
        fileStream.Close();
    }

    //xml反向序列化
    TestSerilize XmlDeSerilize()
    {
        FileStream fs = new FileStream(Application.dataPath + "/test.xml", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        XmlSerializer xs = new XmlSerializer(typeof(TestSerilize));
        TestSerilize testSerilize = (TestSerilize)xs.Deserialize(fs);
        fs.Close();
        return testSerilize;
    }


    //写
    void BinarySerTest()
    {
        TestSerilize testSerilize = new TestSerilize();
        testSerilize.Id = 5;
        testSerilize.Name = "二进制测试";
        testSerilize.List = new List<int>();
        testSerilize.List.Add(18);
        testSerilize.List.Add(10);
        BinarySerilize(testSerilize);
    }

    //读
    /*void BinaryDeSerTest()
    {
        TestSerilize testSerilize = BinaryDeserilize();
        Debug.Log(testSerilize.Id + "   " + testSerilize.Name);
        foreach (int a in testSerilize.List)
        {
            Debug.Log(a);
        }
    }*/

    //二进制序列化 命名空间：System.Runtime.Serialization.Formatters.Binary;
    void BinarySerilize(TestSerilize serilize)
    {
        FileStream fs = new FileStream(Application.dataPath + "/test.bytes", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, serilize);
        fs.Close();
    }

    //二进制反向序列化
    /*TestSerilize BinaryDeserilize()
    {
        TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/test.bytes");
        MemoryStream stream = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        TestSerilize testSerilize = (TestSerilize)bf.Deserialize(stream);
        stream.Close();
        return testSerilize;
    }*/
}
