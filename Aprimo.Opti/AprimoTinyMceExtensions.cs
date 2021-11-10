using EPiServer.Cms.TinyMce.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using System.Reflection;

namespace Aprimo.Opti
{
    public static class AprimoTinyMceExtensions
    {
        public static TinyMceSettings AddAprimoSupport(this TinyMceSettings settings)
        {
            var version = Assembly.Load("Aprimo.Opti").GetName().Version.ToString(3);
            string text = Paths.ToResource("Aprimo.Opti", string.Empty);
            var aprimoTinyMcePluginPath = text + version + "/ClientResources/plugin/";
            var aprimoTinyMcePluginFilePath = aprimoTinyMcePluginPath + "aprimo-tinymce.js";

            settings
                .AddExternalPlugin("aprimo", aprimoTinyMcePluginFilePath)
                     .AddSetting("aprimoSelectContentUrl", $"https://{AprimoAPISettings.AprimoTenantId}.dam.aprimo.com")
                     .AddSetting("aprimoTitle", AprimoAPISettings.Title)
                     .AddSetting("aprimoDescription", AprimoAPISettings.Description)
                     .AddSetting("aprimoLabelButton", AprimoAPISettings.LabelButton)
                     .AddSetting("aprimoDialogMode", AprimoAPISettings.DialogMode)
                     .AddSetting("aprimoSelectMode", "singlerendition")
                     .AddSetting("aprimomodulepath", Paths.ToResource("Aprimo.Opti", string.Empty));

            return settings;
        }
    }
}