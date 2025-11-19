// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Hachiko.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Hachiko.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // Sign out from ASP.NET Core Identity (clears application cookie)
            await _signInManager.SignOutAsync();

            // CRITICAL: Clear ALL authentication schemes to ensure complete logout
            // This removes Google OAuth cookies and any other external provider cookies
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Clear session data
            HttpContext.Session.Clear();

            _logger.LogInformation("User logged out completely - all authentication cookies cleared.");

            // Force redirect to login page to ensure clean state
            returnUrl = returnUrl ?? Url.Content("~/");

            // Redirect to login page with cleared state
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
    }
}
