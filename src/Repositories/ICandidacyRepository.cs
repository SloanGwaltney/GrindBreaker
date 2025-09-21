using GrindBreaker.Models;

namespace GrindBreaker.Repositories
{
    public interface ICandidacyRepository
    {
        List<Candidacy> GetAllCandidacies();
        Candidacy? GetCandidacy(string id);
        bool SaveCandidacy(Candidacy candidacy);
        bool UpdateCandidacy(Candidacy candidacy);
        bool DeleteCandidacy(string id);
        bool UpdateCandidacyStatus(string id, CandidacyStatus status);
    }
}
