# Aprimo Content Provider for Optimizely
![Aprimo Connector](https://github.com/JoshuaFolkerts/Aprimo.Opti/blob/8422d7970033375103604f304075f648d0997118/images/Screenshot.png)
## Requirements
```
<dependency id="Microsoft.AspNet.Mvc" version="5.2.3" />
<dependency id="EPiServer.CMS.UI" version="11.36.0" />
<dependency id="EPiServer.CMS.AspNet" version="11.20.8" />
<dependency id="EPiServer.Labs.ConfigurationManager" version="0.1.1" />
<dependency id="EPiServer.CMS.TinyMce" version="2.13.5" />
<dependency id="RestSharp" version="106.12.0" />
<dependency id="RestSharp.Serializers.NewtonsoftJson" version="106.11.7" />
```
Please note this connector was built and tested using Optimizely v11. We expect that development work will be needed to support Optimizely v12, or other versions of Optimizely.
## Aprimo's Open Source Policy
This code is provided by Aprimo as-is as an example of how you might solve a specific business problem. It is not intended for direct use in Production without modification.

You are welcome to submit issues or feedback to help us improve visibility into potential bugs or enhancements. Aprimo may, at its discretion, address minor bugs, but does not guarantee fixes or ongoing support.

It is expected that developers who clone or use this code take full responsibility for supporting, maintaining, and securing any deployments derived from it.

If you are interested in a production-ready and supported version of this solution, please contact your Aprimo account representative. They can connect you with our technical services team or a partner who may be able to build and support a packaged implementation for you.

Please note: This code may include references to non-Aprimo services or APIs. You are responsible for acquiring any required credentials or API keys to use those services—Aprimo does not provide them.

## Getting Started
Before we can see the data in our Optimizely interface, we need to update the AppSettings values for handling the connection between the aprimo/optimizely connector and Aprimo REST API / Content Seelctor.

Take the keys you saved from Aprimo and update the values in the AppSettings section of the web.config
```xml
<appSettings>
  <add key="aprimo-api-tenantid" value="your_aprimo_username" />
	<add key="aprimo-api-clientid" value="your_aprimo_client_id" />
	<add key="aprimo-api-clientsecret" value="your_aprimo_clientsecret" />
	<add key="aprimo-api-dialogmode" value="default" />
	<add key="aprimo-api-dialogbuttontext" value="Select" />
	<add key="aprimo-api-dialogdescription" value="Select Asset" />
	<add key="aprimo-api-dialogtitle" value="Select" />
</appSettings>
```
and then click save.

## Including code in project (Manual Setup)
If you choose to include the project in your own project.  You will need to setup this in a slightly different way.  You will need to reference the project and complete the same setup configuration as above by updating the configuration settings.  

Next, you will need to copy the folder in the client resources folder "Aprimo.Opti".  You will need to copy the Aprimo.Opti folder into the main web project's modules/protected folder.  This will provide the custom property to render in edit mode.  So your web project modules folders should look like this
```
"/modules/protected/Aprimo.Optio"
```
![Configuration Manager](https://github.com/JoshuaFolkerts/Aprimo.Opti/blob/master/readme-files/Aprimo.opti.png)

Web.config update.  You will need to add the follow line in the web.config 
```xml
      <add name="Aprimo.Opti" />
```
this will need to be added to the episerver.shell/protectedModules section
```xml
  <episerver.shell>
    <publicModules rootPath="~/modules/" autoDiscovery="Modules" />
    <protectedModules rootPath="~/EPiServer/">
      <add name="Aprimo.Opti" />
      <add name="episerver-labs-configuration-manager" />
      <add name="Shell" />
      <add name="CMS" />
      <add name="EPiServer.Cms.TinyMce" />
    </protectedModules>
  </episerver.shell>
```

## Customizing the Configuration Behavior

To customize how assets are rendered, we are going to create a model called AprimoImageAsset.  This allows the configuration system to strongly type the asset coming from the connector.
```csharp
    [ContentType(DisplayName = "Aprimo Image File", GUID = "CAE94870-C2D3-4C08-A8A8-CE6FC7820510", Description = "Respresents aprimo image asset", Order = 1)]
    [AprimoAssetDescriptor(ExtensionString = "jpg,jpeg,png,tif,tiff,gif")]
    public class AprimoImageAsset : AprimoImageData
    {
        [AprimoTransform(Auto = "webp", Width = "400", Crop = "16:9")]
        public virtual string Teaser { get; set; }
    }
```
note the Attribute descriptor to tell the connector what filetypes to support.  we added jpg, jpeg, etc to AprimoImageAsset
```csharp
[AprimoAssetDescriptor(ExtensionString = "jpg,jpeg,png,tif,tiff,gif")]
```
Inherit all image types from AprimoImageData.  

Next we have a teaser property with the AprimoTransform Attribute on the property.
```csharp
[AprimoTransform(Auto = "webp", Width = "400", Crop = "16:9")]
public virtual string Teaser { get; set; }
```
AprimoTransform attribute allows us to give the connector the ability to transform the image as we see fit to custom the returning url from the CDN.  In this case, we are returning an image with the width of 400 with a 16:9 aspect ratio and deliver it to us as webp.

the following attribute properties are available for to you send to the cdn in the attribute.
```csharp
public string Width { get; set; } // Resize the width of the image.

public string Height { get; set; } // Resize the height of the image.

public string Format { get; set; } // Specify the output format to convert the image to.

public string Crop { get; set; } //	Remove pixels from an image.

public string DPR { get; set; } // Serve correctly sized images for devices that expose a device pixel ratio.

public string Fit { get; set; } // 	Set how the image will fit within the size bounds provided.

public string Pad { get; set; } // Add pixels to the edge of an image.

public string Quality { get; set; } // 	Optimize the image to the given compression level for lossy file formatted images.

public string Saturation { get; set; } // Set the saturation of the output image.

public string Sharpen { get; set; } // 	Set the sharpness of the output image.

public string Trim { get; set; } // 	Remove pixels from the edge of an image.

public string Contrast { get; set; } // Set the contrast of the output image.

public string Brightness { get; set; } // Set the brightness of the output image.

public string Blur { get; set; } //	Set the blurriness of the output image.

public string BackgroundColor { get; set; } // Set the background color of an image.

public string Auto { get; set; } // Enable optimization features automatically.
```

You can see all the values here 
https://developer.fastly.com/reference/io/

Add the controller if you need to which in this case, the sample is from Alloy
```csharp
[TemplateDescriptor(Inherited = false, ModelType = typeof(AprimoImageAsset), TemplateTypeCategory = EPiServer.Framework.Web.TemplateTypeCategories.MvcPartialController)]
public class AprimoImageAssetController : PartialContentController<AprimoImageAsset>
{
    /// <summary>
    /// The index action for the image file. Creates the view model and renders the view.
    /// </summary>
    /// <param name="currentContent">The current image file.</param>
    public override ActionResult Index(AprimoImageAsset currentContent)
    {
        var model = new ImageViewModel
        {
            Url = currentContent.Url,
            Name = currentContent.Name
        };

        return PartialView(model);
    }
}
```
You can see here we are just passing the AprimoImageAsset to the viewmodel and sending to view.

As a normal model that needs to be rendered in a view, you can pass the model to the view and then render the image.  For those sites that already have a uihint for ContentReference of Image, you can just get the url as such:
```html
if (Model.ProviderName.Equals(AprimoConstants.ProviderKey))
{
    var originalUrl = Model.GetAprimoUrl();
    if (!string.IsNullOrWhiteSpace(originalUrl))
    {
        <img src="@originalUrl" class="img-fluid" />
    }
}
```
You also have some extension methods available to you for rendering images and different size images based on your property thumbnails
```csharp
public static string GetAprimoUrl(this ContentReference contentReference)  // will return normal image if you are using both optimizely and aprimo image types
public static string GetAprimoUrl(this Url url)
public static string GetAprimoUrl(this ContentReference contentReference, string propertyName) // for thumbnails or different values
```
# Open Source Policy

For more information about Aprimo's Open Source Policies, please refer to
https://community.aprimo.com/knowledgecenter/aprimo-connect/aprimo-connect-open-source
