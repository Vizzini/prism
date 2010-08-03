//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
namespace Microsoft.Practices.Composite.MefExtensions.Modularity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Windows;
    using Microsoft.Practices.Composite.Modularity;

    /// <summary>
    /// Provides a XAP type loader using the MEF DeploymentCatalog.
    /// </summary>
    [Export]
    public class MefXapModuleTypeLoader : IModuleTypeLoader
    {
        private Dictionary<Uri, ModuleInfo> downloadingModules = new Dictionary<Uri, ModuleInfo>();
        private HashSet<Uri> downloadedUris = new HashSet<Uri>();

        /// <summary>
        /// An aggregrate catalog provided by MEF via an import 
        /// </summary>
        /// <remarks>Due to Silverlight/MEF restrictions this must be public.</remarks>
        [Import(AllowRecomposition = false)]
        public AggregateCatalog AggregateCatalog { get; set; }

        /// <summary>
        /// An export provider provided by MEF via an import 
        /// </summary>
        /// <remarks>Due to Silverlight/MEF restrictions this must be public.</remarks>
        [Import(AllowRecomposition = false)]
        public MefDefaultPrismExportProvider DefaultExportProvider { get; set; }

        /// <summary>
        /// Raised repeatedly to provide progress as modules are loaded in the background.
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        private void RaiseModuleDownloadProgressChanged(ModuleInfo moduleInfo, long bytesReceived, long totalBytesToReceive)
        {
            this.RaiseModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, bytesReceived, totalBytesToReceive));
        }

        private void RaiseModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            if (this.ModuleDownloadProgressChanged != null)
            {
                this.ModuleDownloadProgressChanged(this, e);
            }
        }

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        private void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        private void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            if (this.LoadModuleCompleted != null)
            {
                this.LoadModuleCompleted(this, e);
            }
        }

        /// <summary>
        /// Evaluates the <see cref="ModuleInfo.Ref"/> property to see if the current typeloader will be able to retrieve the <paramref name="moduleInfo"/>.
        /// Returns true if the <see cref="ModuleInfo.Ref"/> property is a URI, because this indicates that the file is a downloadable file. 
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        /// <returns>
        /// 	<see langword="true"/> if the current typeloader is able to retrieve the module, otherwise <see langword="false"/>.
        /// </returns>
        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            { 
                throw new ArgumentNullException("moduleInfo"); 
            }

            if (!string.IsNullOrEmpty(moduleInfo.Ref))
            {
                Uri uriRef;
                return Uri.TryCreate(moduleInfo.Ref, UriKind.RelativeOrAbsolute, out uriRef);
            }

            return false;
        }

        /// <summary>
        /// Retrieves the <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are included in the resulting message and are handled by subscribers of the LoadModuleCompleted event.")]
        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            {
                throw new System.ArgumentNullException("moduleInfo");
            }

            try
            {
                Uri uri = new Uri(moduleInfo.Ref, UriKind.RelativeOrAbsolute);

                DownloadModuleFromUri(moduleInfo, uri);
            }
            catch (Exception ex)
            {
                this.RaiseLoadModuleCompleted(moduleInfo, ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The deplyment catalog is added to the container and disposed when the container is disposed.")]
        private void DownloadModuleFromUri(ModuleInfo moduleInfo, Uri uri)
        {
            DeploymentCatalog deploymentCatalog = new DeploymentCatalog(uri);
            try
            {
                // If this module has already been downloaded, fire the completed event.
                if (this.IsSuccessfullyDownloaded(deploymentCatalog.Uri))
                {
                    this.RaiseLoadModuleCompleted(moduleInfo, null);
                }
                // Start downloading if not already in progress.
                else if (!this.IsDownloading(deploymentCatalog.Uri))
                {
                    this.RecordDownloading(deploymentCatalog.Uri, moduleInfo);

                    deploymentCatalog.DownloadProgressChanged += this.DeploymentCatalog_DownloadProgressChanged;
                    deploymentCatalog.DownloadCompleted += this.DeploymentCatalog_DownloadCompleted;
                    deploymentCatalog.DownloadAsync();
                }
            }
            catch (Exception)
            {
                // if there is an exception between creating the deployment catalog and calling DownloadAsync,
                // the deployment catalog needs to be disposed.
                // otherwise, it is added to the compositioncontainer which should handle this.
                deploymentCatalog.Dispose();
                throw;
            }
        }

        private void DeploymentCatalog_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DeploymentCatalog deploymentCatalog = (DeploymentCatalog)sender;

            // I ensure the download progress changed is raised is on the UI thread.
            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(new Action<DeploymentCatalog, DownloadProgressChangedEventArgs>(this.HandleDownloadProgressChanged), deploymentCatalog, e);
            }
            else
            {
                this.HandleDownloadProgressChanged(deploymentCatalog, e);
            }
        }

        private void HandleDownloadProgressChanged(DeploymentCatalog deploymentCatalog, DownloadProgressChangedEventArgs e)
        {
            ModuleInfo moduleInfo = this.GetDownloadingModule(deploymentCatalog.Uri);
            this.RaiseModuleDownloadProgressChanged(moduleInfo, e.BytesReceived, e.TotalBytesToReceive);
        }

        private void DeploymentCatalog_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DeploymentCatalog deploymentCatalog = (DeploymentCatalog)sender;
            deploymentCatalog.DownloadProgressChanged -= DeploymentCatalog_DownloadProgressChanged;
            deploymentCatalog.DownloadCompleted -= DeploymentCatalog_DownloadCompleted;

            // I ensure the download completed is on the UI thread so that types can be loaded into the application domain.
            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(new Action<DeploymentCatalog, AsyncCompletedEventArgs>(this.HandleDownloadCompleted), deploymentCatalog, e);
            }
            else
            {
                this.HandleDownloadCompleted(deploymentCatalog, e);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are included in the resulting message and are handled by subscribers of the LoadModuleCompleted event.")]
        private void HandleDownloadCompleted(DeploymentCatalog deploymentCatalog, AsyncCompletedEventArgs e)
        {
            ModuleInfo moduleInfo = GetDownloadingModule(deploymentCatalog.Uri);
            Exception error = e.Error;
            if (error == null)
            {
                try
                {
                    this.RecordDownloadComplete(deploymentCatalog.Uri);
                    
                    // Because the XAPs loaded will contain assemblies that have references to this assembly, 
                    // and because this assembly exports singletons, I must filter to only modules to prevent DuplicateAssemblyExceptions.
                    FilteredCatalog filteredCatalog = new FilteredCatalog(deploymentCatalog,
                           x => x.ExportDefinitions.Where(def => this.IsTypeImportAllowed(def.ContractName)).SingleOrDefault() != null);

                    this.AggregateCatalog.Catalogs.Add(filteredCatalog);

                    this.RecordDownloadSuccess(deploymentCatalog.Uri);
                    
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }

            this.RaiseLoadModuleCompleted(moduleInfo, error);
        }

        /// <summary>
        /// Determines whether importnig the types is allowed for the specified contract name.
        /// </summary>
        /// <param name="contractName">The name of the imported contract.</param>
        /// <returns>
        /// 	<c>true</c> if importing is allowed; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsTypeImportAllowed(string contractName)
        {
            // Any contract name not found in the default export provider is allowed.
            return this.DefaultExportProvider.GetExports(typeof(object), typeof(object), contractName).FirstOrDefault() == null;
        }

        private bool IsDownloading(Uri uri)
        {
            lock (this.downloadingModules)
            {
                return this.downloadingModules.ContainsKey(uri);
            }
        }

        private void RecordDownloading(Uri uri, ModuleInfo moduleInfo)
        {
            lock (this.downloadingModules)
            {
                if (!this.downloadingModules.ContainsKey(uri))
                {
                    this.downloadingModules.Add(uri, moduleInfo);
                }
            }
        }

        private ModuleInfo GetDownloadingModule(Uri uri)
        {
            lock (this.downloadingModules)
            {
                return this.downloadingModules[uri];
            }
        }

        private void RecordDownloadComplete(Uri uri)
        {
            lock (this.downloadingModules)
            {
                if (!this.downloadingModules.ContainsKey(uri))
                {
                    this.downloadingModules.Remove(uri);
                }
            }
        }

        private bool IsSuccessfullyDownloaded(Uri uri)
        {
            lock (this.downloadedUris)
            {
                return this.downloadedUris.Contains(uri);
            }
        }

        private void RecordDownloadSuccess(Uri uri)
        {
            lock (this.downloadedUris)
            {
                this.downloadedUris.Add(uri);
            }
        }

        /// <summary>
        /// Provides a simple filtered catalog based on a Linq expression.
        /// </summary>       
        private class FilteredCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
        {
            private readonly INotifyComposablePartCatalogChanged innerNotifyChanged;
            private readonly IQueryable<ComposablePartDefinition> partsQuery;

            /// <summary>
            /// Initializes a new instance of the <see cref="FilteredCatalog"/> class.
            /// </summary>
            /// <param name="innerCatalog">The inner catalog.</param>
            /// <param name="expression">The expression.</param>
            public FilteredCatalog(ComposablePartCatalog innerCatalog,
                                   Expression<Func<ComposablePartDefinition, bool>> expression)
            {
                this.innerNotifyChanged = innerCatalog as INotifyComposablePartCatalogChanged;
                this.partsQuery = innerCatalog.Parts.Where(expression);
            }

            /// <summary>
            /// Gets the part definitions that are contained in the catalog.
            /// </summary>
            /// <value></value>
            /// <returns>The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/> contained in the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/>.</returns>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/> object has been disposed of.</exception>
            public override IQueryable<ComposablePartDefinition> Parts
            {
                get
                {
                    return this.partsQuery;
                }
            }

            /// <summary>
            /// Occurs when a <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/> has changed.
            /// </summary>
            public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
            {
                add
                {
                    if (this.innerNotifyChanged != null)
                        this.innerNotifyChanged.Changed += value;
                }
                remove
                {
                    if (this.innerNotifyChanged != null)
                        this.innerNotifyChanged.Changed -= value;
                }
            }

            /// <summary>
            /// Occurs when a <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/> is changing.
            /// </summary>
            public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
            {
                add
                {
                    if (this.innerNotifyChanged != null)
                        this.innerNotifyChanged.Changing += value;
                }
                remove
                {
                    if (this.innerNotifyChanged != null)
                        this.innerNotifyChanged.Changing -= value;
                }
            }
        }

    }
}
