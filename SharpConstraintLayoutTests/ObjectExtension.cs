using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


public static class ObjectExtensio
{
    /// <summary>
    /// https://stackoom.com/question/ZEZ
    /// </summary>
    /// <param name="o"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object RunMethod(this object o, string methodName, params object[] args)
    {
        MethodInfo mi = null;
        var parasCount = args == null ? 0 : args.Length;
        var mis = o.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (var m in mis)//区分多个同名但参数数目不同的方法
        {
            if (m.Name == methodName && m.GetParameters().Length == parasCount)
            {
                mi = m;
                break;
            }
        }

        //mi = o.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (mi != null)
        {
            return mi.Invoke(o, args);
        }
        return null;
    }

    public static T RunMethod<T>(this object o, string methodName, params object[] args)
    {
        return (T)RunMethod(o, methodName, args);
    }

    /// <summary>
    /// https://blog.lindexi.com/post/C-%E4%BD%BF%E7%94%A8%E5%8F%8D%E5%B0%84%E8%8E%B7%E5%8F%96%E7%A7%81%E6%9C%89%E5%B1%9E%E6%80%A7%E7%9A%84%E6%96%B9%E6%B3%95.html
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="o"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static T GetFieldValue<T>(this object o, string fieldName)
    {
        var type = o.GetType();
        const BindingFlags InstanceBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo field = type.GetField(fieldName, InstanceBindFlags);
        if (field == null)//如果在这个类中找不到,查找基类
        {
            while (type != null)
            {
                field = type.GetField(fieldName, InstanceBindFlags);
                if (field != null)
                {
                    break;
                }

                type = type.BaseType;
            }

            if (field == null)//如果所有基类中也找不到就抛出异常
            {
                throw new MissingFieldException(fieldName);
            }
        }

        return (T)(field.GetValue(o));
    }

    public static void SetFieldValue<T>(this object o, string fieldName, T value)
    {
        var type = o.GetType();
        const BindingFlags InstanceBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo field = type.GetField(fieldName, InstanceBindFlags);
        if (field == null)//如果在这个类中找不到,查找基类
        {
            while (type != null)
            {
                field = type.GetField(fieldName, InstanceBindFlags);
                if (field != null)
                {
                    break;
                }

                type = type.BaseType;
            }

            if (field == null)//如果所有基类中也找不到就抛出异常
            {
                throw new MissingFieldException(fieldName);
            }
        }
        field.SetValue(o, value);
    }

    public static T GetPropertyValue<T>(this object o, string propertyName)
    {
        var type = o.GetType();
        const BindingFlags InstanceBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        PropertyInfo property = type.GetProperty(propertyName, InstanceBindFlags);
        if (property == null)//如果在这个类中找不到,查找基类
        {
            while (type != null)
            {
                property = type.GetProperty(propertyName, InstanceBindFlags);
                if (property != null)
                {
                    break;
                }

                type = type.BaseType;
            }

            if (property == null)//如果所有基类中也找不到就抛出异常
            {
                throw new MissingFieldException(propertyName);
            }
        }

        return (T)(property.GetValue(o));
    }
}

