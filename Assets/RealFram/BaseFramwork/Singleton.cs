using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 单例基类
/// </summary>
/// <typeparam name=""></typeparam>
public class Singleton<T> where T : new() 
{
    private static T m_Instance;
    public static T Instance 
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = new T();
            }
            return m_Instance;
        }
    }

}
