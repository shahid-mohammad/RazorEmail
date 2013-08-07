using System;
using System.IO;
using RazorEngine.Templating;

namespace RazorEmail
{
    public class TemplateResolver : ITemplateResolver
    {
        private readonly string _templateDir;

        public TemplateResolver(string templateDir)
        {
            if (templateDir == null) throw new ArgumentNullException("templateDir");

            this._templateDir = templateDir;

            if (!Directory.Exists(templateDir))
                throw new ArgumentException(String.Format("The template directory does not exist! - {0} , Full path: {1}", templateDir, Path.GetFullPath(templateDir)));
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
    }
}