﻿using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Grpc.Auth;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCapDictionaryNoteApp.ViewModel.Helpers
{
    public class VisionAPIHelper
    {




        private static string CredentialJsonString = "{\"type\":\"service_account\",\"project_id\":\"text-capture2\",\"private_key_id\":\"167a3d1c18521ea26e3a1ec32c808472d4e87886\",\"private_key\":\"-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDUIkrkOKrgFo44\nOc3pDgcXq/89lZ8FrLyn1Z6OfOTw90WII2LL6fkQpliGhk82UR8t+YW/OA2EVUe7\nVijheFQMj/iEKdfjjjf0SdCJQuGBAc9unSPKrXyOdc+cZ+kEo6h+4sU8soHaDva3\nZQyQED8Dpdk3XmeZjryALf1l2n/mfcPqrb7Fza+8XQZBNIhSdimRydnVQQAaweZZ\nRzTHO9rR4dU491wkIjjE2BP8fDoM2EjOctjePR0gyFwtTsJaeP0Wy33vcOL5G1rL\nW8CMvDFqi0GGhZ1P9KlLGcEIZn3Gd45R63J8z6BMseHZ01ziXQrlHwaNx7gH6/Up\nhgBBTAJdAgMBAAECggEAB+ASCbD03wbXVs0GkO1dZAqVmjKwyaQlCNIMZdvLIAr+\nANTXeRyBi5WxWaUo0apnTuyhg3zQO6Zy2EiVkfFGpYS+xCFNi3wfJamL3VynRIPN\nsJyS1JZmNl+3SmYtJ2fv/G3sxpkV6wN9M0fEFKEVxcvlaYHOtMuwwc1zR9G5x9O4\nIazvWJ1nkBE4X42KR7gx5tSU2gzC1NtpIeUhnlAx/OW1b4RXSq1KlG/Qydq/8S7M\nqFA52e8p7+waEhzdukCfSX2WAouPrfWq9fPyCT0FqufHC5BP88YfsyAAhRSXwb55\n5JKlI6RcVJUrQxWExLg4xWGPppqbK+qk4S/AWskQ/QKBgQDqL+28HId1y1m/PSLe\n0PLVfsc2xbHaV5bv+mYkj2ouIuISHox2TSJwzWpargCm/PSK+UzDtJEgKMJDaHJG\nItvArbIfYLwZOfisGOtbukKjLbQC3ELwFGDKshXFTdoRWfjoijOeUXUwn7fIW8e3\nNcPYxHbsB9M4VyIWN1E4FlY+MwKBgQDn5IPtdEUM25IeoqT80A/xKjaLNvPADg8U\niVw4yUCmXFQCGy14MuZlZ3I5f8YyX0Cq4ZO0V/eMsK49Jwm/H+cmF5Epg0hIYGap\nr7aToCxwd9KD3SY2iR3eQYEFnFasXK/tvtKwrhbIYcwig8AdY66b6lQHkPnjfUAN\n5FshVnoNLwKBgDbO5MrDuog9AVIaktCnAk4pD6Kn/gnlufOMHN+tmOIXU+V9tM3z\nG35e9OcoFgr+5JSp5+ddmSF3qI9yIFx9yAR+IlPppdwZjzNn2Z1sKDBcf+azeg8K\nFeUabLHymHi7H9/8iDwjQFQS7UY9BK7CLIXM5TA0XKCw889TPvTrdV+1AoGBAJfH\nAu6VjmhnApGYpOJwEC8qEP83nXzT9tzbNwhX+T+p6LZkMXOd5Rz0A1hNIF1x6PKv\nqKx+cyGbtr+B4Kg+6l0NSc7hBZuRtUBVKOwhB1UpqBj004EYZ/tD5n3UoQo5tFCo\n+F/54iGPmTvx/fl+eBfG3O8C3fwey6EMGPWOE16zAoGBAL01XUiuP6MKGasdHyzg\nnZfiGRibY+Qbk8RU85J127aFaL6eFw+eZsvr5RZAFX9LMrRZ+Tas9xYw4S+mUce6\nAIR2N2ArmQ7NihJF+5cDexeDvxDvHv4hrdOECmJ6hGa0TSi0yPs92y+wn3fD30U0\n6wNVLxzOSy9DBbC5BlFtW9sY\n-----END PRIVATE KEY-----\n\",\"client_email\":\"text-capture@text-capture2.iam.gserviceaccount.com\",\"client_id\":\"115823607540998810315\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_x509_cert_url\":\"https://www.googleapis.com/robot/v1/metadata/x509/text-capture%40text-capture2.iam.gserviceaccount.com\"}";



        public static GoogleCredential Credential = GoogleCredential.FromJson(CredentialJsonString);


        public static Task<IReadOnlyList<Google.Cloud.Vision.V1.EntityAnnotation>> TextDetectionAsync(byte[] img_byte)
        {
            var newTask = Task.Factory.StartNew(async () =>
            {
                IReadOnlyList<Google.Cloud.Vision.V1.EntityAnnotation> response;
                Channel channel = new Channel(
                    ImageAnnotatorClient.DefaultEndpoint.Host,
                    ImageAnnotatorClient.DefaultEndpoint.Port,
                   Credential.ToChannelCredentials());
                ImageAnnotatorClient client = ImageAnnotatorClient.Create(channel);
                using (MemoryStream imageStream = new MemoryStream(img_byte))
                {
                    var image = Google.Cloud.Vision.V1.Image.FromStream(imageStream);
                    response = await client.DetectTextAsync(image);
                }
                return response;
            });

            return newTask.Result;

        }

    }
}
