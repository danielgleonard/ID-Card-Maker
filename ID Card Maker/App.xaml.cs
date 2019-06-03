using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Identity.Client;

namespace ID_Card_Maker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            _clientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, Tenant)
                .Build();
            Micorosft.Graph.Templates.TokenCacheHelper.EnableSerialization(_clientApp.UserTokenCache);
        }

        // Tokens for connecting to Microsoft Graph
        private static string ClientId = "66c86f35-8268-4a38-9092-a67ed39247bd";
        private static string Tenant = "organizations";

        private static IPublicClientApplication _clientApp;
        public static IPublicClientApplication PublicClientApp { get { return _clientApp; } }
    }
}
