﻿// ----------------------------------------------------------------------------------
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

namespace Microsoft.WindowsAzure.Management.Utilities.XblCompute
{
    using System;

    using Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract;
    using Microsoft.WindowsAzure.Management.Utilities.XblCompute.Contract;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    ///     Defines interface to communicate with Azure Game Services REST API
    /// </summary>
    public interface IXblComputeClient
    {
        /// <summary>
        ///     Gets the game packages.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <returns></returns>
        Task<XblPackageCollectionResponse> GetXblPackages(string xblComputeName);

        /// <summary>
        ///     Upload package components to a XblCompute instance.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
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
        Task<bool> NewXblPackage(
            string xblComputeName,
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
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <param name="xblPackageId">The xblPackage id.</param>
        /// <returns></returns>
        Task<bool> RemoveXblPackage(string xblComputeName, System.Guid xblPackageId);

        /// <summary>
        ///     Gets the Xbl game modes.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <returns></returns>
        Task<XblGameModeCollectionResponse> GetXblGameModes(string xblComputeName);

        /// <summary>
        ///     Creates a Xbl Game Mode.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <param name="xblGameModeName">The game mode name.</param>
        /// <param name="xblGameModeFileName">The game mode original filename.</param>
        /// <param name="xblGameModeStream">The game mode stream.</param>
        /// <returns></returns>
        Task<NewXblGameModeResponse> NewXblGameMode(string xblComputeName, 
            string xblGameModeName, 
            string xblGameModeFileName, 
            Stream xblGameModeStream);

        /// <summary>
        ///     Remove the Xbl game mode.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <param name="xblGameModeId">The Xbl game mode id.</param>
        /// <returns></returns>
        Task<bool> RemoveXblGameMode(string xblComputeName, System.Guid xblGameModeId);

        /// <summary>
        ///     Gets the XblCertificates.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <returns></returns>
        Task<XblCertificateCollectionResponse> GetXblCertificates(string xblComputeName);

        /// <summary>
        ///     Creates a XblCertificate.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <param name="certificateName">The certificate name.</param>
        /// <param name="certificateFileName">The certificate filename.</param>
        /// <param name="certificatePassword">The certificate password.</param>
        /// <param name="certificateStream">The certificate stream.</param>
        /// <returns></returns>
        Task<XblCertificatePostResponse> NewXblCertificate(string xblComputeName, 
            string certificateName, 
            string certificateFileName, 
            string certificatePassword,
            Stream certificateStream);

        /// <summary>
        ///     Remove the game certificate.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <param name="xblCertificateId">The XblCertificate id to be removed.</param>
        /// <returns></returns>
        Task<bool> RemoveXblCertificate(string xblComputeName, System.Guid xblCertificateId);

        /// <summary>
        ///     Gets the XblAssets.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <returns></returns>
        Task<XblAssetCollectionResponse> GetXblAssets(string xblComputeName);

        /// <summary>
        ///     Creates a new Xbl game asset.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <param name="xblAssetName">The asset name.</param>
        /// <param name="xblAssetFileName">The asset filename.</param>
        /// <param name="xblAssetStream">The asset filestream.</param>
        /// <returns></returns>
        Task<string> NewXblAsset(
            string xblComputeName, 
            string xblAssetName, 
            string xblAssetFileName, 
            Stream xblAssetStream);

        /// <summary>
        ///     Remove the xbl asset.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <param name="xblAssetId">The asset id to be removed.</param>
        /// <returns></returns>
        Task<bool> RemoveXblAsset(string xblComputeName, System.Guid xblAssetId);

        /// <summary>
        ///     Gets the Xbl Compute summary report.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <returns></returns>
        Task<DashboardSummary> GetXblComputeSummaryReport(string xblComputeName);

        /// <summary>
        /// Gets the XblCompute deployments report.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <returns></returns>
        Task<XblComputeDeploymentData> GetXblComputeDeploymentsReport(string xblComputeName);

        /// <summary>
        /// Gets the XblCompute servicepools report.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <returns></returns>
        Task<XblComputePoolData> GetXblComputePoolsReport(string xblComputeName);

        /// <summary>
        ///     Creates a new XblCompute resource.
        /// </summary>
        /// <param name="titleId">The title ID within the subscription to use (in Decimal form)</param>
        /// <param name="selectionOrder">The selection order to use</param>
        /// <param name="sandboxes">A comma seperated list of sandbox names</param>
        /// <param name="resourceSetIds">A comma seperated list of resource set IDs</param>
        /// <param name="name">The name of the Cloud Game</param>
        /// <param name="schemaId">The Id of an existing variant schema</param>
        /// <param name="schemaName">The name of the game mode schema to sue</param>
        /// <param name="schemaFileName">The local schema file name (only used for reference)</param>
        /// <param name="schemaStream">The schema data as a file stream.</param>
        /// <returns>The cloud task for completion</returns>
        Task<bool> NewXblCompute(
			string titleId,
            int selectionOrder,
            string sandboxes,
            string resourceSetIds,
            string name,
            string schemaId,
            string schemaName,
            string schemaFileName, 
            Stream schemaStream);

        /// <summary>
        /// Removes a XblCompute instnace
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <returns></returns>
        Task<bool> RemoveXblCompute(string xblComputeName);

        /// <summary>
        /// Gets the XblCompute instances for the Azure Game Services resource in the current subscription
        /// </summary>
        /// <returns></returns>
        Task<XblComputeColletion> GetXblComputeInstances();

        /// <summary>
        /// Gets the AzureGameServicesProperties for the current subscription
        /// </summary>
        /// <returns>The task for completion.</returns>
        Task<AzureGameServicesPropertiesResponse> GetAzureGameServicesProperties();

        /// <summary>
        /// Deploys the XblCompute instance.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <returns>The task for completion.</returns>
        Task<bool> DeployXblCompute(string xblComputeName);

        /// <summary>
        /// Stops the XblCompute instance.
        /// </summary>
        /// <param name="xblComputeName">The XblCompute Instance Name.</param>
        /// <returns>The task for completion.</returns>
        Task<bool> StopXblCompute(string xblComputeName);

    }
}