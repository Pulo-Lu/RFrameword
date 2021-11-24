using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExcelBase
{
    /// <summary>
    /// 编辑器下初始类转xml
    /// </summary>
    public virtual void Construction() { }

    /// <summary>
    /// 运行时：数据初始化
    /// </summary>
    public virtual void Init() { }
}
