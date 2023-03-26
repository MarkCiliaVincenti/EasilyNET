﻿using EasilyNET.AutoDependencyInjection.Contexts;
using EasilyNET.AutoDependencyInjection.Extensions;
using EasilyNET.AutoDependencyInjection.Modules;
using EasilyNET.Core.BaseType;
using EasilyNET.Mongo;
using EasilyNET.Mongo.ConsoleDebug;
using EasilyNET.Mongo.Extension;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace WebApi.Test.Unit;

/// <summary>
/// MongoDB驱动模块
/// </summary>
public class MongoModule : AppModule
{
    /// <summary>
    /// 配置和注册服务
    /// </summary>
    /// <param name="context"></param>
    public override void ConfigureServices(ConfigureServicesContext context)
    {
        var config = context.Services.GetConfiguration();
        var provider = context.Services.BuildServiceProviderFromFactory();
        context.Services.AddMongoContext<DbContext>(provider, new MongoClientSettings
        {
            Servers = new List<MongoServerAddress> { new("127.0.0.1", 27018) },
            Credential = MongoCredential.CreateCredential("admin", "oneblogs", "&oneblogs789")
            // 新版驱动使用V3版本,有可能会出现一些Linq表达式客户端函数无法执行,需要调整代码,但是工作量太大了,所以可以先使用V2兼容.
            //LinqProvider = LinqProvider.V3
            // 对接 SkyAPM 的 MongoDB探针
            //ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber())
        }, c =>
        {
            c.DatabaseName = "test23";
            c.Options = op =>
            {
                // 配置不需要将Id字段存储为ObjectID的类型.使用$unwind操作符的时候,ObjectId在转换上会有一些问题.
                op.ObjectIdToStringTypes = new() { typeof(MongoTest2) };
                // 是否使用HoyoMongo的一些默认转换配置.包含如下内容:
                // 1.小驼峰字段名称 如: pageSize ,linkPhone 
                // 2.忽略代码中未定义的字段
                // 3.将ObjectID字段 _id 映射到实体中的ID或者Id字段,反之亦然.在存入数据的时候将Id或者ID映射为 _id
                // 4.将枚举类型存储为字符串, 如: Gender.男 存储到数据中为 男,而不是 int 类型
                op.DefaultConventionRegistry = true;
                op.AppendConventionRegistry(new()
                {
                    {
                        $"{SnowId.GenerateNewId()}",
                        new() { new IgnoreIfDefaultConvention(true) }
                    }
                });
            };
            // EasilyNETMongoParams.Options 中的 LinqProvider, ClusterBuilder
            // 会覆盖 MongoClientSettings 中的 LinqProvider 和 ClusterConfigurator 的值,
            // 所以使用MongoClientSettings注册服务时,可仅赋值其中一个
            c.LinqProvider = LinqProvider.V3;
            //c.ClusterBuilder = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
            c.ClusterBuilder = cb => cb.Subscribe(new ActivityEventSubscriber());
            // 传递DbContext构造函数的参数.
            //c.ContextParams = new() { "DbContext测试参数" };
        }).AddMongoContext<DbContext2>(provider, config, c =>
        {
            c.Options = op =>
            {
                op.DefaultConventionRegistry = true;
                op.AppendConventionRegistry(new()
                {
                    {
                        $"{SnowId.GenerateNewId()}",
                        new() { new IgnoreIfDefaultConvention(true) }
                    }
                });
            };
            c.LinqProvider = LinqProvider.V3;
            c.ClusterBuilder = cb => cb.Subscribe(new ActivityEventSubscriber());
        }).RegisterEasilyNETSerializer();
    }
}