﻿using System.ComponentModel.DataAnnotations;

namespace VerifyForgotPasswordEmail.Models
{
    public class ResetPasswordRequest
    {

        [Required, EmailAddress]
        public string Token { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
