namespace SP.Common;

/// <summary>
/// 雪花算法
/// </summary>
public class Snow
{
    private static long _lastTimestamp = -1L;
    private static long _sequence = 0L;
    private static readonly object _lock = new object();

    /// <summary>
    /// 获取生成的ID
    /// </summary>
    /// <returns></returns>
    public static long GetId()
    {
        // 获取当前时间戳
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        // 获取机器ID
        long machineId = GetMachineId();
        // 获取数据中心ID
        long dataCenterId = GetDataCenterId();
        // 获取序列号
        long sequence = GetSequence();
        // 组合ID
        long id = ((timestamp & 0x1FFFFFFFFFF) << 22) | ((dataCenterId & 0x1F) << 17) | ((machineId & 0x1F) << 12) |
                  (sequence & 0xFFF);
        return id;
    }

    /// <summary>
    /// 获取机器ID
    /// </summary>
    /// <returns></returns>
    private static long GetMachineId()
    {
        try
        {
            // 尝试通过MAC地址获取
            var macAddress = System.Net.NetworkInformation.NetworkInterface
                .GetAllNetworkInterfaces()
                .FirstOrDefault(n => n.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                ?.GetPhysicalAddress()
                ?.ToString();
            if (!string.IsNullOrEmpty(macAddress))
            {
                // 计算机器ID
                long machineId = 0;
                for (int i = 0; i < macAddress.Length; i += 2)
                {
                    machineId = (machineId << 8) | Convert.ToByte(macAddress.Substring(i, 2), 16);
                }
                // 取模，确保机器ID在0-31之间
                return machineId % 32;
            }
            else
            {
                // 备选方案：使用主机名哈希
                return Math.Abs(Environment.MachineName.GetHashCode()) % 32;
            }
        }
        catch
        {
            // 异常情况下的备选方案
            return Math.Abs(Environment.MachineName.GetHashCode()) % 32;
        }
    }

    /// <summary>
    /// 获取数据中心ID
    /// </summary>
    /// <returns></returns>
    private static long GetDataCenterId()
    {
        // 从环境变量中获取数据中心ID
        string? dataCenterIdEnv = Environment.GetEnvironmentVariable("DATA_CENTER_ID");
        long dataCenterId = 1; // 默认值

        if (!string.IsNullOrEmpty(dataCenterIdEnv) && long.TryParse(dataCenterIdEnv, out long parsedId))
        {
            dataCenterId = parsedId;
        }

        // 取模，确保数据中心ID在0-31之间
        dataCenterId = dataCenterId % 32;
        return dataCenterId;
    }

    /// <summary>
    /// 获取序列号
    /// </summary>
    /// <returns></returns>
    private static long GetSequence()
    {
        lock (_lock)
        {
            // 获取当前时间戳
            long currentTimestamp = DateTimeOffset.UtcNow.Ticks / 10000;
            if (currentTimestamp == _lastTimestamp)
            {
                // 同一毫秒内，递增序列号
                _sequence = (_sequence + 1) & 0xFFF; // 序列号取模 4096
                if (_sequence == 0)
                {
                    // 如果序列号溢出，等待下一毫秒
                    while (currentTimestamp <= _lastTimestamp)
                    {
                        currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    }
                }
            }
            else
            {
                // 不同毫秒，重置序列号
                _sequence = 0;
            }

            _lastTimestamp = currentTimestamp;
            return _sequence;
        }
    }
}