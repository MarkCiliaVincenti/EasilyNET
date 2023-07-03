using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace EasilyNET.MongoGridFS.AspNetCore;

/// <summary>
/// ������չ
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// ʹ�� <see cref="MongoClientSettings" /> ������MongoGridFS
    /// </summary>
    /// <param name="services"></param>
    /// <param name="mongoSettings"></param>
    /// <param name="dbName">���ݿ�����</param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddMongoGridFS(this IServiceCollection services, MongoClientSettings mongoSettings, string? dbName = null, Action<GridFSBucketOptions>? configure = null)
    {
        var db = new MongoClient(mongoSettings).GetDatabase(dbName ?? Constant.DefaultDbName);
        services.AddMongoGridFS(db, configure);
        return services;
    }

    /// <summary>
    /// ʹ�������ַ��ķ�ʽ����MongoGridFS
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString">���ݿ������ַ���</param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddMongoGridFS(this IServiceCollection services, string connectionString, Action<GridFSBucketOptions>? configure = null)
    {
        var url = new MongoUrl(connectionString);
        var name = string.IsNullOrWhiteSpace(url.DatabaseName) ? Constant.DefaultDbName : url.DatabaseName;
        var db = new MongoClient(url).GetDatabase(name);
        services.AddMongoGridFS(db, configure);
        return services;
    }

    /// <summary>
    /// ʹ�����е� IMongoDatabase
    /// </summary>
    /// <param name="services"></param>
    /// <param name="db"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddMongoGridFS(this IServiceCollection services, IMongoDatabase db, Action<GridFSBucketOptions>? configure = null)
    {
        services.AddMongoGridFS(db, Constant.ConfigName, c =>
        {
            c.BucketName = Constant.BucketName;
            c.ChunkSizeBytes = 1024;
            c.DisableMD5 = true;
            c.ReadConcern = new();
            c.ReadPreference = ReadPreference.Primary;
            c.WriteConcern = WriteConcern.Unacknowledged;
            configure?.Invoke(c);
        });
        return services;
    }

    /// <summary>
    /// ͨ�� <see cref="IMongoDatabase" /> ע�� <see cref="IGridFSBucket" />
    /// </summary>
    /// <param name="services"></param>
    /// <param name="db"></param>
    /// <param name="name">ConfigureName</param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddMongoGridFS(this IServiceCollection services, IMongoDatabase db, string name, Action<GridFSBucketOptions> configure)
    {
        services.Configure(name, configure);
        services.TryAddSingleton<IGridFSBucketFactory, GridFSBucketFactory>();
        services.TryAddSingleton(sp => sp.GetRequiredService<IGridFSBucketFactory>().CreateBucket(db));
        return services;
    }
}