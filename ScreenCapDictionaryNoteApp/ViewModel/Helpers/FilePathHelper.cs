using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScreenCapDictionaryNoteApp.ViewModel.Helpers
{
    public class FilePathHelper
    {
        public static string CorrectImagePath(string filePath)
        {
            string imageName = filePath.Split(@"\").Last();
            string currentDirectory = Environment.CurrentDirectory;
            return Path.Join(Environment.CurrentDirectory, "Screenshot", imageName);
        }
    }
}
