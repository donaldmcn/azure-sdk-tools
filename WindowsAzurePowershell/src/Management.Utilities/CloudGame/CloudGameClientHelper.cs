﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Management.Utilities.CloudGameClientHelper
{
    using System.Linq;

    using Microsoft.WindowsAzure.Management.Utilities.CloudGame;
    using Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract;
    using Microsoft.WindowsAzure.Management.Utilities.Common;
    using Microsoft.WindowsAzure.Management.Utilities.Websites.Services;
    using Microsoft.WindowsAzure.ServiceManagement;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    ///     Implements helper functions used by the ClientGameClient
    /// </summary>
    public class ClientHelper
    {
        public static void RegisterCloudService(HttpClient httpJsonClient, HttpClient httpXmlClient)
        {
            // Check registration.
            var url = httpJsonClient.BaseAddress + String.Format("/services?service=gameservices.xboxlivecompute&action=register");
            httpJsonClient.PutAsync(url, null).ContinueWith(tr => ProcessBooleanJsonResponseAllowConflict(tr));

            // See if the cloud service exists, and create it if it does not.
            url = httpXmlClient.BaseAddress + String.Format(CloudGameUriElements.CloudServiceResourcePath);

            var cloudServiceExists = true;
            CloudService existingCloudService = null;
            try
            {
                existingCloudService = httpXmlClient.GetAsync(url).ContinueWith(tr => ClientHelper.ProcessXmlResponse<CloudService>(tr)).Result;
            }
            catch (ServiceManagementClientException ex)
            {
                if (ex.HttpStatus != HttpStatusCode.NotFound)
                {
                    // Rethrow the exception
                    throw;
                }

                // The cloud service does not exist
                cloudServiceExists = false;
            }

            if ((existingCloudService != null) && !existingCloudService.Name.Equals(CloudGameUriElements.DefaultServiceName))
            {
                cloudServiceExists = false;
            }

            if (cloudServiceExists)
            {
                return;
            }

            // The service does not exists, so create it
            var newCloudService = new CloudService()
                                      {
                                          Name = CloudGameUriElements.DefaultServiceName,
                                          Description = CloudGameUriElements.DefaultServiceName,
                                          GeoRegion = "West US",
                                          Label = CloudGameUriElements.DefaultServiceName
                                      };
            httpXmlClient.PutAsXmlAsync(url, newCloudService).ContinueWith(tr => ProcessBooleanJsonResponse(tr));
        }

        /// <summary>
        ///     Processes the response and handle error cases.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseMessage">The response message.</param>
        /// <returns></returns>
        /// <exception cref="Microsoft.WindowsAzure.ServiceManagement.ServiceManagementClientException"></exception>
        /// <exception cref="ServiceManagementError"></exception>
        public static T ProcessJsonResponse<T>(Task<HttpResponseMessage> responseMessage)
        {
            var message = responseMessage.Result;
            var content = message.Content.ReadAsStringAsync().Result;

            if (message.IsSuccessStatusCode)
            {
                return (T)JsonConvert.DeserializeObject(content, typeof(T));
            }

            throw CreateExceptionFromJson(message.StatusCode, content);
        }

        public static bool ProcessBooleanJsonResponse(Task<HttpResponseMessage> responseMessage)
        {
            var message = responseMessage.Result;
            if (message.IsSuccessStatusCode)
            {
                return true;
            }

            // Error
            throw CreateExceptionFromJson(message.StatusCode, string.Empty);
        }
    
        public static bool ProcessBooleanJsonResponseAllowConflict(Task<HttpResponseMessage> responseMessage)
        {
            var message = responseMessage.Result;
            if (message.IsSuccessStatusCode || (message.StatusCode == HttpStatusCode.Conflict))
            {
                return true;
            }

            // Error
            throw CreateExceptionFromJson(message.StatusCode, string.Empty);
        }

        public static CloudGamesList ProcessCloudServiceResponse(Task<HttpResponseMessage> responseMessage)
        {
            var message = responseMessage.Result;
            var content = message.Content.ReadAsStringAsync().Result;
            var encoding = GetEncodingFromResponseMessage(message);
            var response = new CloudGamesList();

            if (message.IsSuccessStatusCode)
            {
                var ser = new DataContractSerializer(typeof(CloudService));
                using (var stream = new MemoryStream(encoding.GetBytes(content)))
                {
                    stream.Position = 0;
                    var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
                    var serviceResponse = (CloudService)ser.ReadObject(reader, true);

                    foreach (var resource in serviceResponse.Resources.Where(resource => resource.OperationStatus.Error == null))
                    {
                        if (resource.IntrinsicSettings == null || resource.IntrinsicSettings.Length == 0)
                        {
                            response.Add(new CloudGame() { name = resource.Name });
                            continue;
                        }

                        var cbData = resource.IntrinsicSettings[0] as XmlCDataSection;
                        if (cbData == null)
                        {
                            continue;
                        }

                        var jsonSer = new DataContractJsonSerializer(typeof(CloudGame));
                        using (var jsonStream = new MemoryStream(encoding.GetBytes(cbData.Data)))
                        {
                            var game = (CloudGame)jsonSer.ReadObject(jsonStream);
                            response.Add(game);
                        }
                    }
                }

                return response;
            }

            throw CreateExceptionFromXml(content, message);
        }

        public static T ProcessXmlResponse<T>(Task<HttpResponseMessage> responseMessage)
        {
            var message = responseMessage.Result;
            var content = message.Content.ReadAsStringAsync().Result;
            var encoding = GetEncodingFromResponseMessage(message);

            if (message.IsSuccessStatusCode)
            {
                var ser = new DataContractSerializer(typeof(T));
                using (var stream = new MemoryStream(encoding.GetBytes(content)))
                {
                    stream.Position = 0;
                    var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
                    return (T)ser.ReadObject(reader, true);
                }
            }

            throw CreateExceptionFromXml(content, message);
        }

        public static Encoding GetEncodingFromResponseMessage(HttpResponseMessage message)
        {
            var encodingString = message.Content.Headers.ContentType.CharSet;
            var encoding = Encoding.GetEncoding(encodingString);
            return encoding;
        }

        public static ServiceManagementClientException CreateExceptionFromXml(string content, HttpResponseMessage message)
        {
            var encoding = GetEncodingFromResponseMessage(message);

            using (var stream = new MemoryStream(encoding.GetBytes(content)))
            {
                stream.Position = 0;
                var serializer = new XmlSerializer(typeof(ServiceError));
                var serviceError = (ServiceError)serializer.Deserialize(stream);
                return new ServiceManagementClientException(
                    message.StatusCode,
                    new ServiceManagementError
                    {
                        Code = message.StatusCode.ToString(),
                        Message = serviceError.Message
                    },
                    string.Empty);
            }
        }

        /// <summary>
        ///     Unwraps error message and creates ServiceManagementClientException.
        /// </summary>
        public static ServiceManagementClientException CreateExceptionFromJson(HttpStatusCode statusCode, string content)
        {
            var exception = new ServiceManagementClientException(
                statusCode,
                new ServiceManagementError
                {
                    Code = statusCode.ToString(),
                    Message = content
                },
                string.Empty);
            return exception;
        }

        /// <summary>
        ///     Creates and initialize and instance of HttpClient for a specific media type
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateCloudGameHttpClient(SubscriptionData subscription, string mediaType)
        {
            var requestHandler = new WebRequestHandler();
            requestHandler.ClientCertificates.Add(subscription.Certificate);
            var endpoint = new StringBuilder(General.EnsureTrailingSlash(subscription.ServiceEndpoint));
            endpoint.Append(subscription.SubscriptionId);

            var client = HttpClientHelper.CreateClient(endpoint.ToString(), handler: requestHandler);
            client.DefaultRequestHeaders.Add(CloudGameUriElements.XblCorrelationHeader, Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add(Constants.VersionHeaderName, "2012-08-01");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            return client;
        }

        /// <summary>
        /// Write out object to JSON string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A string of JSON</returns>
        public static string ToJson<T>(T value)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, value);
                return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }

        public static TResult DeserializeJsonToObject<TResult>(string json)
        {
            TResult output = default(TResult);
            using (MemoryStream mstream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(TResult));
                output = (TResult)dcs.ReadObject(mstream);
            }

            return output;
        }
    }
}