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
        ///     Gets the game packages.
        /// </summary>
        /// <param name="cloudGameName">The cloud game id.</param>
        /// <returns></returns>
        Task<CloudGameImageCollectionResponse> GetGamePackages(string cloudGameName);

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
        Task<bool> CreateGamePackage(
            string cloudGameName,
            string packageName,
            int maxPlayers,
            string assetId,
            string cspkgFileName,
            Stream cspkgStream,
            string cscfgFileName,
            Stream cscfgStream);

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
        /// <param name="gameModeName">The game mode name.</param>
        /// <param name="gameModeFileName">The game mode original filename.</param>
        /// <param name="gameModeStream">The game mode stream.</param>
        /// <returns></returns>
        Task<CreateGameModeResponse> CreateGameMode(string cloudGameId, string gameModeName, string gameModeFileName, Stream gameModeStream);

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
        /// <param name="cloudGameName">The cloud game id.</param>
        /// <returns></returns>
        Task<CloudGameAssetCollectionResponse> GetGameAssets(string cloudGameName);

        /// <summary>
        ///     Creates the game asset.
        /// </summary>
        /// <param name="cloudGameName">The cloud game name.</param>
        /// <param name="gameAssetName">The asset name.</param>
        /// <param name="gameAssetFileName">The asset filename.</param>
        /// <param name="gameAssetstream">The asset filestream.</param>
        /// <returns></returns>
        Task<PostCloudGameAssetResponse> CreateGameAsset(
            string cloudGameName, 
            string gameAssetName, 
            string gameAssetFileName, 
            Stream gameAssetStream);

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
        Task<bool> CreateGameService(string titleId,
            string sandboxes,
            string resourceSetIds,
            string name,
            string schemaName,
            string schemaFileName, 
            Stream schemaStream);

        /// <summary>
        /// Removes a Game Service
        /// </summary>
        /// <param name="name">The service to remove</param>
        /// <returns></returns>
        Task<bool> RemoveGameService(string name);

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