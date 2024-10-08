// ReSharper disable UnusedMember.Global

namespace EasilyNET.Core.Threading;

/// <summary>
/// ͬ����,���ڽ���һ���̷߳��ʹ�����Դ��
/// </summary>
public sealed class SyncLock
{
    private readonly SyncSemaphore _semaphore = new();
    private int _ownerThreadId = -1;

    /// <summary>
    /// ��ȡ�ڲ��ź�����ռ��״̬
    /// </summary>
    /// <returns></returns>
    public int GetSemaphoreTaken() => _semaphore.GetTaken();

    /// <summary>
    /// ��ȡ�ڲ��ź����Ķ��м���
    /// </summary>
    /// <returns></returns>
    public int GetQueueCount() => _semaphore.GetQueueCount();

    /// <summary>
    /// ����,����һ�� <see cref="Release" /> ����
    /// </summary>
    /// <returns></returns>
    public Release Lock()
    {
        var currentThreadId = Environment.CurrentManagedThreadId;
        if (_ownerThreadId == currentThreadId)
        {
            throw new InvalidOperationException("Reentrant lock detected");
        }
        _semaphore.Wait();
        _ownerThreadId = currentThreadId;
        return new(this);
    }

    /// <summary>
    /// ���������ִ��,�޷���ֵ
    /// </summary>
    /// <param name="action"></param>
    public void Lock(Action action)
    {
        using var r = Lock();
        action();
    }

    /// <summary>
    /// ���������ִ��,�ɷ���ִ�к����Ľ��
    /// </summary>
    /// <param name="action"></param>
    public T Lock<T>(Func<T> action)
    {
        using var r = Lock();
        return action();
    }

    /// <remarks>
    /// Release
    /// </remarks>
    /// <param name="syncLock"></param>
    public readonly struct Release(SyncLock syncLock) : IDisposable
    {
        /// <inheritdoc />
        public void Dispose()
        {
            syncLock._ownerThreadId = -1;
            syncLock._semaphore.Release();
        }
    }
}