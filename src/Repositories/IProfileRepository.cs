using GrindBreaker.Models;

namespace GrindBreaker.Repositories
{
    public interface IProfileRepository
    {
        Profile? GetProfile();
        bool SaveProfile(Profile profile);
        bool DeleteProfile();
    }
}
