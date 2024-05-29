using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AccountProvider.Functions;

public class SignUp(ILogger<SignUp> logger, UserManager<UserAccount> userManager)
{
    private readonly ILogger<SignUp> _logger = logger;
    private readonly UserManager<UserAccount> _userManager = userManager;


    [Function("SignUp")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        string body = null!;
        try
        {
            body = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation($"Request body: {body}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"StreamReader :: {ex.Message}");
            return new BadRequestObjectResult($"Error reading request body: {ex.Message}");
        }

        UserRegistrationRequest urr = null!;
        try
        {
            urr = JsonConvert.DeserializeObject<UserRegistrationRequest>(body)!;
            _logger.LogInformation($"Deserialized request: {JsonConvert.SerializeObject(urr)}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"JsonConvert.DeserializeObject<UserRegistrationRequest> :: {ex.Message}");
            return new BadRequestObjectResult($"Error deserializing request body: {ex.Message}");
        }

        if (urr != null && !string.IsNullOrEmpty(urr.Email) && !string.IsNullOrEmpty(urr.Password))
        {
            try
            {
                if (!await _userManager.Users.AnyAsync(x => x.Email == urr.Email))
                {
                    var userAccount = new UserAccount
                    {
                        FirstName = urr.FirstName,
                        LastName = urr.LastName,
                        Email = urr.Email,
                        UserName = urr.Email
                    };

                    var result = await _userManager.CreateAsync(userAccount, urr.Password);
                    if (result.Succeeded)
                    {
                        // om du vill kfira det som en HTTP REQUEST - kraver att vi vantar pa svar tillbaka
                        try
                        {
                            using var http = new HttpClient();
                            StringContent content = new StringContent(JsonConvert.SerializeObject(new { Email = userAccount.Email }), Encoding.UTF8, "application/json");
                            var response = await http.PostAsync("https://verificationprovider-silicon-nor.azurewebsites.net/api/GenerateVerificationCode", content);
                        
                            if (!response.IsSuccessStatusCode)
                            {
                                _logger.LogError($"Error sending verification request: {response.ReasonPhrase}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"HTTP client error :: {ex.Message}");
                        }

                        return new OkResult();
                    }
                    else
                    {
                        _logger.LogError($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        return new BadRequestObjectResult($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    _logger.LogInformation("Email already exists.");
                    return new ConflictObjectResult("Email already exists.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database query error :: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        else
        {
            _logger.LogInformation("Invalid request data.");
            return new BadRequestObjectResult("Invalid request data.");
        }
    }
}
