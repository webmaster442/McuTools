using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCUTools.Loader
{
    public class RepositoryItem
    {
        public string FileUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReleaseDate { get; set; }
        public bool Selected { get; set; }
    }

    public static class InstallFunctions
    {
        public static RepositoryItem[] ParseRepoFile(string file)
        {
            List<RepositoryItem> items = new List<RepositoryItem>();
            using (var f = File.OpenText(file))
            {
                string line;
                do
                {
                    line = f.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] columns = line.Split(';');
                        if (columns.Length < 4) continue;
                        items.Add(new RepositoryItem()
                            {
                                FileUrl = columns[0],
                                Name = columns[1],
                                Description = columns[2],
                                ReleaseDate = columns[3],
                                Selected = true
                            });
                    }
                    
                }
                while (line != null);
                return items.ToArray();
            }
        }
    }
}
