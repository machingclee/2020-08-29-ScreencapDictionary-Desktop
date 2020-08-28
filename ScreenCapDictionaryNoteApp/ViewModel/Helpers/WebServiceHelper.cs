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

namespace ScreenCapDictionaryNoteApp.ViewModel.Helpers
{
    public class WebServiceHelper
    {
        private static string postRequestEndPoint = Config.SERVICE_HOST + "/api/screencaps";

        private string token;


        public static event EventHandler UpdateMessageBoxHandler;



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

                var syncInfo = new SyncInfoModel()
                {
                    Notes = allNotes,
                    Pages = allPages,
                    Vocabs = allVocabs
                };


                //UpdateMessageBox("Sending Images to S3 Bucket ...");
                //uploadedImageCount = 0;
                //foreach (Page page in allPages)
                //{

                //    if (!page.IsSyncToS3)
                //    {
                //        if (page.CroppedScreenshotByteArray != null)
                //        {
                //            try
                //            {
                //                string adjustedFilePath = FilePathHelper.CorrectImagePath(page.CroppedScreenshotByteArray);
                //                // the file path need to be adjusted since the absolute path is saved in the database.

                //                await AWSHelper.UploadFileAsync("cclee", adjustedFilePath);
                //                uploadedImageCount++;
                //                string message = uploadedImageCount + "Image(s) have(s) been uploaded";
                //                UpdateMessageBox(message);
                //                //page.IsSyncToS3 = true;
                //                //DatabaseHelper.Update(page);
                //            }
                //            catch (Exception err)
                //            {
                //                Debug.Write(err.Message);
                //            }

                //        }
                //    }
                //}
                //UpdateMessageBox("Done");



                var jsonStringForUpdate = JsonConvert.SerializeObject(syncInfo);


                using (var client = new HttpClient())
                {
                    UpdateMessageBox("Uploading Vocabs ...");


                    token = DatabaseHelper.Read<ApplicationConfig>()[0]?.jwtToken;

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

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
            catch (Exception err)
            {

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
