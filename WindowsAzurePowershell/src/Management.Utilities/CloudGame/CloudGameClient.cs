// ----------------------------------------------------------------------------------
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;
using Microsoft.WindowsAzure.Management.Utilities.Common;
using Microsoft.WindowsAzure.Management.Utilities.MediaService.Services.MediaServicesEntities;
using Microsoft.WindowsAzure.Management.Utilities.Websites.Services;
using Microsoft.WindowsAzure.ServiceManagement;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract;

namespace Microsoft.WindowsAzure.Management.Utilities.CloudGame
{
    using System.Linq;

    /// <summary>
    ///     Implements ICloudGameClient to use HttpClient for communication
    /// </summary>
    public class CloudGameClient : ICloudGameClient
    {
        public const string CloudGameVersion = "2013-09-01";
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpXmlClient;
        private readonly string _subscriptionId;

        /// <summary>
        ///     Creates new MediaServicesClient.
        /// </summary>
        /// <param name="subscription">The Windows Azure subscription data object</param>
        /// <param name="logger">The logger action</param>
        public CloudGameClient(SubscriptionData subscription, Action<string> logger, HttpClient httpClient, HttpClient httpXmlClient)
        {
            _subscriptionId = subscription.SubscriptionId;
            Subscription = subscription;
            Logger = logger;
            _httpClient = httpClient;
            _httpXmlClient = httpXmlClient;
        }

        /// <summary>
        ///     Creates new MediaServicesClient.
        /// </summary>
        /// <param name="subscription">The Windows Azure subscription data object</param>
        /// <param name="logger">The logger action</param>
        public CloudGameClient(SubscriptionData subscription, Action<string> logger)
            : this(subscription, logger, CreateCloudGameHttpClient(subscription), CreateCloudGameXmlClient(subscription))
        {
        }

        /// <summary>
        ///     Gets or sets the subscription.
        /// </summary>
        /// <value>
        ///     The subscription.
        /// </value>
        public SubscriptionData Subscription { get; set; }

        /// <summary>
        ///     Gets or sets the logger
        /// </summary>
        /// <value>
        ///     The logger.
        /// </value>
        public Action<string> Logger { get; set; }


        /// <summary>
        ///     Gets the game images.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<CloudGameImageCollectionResponse> GetGameImages(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameImagesResourcePath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ProcessJsonResponse<CloudGameImageCollectionResponse>(tr));
        }

        /// <summary>
        ///     Creates the cloud game image.
        /// </summary>
        /// <param name="cloudGameId">The cloud game name.</param>
        /// <param name="image">The game image.</param>
        /// <param name="cspkg">The game CSPKG.</param>
        /// <param name="cscfg">The game CSCFG.</param>
        /// <param name="asset">The game asset.</param>
        /// <returns></returns>
        public Task<CloudGameImage> CreateGameImage(string cloudGameId,
                            CloudGameImage image,
                            Stream cspkg,
                            Stream cscfg,
                            Stream asset)
        {
            string assetId = null;
            if (asset != null)
            {
                CloudGameAssetRequest assetRequest = new CloudGameAssetRequest()
                {
                    Filename = image.assetFile,// asset.FileName,
                    Name = image.assetFile//asset.FileName
                };
                PostCloudGameAssetResponse assetResponse = this.CreateGameAsset(cloudGameId, assetRequest, asset).Result;
                assetId = assetResponse.GameAssetId;
            }

            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameImagesResourcePath, cloudGameId);
            var cloudGameRequestMetadata = new CloudGameImageRequest()
            {
                CspkgFilename = image.packageFile,//cspkg.FileName,
                CscfgFilename = image.configFile,//cscfg.FileName,
                MinRequiredPlayers = 2,
                MaxAllowedPlayers = Convert.ToInt32(image.maxPlayers),
                Name = image.name,
                AssetId = assetId
            };

            PostCloudGameImageResponse responseMetadata = null;
            using (var multipartFormContent = new MultipartFormDataContent())
            {
                multipartFormContent.Add(new StringContent(ToJson(cloudGameRequestMetadata)), "metadata");
                multipartFormContent.Add(new StreamContent(cscfg), "packageconfig");

                responseMetadata = _httpClient.PostAsync(url, multipartFormContent).ContinueWith(tr => ProcessJsonResponse<PostCloudGameImageResponse>(tr)).Result;
                image.id = responseMetadata.GsiId;
            }

