using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class BuffData : ExcelBase
{
    /// <summary>
    /// 编辑器下初始类转xml
    /// </summary>
    public override void Construction()
    {
        AllBuffList = new List<BuffBase>();
        for(int i = 0; i < 10; i++)
        {
            BuffBase buff = new BuffBase();
            buff.Id = i + 1;
            buff.Name = "全BUFF" + i;
            buff.OutLook = "Assets/GameData/..." + i;
            buff.Time = Random.Range(0.5f, 10f);
            buff.BuffType =(BuffEnum)Random.Range(0, 4);
            AllBuffList.Add(buff);
        }

        MonsterBuffList = new List<BuffBase>();
        for (int i = 0; i < 5; i++)
        {
            BuffBase buff = new BuffBase();
            buff.Id = i + 1;
            buff.Name = "全BUFF" + i;
            buff.OutLook = "Assets/GameData/..." + i;
            buff.Time = Random.Range(0.5f, 10f);
            buff.BuffType = (BuffEnum)Random.Range(0, 4);
            MonsterBuffList.Add(buff);
        }
    }

    /// <summary>
    /// 运行时：数据初始化
    /// </summary>
    public override void Init()
    {
        m_AllBuffDic.Clear();
        //for(int i = 0; i < AllBuffList.Count; i++)
        //{
        //    m_AllBuffDic.Add(AllBuffList[i].Id, AllBuffList[i]);
        //}
        foreach (BuffBase buff in AllBuffList)
        {
            if (m_AllBuffDic.ContainsKey(buff.Id))
            {
                Debug.LogError("有重复ID: " + buff.Id);
            }
            else
            {
                m_AllBuffDic.Add(buff.Id, buff);
            }
        }
    }

    /// <summary>
    /// 根据ID查找BUFF数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public BuffBase FindBuffById(int id)
    {
        return m_AllBuffDic[id];
    }


    //根据ID保存 BUFF数据
    [XmlIgnore]
    public Dictionary<int, BuffBase> m_AllBuffDic = new Dictionary<int, BuffBase>();

    //所有Buff配置   
    [XmlElement("AllBuffList")]
    public List<BuffBase> AllBuffList { get; set; }

    //所有怪物Buff配置
    [XmlElement("MonsterBuffList")]
    public List<BuffBase> MonsterBuffList { get; set; }
}


public enum BuffEnum
{
    None = 0,
    Ranshao = 1,
    Bingdong = 2,
    Du = 3,
}

[System.Serializable]
public class BuffBase
{
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("Name")]
    public string Name { get; set; }

    [XmlAttribute("OutLook")]
    public string OutLook { get; set; }

    [XmlAttribute("Time")]
    public float Time { get; set; }

    [XmlAttribute("BuffType")]
    public BuffEnum BuffType { get; set; }

}
