namespace SP.Common;

// 雪花算法
public class Snow
{
    private static long _lastTimestamp = -1L;
    private static long _sequence = 0L;
    private static readonly object _lock = new object();

    // 获取生成的ID
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
        long id = ((timestamp << 22) | (dataCenterId << 17) | (machineId << 12) | sequence);
        return id;
    }

    // 获取机器ID
    private static long GetMachineId()
    {
        // 获取mac地址
        var macAddress = System.Net.NetworkInformation.NetworkInterface
            .GetAllNetworkInterfaces()
            .FirstOrDefault(n => n.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
            ?.GetPhysicalAddress()
            ?.ToString();
        // 计算机器ID
        long machineId = 0;
        if (macAddress != null)
        {
            for (int i = 0; i < macAddress.Length; i += 2)
            {
                machineId = (machineId << 8) | Convert.ToByte(macAddress.Substring(i, 2), 16);
            }
        }

        // 取模，确保机器ID在0-31之间
        machineId = machineId % 32;
        return machineId;
    }

    // 获取数据中心ID
    // 获取数据中心ID
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
    // 获取序列号
    private static long GetSequence()
    {
        lock (_lock)
        {
            // 获取当前时间戳
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

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