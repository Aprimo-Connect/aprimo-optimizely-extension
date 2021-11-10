define(["dojo", "epi/dependency", "epi/shell/TypeDescriptorManager"], function (
    dojo,
    dependency,
    TypeDescriptorManager
) {
    return dojo.declare("aprimointegration.AprimoAssetDataConverter", null, {
        _store: null,

        sourceDataType: "aprimointegration.contenttypes.aprimoassetdata",
        targetDataType: "aprimointegration.contenttypes.aprimoassetdata.link",

        constructor: function (params) {
            dojo.mixin(this, params);
            this._store = this._store || dependency.resolve("epi.storeregistry").get("aprimoassetstore");
        },

        convert: function (sourceDataType, targetDataType, data) {
            // pulled from patrick vak kleef post (https://www.patrickvankleef.com/2017/02/26/episerver-customize-the-drop-behavior-in-tinymce/)
            var types = TypeDescriptorManager.getInheritanceChain(sourceDataType);
            var isAprimoAsset = types.indexOf(this.sourceDataType) !== -1;
            if (!isAprimoAsset) {
                // Can't render, this will show an error to the user
                return {};
            }

            return dojo.when(this._store.get(data.contentLink), function (content) {
                return {
                    url: content.properties.imageUrl,
                    previewUrl: "", // TODO: set and use previewUrl -> data.previewUrl
                    permanentUrl: content.permanentLink,
                    text: content.name,
                    typeIdentifier: content.typeIdentifier,
                };
            });
        },
    });
});