            try
            {
                var cloudblob = new CloudBlob(responseMetadata.CspkgPreAuthUrl);
                Task.Factory.FromAsync(
                    (callback, state) => cloudblob.BeginUploadFromStream(cspkg, callback, state),
                    (ar) => cloudblob.EndUploadFromStream(ar),
                    TaskCreationOptions.None);
            }
            catch (StorageException)
            {
                string errorMessage = string.Format("Failed to upload cspkg for cloud game. gameId {0} cspkgName {1}", cloudGameId, image.packageFile);
                throw CreateExceptionFromJson(HttpStatusCode.Ambiguous, errorMessage);
            }

            url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameImageResourcePath, cloudGameId, responseMetadata.GsiId);
            using (var multipartFormContent = new MultipartFormDataContent())
            {
                multipartFormContent.Add(new StringContent(ToJson(cloudGameRequestMetadata)), "metadata");
                bool result = _httpClient.PutAsync(url, multipartFormContent).ContinueWith(tr => ProcessJsonResponse<bool>(tr)).Result;
            }

            return Task<CloudGameImage>.Factory.StartNew(() => image);
        }

        /// <summary>
        ///     Remove the game images.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="cloudGameId">The cloud game image id.</param>
        /// <returns></returns>
        public Task<bool> RemoveGameImages(string cloudGameId, System.Guid gsiId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameImageResourcePath, cloudGameId, gsiId);
            return _httpClient.DeleteAsync(url).ContinueWith(tr => ProcessJsonResponse<bool>(tr));
        }

        /// <summary>
        ///     Gets the game modes.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<GameModeCollectionResponse> GetGameModes(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameModesResourcePath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ProcessJsonResponse<GameModeCollectionResponse>(tr));
        }

        /// <summary>
        ///     Creates the cloud game mode.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="gameMode">The game mode.</param>
        /// <param name="gameModeStream">The game mode stream.</param>
        /// <returns></returns>
        public Task<CreateGameModeResponse> CreateGameMode(string cloudGameId, GameMode gameMode, Stream gameModeStream)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameModesResourcePath, cloudGameId);
            var multipartContent = new MultipartFormDataContent();
            {
                multipartContent.Add(new StringContent(ToJson(gameMode)), "metadata");
                multipartContent.Add(new StreamContent(gameModeStream), "variant");
                return _httpClient.PostAsync(url, multipartContent).ContinueWith(tr => ProcessJsonResponse<CreateGameModeResponse>(tr));
            }
        }

        /// <summary>
        ///     Remove the game mode.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="cloudGameId">The cloud game variant id.</param>
        /// <returns></returns>
        public Task<bool> RemoveGameMode(string cloudGameId, System.Guid variantid)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameModeResourcePath, cloudGameId, variantid);
            return _httpClient.DeleteAsync(url).ContinueWith(tr => ProcessJsonResponse<bool>(tr));
        }

        /// <summary>
        ///     Gets the game certificates.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<CloudGameCertificateCollectionResponse> GetGameCertificates(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.CertificatesResourcePath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ProcessJsonResponse<CloudGameCertificateCollectionResponse>(tr));
        }

        /// <summary>
        ///     Creates the game certificate.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="certificateStream">The certificate stream.</param>
        /// <returns></returns>
        public Task<PostCloudGameCertificateResponse> CreateGameCertificate(string cloudGameId, GameCertificate certificate, Stream certificateStream)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.CertificatesResourcePath, cloudGameId);
            var multipartContent = new MultipartFormDataContent();
            {
                multipartContent.Add(new StringContent(ToJson(certificate)), "metadata");
                multipartContent.Add(new StreamContent(certificateStream), "certificate");
                return _httpClient.PostAsync(url, multipartContent).ContinueWith(tr => ProcessJsonResponse<PostCloudGameCertificateResponse>(tr));
            }
        }

        /// <summary>
        ///     Remove the game certificate.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="cloudGameId">The certificate id to be removed.</param>
        /// <returns></returns>
        public Task<bool> RemoveGameCertificate(string cloudGameId, System.Guid certificateId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.CertificateResourcePath, cloudGameId, certificateId);
            return _httpClient.DeleteAsync(url).ContinueWith(tr => ProcessJsonResponse<bool>(tr));
        }

        /// <summary>
        ///     Gets the game assets.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<CloudGameAssetCollectionResponse> GetGameAssets(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameAssetsResourcePath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ProcessJsonResponse<CloudGameAssetCollectionResponse>(tr));
        }

        /// <summary>
        ///     Creates the game asset.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="assetFile">The asset file.</param>
        /// <returns></returns>
        public Task<PostCloudGameAssetResponse> CreateGameAsset(string cloudGameId, CloudGameAssetRequest assetRequest, Stream assetFile)
        {
            string assetId = string.Empty;
            string assetUrl = string.Empty;
            PostCloudGameAssetResponse result = null;

            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameAssetsResourcePath, cloudGameId);
            var multpartFormContent = new MultipartFormDataContent();
            {
                multpartFormContent.Add(new StringContent(ToJson(assetRequest)), "metadata");
                result = _httpClient.PostAsync(url, multpartFormContent).ContinueWith(tr => ProcessJsonResponse<PostCloudGameAssetResponse>(tr)).Result;
                assetId = result.GameAssetId;
                assetUrl = result.GameAssetPreAuthUrl;
            }

            try
            {
                var cloudblob = new CloudBlob(assetUrl);
                Task.Factory.FromAsync(
                    (callback, state) => cloudblob.BeginUploadFromStream(assetFile, callback, state),
                    (ar) => cloudblob.EndUploadFromStream(ar),
                    TaskCreationOptions.None);
            }
            catch (StorageException)
            {
                string errorMessage = string.Format("Failed to asset file for cloud game to azure storage. gameId {0}, assetId {1}", cloudGameId, assetId);
                throw CreateExceptionFromJson(HttpStatusCode.Ambiguous, errorMessage);
            }

            var multpartFormContentMetadata = new MultipartFormDataContent();
            {
                url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameAssetResourcePath, cloudGameId, assetId);
                multpartFormContentMetadata.Add(new StringContent(ToJson(assetRequest)), "metadata");
                bool uploadResult = _httpClient.PutAsync(url, multpartFormContentMetadata).ContinueWith(tr => ProcessJsonResponse<bool>(tr)).Result;
            }

            return Task<PostCloudGameAssetResponse>.Factory.StartNew(() => result);
        }

        /// <summary>
        ///     Remove the game asset.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="cloudGameId">The asset id to be removed.</param>
        /// <returns></returns>
        public Task<bool> RemoveGameAsset(string cloudGameId, System.Guid assetId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameAssetResourcePath, cloudGameId, assetId);
            return _httpClient.DeleteAsync(url).ContinueWith(tr => ProcessJsonResponse<bool>(tr));
        }

        /// <summary>
        ///     Gets the game service summary report.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<DashboardSummary> GetGameServiceSummaryReport(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.DashboardSummaryPath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ProcessJsonResponse<DashboardSummary>(tr));
        }

        /// <summary>
        ///     Gets the game deployments report.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<ServiceDeploymentData> GetGameServiceDeploymentsReport(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.DeploymentsReportPath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ProcessJsonResponse<ServiceDeploymentData>(tr));
        }

        /// <summary>
        ///     Gets the game servicepools report.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<ServicePoolData> GetGameServicepoolsReport(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.ServicepoolsReportPath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ProcessJsonResponse<ServicePoolData>(tr));
        }

        /// <summary>
        ///     Creates the cloud game resource.
        /// </summary>
        /// <param name="game">The game resource.</param>
        /// <param name="schemaFileName">The schema file name.</param>
        /// <param name="schemaStream">The schema stream.</param>
        /// <returns>The cloud task for completion</returns>
        public Task<bool> CreateGameService(CloudGames game, string schemaFileName, Stream schemaStream)
        {
            // Check registration.
            string url = _httpClient.BaseAddress + String.Format("/services?service=gameservices.xboxlivecompute&action=register");
            bool result = _httpClient.PutAsync(url, null).ContinueWith(tr => ProcessJsonResponse<bool>(tr, true, HttpStatusCode.Conflict)).Result;

            //var preferredRegion = await GetPreferredRegionAsync(game.subscriptionId, game.name);
            // TODO Get the preferred region and set it into cloud service object

            // Create cloud service.
            url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.PutCloudServiceResourcePath);
            
            Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract.CloudService cloudService = new Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract.CloudService()// CloudServices()
            {
                Name = CloudGameUriElements.DefaultServiceName,
                Description = CloudGameUriElements.DefaultServiceName,
                GeoRegion = "West US",
                Label = CloudGameUriElements.DefaultServiceName
            };

            result = _httpClient.PutAsXmlAsync(url, cloudService).ContinueWith(tr => ProcessJsonResponse<bool>(tr, true, HttpStatusCode.BadRequest)).Result;

            // Create gsiset.
            game.gsiSetId = Guid.NewGuid().ToString();
            //game.publisherId = Guid.NewGuid().ToString();

            string schemaContent;
            using (var streamReader = new StreamReader(schemaStream))
            {
                schemaContent = streamReader.ReadToEnd();
            }

            var varSchemaRequestData = new CreateVariantSchemaRequest()
            {
                Metadata = new CloudGameVariantSchemaRequest() { Name = game.schemaName, Filename = schemaFileName, TitleId = game.titleId },
                Content = schemaContent
            };

            var putGameRequest = new PutCloudGameRequest()
            {
                cloudGame = game,
                variantSchema = varSchemaRequestData
            };

            var doc = new XmlDocument();
            var resource = new Resource()
            {
                Name = game.name,
                ETag = Guid.NewGuid().ToString(),
                Plan = string.Empty,
                ResourceProviderNamespace = CloudGameUriElements.NamespaceName,
                Type = CloudGameUriElements.XboxLiveComputeResourceType,
                SchemaVersion = CloudGameUriElements.SchemaVersion,
                IntrinsicSettings = new XmlNode[]
                {
                    doc.CreateCDataSection(ToJson(putGameRequest))
                }
            }; 

            url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.PutCloudGamesResourcePath, game.name);
            result = _httpClient.PutAsXmlAsync(url, resource).ContinueWith(tr => ProcessJsonResponse<bool>(tr)).Result;

            return Task<bool>.Factory.StartNew(() => result);
        }

        /// <summary>
        /// Gets the cloud service.
        /// </summary>
        /// <returns></returns>
        public Task<CloudGamesList> GetGameService()
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GetCloudServicesResourcePath);
            return _httpXmlClient.GetAsync(url, Logger).ContinueWith(tr => ProcessCloudServiceResponse(tr));

        }

        /// <summary>
        /// Gets the resource properties.
        /// </summary>
        /// <returns>The task for completion.</returns>
        public Task<PublisherCloudGameInfoResponse> GetResourceProperties()
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.ResourcePropertiesPath);

            ResourceProviderProperties propertyList;
            propertyList = _httpXmlClient.GetAsync(url, Logger).ContinueWith(tr => ProcessXmlResponse<ResourceProviderProperties>(tr)).Result;

            if (propertyList == null)
            {
                return null;
            }

            var property = propertyList.Find((prop) => prop.Key == "publisherInfo");
            if (property == null ||
                property.Value == null)
            {
                return null;
            }

            PublisherCloudGameInfoResponse result = DeserializeJsonToObject<PublisherCloudGameInfoResponse>(property.Value);
            return Task<PublisherCloudGameInfoResponse>.Factory.StartNew(() => result);
        }

        /// <summary>
        /// Publishes the cloud game async.
        /// </summary>
        /// <param name="cloudGameName">Name of the cloud game.</param>
        /// <returns>The task for completion.</returns>
        public Task<bool> PublishCloudGameAsync(string cloudGameName)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.PublishCloudGamePath, cloudGameName);
            return _httpClient.PutAsync(url, null).ContinueWith(tr => ProcessJsonResponse<bool>(tr));
        }

        /// <summary>
        ///     Processes the response and handle error cases.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseMessage">The response message.</param>
        /// <returns></returns>
        /// <exception cref="Microsoft.WindowsAzure.ServiceManagement.ServiceManagementClientException"></exception>
        /// <exception cref="ServiceManagementError"></exception>
        private static T ProcessJsonResponse<T>(Task<HttpResponseMessage> responseMessage, bool ignore = false, HttpStatusCode ignoreStatusCode = HttpStatusCode.Conflict)
        {
            HttpResponseMessage message = responseMessage.Result;
            if (typeof(T) == typeof(bool) && (message.IsSuccessStatusCode || (ignore == true && message.StatusCode == ignoreStatusCode)))
            {
                return (T) (object) true;
            }

            string content = message.Content.ReadAsStringAsync().Result;

            if (message.IsSuccessStatusCode)
            {
                return (T) JsonConvert.DeserializeObject(content, typeof (T));
            }

            throw CreateExceptionFromJson(message.StatusCode, content);
        }

        private static CloudGamesList ProcessCloudServiceResponse(Task<HttpResponseMessage> responseMessage)
        {
            var message = responseMessage.Result;
            var content = message.Content.ReadAsStringAsync().Result;
            var encoding = GetEncodingFromResponseMessage(message);
            var response = new CloudGamesList();

            if (message.IsSuccessStatusCode)
            {
                var ser = new DataContractSerializer(typeof(Contract.CloudService));
                using (var stream = new MemoryStream(encoding.GetBytes(content)))
                {
                    stream.Position = 0;
                    var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
                    var serviceResponse = (Contract.CloudService)ser.ReadObject(reader, true);

                    foreach (var resource in serviceResponse.Resources.Where(resource => resource.OperationStatus.Error != null))
                    {
                        if (resource.IntrinsicSettings == null || resource.IntrinsicSettings.Length == 0)
                        {
                            response.Add(new CloudGames() { name = resource.Name });
                            continue;
                        }

                        var cbData = resource.IntrinsicSettings[0] as XmlCDataSection;
                        if (cbData == null)
                        {
                            continue;
                        }

                        var jsonSer = new DataContractJsonSerializer(typeof(CloudGames));
                        using (var jsonStream = new MemoryStream(encoding.GetBytes(cbData.Data)))
                        {
                            var game = (CloudGames)jsonSer.ReadObject(jsonStream);
                            response.Add(game);                          
                        }
                    }
                }

                return response;
            }

            throw CreateExceptionFromXml(content, message);
        }

        private static T ProcessXmlResponse<T>(Task<HttpResponseMessage> responseMessage)
        {
            HttpResponseMessage message = responseMessage.Result;
            string content = message.Content.ReadAsStringAsync().Result;
            Encoding encoding = GetEncodingFromResponseMessage(message);

            if (message.IsSuccessStatusCode)
            {
                var ser = new DataContractSerializer(typeof(T));
                using (var stream = new MemoryStream(encoding.GetBytes(content)))
                {
                    stream.Position = 0;
                    XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
                    return (T)ser.ReadObject(reader, true);
                }
            }

            throw CreateExceptionFromXml(content, message);
        }

        private static Encoding GetEncodingFromResponseMessage(HttpResponseMessage message)
        {
            string encodingString = message.Content.Headers.ContentType.CharSet;
            Encoding encoding = Encoding.GetEncoding(encodingString);
            return encoding;
        }

        private static ServiceManagementClientException CreateExceptionFromXml(string content, HttpResponseMessage message)
        {
            Encoding encoding = GetEncodingFromResponseMessage(message);

            using (var stream = new MemoryStream(encoding.GetBytes(content)))
            {
                stream.Position = 0;
                var serializer = new XmlSerializer(typeof(ServiceError));
                var serviceError = (ServiceError)serializer.Deserialize(stream);
                return new ServiceManagementClientException(message.StatusCode,
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
        private static ServiceManagementClientException CreateExceptionFromJson(HttpStatusCode statusCode, string content)
        {
            var exception = new ServiceManagementClientException(statusCode,
                new ServiceManagementError
                {
                    Code = statusCode.ToString(),
                    Message = content
                },
                string.Empty);
            return exception;
        }

        /// <summary>
        ///     Creates and initialise instance of HttpClient
        /// </summary>
        /// <returns></returns>
        private static HttpClient CreateCloudGameHttpClient(SubscriptionData subscription)
        {
            var requestHandler = new WebRequestHandler();
            requestHandler.ClientCertificates.Add(subscription.Certificate);
            var endpoint = new StringBuilder(General.EnsureTrailingSlash(subscription.ServiceEndpoint));
            endpoint.Append(subscription.SubscriptionId);

            HttpClient client = HttpClientHelper.CreateClient(endpoint.ToString(), handler: requestHandler);
            client.DefaultRequestHeaders.Add(CloudGameUriElements.XblCorrelationHeader, Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add(Microsoft.WindowsAzure.ServiceManagement.Constants.VersionHeaderName, "2012-08-01");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(CloudGameUriElements.ApplicationJsonMediaType));
            return client;
        }

        /// <summary>
        ///     Creates and initialise instance of HttpClient
        /// </summary>
        /// <returns></returns>
        private static HttpClient CreateCloudGameXmlClient(SubscriptionData subscription)
        {
            var requestHandler = new WebRequestHandler();
            requestHandler.ClientCertificates.Add(subscription.Certificate);
            var endpoint = new StringBuilder(General.EnsureTrailingSlash(subscription.ServiceEndpoint));
            endpoint.Append(subscription.SubscriptionId);

            HttpClient client = HttpClientHelper.CreateClient(endpoint.ToString(), handler: requestHandler);
            client.DefaultRequestHeaders.Add(CloudGameUriElements.XblCorrelationHeader, Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add(Microsoft.WindowsAzure.ServiceManagement.Constants.VersionHeaderName, "2012-08-01");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(CloudGameUriElements.ApplicationXmlMediaType));
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