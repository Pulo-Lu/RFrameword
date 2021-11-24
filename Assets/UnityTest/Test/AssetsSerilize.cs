 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这行代码是在Create菜单中增加 CreatAssets 选项
//[CreateAssetMenu(fileName ="TestAssets",menuName ="CreatAssets",order = 0)]

public class AssetsSerilize :ScriptableObject
{
    public int Id;
    public string Name;
    public List<string> TestList;
}
