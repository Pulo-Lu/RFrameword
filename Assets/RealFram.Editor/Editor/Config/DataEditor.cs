using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Xml;
using OfficeOpenXml;
using System.ComponentModel;

public class DataEditor
{
    public static string XmlPath = "Assets/GameData/Data/Xml/";
    public static string BinaryPath = "Assets/GameData/Data/Binary/";
    public static string ScriptsPath = "Assets/Scripts/Data/";

    [MenuItem("Assets/类转xml")]
    public static void AssetsClassToXml()
    {
        UnityEngine.Object[] objs = Selection.objects;
        for(int i = 0; i < objs.Length; i++)
        {
            EditorUtility.DisplayCancelableProgressBar("文件下的类转xml", "正在扫描" + objs[i].name + "... ...", 1.0f / objs.Length * i);
            ClassToXml(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/Xml转Binary")]
    public static void AssetsXmlToBinary()
    {
        UnityEngine.Object[] objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++)
        {
            EditorUtility.DisplayCancelableProgressBar("文件下的Xml转二进制", "正在扫描" + objs[i].name + "... ...", 1.0f / objs.Length * i);
            XmlToBinary(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Tools/Xml/Xml转成二进制")]
    public static void AllXmlToBinary()
    {
        string path = Application.dataPath.Replace("Assets", "") + XmlPath;
        string[] filesPath = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        for(int i = 0; i < filesPath.Length; i++)
        {
            EditorUtility.DisplayCancelableProgressBar("查找文件夹下面的Xml", "正在扫描" + filesPath[i] + "... ...", 1.0f / filesPath.Length * i);
            if (filesPath[i].EndsWith(".xml"))
            {
                string tempPath = filesPath[i].Substring(filesPath[i].LastIndexOf("/") + 1);
                tempPath = tempPath.Replace(".xml", "");
                XmlToBinary(tempPath);
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Tools/测试/测试读取Xml")]
    public static void TestReadXml()
    {
        string xmlPath = Application.dataPath + "/../Data/Reg/MonsterData.xml";
        XmlReader reader = null;
        try
        {
            //读xml
            XmlDocument xml = new XmlDocument();
            reader = XmlReader.Create(xmlPath);
            xml.Load(reader);

            XmlNode xn = xml.SelectSingleNode("data");
            XmlElement xe = (XmlElement)xn;
            string className = xe.GetAttribute("name");
            string xmlName = xe.GetAttribute("to");
            string excelName = xe.GetAttribute("from");
            reader.Close();
            Debug.Log(className + " " + xmlName + " " + excelName);
            foreach(XmlNode node in xe.ChildNodes)
            {
                XmlElement tempXe = (XmlElement)node;
                string nama = tempXe.GetAttribute("name");
                string type = tempXe.GetAttribute("type");
                Debug.Log(nama + " " + type);
                XmlNode listNode = tempXe.FirstChild;
                XmlElement listElement = (XmlElement)listNode;
                string listName = listElement.GetAttribute("name");
                string sheetName = listElement.GetAttribute("sheetname");
                string mainKey = listElement.GetAttribute("mainKey");
                Debug.Log("List: " + listName + " " + sheetName + " " + mainKey);
                foreach(XmlNode xmlNode in listElement.ChildNodes)
                {
                    XmlElement txe = (XmlElement)xmlNode;
                    Debug.Log(txe.GetAttribute("name") + " " + txe.GetAttribute("col") + " " +
                        txe.GetAttribute("type"));
                }
            }

        }
        catch(Exception e)
        {
            if(reader != null)
            {
                reader.Close();
            }
            Debug.LogError(e);
        }
  
            
    }

    [MenuItem("Tools/测试/测试写入Excel")]
    public static void TestWriteExcel()
    {
        string xlsxPath = Application.dataPath + "/../Data/Excel/怪物.xlsx";
        FileInfo xlsxFile = new FileInfo(xlsxPath);
        if (xlsxFile.Exists)
        {
            xlsxFile.Delete();
            xlsxFile = new FileInfo(xlsxPath);
        }
        using (ExcelPackage package = new ExcelPackage(xlsxFile))
        {

            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("怪物配置");

            #region Excel 常用函数
            //worksheet.DefaultColWidth = 10;           // sheet页面默认行高
            //worksheet.DefaultRowHeight = 30;          // sheet页面默认列高
            //worksheet.Cells.Style.WrapText = true;    // 设置所有单元格自动换行

            //worksheet.InsertColumn();                 // 插入行，从某一行开始插入多少行
            //worksheet.InsertRow();                    // 插入列，从某一列开始插入多少列

            //worksheet.DeleteColumn();                 // 删除行，从某一行开始删除多少行
            //worksheet.DeleteRow();                    // 删除列，从某一列开始删除多少列

            //worksheet.Column(1).Width = 10;           //设置第几行的宽度
            //worksheet.Row(1).Height = 30;             //设置第几列的高度

            //worksheet.Column(1).Hidden = true;        //设置第几行隐藏
            //worksheet.Row(1).Hidden = true;           //设置第几列隐藏

            //worksheet.Column(1).Style.Locked = true;  //设置第几行锁定
            //worksheet.Row(1).Style.Locked = true;     //设置第几列 锁定
            #endregion


            //单元格从1开始
            ExcelRange range = worksheet.Cells[1, 1];
            range.Value = "测试aaaaaaaaass\nsssssssssssssssdddddddddddddddd";
            //背景样式
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            //设置单元格内背景颜色
            range.Style.Fill.BackgroundColor.SetColor(1, 0, 200, 0);
            //设置单元格内字体颜色
            range.Style.Font.Color.SetColor(1, 0, 0, 0);
            //对齐方式
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            //自适应宽度
            range.AutoFitColumns();
            //自动换行
            range.Style.WrapText = true;
            //保存
            package.Save();
        }
        
    }

    [MenuItem("Tools/测试/测试已有类进行反射")]
    public static void TestReflection1()
    {
        TestInfo testInfo = new TestInfo()
        {
            Id = 2,
            Name = "测试反射",
            IsA = false,
            AllStrList = new List<string>(),
            AllTestInfoList = new List<TestInfoTwo>(),
        };

        testInfo.AllStrList.Add("测试1111");
        testInfo.AllStrList.Add("测试2222");
        testInfo.AllStrList.Add("测试3333");

        for (int i = 0; i < 3; i++)
        {
            TestInfoTwo test = new TestInfoTwo();
            test.Id = i + 1;
            test.Name = "Test" + i;
            testInfo.AllTestInfoList.Add(test);
        }

        #region List 读取测试 List 元素为常用类型
        /*object list = GetMemberValue(testInfo, "AllStrList");

        //InvokeMember(string,   BindingFlags,   Binder,   object,   object[]);   
        //string:你所要调用的函数名   
        //BindingFlags: 你所要调用的函数的属性,可以组合
        //Binder:高级内容，可以先不看,一般为空
        //object:调用该成员函数的实例
        //object[]:参数，  

        //获取List的元素个数   new object[] { }
        int listCount =System.Convert.ToInt32(list.GetType().InvokeMember("get_Count",
            BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { }));

        Debug.Log("List_Count:" + listCount);

        for(int i = 0; i < listCount; i++)
        {
            //获取List的元素 new object[] { i }
            object item = list.GetType().InvokeMember("get_Item",
            BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { i });
            Debug.Log(item);
        }*/
        #endregion

        #region List 读取测试2 List 元素为Class类型
        object list = GetMemberValue(testInfo, "AllTestInfoList");
        int listCount = System.Convert.ToInt32(list.GetType().InvokeMember("get_Count",
            BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { }));

        for (int i = 0; i < listCount; i++)
        {
            object item = list.GetType().InvokeMember("get_Item",
                BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { i });

            object id = GetMemberValue(item, "Id");
            object name = GetMemberValue(item, "Name");

            Debug.Log(id + " " + name);
        }

        #endregion

        //属性读取测试
        //Debug.Log(GetMemberValue(testInfo, "Name") + " " + GetMemberValue(testInfo, "Id") + " " + GetMemberValue(testInfo, "IsA"));
    }

    [MenuItem("Tools/测试/测试已有数据进行反射")]
    public static void TestReflection2()
    {
        object obj = CreateClass("TestInfo");
        //获取Id属性 
        PropertyInfo idInfo = obj.GetType().GetProperty("Id");
        SetValue(idInfo, obj, "21", "int");
        //idInfo.SetValue(obj, System.Convert.ToInt32("20"));
        
        //获取Name属性 
        PropertyInfo nameInfo = obj.GetType().GetProperty("Name");
        SetValue(nameInfo, obj, "雨", "string");
        //nameInfo.SetValue(obj, "雨");
       
        //获取IsA属性 
        PropertyInfo isInfo = obj.GetType().GetProperty("IsA");
        SetValue(isInfo, obj, "true", "bool");
        //isInfo.SetValue(obj, System.Convert.ToBoolean("false"));

        //获取Heigh属性 
        PropertyInfo heighInfo = obj.GetType().GetProperty("Heigh");
        SetValue(heighInfo, obj, "12.125", "float");
        //heighInfo.SetValue(obj, System.Convert.ToSingle("12.125"));

        //获取 枚举 属性 
        PropertyInfo enumInfo = obj.GetType().GetProperty("TestType");
        SetValue(enumInfo, obj, "Var", "enum");
        //object infoValue = TypeDescriptor.GetConverter(enumInfo.PropertyType).ConvertFromInvariantString("Var");
        //enumInfo.SetValue(obj, infoValue);

        //获取 List 属性 List 中元素为 string
        //获取 string类型
        Type type = typeof(string);

        // new 一个 list
        object list = CreateList(type);

        for (int i = 0; i < 3; i++)
        {
            object addItem = "测试填入List数据" + i;
            //调用List的Add方法添加数据
            list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });
        }
        obj.GetType().GetProperty("AllStrList").SetValue(obj, list);

        //获取 List 属性 List 中元素为 class
        object twoList = CreateList(typeof(TestInfoTwo));
        for (int i = 0; i < 3; i++)
        {
            //创建 TestInfoTwo 类
            object addItem = CreateClass("TestInfoTwo");

            PropertyInfo itemIdInfo = addItem.GetType().GetProperty("Id");
            SetValue(itemIdInfo, addItem, "151" + i, "int");
            
            PropertyInfo itemNameInfo = addItem.GetType().GetProperty("Name");
            SetValue(itemNameInfo, addItem, "雨点" + i, "string");

            twoList.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, twoList, new object[] { addItem });
        }
        obj.GetType().GetProperty("AllTestInfoList").SetValue(obj, twoList);

        //将obj保存为 TestInfo类型
        TestInfo testInfo = (obj as TestInfo);

        Debug.Log(testInfo.Id + " " + testInfo.Name + " " + testInfo.IsA + " " + testInfo.Heigh + " "  + testInfo.TestType);
        foreach (string str in testInfo.AllStrList)
        {
            //Debug.Log(str);
        }   
        foreach (TestInfoTwo test in testInfo.AllTestInfoList)
        {
            Debug.Log(test.Id + " " + test.Name);
        }
    }

    [MenuItem("Tools/Xml/Xml转Excel")]
    public static void XmlToExcel()
    {
        string name = "MonsterData";
        string regPath = Application.dataPath + "/../Data/Reg/" + name + ".xml";
        if (!File.Exists(regPath))
        {
            Debug.LogError("此数据不存在配置变化xml: " + name);
            return;
        }

        //读xml
        XmlDocument xml = new XmlDocument();
        XmlReader reader = XmlReader.Create(regPath);

        //忽略xml注释
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true; //忽略xml里面的注释
        //加载xml
        xml.Load(reader);

        //读取XML 表头
        XmlNode xn = xml.SelectSingleNode("data");
        XmlElement xe = (XmlElement)xn;
        //读取 reg 类名
        string className = xe.GetAttribute("name");
        //读取 xml 名
        string xmlName = xe.GetAttribute("to");
        //读取 Excel 名
        string excelName = xe.GetAttribute("from");

        //存储所有变量的表
        Dictionary<string, SheetClass> allSheetClassDic = new Dictionary<string, SheetClass>();
        ReadXmlNode(xe, allSheetClassDic);
        reader.Close();

        #region 读取类的具体值
        object data = null;
        Type type = null;

        //遍历程序集
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            //获取类名
            Type tempType = asm.GetType(className);

            if (tempType != null)
            {
                type = tempType;
                break;
            }
        }
        if (type != null)
        {
            string xmlPath = XmlPath + className + ".xml";
            data = BinarySerializeOpt.XmlDeserialize(xmlPath, type);
        }
        #endregion


    }

    /// <summary>
    /// 递归读取配置
    /// </summary>
    /// <param name="xe"></param>
    private static void ReadXmlNode(XmlElement xmlElement, Dictionary<string, SheetClass> allSheetClassDic)
    {
        foreach(XmlNode node in xmlElement.ChildNodes)
        {
            XmlElement xe = (XmlElement)node;
            //存在list  进行递归
            if(xe.GetAttribute("type") == "list")
            {
                //获取 List
                XmlElement listEle = (XmlElement)node.FirstChild;

                VarClass parentVar = new VarClass()
                {
                    Name = xe.GetAttribute("name"),
                    Type = xe.GetAttribute("type"),
                    Col = xe.GetAttribute("col"),
                    DeafultValue = xe.GetAttribute("defaultVaule"),
                    Foregin = xe.GetAttribute("foregin"),
                    SplitStr = xe.GetAttribute("split"),
                };

                SheetClass sheetClass = new SheetClass()
                {
                    Name = listEle.GetAttribute("name"),
                    SheetName = listEle.GetAttribute("sheetname"),
                    SplitStr = listEle.GetAttribute("split"),
                    MainKey = listEle.GetAttribute("mainkey"),
                    ParentVar = parentVar,
                };

                //SheetName 不为空
                if (!string.IsNullOrEmpty(sheetClass.SheetName))
                {
                    // sheetClass 不在字典中
                    if (!allSheetClassDic.ContainsKey(sheetClass.SheetName))
                    {
                        //获取该类下面所有变量
                        foreach(XmlNode insideNode in listEle.ChildNodes)
                        {
                            XmlElement insideXe = (XmlElement)insideNode;

                            VarClass varClass = new VarClass()
                            {
                                Name = insideXe.GetAttribute("name"),
                                Type = insideXe.GetAttribute("type"),
                                Col = insideXe.GetAttribute("col"),
                                DeafultValue = insideXe.GetAttribute("defaultVaule"),
                                Foregin = insideXe.GetAttribute("foregin"),
                                SplitStr = insideXe.GetAttribute("split"),
                            };

                            sheetClass.VarList.Add(varClass);

                        }
                        allSheetClassDic.Add(sheetClass.SheetName, sheetClass);
                    }
                }

                //递归遍历 List Child
                ReadXmlNode(listEle, allSheetClassDic);
            }
        }
    }

    //private static object GetObjFromXml(string str, string path)
    //{

    //}

    /// <summary>
    /// 反射中new一个List
    /// </summary>
    /// <param name="type">List<T> 中T的类型</param>
    /// <returns></returns>
    private static object CreateList(Type type)
    {
        //获取 List泛型类型
        Type listType = typeof(List<>);
        // MakeGenericType ：构造 List泛型 中的类型，即确定List<>里面T的类型
        Type specType = listType.MakeGenericType(new System.Type[] { type });
        // new 一个 list
        return Activator.CreateInstance(specType, new object[] { });
    }

    /// <summary>
    /// 反射变量赋值
    /// </summary>
    /// <param name="info">属性</param>
    /// <param name="var">要写入的类</param>
    /// <param name="value">要写入的值</param>
    /// <param name="type">类型</param>
    private static void SetValue(PropertyInfo info, object var,string value,string type)
    {
        //临时变量，保存要写入的值
        object val = (object)value;

        //将要写入的值 转换成各种类型
        if (type == "int")
        {
            val = System.Convert.ToInt32(val);
        }
        else if (type == "bool")
        {
            val = System.Convert.ToBoolean(val);
        }  
        else if (type == "float")
        {
            val = System.Convert.ToSingle(val);
        }  
        else if (type == "enum")
        {
            val = TypeDescriptor.GetConverter(info.PropertyType).ConvertFromInvariantString(value.ToString());
        }
        info.SetValue(var, val);
    }


    /// <summary>
    /// 反射类里面的变量的具体数值
    /// </summary>
    /// <param name="obj">反射的类</param>
    /// <param name="memberName">具体的变量名</param>
    /// <param name="bindingFlags">变量在类里面</param>
    /// <returns></returns>
    private static object GetMemberValue(object obj,string memberName,BindingFlags bindingFlags = 
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
    {
        Type type = obj.GetType();

        //获取当前Type的指定成员 返回的是数组
        MemberInfo[] members = type.GetMember(memberName, bindingFlags);
        
        //保护措施
        while (members == null || members.Length == 0) 
        {
            type = type.BaseType;
            if (type == null)
                return null;

            members = type.GetMember(memberName, bindingFlags);
        }

        switch (members[0].MemberType)
        {
            //成员变量信息 
            case MemberTypes.Field:
                return type.GetField(memberName, bindingFlags).GetValue(obj);
           
            //属性信息
            case MemberTypes.Property:
                return type.GetProperty(memberName, bindingFlags).GetValue(obj);
                
            default:
                return null;
        }
    }

    /// <summary>
    /// 反射创建类的实例
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static object CreateClass(string name)
    {
        object obj = null;
        Type type = null;
        //遍历程序集
        foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            //获取类名
            Type tempType = asm.GetType(name);
            if (tempType != null)
            {
                type = tempType;
                break;
            }
        }
        if (type != null)
        {
            //实例化类
            obj = Activator.CreateInstance(type);
        }
        return obj;
    }

    /// <summary>
    /// Xml转二进制
    /// </summary>
    /// <param name="name"></param>
    private static void XmlToBinary(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        try
        {
            Type type = null;

            //遍历程序集
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                //获取类名
                Type tempType = asm.GetType(name);

                if (tempType != null)
                {
                    type = tempType;
                    break;
                }
            }
            if (type != null)
            {
                string xmlPath = XmlPath + name + ".xml";
                string binaryPath = BinaryPath + name + ".bytes";
                object obj = BinarySerializeOpt.XmlDeserialize(xmlPath, type);
                BinarySerializeOpt.BinarySerialize(binaryPath, obj);
                Debug.Log("Xml转二进制成功，二进制路径为 " + binaryPath);
            }
        }
        catch
        {
            Debug.LogError("Xml转二进制失败！ " + name);
        }
    }

    /// <summary>
    /// 实际的类转Xml
    /// </summary>
    /// <param name="name"></param>
    private static void ClassToXml(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        try
        {
            Type type = null;

            //遍历程序集
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                //获取类名
                Type tempType = asm.GetType(name);

                if (tempType != null)
                {
                    type = tempType;
                    break;
                }
            }
            if (type != null)
            {
                //实例化类
                var temp = Activator.CreateInstance(type);

                if (temp is ExcelBase)
                {
                    (temp as ExcelBase).Construction();
                }
                string xmlPath = XmlPath + name + ".xml";
                BinarySerializeOpt.Xmlserialize(xmlPath, temp);
                Debug.Log("类转Xml成功，xml路径为 " + xmlPath);
            }
        }
        catch
        {
            Debug.LogError("类转Xml失败： " + name);
        }    
    }
}

/// <summary>
/// 测试类
/// </summary>
public class TestInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsA { get; set; }
    public float Heigh { get; set; }
    public TestEnum TestType { get; set; }
    public List<string> AllStrList { get; set; }
    public List<TestInfoTwo> AllTestInfoList { get; set; }
}

public class SheetClass
{
    //所属父级 Var 变量
    public VarClass ParentVar { get; set; }
    //类名
    public string Name { get; set; }
    //类对应的sheet名
    public string SheetName { get; set; }
    //主键
    public string MainKey { get; set; }
    //分隔符
    public string SplitStr { get; set; }
    //所包含的变量
    public List<VarClass> VarList = new List<VarClass>();
}

//xml 对应 的类结构
public class VarClass
{
    //原类里面变量名称
    public string Name { get; set; }
    //变量类型
    public string Type { get; set; } 
    //变量对应Excel 里的列
    public string Col { get; set; }
    //变量的默认值
    public string DeafultValue { get; set; }  
    //变量是List的话 外联部分列
    public string Foregin { get; set; }
    //分隔符
    public string SplitStr { get; set; }
}

//sheet 表里变量具体的值
public class SheetData
{   //Excel 所有的列名
    public List<string> AllName = new List<string>();
    //Excel 所有的行名
    public List<string> AllType = new List<string>();
    //有多少行这样的数据
    public List<RowData> AllData = new List<RowData>();
}

public class RowData
{
    public Dictionary<string, string> RowDataDic = new Dictionary<string, string>();
}

public enum TestEnum
{
    None = 0,
    Var = 1,
    Test = 2,
}

public class TestInfoTwo
{
    public int Id { get; set; }
    public string Name { get; set; }
}