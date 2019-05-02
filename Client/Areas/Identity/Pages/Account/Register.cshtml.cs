using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Client.Clients;
using Client.Helpers;
using Client.Models;
using Client.Models.Requests;
using Client.Models.Requests.AuthorizationService;
using Client.Models.Requests.BankService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Client.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly AuthorizationClient _authorizationClient;
        private readonly IBankClient _bankClient;

        public RegisterModel(ILogger<RegisterModel> logger, IEmailSender emailSender, AuthorizationClient client, IBankClient bankClient)
        {
            _logger = logger;
            _emailSender = emailSender;
            _authorizationClient = client ?? throw new ArgumentNullException(nameof(client));
            _bankClient = bankClient;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Stock Provider")]
            public string IsStockProvider { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid) return Page();
            var registerRequest = new RegisterRequest
            {
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Password = Input.Password,
                IsStockProvider = Input.IsStockProvider.Equals("StockProvider")
            };

            try
            {
                var response = await _authorizationClient.Register(registerRequest);
                Response.Cookies.Append("jwtCookie", response.AccessToken, new CookieOptions { HttpOnly = true });
                await _bankClient.CreateAccount(new CreateAccountRequest
                    {
                        Balance = 0,
                        OwnerName = $"{Input.FirstName} {Input.LastName}",
                        OwnerId =  JwtHelper.GetIdFromToken(response.AccessToken)
                    }, 
                    response.AccessToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to register person with email {Email} ", Input.Email);
                return Page();
            }

            return LocalRedirect(returnUrl);

            // If we got this far, something failed, redisplay form
        }
    }
}
