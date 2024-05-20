using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AccountProvider.Functions;

public class Verify(ILogger<Verify> logger, UserManager<UserAccount> userManager)
{
    private readonly ILogger<Verify> _logger = logger;
    private readonly UserManager<UserAccount> _userManager = userManager;

    [Function("Verify")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        string body = null!;
        try
        {
            body = await new StreamReader(req.Body).ReadToEndAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError($"StreamReader :: {ex.Message}");
        }

        if (body != null)
        {
            VerificationRequest vr = null!;
            try
            {
                vr = JsonConvert.DeserializeObject<VerificationRequest>(body)!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"JsonConvert.DeserializeObject<VerificationRequest> :: {ex.Message}");
            }
            if (vr != null && !string.IsNullOrEmpty(vr.Email) && !string.IsNullOrEmpty(vr.VerificationCode))
            {
                // om du vill kora det som en HTTP REQUEST - kraver att vi vantar pa svar tillbakfl
                try
                {
                    using var http = new HttpClient();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(vr), Encoding.UTF8, "application/json");
                    // var response = await http.PostAsync("https://verificationprovider.siliconas.azurewebsite.net/api/verify", content);

                    if (true)
                    {
                        var userAccount = await _userManager.FindByEmailAsync(vr.Email);
                        if (userAccount != null)
                        {
                            userAccount.EmailConfirmed = true;
                            await _userManager.UpdateAsync(userAccount);

                            if (await _userManager.IsEmailConfirmedAsync(userAccount))
                            {
                                return new OkResult();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"JsonConvert.DeserializeObject<UserRegistrationRequest> :: {ex.Message}");
                }

            }
        }
        return new UnauthorizedResult();
    }
}

//https://accountprovider-siliconas-nor.azurewebsites.net/api/SignUp?code=-syJy4HWS3GUZTEvOBNtmHcSXIRrw8JEEcw2kjcFcsLWAzFuKIlH1g%3D%3D



//https://accountprovider-siliconas-nor.azurewebsites.net/api/SignIn?code=QllATzSQuwpTeG0jPX5-TOKsCAPde3jcnOntmWS6JHY2AzFuZvse2Q%3D%3D


//https://accountprovider-siliconas-nor.azurewebsites.net/api/Verify?code=3hwpjDYEz-6_wSCaF-d7oudlGkNXl2j-lJqLwY5UJq0uAzFue6aP2Q%3D%3D