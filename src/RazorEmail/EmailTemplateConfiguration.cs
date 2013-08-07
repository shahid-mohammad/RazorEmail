using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using RazorEngine;
using RazorEngine.Compilation;
using RazorEngine.Compilation.Inspectors;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace RazorEmail
{
    public class EmailTemplateConfiguration : ITemplateServiceConfiguration
	{
		private readonly TemplateServiceConfiguration _innerConfig;

		public EmailTemplateConfiguration()
		{
			_innerConfig = new TemplateServiceConfiguration
			{
				Language = Language.CSharp
			};
		}

        /// <summary>
        /// Gets or sets the activator.
        /// </summary>
        public IActivator Activator { get { return _innerConfig.Activator; } }

        /// <summary>
        /// Gets or sets the base template type.
        /// </summary>
        public Type BaseTemplateType { get { return _innerConfig.BaseTemplateType; } }

        /// <summary>
        /// Gets the set of code inspectors.
        /// </summary>
        IEnumerable<ICodeInspector> ITemplateServiceConfiguration.CodeInspectors
        {
            get { return CodeInspectors; }
        }

        /// <summary>
        /// Gets the set of code inspectors.
        /// </summary>
        public IList<ICodeInspector> CodeInspectors { get { return _innerConfig.CodeInspectors; } }

        /// <summary>
        /// Gets or sets the compiler service factory.
        /// </summary>
        public ICompilerServiceFactory CompilerServiceFactory { get { return _innerConfig.CompilerServiceFactory; } }

        /// <summary>
        /// Gets whether the template service is operating in debug mode.
        /// </summary>
        public bool Debug { get { return _innerConfig.Debug; } }

        /// <summary>
        /// Gets or sets the encoded string factory.
        /// </summary>
        public IEncodedStringFactory EncodedStringFactory { get { return _innerConfig.EncodedStringFactory; } }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public Language Language { get { return _innerConfig.Language; } }

        /// <summary>
        /// Gets or sets the collection of namespaces.
        /// </summary>
        public ISet<string> Namespaces { get { return _innerConfig.Namespaces; } }

        /// <summary>
        /// Gets or sets the template resolver.
        /// </summary>
        public ITemplateResolver Resolver { get; set; } 
    }
}