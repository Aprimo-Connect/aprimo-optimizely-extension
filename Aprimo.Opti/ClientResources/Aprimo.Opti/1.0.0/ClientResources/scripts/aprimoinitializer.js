define("aprimointegration/aprimoinitializer", [
    "dojo/_base/declare",
    "epi/_Module",
    "epi/routes",
    "epi/shell/conversion/ObjectConverterRegistry",
    "aprimointegration/AprimoAssetDataConverter",
], function (
    declare,
    _Module,
    routes,
    ObjectConverterRegistry,
    AprimoAssetDataConverter
) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            var registry = this.resolveDependency("epi.storeregistry");

            var storeOptions = { idProperty: "contentLink" };

            registry.create("aprimoassetstore",
                routes.getRestPath({
                    moduleArea: "aprimo.opti",
                    storeName: "aprimoassetstore",
                }),
                storeOptions);

            registry.create("aprimoassetmappingstore",
                routes.getRestPath({
                    moduleArea: "aprimo.opti",
                    storeName: "aprimoassetmappingstore",
                }),
                storeOptions);

            // Register data converter for TinyMCE drop
            var converter = new AprimoAssetDataConverter();
            ObjectConverterRegistry.registerConverter(
                converter.sourceDataType,
                converter.targetDataType,
                converter
            );
        },
    });
});