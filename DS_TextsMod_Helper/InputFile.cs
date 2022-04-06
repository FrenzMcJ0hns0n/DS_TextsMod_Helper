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

        public string Edit { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string PDir { get; set; }
        public long Size { get; set; }

        public string Error { get; set; }

        public int TotalEntries { get; set; }
        public int NonNullEntries { get; set; }
        public string Version { get; set; }



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
                Edit = Tools.GetTimeSinceLastEdit(filepath);
                Name = Tools.GetFileName(filepath);
                PDir = Tools.GetParentDirPath(filepath);
                Size = Tools.GetFileSize(filepath);

                FMG fmg = FMG.Read(filepath);

                TotalEntries = fmg.Entries.Count;
                NonNullEntries = ParseNonNullEntries(fmg);
                SetVersion(fmg.Version);
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
                sb.AppendLine($"Name = {Name}.fmg");
                sb.AppendLine($"Modified = {Edit} ago");
                sb.AppendLine($"Size = {Size} bytes");
                sb.AppendLine();
                sb.AppendLine("[FMG info]");
                sb.AppendLine($"Version = {Version}");
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


        private int ParseNonNullEntries(FMG fmg)
        {
            List<FMG.Entry> tranformedList = fmg.Entries
                                             .Select(x => x)
                                             .Where(x => x.Text != null)
                                             .ToList();
            return tranformedList.Count;
        }


        private void SetVersion(FMG.FMGVersion version)
        {
            switch (version)
            {
                case FMG.FMGVersion.DemonsSouls:
                    Version = "Demon's Souls";
                    break;

                case FMG.FMGVersion.DarkSouls1:
                    Version = "Dark Souls 1 / Dark Souls 2";
                    break;

                case FMG.FMGVersion.DarkSouls3:
                    Version = "Dark Souls 3 / Bloodborne";
                    break;
            }
        }
    }
}
