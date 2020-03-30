using System;
using Eron.mailClient.Application.Core;
using Eron.mailClient.Application.Core.Enums;

namespace Eron.MailClient.Business.Core.TokenGenerator
{
    public static class TokenGenerator
    {
        public static string Generate(TokenType type)
        {
            var tokenString = GetStringToken(type);
            var currentYear = (DateTime.Now.Year).ToString("YYYY");
            var firstRandomNumber = GetRandomNumber(6);
            var secondRandomNumber = GetRandomNumber(2);
            var heroChars = ApplicationSettings.DefaultHeroChars.ToUpper();

            return heroChars + "-" + currentYear + firstRandomNumber + "-" + tokenString + "-" + secondRandomNumber;
        }

        private static string GetStringToken(TokenType type)
        {
            string tokenString;
            switch (type)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            //todo: check this line for implementation
            #pragma warning disable 162
            return tokenString;
            #pragma warning restore 162
        }

        private static string GetRandomNumber(int length)
        {
            var random = new Random();
            var decimalRandom = random.Next(0, 1);
            var randomNumber = (decimalRandom * (10 ^ length)).ToString().PadLeft(length);

            return randomNumber;
        }
    }
}
