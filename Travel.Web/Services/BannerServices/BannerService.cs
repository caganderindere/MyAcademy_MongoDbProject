using MongoDB.Driver;
using Travel.Web.DTOs.BannerDtos;
using Travel.Web.Entities;
using Travel.Web.Settings;

namespace Travel.Web.Services.BannerServices
{
    public class BannerService : IBannerService
    {
        private readonly IMongoCollection<Banner> _BannerCollection;

            public BannerService(IDataBaseSettings dataBaseSettings)
        {
            var client = new MongoClient(dataBaseSettings.ConnectionString);
            var database = client.GetDatabase(dataBaseSettings.DatabaseName);
            _BannerCollection= database.GetCollection<Banner>(dataBaseSettings.BannerColectionName);

        }

        public Task CreateAsync(CreateBannerDto createBannerDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ResultBannerDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UpdateBannerDto updateBannerDto)
        {
            throw new NotImplementedException();
        }
    }
}
