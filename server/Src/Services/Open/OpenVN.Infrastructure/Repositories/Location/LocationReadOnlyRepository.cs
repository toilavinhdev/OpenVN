using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;

namespace OpenVN.Infrastructure
{
    public class LocationReadOnlyRepository : BaseReadOnlyRepository<BaseLocation>, ILocationReadOnlyRepository
    {
        public LocationReadOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IServiceProvider provider) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public async Task<IEnumerable<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken)
        {
            var cacheKey = string.Format(BaseCacheKeys.GetSystemFullRecordsKey(_tableName), new LocationProvince().GetTableName());
            var cacheData = await _sequenceCaching.GetAsync<IEnumerable<ProvinceDto>>(cacheKey, CachingType.Memory, cancellationToken: cancellationToken);
            if (cacheData != null && cacheData.Any())
            {
                return cacheData;
            }

            var cmd = $@"SELECT lp.Id, lp.Name, lp.Type, lp.Slug, COUNT(lp.Id) AS 'ChildrenCount' 
                        FROM {new LocationProvince().GetTableName()} lp INNER JOIN location_district ld ON lp.Id = ld.ProvinceId 
                        GROUP BY lp.Id
                        ORDER BY FIELD(lp.Slug, 'CANTHO', 'DANANG', 'HAIPHONG', 'HOCHIMINH', 'HANOI') DESC, lp.Type, lp.Name";
            var result = await _dbConnection.QueryAsync<ProvinceDto>(cmd);
            if (result != null && result.Any())
            {
                await _sequenceCaching.SetAsync(cacheKey, result, TimeSpan.FromDays(30), onlyUseType: CachingType.Memory);
            }
            return result;
        }

        public async Task<IEnumerable<DistrictDto>> GetDistrictsAsync(string provinceId, CancellationToken cancellationToken)
        {
            var cacheKey = BaseCacheKeys.GetSystemRecordByForeignIdKey(new LocationDistrict().GetTableName(), provinceId);
            var cacheData = await _sequenceCaching.GetAsync<IEnumerable<DistrictDto>>(cacheKey, CachingType.Memory, cancellationToken: cancellationToken);
            if (cacheData != null && cacheData.Any())
            {
                return cacheData;
            }

            var cmd = $@"SELECT ld.Id, ld.Name, ld.Type, ld.ProvinceId, COUNT(lw.Id) AS 'ChildrenCount'  
                        FROM {new LocationDistrict().GetTableName()} ld INNER JOIN {new LocationWard().GetTableName()} lw ON ld.Id = lw.DistrictId 
                        WHERE ld.ProvinceId = @ProvinceId
                        GROUP BY ld.Id
                        ORDER BY ld.Type, ld.Name";
            var result = await _dbConnection.QueryAsync<DistrictDto>(cmd, new { ProvinceId = provinceId });
            if (result != null && result.Any())
            {
                await _sequenceCaching.SetAsync(cacheKey, result, TimeSpan.FromDays(30), onlyUseType: CachingType.Memory);
            }
            return result;
        }

        public async Task<IEnumerable<WardDto>> GetWardsAsync(string districtId, CancellationToken cancellationToken)
        {
            var cacheKey = BaseCacheKeys.GetSystemRecordByForeignIdKey(new LocationWard().GetTableName(), districtId);
            var cacheData = await _sequenceCaching.GetAsync<IEnumerable<WardDto>>(cacheKey, CachingType.Memory, cancellationToken: cancellationToken);
            if (cacheData != null && cacheData.Any())
            {
                return cacheData;
            }

            var cmd = $@"SELECT ld.Id, ld.Name, ld.Type, ld.DistrictId, 0 AS 'ChildrenCount'   
                        FROM {new LocationWard().GetTableName()} ld 
                        WHERE ld.DistrictId = @DistrictId
                        ORDER BY ld.Type, ld.Name";
            var result = await _dbConnection.QueryAsync<WardDto>(cmd, new { DistrictId = districtId });
            if (result != null && result.Any())
            {
                await _sequenceCaching.SetAsync(cacheKey, result, TimeSpan.FromDays(30), onlyUseType: CachingType.Memory);
            }
            return result;
        }

        public async Task<IEnumerable<ProvinceDto>> SearchLocationsAsync(string query, CancellationToken cancellationToken)
        {
            var queryProvinceCmd = $@"SELECT * FROM {new LocationProvince().GetTableName()}";
            var queryDistrictCmd = $@"SELECT * FROM {new LocationDistrict().GetTableName()}";
            var queryWardCmd = $@"SELECT * FROM {new LocationWard().GetTableName()} WHERE REPLACE(REPLACE(Name, 'Đ', 'D'), 'đ', 'd') LIKE CONCAT('%', @Query, '%') ORDER BY Type, Name";

            query = query == null ? "" : query.ToLower();
            query = query.ViToEn();
            query = query.Equals("_") || query.Equals("%") ? $"\\{query}" : query;

            var tasks = new List<Task>();
            var task = _dbConnection.QueryAsync<ProvinceDto>(queryProvinceCmd);
            var task2 = _dbConnection.QueryAsync<DistrictDto>(queryDistrictCmd);
            var task3 = _dbConnection.QueryAsync<WardDto>(queryWardCmd, new { Query = query });

            await Task.WhenAll(task, task2, task3);

            var provinces = (await task).ToList();
            var districts = (await task2).ToList();
            var wards = (await task3).ToList();

            foreach (var district in districts)
            {
                district.Children = wards.Where(x => x.DistrictId == district.Id).ToList();
                district.ChildrenCount = district.Children.Count;
            }

            foreach (var province in provinces)
            {
                province.Children = districts.Where(x => x.ProvinceId == province.Id
                                              && (x.Name.ToLower().ViToEn().Contains(query) || x.Children.Any()))
                                             .OrderBy(x => x.Name)
                                             .ToList();
            }

            var result = provinces.Where(p => p.Name.ToLower().ViToEn().Contains(query) || p.Children.Any() || p.Children.FirstOrDefault(d => d.Children.Any()) != null);
            foreach (var item in result)
            {
                item.ChildrenCount = item.Children.Count;
            }

            return result.OrderBy(x => x.Name).ToList();
        }

        public async Task<PagingResult<RankDto>> GetRankPagingAsync(PagingRequest request, CancellationToken cancellationToken)
        {
            var cmd = $@"SELECT Data as Keyword, COUNT(t.Data) as SearchCount 
                         FROM {new Tracking().GetTableName()} t 
                         WHERE t.EventId = '{TrackingEventType.SEARCH}'
                         GROUP BY t.Data 
                         ORDER BY SearchCount DESC LIMIT {request.Offset}, {request.Size};";

            var countCmd = $@"SELECT COUNT(k.c) FROM (SELECT COUNT(Data) AS c FROM {new Tracking().GetTableName()} 
                              WHERE EventId = '{TrackingEventType.SEARCH}' 
                              GROUP BY Data) AS k";
            var task = _dbConnection.QueryAsync<RankDto>(cmd);
            var task2 = _dbConnection.QuerySingleOrDefaultAsync<int>(countCmd);

            await Task.WhenAll(task, task2);
            return new PagingResult<RankDto>
            {
                Data = await task,
                Count = await task2
            };
        }
    }
}
