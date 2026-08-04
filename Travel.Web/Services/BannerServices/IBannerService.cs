using Travel.Web.DTOs.BannerDtos;

namespace Travel.Web.Services.BannerServices
{
    public interface IBannerService
    {
        Task<List<ResultBannerDto>> GetAllAsync();

        Task CreateAsync(CreateBannerDto createBannerDto);

        Task DeleteAsync(string id);
        Task UpdateAsync(UpdateBannerDto updateBannerDto);
    }
}
