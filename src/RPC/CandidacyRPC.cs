using GrindBreaker.RPC.Models;
using GrindBreaker.Models;
using GrindBreaker.Repositories;
using Newtonsoft.Json;

namespace GrindBreaker.RPC
{
    public class CandidacyRPC
    {
        private readonly ICandidacyRepository _candidacyRepository;

        public CandidacyRPC(ICandidacyRepository candidacyRepository)
        {
            _candidacyRepository = candidacyRepository;
        }

        /// <summary>
        /// Gets all candidacies
        /// </summary>
        /// <param name="webview">The webview wrapper</param>
        /// <param name="id">The id of the request</param>
        /// <param name="req">The request array</param>
        public void GetAllCandidacies(IWebviewWrapper webview, string id, string req)
        {
            try
            {
                var candidacies = _candidacyRepository.GetAllCandidacies();
                var result = RPCResult<List<Candidacy>>.Success(candidacies);
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Success, json);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error serializing candidacies JSON: {ex.Message}");
                var result = RPCResult<List<Candidacy>>.Error("Error serializing candidacies data");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving candidacies: {ex.Message}");
                var result = RPCResult<List<Candidacy>>.Error("An error occurred while retrieving candidacies");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
        }

        /// <summary>
        /// Gets a specific candidacy by ID
        /// </summary>
        /// <param name="webview">The webview wrapper</param>
        /// <param name="id">The id of the request</param>
        /// <param name="req">The request array containing the candidacy ID</param>
        public void GetCandidacy(IWebviewWrapper webview, string id, string req)
        {
            try
            {
                var candidacyId = JsonConvert.DeserializeObject<string[]>(req);
                
                if (candidacyId == null || candidacyId.Length == 0 || string.IsNullOrEmpty(candidacyId[0]))
                {
                    var result = RPCResult<Candidacy>.Error("Invalid candidacy ID");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                    return;
                }

                var candidacy = _candidacyRepository.GetCandidacy(candidacyId[0]);
                
                if (candidacy == null)
                {
                    var notFoundResult = RPCResult<Candidacy>.NotFound();
                    var notFoundJson = JsonConvert.SerializeObject(notFoundResult);
                    webview.Return(id, RPCResultType.Success, notFoundJson);
                    return;
                }
                
                var successResult = RPCResult<Candidacy>.Success(candidacy);
                var successJson = JsonConvert.SerializeObject(successResult);
                webview.Return(id, RPCResultType.Success, successJson);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing candidacy ID JSON: {ex.Message}");
                var result = RPCResult<Candidacy>.Error("Invalid JSON format");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving candidacy: {ex.Message}");
                var result = RPCResult<Candidacy>.Error("An error occurred while retrieving the candidacy");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
        }

        /// <summary>
        /// Saves a new candidacy
        /// </summary>
        /// <param name="webview">The webview wrapper</param>
        /// <param name="id">The id of the request</param>
        /// <param name="req">The request array containing the candidacy data</param>
        public void SaveCandidacy(IWebviewWrapper webview, string id, string req)
        {
            try
            {
                var candidacies = JsonConvert.DeserializeObject<Candidacy[]>(req);
                
                if (candidacies == null || candidacies.Length == 0)
                {
                    var result = RPCResult<string>.Error("Invalid candidacy data");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                    return;
                }
                
                var candidacy = candidacies[0];
                
                // Validate required fields
                if (string.IsNullOrWhiteSpace(candidacy.Company) || string.IsNullOrWhiteSpace(candidacy.Title))
                {
                    var result = RPCResult<string>.Error("Company and Title are required fields");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                    return;
                }
                
                var success = _candidacyRepository.SaveCandidacy(candidacy);
                if (success)
                {
                    var result = RPCResult<string>.Success("Candidacy saved successfully");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Success, json);
                }
                else
                {
                    var result = RPCResult<string>.Error("Failed to save candidacy");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing candidacy JSON: {ex.Message}");
                var result = RPCResult<string>.Error("Invalid JSON format");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving candidacy: {ex.Message}");
                var result = RPCResult<string>.Error("An error occurred while saving the candidacy");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
        }

        /// <summary>
        /// Updates an existing candidacy
        /// </summary>
        /// <param name="webview">The webview wrapper</param>
        /// <param name="id">The id of the request</param>
        /// <param name="req">The request array containing the candidacy data</param>
        public void UpdateCandidacy(IWebviewWrapper webview, string id, string req)
        {
            try
            {
                var candidacies = JsonConvert.DeserializeObject<Candidacy[]>(req);
                
                if (candidacies == null || candidacies.Length == 0)
                {
                    var result = RPCResult<string>.Error("Invalid candidacy data");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                    return;
                }
                
                var candidacy = candidacies[0];
                
                // Validate required fields
                if (string.IsNullOrWhiteSpace(candidacy.Company) || string.IsNullOrWhiteSpace(candidacy.Title))
                {
                    var result = RPCResult<string>.Error("Company and Title are required fields");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                    return;
                }
                
                var success = _candidacyRepository.UpdateCandidacy(candidacy);
                if (success)
                {
                    var result = RPCResult<string>.Success("Candidacy updated successfully");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Success, json);
                }
                else
                {
                    var result = RPCResult<string>.Error("Failed to update candidacy");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing candidacy JSON: {ex.Message}");
                var result = RPCResult<string>.Error("Invalid JSON format");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating candidacy: {ex.Message}");
                var result = RPCResult<string>.Error("An error occurred while updating the candidacy");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
        }

        /// <summary>
        /// Deletes a candidacy by ID
        /// </summary>
        /// <param name="webview">The webview wrapper</param>
        /// <param name="id">The id of the request</param>
        /// <param name="req">The request array containing the candidacy ID</param>
        public void DeleteCandidacy(IWebviewWrapper webview, string id, string req)
        {
            try
            {
                var candidacyId = JsonConvert.DeserializeObject<string[]>(req);
                
                if (candidacyId == null || candidacyId.Length == 0 || string.IsNullOrEmpty(candidacyId[0]))
                {
                    var result = RPCResult<string>.Error("Invalid candidacy ID");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                    return;
                }

                var success = _candidacyRepository.DeleteCandidacy(candidacyId[0]);
                if (success)
                {
                    var result = RPCResult<string>.Success("Candidacy deleted successfully");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Success, json);
                }
                else
                {
                    var result = RPCResult<string>.Error("Failed to delete candidacy");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing candidacy ID JSON: {ex.Message}");
                var result = RPCResult<string>.Error("Invalid JSON format");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting candidacy: {ex.Message}");
                var result = RPCResult<string>.Error("An error occurred while deleting the candidacy");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
        }

        /// <summary>
        /// Updates only the status of a candidacy
        /// </summary>
        /// <param name="webview">The webview wrapper</param>
        /// <param name="id">The id of the request</param>
        /// <param name="req">The request array containing the candidacy ID and new status</param>
        public void UpdateCandidacyStatus(IWebviewWrapper webview, string id, string req)
        {
            try
            {
                var requestData = JsonConvert.DeserializeObject<object[]>(req);
                
                if (requestData == null || requestData.Length < 2)
                {
                    var result = RPCResult<string>.Error("Invalid request data. Expected candidacy ID and status.");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                    return;
                }

                var candidacyId = requestData[0]?.ToString();
                var statusValue = requestData[1]?.ToString();

                if (string.IsNullOrEmpty(candidacyId) || string.IsNullOrEmpty(statusValue))
                {
                    var result = RPCResult<string>.Error("Candidacy ID and status are required");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                    return;
                }

                if (!Enum.TryParse<CandidacyStatus>(statusValue, out var status))
                {
                    var result = RPCResult<string>.Error("Invalid status value");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                    return;
                }

                var success = _candidacyRepository.UpdateCandidacyStatus(candidacyId, status);
                if (success)
                {
                    var result = RPCResult<string>.Success("Candidacy status updated successfully");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Success, json);
                }
                else
                {
                    var result = RPCResult<string>.Error("Failed to update candidacy status");
                    var json = JsonConvert.SerializeObject(result);
                    webview.Return(id, RPCResultType.Error, json);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing status update JSON: {ex.Message}");
                var result = RPCResult<string>.Error("Invalid JSON format");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating candidacy status: {ex.Message}");
                var result = RPCResult<string>.Error("An error occurred while updating the candidacy status");
                var json = JsonConvert.SerializeObject(result);
                webview.Return(id, RPCResultType.Error, json);
            }
        }
    }
}
