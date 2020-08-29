using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ScreenCapDictionaryNoteApp.Model;
using ScreenCapDictionaryNoteApp.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScreenCapDictionaryNoteApp.ViewModel.Helpers
{
    public class WebServiceHelper
    {
        private static string postRequestEndPoint = Config.SERVICE_HOST + "/api/screencaps";

        private ApplicationConfig ApplicationConfig;

        public static event EventHandler UpdateMessageBoxHandler;



        public WebServiceHelper(ApplicationConfig applicationConfig)
        {
            ApplicationConfig = applicationConfig;
        }


        private void OpenMessageBox()
        {
            var messageBox = new Notification("***** Status *****");
            messageBox.Show();

        }

        private void UpdateMessageBox(String message)
        {
            UpdateMessageBoxHandler(this, new MessageBoxEventArgs(message));
        }

        public async void SyncToWebServer()
        {
            OpenMessageBox();

            try
            {
                UpdateMessageBox("Uploading ...");
                var processedPage = new List<string>();

                List<Note> allNotes = DatabaseHelper.Read<Note>();
                List<Page> allPages = DatabaseHelper.Read<Page>();
                List<Vocab> allVocabs = DatabaseHelper.Read<Vocab>();

                // just sync pages with cropped image:
                foreach (var page in allPages.ToList())
                {
                    if (page.croppedImageFilePath == null)
                    {
                        var list = allVocabs.RemoveAll(vocab => vocab.PageId == page.Id);
                        allPages.Remove(page);
                    }
                }
                // ------------------- add necessary update  ------------------- 


                allPages.ForEach(page =>
                {
                    String croppedImageFilePath = page.croppedImageFilePath;
                    Regex pattern = new Regex(@"(?<=Screenshot\\).*\.png$");
                    Match match = pattern.Match(croppedImageFilePath);

                    page.croppedImageFilePath = match.Groups[0].Value;

                });

                await SyncSqliteDbVocabsToWebDb(allNotes, allPages, allVocabs);

                UpdateMessageBox("All vocabs uploaded");

                UpdateMessageBox("Sending images to S3 bucket ...");

                await UploadImagesPerPage(allPages);

                UpdateMessageBox("Done");



            }
            catch (Exception err)
            {

            }
        }

        private async Task SyncSqliteDbVocabsToWebDb(List<Note> allNotes, List<Page> allPages, List<Vocab> allVocabs)
        {
            var syncInfo = new SyncInfoModel()
            {
                Notes = allNotes,
                Pages = allPages,
                Vocabs = allVocabs
            };
            var jsonStringForUpdate = JsonConvert.SerializeObject(syncInfo);


            using (var client = new HttpClient())
            {
                UpdateMessageBox("Uploading Vocabs ...");


                string token = ApplicationConfig.jwtToken;

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ApplicationConfig.jwtToken);

                var updateContent = new StringContent(jsonStringForUpdate, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(postRequestEndPoint, updateContent);
                    var responseStatus = response.StatusCode.ToString();


                    UpdateMessageBox(responseStatus + "!!!");
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                    UpdateMessageBox(err.Message);
                }
            }
        }

        private async Task UploadImagesPerPage(List<Page> allPages)
        {
            int uploadedImageCount = 0;
            foreach (Page page in allPages)
            {

                if (!page.IsSyncToS3)
                {
                    if (page.croppedImageFilePath != null)
                    {
                        try
                        {
                            string adjustedFilePath = FilePathHelper.CorrectImagePath(page.croppedImageFilePath);
                            // the file path need to be adjusted since the absolute path is saved in the database.

                            await AWSHelper.UploadFileAsync(ApplicationConfig.username, adjustedFilePath);
                            uploadedImageCount++;
                            string message = uploadedImageCount + "Image(s) have(s) been uploaded";
                            UpdateMessageBox(message);
                            page.IsSyncToS3 = true;
                            DatabaseHelper.Update(page);
                        }
                        catch (Exception err)
                        {
                            Debug.Write(err.Message);
                        }

                    }
                }
            }
        }

        public class MessageBoxEventArgs : EventArgs
        {
            public string Message { get; set; }
            public MessageBoxEventArgs(string msg)
            {
                Message = msg;
            }
        }
        private class SyncInfoModel
        {
            public List<Note> Notes { get; set; }
            public List<Page> Pages { get; set; }
            public List<Vocab> Vocabs { get; set; }
        }
    }
}
