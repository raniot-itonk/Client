using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Client.Clients;
using Client.Models.Requests.AuthorizationService;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly AuthorizationClient _authorizationClient;

        public LoginModel(ILogger<LoginModel> logger, AuthorizationClient client)
        {
            _logger = logger;
            _authorizationClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var response = await _authorizationClient.Login(new LoginRequest
                {
                    Email = Input.Email,
                    Password = Input.Password
                });

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, response.AccessToken)
                };

                var userIdentity = new ClaimsIdentity(claims);
                var principal = new ClaimsPrincipal(userIdentity);

                await HttpContext.SignInAsync(principal);

                Response.Cookies.Append("jwtCookie", response.AccessToken, new CookieOptions { HttpOnly = true });

                return LocalRedirect("~/");
            }


            return Page();
        }
    }
}
