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

    using Microsoft.WindowsAzure.Management.Utilities.CloudGameClientHelper;

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
        ///     Creates new CloudGameClient.
        /// </summary>
        /// <param name="subscription">The Windows Azure subscription data object</param>
        /// <param name="logger">The logger action</param>
        /// <param name="httpClient">The HTTP Client to use to communicate with RDFE</param>
        /// <param name="httpXmlClient">The HTTP Client for processing XML data</param>
        public CloudGameClient(SubscriptionData subscription, Action<string> logger, HttpClient httpClient, HttpClient httpXmlClient)
        {
            _subscriptionId = subscription.SubscriptionId;
            Subscription = subscription;
            Logger = logger;
            _httpClient = httpClient;
            _httpXmlClient = httpXmlClient;
        }

        /// <summary>
        ///     Creates new CloudGameClient.
        /// </summary>
        /// <param name="subscription">The Windows Azure subscription data object</param>
        /// <param name="logger">The logger action</param>
        public CloudGameClient(SubscriptionData subscription, Action<string> logger)
            : this(subscription, 
                   logger, 
                   ClientHelper.CreateCloudGameHttpClient(subscription, CloudGameUriElements.ApplicationJsonMediaType), 
                   ClientHelper.CreateCloudGameHttpClient(subscription, CloudGameUriElements.ApplicationXmlMediaType))
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
        ///     Gets the game packages.
        /// </summary>
        /// <param name="cloudGameName">The cloud game name.</param>
        /// <returns></returns>
        public Task<CloudGameImageCollectionResponse> GetGamePackages(string cloudGameName)
        {
            var url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameImagesResourcePath, cloudGameName);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ClientHelper.ProcessJsonResponse<CloudGameImageCollectionResponse>(tr));
        }

        /// <summary>
        ///     Upload package components to a cloud game.
        /// </summary>
        /// <param name="cloudGameName">The cloud game name.</param>
        /// <param name="packageName">The name of the package</param>
        /// <param name="maxPlayers">The max number of players allowed</param>
        /// <param name="assetId">The id of a previously uploaded asset file</param>
        /// <param name="cspkgFileName">The name of the local cspkg file name</param>
        /// <param name="cspkgStream">The cspkg file stream</param>
        /// <param name="cscfgFileName">The name of the local cscfg file name</param>
        /// <param name="cscfgStream">The game cscfg file stream</param>
        /// <returns>
        /// True if successful
        /// </returns>
        public Task<bool> CreateGamePackage(
            string cloudGameName,
            string packageName,
            int maxPlayers,
            string assetId,
            string cspkgFileName,
            Stream cspkgStream,
            string cscfgFileName,
            Stream cscfgStream)
        {
            Guid assetIdGuid;
            var haveAsset = Guid.TryParse(assetId, out assetIdGuid);
            var requestMetadata = new CloudGameImageRequest()
            {
                CspkgFilename = cspkgFileName,
                CscfgFilename = cscfgFileName,
                MaxAllowedPlayers = maxPlayers,
                MinRequiredPlayers = 1,
                Name = packageName,
                AssetId = haveAsset ? assetId : null        // GSRM currently requires NULL or a valid Guid
            };

            PostCloudGameImageResponse responseMetadata;
            using (var multipartFormContent = new MultipartFormDataContent())
            {
                multipartFormContent.Add(new StringContent(ClientHelper.ToJson(requestMetadata)), "metadata");
                multipartFormContent.Add(new StreamContent(cscfgStream), "packageconfig");

                var url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameImagesResourcePath, cloudGameName);
                responseMetadata = _httpClient.PostAsync(url, multipartFormContent).ContinueWith(tr => ClientHelper.ProcessJsonResponse<PostCloudGameImageResponse>(tr)).Result;
           }

            try
            {
                // Use the pre-auth URL received in the response to upload the cspkg file. Wait for it to complete
                var cloudblob = new CloudBlob(responseMetadata.CspkgPreAuthUrl);
                Task.Factory.FromAsync(
                    (callback, state) => cloudblob.BeginUploadFromStream(cspkgStream, callback, state),
                    cloudblob.EndUploadFromStream,
                    TaskCreationOptions.None).Wait();
            }
            catch (StorageException)
            {
                var errorMessage = string.Format("Failed to upload cspkg for cloud game. gameId {0} cspkgName {1}", cloudGameName, cspkgFileName);
                throw ClientHelper.CreateExceptionFromJson(HttpStatusCode.Ambiguous, errorMessage);
            }

            var result = false;
            using (var multipartFormContent = new MultipartFormDataContent())
            {
                multipartFormContent.Add(new StringContent(ClientHelper.ToJson(requestMetadata)), "metadata");
                var url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameImageResourcePath, cloudGameName, responseMetadata.GsiId);
                result = _httpClient.PutAsync(url, multipartFormContent).ContinueWith(
                    tr =>
                    {
                        var message = tr.Result;
                        if (message.IsSuccessStatusCode)
                        {
                            return true;
                        }

                        // Error result, so throw an exception
                        throw new ServiceManagementClientException(message.StatusCode,
                            new ServiceManagementError
                            {
                                Code = message.StatusCode.ToString()
                            },
                            string.Empty);
                    }).Result;
            }

            return Task<bool>.Factory.StartNew(() => result);
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
            return _httpClient.DeleteAsync(url).ContinueWith(tr => ClientHelper.ProcessJsonResponse<bool>(tr));
        }

        /// <summary>
        ///     Gets the game modes.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<GameModeCollectionResponse> GetGameModes(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameModesResourcePath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ClientHelper.ProcessJsonResponse<GameModeCollectionResponse>(tr));
        }

        /// <summary>
        ///     Creates the cloud game mode.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="gameModeName">The game mode name</param>
        /// <param name="gameModeFileName">The game mode oringal filename</param>
        /// <param name="gameModeStream">The game mode stream.</param>
        /// <returns></returns>
        public Task<CreateGameModeResponse> CreateGameMode(string cloudGameId, string gameModeName, string gameModeFileName, Stream gameModeStream)
        {
            var url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameModesResourcePath, cloudGameId);
            var multipartContent = new MultipartFormDataContent();
            {
                var newGameMode = new GameMode()
                {
                    name = gameModeName,
                    fileName = gameModeFileName
                };
                multipartContent.Add(new StringContent(ClientHelper.ToJson(newGameMode)), "metadata");
                multipartContent.Add(new StreamContent(gameModeStream), "variant");
                return _httpClient.PostAsync(url, multipartContent).ContinueWith(tr => ClientHelper.ProcessJsonResponse<CreateGameModeResponse>(tr));
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
            return _httpClient.DeleteAsync(url).ContinueWith(tr => ClientHelper.ProcessJsonResponse<bool>(tr));
        }

        /// <summary>
        ///     Gets the game certificates.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<CloudGameCertificateCollectionResponse> GetGameCertificates(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.CertificatesResourcePath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ClientHelper.ProcessJsonResponse<CloudGameCertificateCollectionResponse>(tr));
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
                multipartContent.Add(new StringContent(ClientHelper.ToJson(certificate)), "metadata");
                multipartContent.Add(new StreamContent(certificateStream), "certificate");
                return _httpClient.PostAsync(url, multipartContent).ContinueWith(tr => ClientHelper.ProcessJsonResponse<PostCloudGameCertificateResponse>(tr));
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
            return _httpClient.DeleteAsync(url).ContinueWith(tr => ClientHelper.ProcessJsonResponse<bool>(tr));
        }

        /// <summary>
        ///     Gets the game assets.
        /// </summary>
        /// <param name="cloudGameName">The cloud game id.</param>
        /// <returns></returns>
        public Task<CloudGameAssetCollectionResponse> GetGameAssets(string cloudGameName)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameAssetsResourcePath, cloudGameName);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ClientHelper.ProcessJsonResponse<CloudGameAssetCollectionResponse>(tr));
        }

        /// <summary>
        ///     Creates the game asset.
        /// </summary>
        /// <param name="cloudGameName">The cloud game name.</param>
        /// <param name="gameAssetName">The asset name.</param>
        /// <param name="gameAssetFileName">The asset filename.</param>
        /// <param name="gameAssetstream">The asset filestream.</param>
        /// <returns></returns>
        public Task<PostCloudGameAssetResponse> CreateGameAsset(
            string cloudGameName, 
            string gameAssetName, 
            string gameAssetFileName, 
            Stream gameAssetStream)
        {
            // Call in to get an AssetID and preauthURL to use for upload of the asset
            var newGameAssetRequest = new CloudGameAssetRequest()
            {
                Filename = gameAssetFileName,
                Name = gameAssetName
            };

            var multipartFormContent = new MultipartFormDataContent
            {
                {
                    new StringContent(ClientHelper.ToJson(newGameAssetRequest)),"metadata"
                }
            };

            var url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameAssetsResourcePath, cloudGameName);
            var postAssetResult = _httpClient.PostAsync(url, multipartFormContent).ContinueWith(tr => ClientHelper.ProcessJsonResponse<PostCloudGameAssetResponse>(tr)).Result;

            try
            {
                var cloudblob = new CloudBlob(postAssetResult.GameAssetPreAuthUrl);
                Task.Factory.FromAsync(
                    (callback, state) => cloudblob.BeginUploadFromStream(gameAssetStream, callback, state),
                    cloudblob.EndUploadFromStream,
                    TaskCreationOptions.None).Wait();
            }
            catch (StorageException)
            {
                var errorMessage = string.Format("Failed to asset file for cloud game to azure storage. gameId {0}, assetId {1}", cloudGameName, postAssetResult.GameAssetId);
                throw ClientHelper.CreateExceptionFromJson(HttpStatusCode.Ambiguous, errorMessage);
            }

            var multpartFormContentMetadata = new MultipartFormDataContent
            {
                {
                    new StringContent(ClientHelper.ToJson(newGameAssetRequest)),"metadata"
                }
            };

            url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GameAssetResourcePath, cloudGameName, postAssetResult.GameAssetId);
            _httpClient.PutAsync(url, multpartFormContentMetadata).ContinueWith(
                tr =>
                    {
                        var message = tr.Result;
                        if (message.IsSuccessStatusCode)
                        {
                            return true;
                        }

                        // Error result, so throw an exception
                        throw new ServiceManagementClientException(
                            message.StatusCode,
                            new ServiceManagementError { Code = message.StatusCode.ToString() },
                            string.Empty);
                    });

            // Return the Asset info
            return Task<PostCloudGameAssetResponse>.Factory.StartNew(() => postAssetResult);
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
            return _httpClient.DeleteAsync(url).ContinueWith(tr => ClientHelper.ProcessJsonResponse<bool>(tr));
        }

        /// <summary>
        ///     Gets the game service summary report.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<DashboardSummary> GetGameServiceSummaryReport(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.DashboardSummaryPath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ClientHelper.ProcessJsonResponse<DashboardSummary>(tr));
        }

        /// <summary>
        ///     Gets the game deployments report.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<ServiceDeploymentData> GetGameServiceDeploymentsReport(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.DeploymentsReportPath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ClientHelper.ProcessJsonResponse<ServiceDeploymentData>(tr));
        }

        /// <summary>
        ///     Gets the game servicepools report.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        public Task<ServicePoolData> GetGameServicepoolsReport(string cloudGameId)
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.ServicepoolsReportPath, cloudGameId);
            return _httpClient.GetAsync(url, Logger).ContinueWith(tr => ClientHelper.ProcessJsonResponse<ServicePoolData>(tr));
        }

        /// <summary>
        ///     Creates a new cloud game resource.
        /// </summary>
        /// <param name="titleId">The title ID within the subscription to use (in Decimal form)</param>
        /// <param name="sandboxes">A comma seperated list of sandbox names</param>
        /// <param name="resourceSetIds">A comma seperated list of resource set IDs</param>
        /// <param name="name">The name of the Cloud Game</param>
        /// <param name="schemaName">The name of the game mode schema to sue</param>
        /// <param name="schemaFileName">The local schema file name (only used for reference)</param>
        /// <param name="schemaStream">The schema data as a file stream.</param>
        /// <returns>The cloud task for completion</returns>
        public Task<bool> CreateGameService(
            string titleId,
            string sandboxes,
            string resourceSetIds,
            string name,
            string schemaName,
            string schemaFileName, 
            Stream schemaStream)
        {      
            // Idempotent call to to a first time registration of the cloud service wrapping container.
            ClientHelper.RegisterCloudService(_httpClient, _httpXmlClient);

            string schemaContent;
            using (var streamReader = new StreamReader(schemaStream))
            {
                schemaContent = streamReader.ReadToEnd();
            }

            var varSchemaRequestData = new CreateVariantSchemaRequest()
            {
                Metadata = new CloudGameVariantSchemaRequest()
                {
                    Name = schemaName, 
                    Filename = schemaFileName, 
                    TitleId = titleId
                },
                Content = schemaContent
            };

            var newCloudGame = new Contract.CloudGame()
            {
                displayName = name,
                name = name,
                resourceSets = resourceSetIds,
                sandboxes = sandboxes,
                schemaName = schemaName,
                titleId = titleId,
                status = "Creating",                    // BUGBUG... GSRM needs to fix depending on this property being set
                schemaId = Guid.NewGuid().ToString()    // BUGBUG... GSRM needs to fix depending on this property being set
            };

            var putGameRequest = new PutCloudGameRequest()
            {
                cloudGame = newCloudGame,
                variantSchema = varSchemaRequestData
            };

            var doc = new XmlDocument();
            var resource = new Resource()
            {
                Name = name,
                ETag = Guid.NewGuid().ToString(),       // BUGBUG What should this ETag value be?
                Plan = string.Empty,
                ResourceProviderNamespace = CloudGameUriElements.NamespaceName,
                Type = CloudGameUriElements.XboxLiveComputeResourceType,
                SchemaVersion = CloudGameUriElements.SchemaVersion,
                IntrinsicSettings = new XmlNode[]
                {
                    doc.CreateCDataSection(ClientHelper.ToJson(putGameRequest))
                }
            }; 

            var url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.CloudGameResourcePath, name);
            var result = _httpClient.PutAsXmlAsync(url, resource).ContinueWith(
                tr =>
                {
                    var message = tr.Result;
                    if (message.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    // Error result, so throw an exception
                    throw new ServiceManagementClientException(message.StatusCode,
                        new ServiceManagementError
                        {
                            Code = message.StatusCode.ToString()
                        },
                        string.Empty);
                }).Result;

            return Task<bool>.Factory.StartNew(() => result);
        }

        /// <summary>
        /// Removes a Game Service
        /// </summary>
        /// <param name="name">The service to remove</param>
        /// <returns></returns>
        public Task<bool> RemoveGameService(string name)
        {
            var url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.CloudGameResourcePath, name);
            return _httpClient.DeleteAsync(url).ContinueWith(
                tr =>
                    {
                        var message = tr.Result;
                        if (message.IsSuccessStatusCode)
                        {
                            return true;
                        }

                        // Error result, so throw an exception
                        throw new ServiceManagementClientException(
                            message.StatusCode,
                            new ServiceManagementError { Code = message.StatusCode.ToString() },
                            string.Empty);
                    });
        }

        /// <summary>
        /// Gets the cloud service.
        /// </summary>
        /// <returns></returns>
        public Task<CloudGamesList> GetGameService()
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.GetCloudServicesResourcePath);
            return _httpXmlClient.GetAsync(url, Logger).ContinueWith(tr => ClientHelper.ProcessCloudServiceResponse(tr));

        }

        /// <summary>
        /// Gets the resource properties.
        /// </summary>
        /// <returns>The task for completion.</returns>
        public Task<PublisherCloudGameInfoResponse> GetResourceProperties()
        {
            string url = _httpClient.BaseAddress + String.Format(CloudGameUriElements.ResourcePropertiesPath);

            ResourceProviderProperties propertyList;
            propertyList = _httpXmlClient.GetAsync(url, Logger).ContinueWith(tr => ClientHelper.ProcessXmlResponse<ResourceProviderProperties>(tr)).Result;

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

            PublisherCloudGameInfoResponse result = ClientHelper.DeserializeJsonToObject<PublisherCloudGameInfoResponse>(property.Value);
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
            return _httpClient.PutAsync(url, null).ContinueWith(tr => ClientHelper.ProcessJsonResponse<bool>(tr));
        }
    }
}