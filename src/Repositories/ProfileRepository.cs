using Newtonsoft.Json;
using GrindBreaker.Models;

namespace GrindBreaker.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly string _dataPath;
        private const string ProfileFileName = "profile.json";

        public ProfileRepository(string? dataPath = null)
        {
            _dataPath = dataPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GrindBreaker");
            
            // Ensure the data directory exists
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
        }

        public Profile? GetProfile()
        {
            var filePath = Path.Combine(_dataPath, ProfileFileName);
            
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Profile>(json);
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging configured
                Console.WriteLine($"Error reading profile: {ex.Message}");
                return null;
            }
        }

        public bool SaveProfile(Profile profile)
        {
            var filePath = Path.Combine(_dataPath, ProfileFileName);
            
            if (profile == null)
            {
                return false;
            }

            try
            {
                var json = JsonConvert.SerializeObject(profile, Formatting.Indented);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging configured
                Console.WriteLine($"Error saving profile: {ex.Message}");
                return false;
            }
        }

        public bool DeleteProfile()
        {
            var filePath = Path.Combine(_dataPath, ProfileFileName);
            
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging configured
                Console.WriteLine($"Error deleting profile: {ex.Message}");
                return false;
            }
        }
    }
}
