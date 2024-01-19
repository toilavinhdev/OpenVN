namespace SharedKernel.Application
{
    public static class BaseCacheKeys
    {
        public static string GetAccessTokenKey(object userId) => $"access-token:{userId}";
        public static string GetRevokeAccessTokenKey(string accessToken) => $"revoked-token:{accessToken}";
        public static string GetClientInformationKey(string ip) => $"client-information:{ip}";
        public static string GetConfigKey(object tenantId, object ownerId) => $"config:{tenantId}:{ownerId}";
        public static string GetSystemFullRecordsKey(string tableName) => $"system-full-records:{tableName}";
        public static string GetSystemRecordByIdKey(string tableName, object recordId) => $"system-record-by-id:{tableName}:{recordId}";
        public static string GetSystemRecordByForeignIdKey(string tableName, object foreignKeyId) => $"system-record-by-foreignkey-id:{tableName}:{foreignKeyId}";
        public static string GetFullRecordsKey(string tableName, object tenantId, object ownerId) => $"full-records:{tableName}:{tenantId}:{ownerId}";
        public static string GetRecordByIdKey(string tableName, object recordId, object tenantId, object ownerId) => $"record-by-id:{tableName}:{recordId}:{tenantId}:{ownerId}";
        public static string GetSecretKey(string keyName, object tenantId, object ownerId) => $"{keyName}:{tenantId}:{ownerId}";

    }
}
 