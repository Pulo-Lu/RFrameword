using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class MonsterData : ExcelBase
{
    /// <summary>
    /// 编辑器下初始类转xml
    /// </summary>
    public override void Construction()
    {
        AllMonster = new List<MonsterBase>();
        for(int i =0; i < 5; i++)
        {
            MonsterBase monster = new MonsterBase();
            monster.Id = i + 1;
            monster.Name = i + "sq";
            monster.OutLook = ConStr.ATTACK;
            monster.Rare = 2;
            monster.Level = 10 + i;
            monster.Height = 2 + i;
            AllMonster.Add(monster);
        }
    }

    /// <summary>
    /// 运行时：数据初始化
    /// </summary>
    public override void Init()
    {
        m_AllMonsterDic.Clear();
        foreach(MonsterBase monster in AllMonster)
        {
            if (m_AllMonsterDic.ContainsKey(monster.Id))
            {
                Debug.LogError("有重复ID: " + monster.Id);
            }
            else
            {
                m_AllMonsterDic.Add(monster.Id, monster);
            }
        }
    }

    /// <summary>
    /// 根据ID查找Monster数据
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public MonsterBase FindMonsterById(int Id)
    {
        return m_AllMonsterDic[Id];
    }

    [XmlIgnore]
    public Dictionary<int, MonsterBase> m_AllMonsterDic = new Dictionary<int, MonsterBase>();

    [XmlElement("AllMonster")]
    public List<MonsterBase> AllMonster { get; set; }
}


[System.Serializable]
public class MonsterBase 
{
    //怪物ID
    [XmlAttribute("ID")]
    public int Id { get; set; }
    //怪物Name
    [XmlAttribute("Name")]
    public string Name { get; set; }
    //怪物预置路径
    [XmlAttribute("OutLook")]
    public string OutLook { get; set; }
    //怪物等级
    [XmlAttribute("Level")] 
    public int Level { get; set; }
    //怪物稀有度
    [XmlAttribute("Rare")]
    public int Rare { get; set; }
    //怪物高度
    [XmlAttribute("Height")]
    public float Height { get; set; }
   
}

