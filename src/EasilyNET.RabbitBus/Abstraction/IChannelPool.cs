﻿using RabbitMQ.Client;

namespace EasilyNET.RabbitBus.Abstraction;

internal interface IChannelPool : IDisposable
{
    /// <summary>
    /// 从池中获取Channel
    /// </summary>
    /// <returns></returns>
    public IModel BorrowChannel();

    /// <summary>
    /// 归还Channel到池或者释放
    /// </summary>
    /// <param name="channel"></param>
    public void RepaidChannel(IModel channel);
}