using AccountProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AccountProvider.Functions
{
    public class ChangePassword(ILogger<ChangePassword> logger, UserManager<UserRegistrationRequest> userManager, SignInManager<UserRegistrationRequest> signInManager)
    {
        private readonly ILogger<ChangePassword> _logger = logger;
        private readonly UserManager<UserRegistrationRequest> _userManager = userManager;
        private readonly SignInManager<UserRegistrationRequest> signInManager = signInManager;

        [Function("ChangePassword")]
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

            ChangePasswordRequest urr = null!;
            try
            {
                urr = JsonConvert.DeserializeObject<ChangePasswordRequest>(body)!;
                _logger.LogInformation($"Deserialized request: {JsonConvert.SerializeObject(urr)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"JsonConvert.DeserializeObject<UserRegistrationRequest> :: {ex.Message}");
                return new BadRequestObjectResult($"Error deserializing request body: {ex.Message}");
            }

            if (urr != null && !string.IsNullOrEmpty(urr.Email) && !string.IsNullOrEmpty(urr.OldPassword)
                && !string.IsNullOrEmpty(urr.NewPassword) && !string.IsNullOrEmpty(urr.ConfirmPassword))
            {
                try
                {
                    if (urr.NewPassword == urr.ConfirmPassword)
                    {
                        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == urr.Email);
                        var login = await signInManager.CheckPasswordSignInAsync(user, urr.OldPassword, false);
                        if (login.Succeeded)
                        {
                            var change = await signInManager.CheckPasswordSignInAsync(user, urr.NewPassword, true);
                            if (change.Succeeded)
                            {
                                return new OkResult();
                            }
                        }
                        return new BadRequestObjectResult($"Changing Password failed: ");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Database query error :: {ex.Message}");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
                return new BadRequestObjectResult($"Changing Password failed: ");
            }
            else
            {
                _logger.LogInformation("Invalid request data.");
                return new BadRequestObjectResult("Invalid request data.");
            }
        }
    }
}
