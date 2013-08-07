using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Xml.Serialization;
using RazorEngine;
using RazorEngine.Templating;

namespace RazorEmail
{
    public class RazorMailer
	{
		private readonly string _templateDir;

		public RazorMailer(string templateDir)
		{
			if (string.IsNullOrEmpty(templateDir)) throw new ApplicationException("No email template path specified in configuration");

			_templateDir = templateDir;

			if (!Directory.Exists(_templateDir)) throw new ArgumentException("The templateDir supplied doesn't exist: " + _templateDir);

			Razor.SetTemplateService(new TemplateService(new EmailTemplateConfiguration()
			{
				Resolver = new TemplateResolver(_templateDir)
			}));
		}

	    private string GetFullTemplateName(string templateName)
	    {
		    if (templateName.Contains(@"\")) return templateName;

			//Add directory if exists
			return Directory.Exists(Path.Combine(_templateDir, templateName)) ? string.Concat(templateName, @"\", templateName) : templateName;
	    }

	    /// <summary>
		/// Create Email
		/// </summary>
		/// <typeparam name="T">Model of type BaseEmailModel</typeparam>
		/// <param name="templateName">f.e. LogError. Folders with same name will be added automatically</param>
		/// <param name="model">Model of type BaseEmailModel</param>
		/// <param name="subject">Will also be rendered</param>
		/// <param name="toAddress">Either just one address. More than one address must be seperated by ;</param>
		/// <param name="toDisplayName">Just in case of one address</param>
		/// <returns></returns>
		public Email Create<T>(string templateName, T model, string subject, string toAddress = null, string toDisplayName = null)
		{
			if (templateName == null) throw new ArgumentNullException("templateName");
			if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException("subject");

			//Add directory if it is not already contained
			templateName = GetFullTemplateName(templateName);

			var email = CreateFromFile(templateName);

			var toAddressList = new List<Email.Address>();

			if (toAddress != null)
			{
				if (toAddress.Contains(";"))
					toAddress.Split(';').ToList().ForEach(t => toAddressList.Add(new Email.Address { Email = t }));
				else
					toAddressList.Add(new Email.Address { Email = toAddress, Name = toDisplayName });
			}

			if (email.To != null)
				toAddressList.AddRange(email.To);

			email.To = toAddressList.ToArray();

			email.Subject = Razor.Parse(subject, model, subject);// razorEngine.RenderContentToString(email.Subject, model);

			if (email.Subject.Contains("\n")) throw new ApplicationException("The subject line cannot contain any newline characters");

			foreach (var view in email.Views)
			{
				var viewTemplateName = templateName + "." + view.MediaType.Replace('/', '_');

				var fileContent = Resolve(viewTemplateName);

				var templateExists = fileContent != null;

				view.Content = templateExists ? Razor.Parse(fileContent, model, viewTemplateName) :
												Razor.Parse(view.Content, model, viewTemplateName); //razorEngine.RenderContentToString(view.Content, model);
			}

			return email;
		}


		public string Resolve(string name)
		{
			var path = Path.Combine(_templateDir, name);

			if (File.Exists(path))
				return File.ReadAllText(path);

			if (File.Exists(path + ".cshtml"))
				return File.ReadAllText(path + ".cshtml");

			return null;
		}

		private Email CreateFromFile(string templateName)
		{
			var template = new Email();

			var defaultTextFilename = templateName + ".text_plain.cshtml";
			var defaultHtmlFilename = templateName + ".text_html.cshtml";

			var defaultViews = new List<Email.View>();

			if (template.Views == null && File.Exists(Path.Combine(_templateDir, defaultTextFilename)))
			{
				defaultViews.Add(new Email.View
				{
					MediaType = MediaTypeNames.Text.Plain
				});
			}

			if (template.Views == null && File.Exists(Path.Combine(_templateDir, defaultHtmlFilename)))
			{
				defaultViews.Add(new Email.View
				{
					MediaType = MediaTypeNames.Text.Html,
				});
			}

			if (template.Views == null)
				template.Views = defaultViews.ToArray();

			return template;
		}
    }
}