using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Patterns.LogParsers
{
    public class MapReduceLogParser : ILogParser
    {
        public KeyValuePair<string, int>[] GetTop10Users(string logPath)
        {
            var usersStats = new ConcurrentDictionary<string, int>();

            Parallel.ForEach(File.ReadLines(logPath),
                () => new Dictionary<string, int>(),
                (line, _, localDictionary) =>
                {
                    var ipInfo = IpInfo.Parse(line);

                    if (!localDictionary.ContainsKey(ipInfo.Ip))
                        localDictionary.Add(ipInfo.Ip, ipInfo.CallDuration);
                    else
                        localDictionary[ipInfo.Ip] += ipInfo.CallDuration;

                    return localDictionary;
                },
                localDictionary =>
                {
                    foreach (var ip in localDictionary.Keys)
                        usersStats.AddOrUpdate(ip, key => localDictionary[key],
                            (key, oldValue) => oldValue + localDictionary[key]);
                }
                );

            return usersStats.OrderByDescending(keyValuePair => keyValuePair.Value).Take(10).ToArray();
        }
    }
}