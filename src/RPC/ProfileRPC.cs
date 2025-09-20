using GrindBreaker.RPC.Models;
using GrindBreaker.Models;
using GrindBreaker.Repositories;
using Newtonsoft.Json;

namespace GrindBreaker.RPC
{
    public class ProfileRPC
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileRPC(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        /// <summary>
        /// Gets the user profile
        /// </summary>
        /// <param name="id">The id of the request</param>
        /// <param name="req">The request array. This is a array of JSON types that the user provided as arguments to the RPC call</param>
        /// <returns>The profile for the given id</returns>
        public void GetProfile(IWebviewWrapper webview, string id, string req)
        {
            try
            {
                var profile = _profileRepository.GetProfile();
                
                if (profile == null)
                {
                    var result = RPCResult<Profile>.NotFound();
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Success, json);
                    return;
                }
                
                var successResult = RPCResult<Profile>.Success(profile);
                var successJson = JsonConvert.SerializeObject(successResult);
                webview.Return(id, RPCResultType.Success, successJson);
            }
            catch (JsonException ex)
            {
                // Log the exception if you have logging configured
                Console.WriteLine($"Error serializing profile JSON: {ex.Message}");
                var result = RPCResult<Profile>.Error("Error serializing profile data");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging configured
                Console.WriteLine($"Error retrieving profile: {ex.Message}");
                var result = RPCResult<Profile>.Error("An error occurred while retrieving the profile");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
        }

        /// <summary>
        /// Saves the user profile
        /// </summary>
        /// <param name="id">The id of the request</param>
        /// <param name="req">The request array. This is a array of JSON types that the user provided as arguments to the RPC call</param>
        /// <returns>True if the profile was saved, false otherwise</returns>
        public void SaveProfile(IWebviewWrapper webview, string id, string req)
        {
            try
            {
                Console.WriteLine(req);
                // Parse the JSON request to get the profile data array
                var profiles = JsonConvert.DeserializeObject<Profile[]>(req);
                
                if (profiles == null || profiles.Length == 0)
                {
                    var result = RPCResult<string>.Error("Invalid profile data");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                    return;
                }
                
                // Use the first profile in the array
                var profile = profiles[0];
                
                var success = _profileRepository.SaveProfile(profile);
                if (success)
                {
                    var result = RPCResult<string>.Success("Profile saved successfully");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Success, json);
                }
                else
                {
                    var result = RPCResult<string>.Error("Failed to save profile");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                }
            }
            catch (JsonException ex)
            {
                // Log the exception if you have logging configured
                Console.WriteLine($"Error deserializing profile JSON: {ex.StackTrace}");
                var result = RPCResult<string>.Error("Invalid JSON format");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging configured
                Console.WriteLine($"Error saving profile: {ex.Message}");
                var result = RPCResult<string>.Error("An error occurred while saving the profile");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
        }
    }
}
