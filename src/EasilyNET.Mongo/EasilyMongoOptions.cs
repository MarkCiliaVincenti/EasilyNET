﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
#if !NETSTANDARD
#pragma warning disable CA1822
#endif

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace EasilyNET.Mongo;

/// <summary>
/// Mongodb配置选项
/// </summary>
public sealed class EasilyMongoOptions
{
    private static bool TypesFirst { get; set; }

    /// <summary>
    /// ObjectId到String转换的类型[该列表中的对象,不会将Id,ID字段转化为ObjectId类型.在数据库中存为字符串格式]
    /// </summary>
    // ReSharper disable once CollectionNeverUpdated.Global
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    internal static List<Type> ObjIdToStringTypes { get; } = new();

    /// <summary>
    /// ObjectId到String转换的类型[该列表中的对象,不会将Id,ID字段转化为ObjectId类型.在数据库中存为字符串格式]
    /// </summary>
    // ReSharper disable once MemberCanBeMadeStatic.Global
    public List<Type> ObjectIdToStringTypes
    {
        get => ObjIdToStringTypes;
        set
        {
            if (TypesFirst) throw new("请在应用启动的时候初始化,不要动态调整.");
            TypesFirst = true;
            ObjIdToStringTypes.AddRange(value);
        }
    }

    /// <summary>
    /// 是否使用本库提供的默认转换,默认:true
    /// 1.驼峰名称格式
    /// 2.忽略代码中未定义的字段
    /// 3._id映射为实体中的ID或者Id,反之亦然
    /// 4.将枚举类型存储为字符串格式
    /// </summary>
    public bool DefaultConventionRegistry { get; set; } = true;

    internal Dictionary<string, ConventionPack> ConventionRegistry { get; } = new()
    {
        {
            $"{Constant.Pack}-{ObjectId.GenerateNewId()}", new()
            {
                new CamelCaseElementNameConvention(),             // 驼峰名称格式
                new IgnoreExtraElementsConvention(true),          // 忽略掉实体中不存在的字段
                new NamedIdMemberConvention("Id", "ID"),          // _id映射为实体中的ID或者Id
                new EnumRepresentationConvention(BsonType.String) // 将枚举类型存储为字符串格式
            }
        }
    };

    /// <summary>
    /// 添加自己的一些Convention配置,用于设置mongodb序列化反序列化的一些表现.
    /// </summary>
    /// <param name="dic"></param>
    // ReSharper disable once UnusedMember.Global
    public void AppendConventionRegistry(Dictionary<string, ConventionPack> dic)
    {
        foreach (var item in dic) ConventionRegistry.Add(item.Key, item.Value);
    }
}