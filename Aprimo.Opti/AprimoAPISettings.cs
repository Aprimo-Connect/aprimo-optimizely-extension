using System.Configuration;

namespace Aprimo.Opti
{
    public class AprimoAPISettings
    {
        static AprimoAPISettings()
        {
        }

        public static string ClientId { get; } = ConfigurationManager.AppSettings["aprimo-api-clientid"];

        public static string ClientSecret { get; } = ConfigurationManager.AppSettings["aprimo-api-clientsecret"];

        public static string AprimoTenantId { get; } = ConfigurationManager.AppSettings["aprimo-api-tenantid"];

        public static string DialogMode { get; } = ConfigurationManager.AppSettings["aprimo-api-dialogmode"];

        public static string LabelButton { get; } = ConfigurationManager.AppSettings["aprimo-api-dialogbuttontext"];

        public static string Description { get; } = ConfigurationManager.AppSettings["aprimo-api-dialogdescription"];

        public static string Title { get; } = ConfigurationManager.AppSettings["aprimo-api-dialogtitle"];
    }
}