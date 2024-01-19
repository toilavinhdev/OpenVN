using SharedKernel.Runtime.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Net.Mail;
using IdGen;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.RegularExpressions;
using SharedKernel.Application;

namespace SharedKernel.Libraries
{
    public static class Utility
    {
        #region Fields
        public static string NETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
        public static string NETCORE_PROJECTNAME = "PROJECT_NAME";
        #endregion

        public static string GetEnvironment()
        {
            return Environment.GetEnvironmentVariable(NETCORE_ENVIRONMENT) ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        }

        public static string GetEnvironmentLower()
        {
            return GetEnvironment().ToLower();
        }

        public static string GetShortEnvironmentLower()
        {
            var env = GetEnvironment().ToLower();
            switch (env)
            {
                case "development":
                    return "dev";
                case "production":
                    return "prod";
                default:
                    return "";
            }
        }

        public static string GetProjectName()
        {
            return Environment.GetEnvironmentVariable(NETCORE_PROJECTNAME);
        }

        public static object ConvertToCorrect(object from, object to)
        {
            switch (to?.GetType().Name)
            {
                case "Boolean":
                    return Convert.ToBoolean(from);
                case "Int32":
                    return Convert.ToInt32(from);
                case "Int64":
                    return Convert.ToInt64(from);
                case "String":
                    return Convert.ToString(from);
                case "Double":
                    return Convert.ToDouble(from);
                case "Single":
                    return Convert.ToSingle(from);
                case "Decimal":
                    return Convert.ToDecimal(from);
                case "DateTime":
                    return Convert.ToDateTime(from);
                case "Char":
                    return Convert.ToChar(from);
                default:
                    return from;
            }
        }

        public static bool IsPrimitiveType(object obj)
        {
            if (obj == null)
                return false;

            switch (obj.GetType().Name)
            {
                case "Boolean":
                case "Byte":
                case "SByte":
                case "Int16":
                case "Int32":
                case "Int64":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                case "Char":
                case "Double":
                case "Single":
                    return true;

                default:
                    return false;
            }
        }

        public static string RandomNumber(int length)
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            while (sb.Length < length)
            {
                int num = random.Next(0, 9);
                sb.Append(num + "");
            }

            return sb.ToString();
        }

        public static string RandomString(int length, bool hasNumber = true)
        {
            var random = new Random();
            var mix = Enumerable.Range(65, 26).Concat(Enumerable.Range(97, 26)).ToList();
            if (hasNumber)
            {
                mix.Concat(Enumerable.Range(48, 10));
            }

            var result = new List<char>();
            var mixCount = mix.Count;
            if (length <= mixCount)
            {
                return string.Join("", mix.OrderBy(x => random.Next()).Take(length).Select(x => (char)x));
            }

            while (length > 0)
            {
                result.AddRange(mix.OrderBy(x => random.Next()).Take(length).Select(x => (char)x));
                length -= mixCount;
            }

            return string.Join("", result);
        }

        public static bool IsEmail(string input)
        {
            try
            {
                new MailAddress(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPhoneNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return Regex.IsMatch(input, RegexPattern.PHONE_NUMBER_PATTERN);
        }

        /// <summary>
        /// Tra ve content type
        /// </summary>
        public static string GetContentType(string subpath)
        {
            var success = new FileExtensionContentTypeProvider().TryGetContentType(subpath, out var contentType);
            if (success)
            {
                return contentType;
            }
            return "other";
        }
    }
}
