// ----------------------------------------------------------------------------------
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

using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Management.Utilities.MediaService.Services.MediaServicesEntities;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract;

namespace Microsoft.WindowsAzure.Management.Utilities.CloudGame
{
    /// <summary>
    ///     Defines interface to communicate with Azure Cloud Game Services REST API
    /// </summary>
    public interface ICloudGameClient
    {
        /// <summary>
        ///     Gets the game images.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        Task<CloudGameImageCollectionResponse> GetGameImages(string cloudGameId);

        /// <summary>
        ///     Creates the cloud game image.
        /// </summary>
        /// <param name="cloudGameId">The cloud game name.</param>
        /// <param name="image">The game image.</param>
        /// <param name="cspkg">The game CSPKG.</param>
        /// <param name="cscfg">The game CSCFG.</param>
        /// <param name="asset">The game asset.</param>
        /// <returns></returns>
        Task<CloudGameImage> CreateGameImage(string cloudGameId,
                            CloudGameImage image,
                            Stream cspkg,
                            Stream cscfg,
                            Stream asset);

        /// <summary>
        ///     Remove the game images.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="cloudGameId">The cloud game image id.</param>
        /// <returns></returns>
        Task<bool> RemoveGameImages(string cloudGameId, System.Guid gsiId);

        /// <summary>
        ///     Gets the game modes.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        Task<GameModeCollectionResponse> GetGameModes(string cloudGameId);

        /// <summary>
        ///     Creates the cloud game mode.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="gameMode">The game mode.</param>
        /// <param name="gameModeStream">The game mode stream.</param>
        /// <returns></returns>
        Task<CreateGameModeResponse> CreateGameMode(string cloudGameId, GameMode gameMode, Stream gameModeStream);

        /// <summary>
        ///     Remove the game mode.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="cloudGameId">The cloud game variant id.</param>
        /// <returns></returns>
        Task<bool> RemoveGameMode(string cloudGameId, System.Guid variantid);

        /// <summary>
        ///     Gets the game certificates.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        Task<CloudGameCertificateCollectionResponse> GetGameCertificates(string cloudGameId);

        /// <summary>
        ///     Creates the game certificate.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="certificateStream">The certificate stream.</param>
        /// <returns></returns>
        Task<PostCloudGameCertificateResponse> CreateGameCertificate(string cloudGameId, GameCertificate certificate, Stream certificateStream);

        /// <summary>
        ///     Remove the game certificate.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="cloudGameId">The certificate id to be removed.</param>
        /// <returns></returns>
        Task<bool> RemoveGameCertificate(string cloudGameId, System.Guid certificateId);

        /// <summary>
        ///     Gets the game assets.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        Task<CloudGameAssetCollectionResponse> GetGameAssets(string cloudGameId);

        /// <summary>
        ///     Creates the game asset.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="assetFile">The asset file.</param>
        /// <returns></returns>
        Task<PostCloudGameAssetResponse> CreateGameAsset(string cloudGameId, CloudGameAssetRequest assetRequest, Stream assetFile);

        /// <summary>
        ///     Remove the game asset.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <param name="cloudGameId">The asset id to be removed.</param>
        /// <returns></returns>
        Task<bool> RemoveGameAsset(string cloudGameId, System.Guid assetId);

        /// <summary>
        ///     Gets the game service summary report.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        Task<DashboardSummary> GetGameServiceSummaryReport(string cloudGameId);

        /// <summary>
        /// Gets the game deployments report.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        Task<ServiceDeploymentData> GetGameServiceDeploymentsReport(string cloudGameId);

        /// <summary>
        /// Gets the game servicepools report.
        /// </summary>
        /// <param name="cloudGameId">The cloud game id.</param>
        /// <returns></returns>
        Task<ServicePoolData> GetGameServicepoolsReport(string cloudGameId);

        /// <summary>
        ///     Creates the cloud game resource.
        /// </summary>
        /// <param name="game">The game resource.</param>
        /// <param name="schemaFileName">The schema file name.</param>
        /// <param name="schemaStream">The schema stream.</param>
        /// <returns>The cloud task for completion</returns>
        Task<bool> CreateGameService(CloudGames game, string schemaFileName, Stream schemaStream);

        /// <summary>
        /// Gets the cloud service.
        /// </summary>
        /// <returns></returns>
        Task<CloudGamesList> GetGameService();

        /// <summary>
        /// Gets the resource properties.
        /// </summary>
        /// <returns>The task for completion.</returns>
        Task<PublisherCloudGameInfoResponse> GetResourceProperties();

        /// <summary>
        /// Publishes the cloud game async.
        /// </summary>
        /// <param name="cloudGameName">Name of the cloud game.</param>
        /// <returns>The task for completion.</returns>
        Task<bool> PublishCloudGameAsync(string cloudGameName);
    }
}