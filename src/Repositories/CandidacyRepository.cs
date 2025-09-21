using Newtonsoft.Json;
using GrindBreaker.Models;

namespace GrindBreaker.Repositories
{
    public class CandidacyRepository : ICandidacyRepository
    {
        private readonly string _dataPath;
        private const string CandidaciesFileName = "candidacies.json";

        public CandidacyRepository(string? dataPath = null)
        {
            _dataPath = dataPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GrindBreaker");
            
            // Ensure the data directory exists
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
        }

        public List<Candidacy> GetAllCandidacies()
        {
            var filePath = Path.Combine(_dataPath, CandidaciesFileName);
            
            if (!File.Exists(filePath))
            {
                return new List<Candidacy>();
            }

            try
            {
                var json = File.ReadAllText(filePath);
                var candidacies = JsonConvert.DeserializeObject<List<Candidacy>>(json);
                return candidacies ?? new List<Candidacy>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading candidacies: {ex.Message}");
                return new List<Candidacy>();
            }
        }

        public Candidacy? GetCandidacy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var candidacies = GetAllCandidacies();
            return candidacies.FirstOrDefault(c => c.Id == id);
        }

        public bool SaveCandidacy(Candidacy candidacy)
        {
            if (candidacy == null)
            {
                return false;
            }

            try
            {
                var candidacies = GetAllCandidacies();
                candidacies.Add(candidacy);
                return SaveCandidaciesToFile(candidacies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving candidacy: {ex.Message}");
                return false;
            }
        }

        public bool UpdateCandidacy(Candidacy candidacy)
        {
            if (candidacy == null || string.IsNullOrEmpty(candidacy.Id))
            {
                return false;
            }

            try
            {
                var candidacies = GetAllCandidacies();
                var index = candidacies.FindIndex(c => c.Id == candidacy.Id);
                
                if (index == -1)
                {
                    return false;
                }

                candidacies[index] = candidacy;
                return SaveCandidaciesToFile(candidacies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating candidacy: {ex.Message}");
                return false;
            }
        }

        public bool DeleteCandidacy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            try
            {
                var candidacies = GetAllCandidacies();
                var candidacyToRemove = candidacies.FirstOrDefault(c => c.Id == id);
                
                if (candidacyToRemove == null)
                {
                    return false;
                }

                candidacies.Remove(candidacyToRemove);
                return SaveCandidaciesToFile(candidacies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting candidacy: {ex.Message}");
                return false;
            }
        }

        public bool UpdateCandidacyStatus(string id, CandidacyStatus status)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            try
            {
                var candidacies = GetAllCandidacies();
                var candidacy = candidacies.FirstOrDefault(c => c.Id == id);
                
                if (candidacy == null)
                {
                    return false;
                }

                candidacy.Status = status;
                return SaveCandidaciesToFile(candidacies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating candidacy status: {ex.Message}");
                return false;
            }
        }

        private bool SaveCandidaciesToFile(List<Candidacy> candidacies)
        {
            var filePath = Path.Combine(_dataPath, CandidaciesFileName);
            
            try
            {
                var json = JsonConvert.SerializeObject(candidacies, Formatting.Indented);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving candidacies to file: {ex.Message}");
                return false;
            }
        }
    }
}
