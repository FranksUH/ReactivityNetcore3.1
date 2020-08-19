using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Extensions
{
    public static class PasswordValidator
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilderOptions)
        {
            var options = ruleBuilderOptions.NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password most contain an uppercase")
            .Matches("[0-9]").WithMessage("Password most contain a number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password most contain a special character");

            return options;
        }
    }
}
