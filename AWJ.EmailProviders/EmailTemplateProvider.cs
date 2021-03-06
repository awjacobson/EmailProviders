﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AWJ.EmailProviders
{
    public interface IEmailTemplateProvider
    {
        Task<string> GetEmailTemplateAsync(string name);
        Task<string> GetHtmlMessageAsync(string name, IDictionary<string, string> keyValuePairs, bool htmlEncode = true);
    }

    public class EmailTemplateProvider : IEmailTemplateProvider
    {
        /// <summary>
        /// Return the email template that is read from the given embedded resource
        /// </summary>
        /// <param name="name">
        /// The case-sensitive name of the manifest resource being requested.
        /// Example "gwfoods.EmailTemplates.ResetPassword.html"
        /// </param>
        /// <returns></returns>
        public async Task<string> GetEmailTemplateAsync(string name)
        {
            string emailTemplate;
            var assembly = Assembly.GetEntryAssembly();
            var resourceStream = assembly.GetManifestResourceStream(name);
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                emailTemplate = await reader.ReadToEndAsync();
            }
            return emailTemplate;
        }

        /// <summary>
        /// Performs the keyword replacement on the provide email template.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="keyValuePairs"></param>
        /// <param name="htmlEncode">Indicate to HTML encode or not</param>
        /// <returns></returns>
        public static string Replace(string template, IDictionary<string, string> keyValuePairs, bool htmlEncode = true)
        {
            return keyValuePairs.Aggregate(template, (current, value) =>
                current.Replace($"{{{value.Key}}}", htmlEncode ? HtmlEncoder.Default.Encode(value.Value) : value.Value));
        }

        /// <summary>
        /// Return the email template that is read from the given embedded resource
        /// with keywords replaced with the key/value pairs.
        /// </summary>
        /// <param name="name">
        /// The case-sensitive name of the manifest resource being requested.
        /// Example "gwfoods.EmailTemplates.ResetPassword.html"
        /// </param>
        /// <param name="keyValuePairs">
        /// Key/value pairs used for keyword replacement in the email template.
        /// </param>
        /// <param name="htmlEncode">Indicate to HTML encode or not</param>
        /// <returns></returns>
        public async Task<string> GetHtmlMessageAsync(string name, IDictionary<string, string> keyValuePairs, bool htmlEncode = true)
        {
            var template = await GetEmailTemplateAsync(name);
            var output = Replace(template, keyValuePairs, htmlEncode);
            return output;
        }
    }
}
