using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SoulsFormats;

namespace DS_TextsMod_Helper
{
    internal class InputFile
    {
        public string Path { get; set; }
        public string Directory { get; set; }
        public string Name { get; set; }
        public string NameExt { get; set; }
        public long Size { get; set; }
        public string Edit { get; set; }

        public string Error { get; set; }

        public int TotalEntries { get; set; }
        public int NonNullEntries { get; set; }
        public string VersionSm { get; set; }
        public string VersionLg { get; set; }



        public InputFile(string filepath)
        {
            Path = filepath;

            if (!File.Exists(filepath))
            {
                Error = "Submitted element was not a file";
                return;
            }

            if (new FileInfo(filepath).Extension.ToLowerInvariant() != ".fmg")
            {
                Error = "File extension was not \".fmg\"";
                return;
            }

            try
            {
                Directory = Tools.GetParentDirPath(filepath);
                Name = Tools.GetFileName(filepath, false);
                NameExt = Tools.GetFileName(filepath, true);
                Size = Tools.GetFileSize(filepath);
                Edit = Tools.GetTimeSinceLastEdit(filepath);

                FMG fmg = FMG.Read(filepath);

                TotalEntries = fmg.Entries.Count;
                NonNullEntries = fmg.Entries.Where(e => e.Text != null).Count();
                VersionSm = GetVersion(fmg.Version, true);
                VersionLg = GetVersion(fmg.Version, false);
            }
            catch (Exception exception)
            {
                Error = $"Error while registering file :\n{exception}";
            }
        }


        public string GetToolTipText(bool fileIsValid)
        {
            StringBuilder sb = new StringBuilder();

            if (fileIsValid)
            {
                sb.AppendLine("[File info]");
                sb.AppendLine($"Name = \"{Name}.fmg\"");
                sb.AppendLine($"Modified = {Edit} ago");
                sb.AppendLine($"Size = {Size} bytes");
                sb.AppendLine();
                sb.AppendLine("[FMG info]");
                sb.AppendLine($"Version = {VersionLg}");
                sb.Append($"Total entries = {TotalEntries} ({NonNullEntries} non null)");
            }
            else
            {
                sb.AppendLine("[Error on input file]");
                sb.AppendLine($"Path = \"{Path}\"");
                sb.Append($"Error = \"{Error}\"");
            }

            return sb.ToString();
        }


        /// <summary>
        /// Translate FMG version from FMG.FMGVersion to more readable and displayable String
        /// </summary>
        private string GetVersion(FMG.FMGVersion version, bool shortened)
        {
            switch (version)
            {
                case FMG.FMGVersion.DemonsSouls: return shortened ? "DeS" : "Demon's Souls";
                case FMG.FMGVersion.DarkSouls1: return shortened ? "DS1 / DS2" : "Dark Souls 1 / Dark Souls 2";
                case FMG.FMGVersion.DarkSouls3: return shortened ? "DS3 / BB" : "Dark Souls 3 / Bloodborne";
                default: return shortened ? "DS1 / DS2" : "Dark Souls 1 / Dark Souls 2";
            }
        }
    }
}
