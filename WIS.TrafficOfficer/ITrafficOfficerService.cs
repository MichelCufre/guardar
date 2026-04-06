using System.Collections.Generic;

namespace WIS.TrafficOfficer
{
    public interface ITrafficOfficerService
    {
        string CreateToken();
        string AddLock(string entity, string key, TrafficOfficerTransaction transaction = default, bool isGlobal = false);
        bool IsLocked(string entity, string key, bool global = false);
        bool ClearToken(bool includeGlobal = false);
        string TransferToken(string destination);
        TrafficOfficerTransaction CreateTransaccion();
        bool DeleteTransaccion(TrafficOfficerTransaction transaction);
        bool RemoveLockByIdLock(string entity, string idLock, int userId);
        bool RemoveLockListByIdLock(string entity, List<string> idLocks, int userId);
        string AddLockList(string entity, List<string> keys, TrafficOfficerTransaction transaction = null, bool isGlobal = false);
        List<ItemLock> GetLockList(string entity, List<string> keys, TrafficOfficerTransaction transaction = null, bool isGlobal = false);
        List<ItemLock> GetLockListWithKeyPrefixes(string entity, List<string> keys, TrafficOfficerTransaction transaction = default);
    }
}
