using System;
using System.IO;
using PdfSharp.Fonts;

namespace epos
{
    
    public class WindowsFontResolver : IFontResolver
    {
        public static void Apply()
        {
            if (GlobalFontSettings.FontResolver == null)
                GlobalFontSettings.FontResolver = new WindowsFontResolver();
        }

        public string DefaultFontName => "Arial";

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string name = familyName.ToLowerInvariant();

            // Arial a Courier New
            if (name == "arial" || name == "courier new")
            {
                string key = name;
                if (isBold) key += "-b";
                if (isItalic) key += "-i";
                return new FontResolverInfo(key);
            }

            // fallback = Arial
            string fallback = "arial";
            if (isBold) fallback += "-b";
            if (isItalic) fallback += "-i";
            return new FontResolverInfo(fallback);
        }

        public byte[] GetFont(string faceName)
        {
            
            string fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            string path;

            switch (faceName)
            {
                case "arial":
                    path = Path.Combine(fontsDir, "arial.ttf");
                    break;
                case "arial-b":
                    path = Path.Combine(fontsDir, "arialbd.ttf");
                    break;
                case "arial-i":
                    path = Path.Combine(fontsDir, "ariali.ttf");
                    break;
                case "arial-b-i":
                    path = Path.Combine(fontsDir, "arialbi.ttf");
                    break;

                case "courier new":
                    path = Path.Combine(fontsDir, "cour.ttf");
                    break;
                case "courier new-b":
                    path = Path.Combine(fontsDir, "courbd.ttf");
                    break;
                case "courier new-i":
                    path = Path.Combine(fontsDir, "couri.ttf");
                    break;
                case "courier new-b-i":
                    path = Path.Combine(fontsDir, "courbi.ttf");
                    break;

                default:
                    
                    path = Path.Combine(fontsDir, "arial.ttf");
                    break;
            }

            return File.ReadAllBytes(path);
        }
    }
}
