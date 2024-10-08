using System.Collections.Concurrent;

namespace EasilyNET.Core.Threading;

/// <summary>
/// ͬ���ź�����
/// </summary>
internal sealed class SyncSemaphore
{
    private readonly ConcurrentQueue<ManualResetEventSlim> _waiters = [];
    private int _isTaken;

    /// <summary>
    /// ��ȡ�Ƿ�ռ��
    /// </summary>
    /// <returns></returns>
    public int GetTaken() => _isTaken;

    /// <summary>
    /// ��ȡ�ȴ�����������
    /// </summary>
    /// <returns></returns>
    public int GetQueueCount() => _waiters.Count;

    /// <summary>
    /// ͬ���ȴ�
    /// </summary>
    public void Wait()
    {
        // ��� _isTaken ��ֵ�� 0����������Ϊ 1�������ء�
        if (Interlocked.CompareExchange(ref _isTaken, 1, 0) == 0)
        {
            return;
        }
        // ��� _isTaken ��ֵ���� 0������һ���µ� ManualResetEventSlim������������Ϊδ��ֹ״̬��
        var mre = new ManualResetEventSlim(false);
        // �� ManualResetEventSlim ʵ����ӵ��ȴ������С�
        _waiters.Enqueue(mre);
        // �ȴ� ManualResetEventSlim ����ֹ��
        mre.Wait();
    }

    /// <summary>
    /// �ͷ�
    /// </summary>
    public void Release()
    {
        if (_waiters.TryDequeue(out var toRelease))
        {
            toRelease.Set();
        }
        else
        {
            Interlocked.Exchange(ref _isTaken, 0);
        }
    }
}