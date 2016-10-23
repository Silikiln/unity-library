using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Reflection;
using System.Linq;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
public class XmlParseAttribute : Attribute {
    private string name;

    public XmlParseAttribute()
    {
        name = null;
    }

    public XmlParseAttribute(string name)
    {
        this.name = name;
    }

    static bool TryGetAttribute(MemberInfo member, out XmlParseAttribute parseAttr)
    {
        foreach (object attr in member.GetCustomAttributes(false))
            if (attr is XmlParseAttribute)
            {
                parseAttr = attr as XmlParseAttribute;
                return true;
            }
        parseAttr = null;
        return false;
    }

    static Dictionary<string, MemberInfo> GenerateMemberDictionary(Type type)
    {
        Dictionary<string, MemberInfo> memberDict = new Dictionary<string, MemberInfo>();
        XmlParseAttribute attr;

        foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (TryGetAttribute(fieldInfo, out attr))
            {
                if (attr.name == null && fieldInfo.FieldType.IsClass)
                {
                    if (TryGetAttribute(fieldInfo.FieldType, out attr))
                        memberDict.Add(attr.name, fieldInfo);
                }
                else
                    memberDict.Add(attr.name, fieldInfo);
            }
        }

        foreach (PropertyInfo propertyInfo in type.GetProperties())
            if (TryGetAttribute(propertyInfo, out attr))
            {
                if (attr.name == null && propertyInfo.PropertyType.IsClass)
                {
                    if (TryGetAttribute(propertyInfo.PropertyType, out attr))
                        memberDict.Add(attr.name, propertyInfo);
                }
                else
                    memberDict.Add(attr.name, propertyInfo);
            }

        return memberDict;
    }
    static Dictionary<string, MemberInfo> GenerateMemberDictionary<T>()
    {
        return GenerateMemberDictionary(typeof(T));
    }

    static Dictionary<Type, Func<string, object>> parseMethods = new Dictionary<Type, Func<string, object>>
    {
        { typeof(bool), data => float.Parse(data) },

        { typeof(sbyte), data => sbyte.Parse(data) },
        { typeof(byte), data => byte.Parse(data) },
        { typeof(short), data => short.Parse(data) },
        { typeof(ushort), data => ushort.Parse(data) },
        { typeof(int), data => int.Parse(data) },
        { typeof(uint), data => uint.Parse(data) },
        { typeof(long), data => long.Parse(data) },
        { typeof(ulong), data => ulong.Parse(data) },
        { typeof(float), data => float.Parse(data) },
        { typeof(double), data => double.Parse(data) },

        { typeof(char), data => char.Parse(data) },
    };

    static void ConvertToMemberType(object targetObject, MemberInfo memberInfo, XmlReader reader) {
        Type type;
        if (memberInfo is FieldInfo)
            type = (memberInfo as FieldInfo).FieldType;
        else
            type = (memberInfo as PropertyInfo).PropertyType;

        object value;
        if (type == typeof(string))
            value = reader.ReadInnerXml();
        else if (type.IsSubclassOf(typeof(Enum)))
        {
            string enumString = reader.ReadInnerXml();
            try
            {
                value = Convert.ChangeType(Enum.Parse(type, enumString), type);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Could not parse enum data, \"" + enumString + "\" is not valid");
            }
        }
        else if (type.GetInterfaces().Contains(typeof(IEnumerable)))
        {
            Type enumerableType = type.GetGenericArguments()[0];
            List<object> data = ParseList(enumerableType, reader.ReadSubtree());
            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(enumerableType));
            foreach (object obj in data)
                (list as IList).Add(Convert.ChangeType(obj, enumerableType));
            value = list;
        }
        else if (type.IsClass)
        {
            Debug.Log("Class found");
            value = ParseType(type, reader);
        }
        else
            value = parseMethods[type](reader.ReadInnerXml());

        if (memberInfo is FieldInfo)
            (memberInfo as FieldInfo).SetValue(targetObject, value);
        else
            (memberInfo as PropertyInfo).SetValue(targetObject, value, null);
    }

    static List<object> ParseList(Type enumerableType, XmlReader reader)
    {
        List<object> list = new List<object>();
        XmlParseAttribute attr;
        if (!TryGetAttribute(enumerableType, out attr) && !parseMethods.ContainsKey(enumerableType))
            return list;

        string itemTag = attr != null ? attr.name : "Item";
        Dictionary<string, MemberInfo> memberDict = GenerateMemberDictionary(enumerableType);

        reader.ReadToFollowing(itemTag);
        while (!reader.EOF)
        {
            object obj;
            if (attr != null)
                obj = ParseType(enumerableType, reader, memberDict);
            else
                obj = parseMethods[enumerableType](reader.ReadInnerXml());
            if (obj != null)
                list.Add(obj);
            reader.ReadToFollowing(itemTag);
        }

        reader.Close();

        return list;
    }

    static object ParseType(Type type, XmlReader reader, Dictionary<string, MemberInfo> memberDict = null)
    {
        XmlParseAttribute attr;
        object obj = Activator.CreateInstance(type);
        if (!TryGetAttribute(type, out attr))
            return obj;

        string parentTag = attr.name;
        if (memberDict == null)
            memberDict = GenerateMemberDictionary(type);

        reader.Read();
        while (!reader.EOF && reader.Name != parentTag)
        {
            if (reader.Name.Length > 0 && reader.NodeType != XmlNodeType.EndElement)
            {
                if (memberDict.ContainsKey(reader.Name))
                    ConvertToMemberType(obj, memberDict[reader.Name], reader);
                else
                    reader.ReadToNextSibling(reader.Name);
            }
            else
                reader.Read();
        } 

        return obj;
    }

    public static T ReadFile<T>(string fileName) where T : new()
    {
        T obj;
        using (XmlReader reader = XmlReader.Create(fileName))
            obj = (T)ParseType(typeof(T), reader);
        return obj;
    }

    public static List<T> ReadFileIntoList<T>(string fileName) where T : new()
    {
        List<T> list = new List<T>();

        XmlParseAttribute attr;
        if (!TryGetAttribute(typeof(T), out attr))
            return list;

        Dictionary<string, MemberInfo> memberDict = GenerateMemberDictionary<T>();

        using (XmlReader reader = XmlReader.Create(fileName))
        {
            reader.ReadToFollowing(attr.name);
            while (!reader.EOF)
            {
                T obj = (T)ParseType(typeof(T), reader, memberDict);
                if (obj != null)
                    list.Add(obj);
                reader.ReadToFollowing(attr.name);
            }
        }

        return list;
    }
}
