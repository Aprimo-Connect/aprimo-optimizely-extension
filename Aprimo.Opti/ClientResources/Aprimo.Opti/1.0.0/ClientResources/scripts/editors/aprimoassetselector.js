define("aprimointegration/editors/aprimoassetselector", [
    // dojo
    "dojo/_base/declare",
    "dojo/when",
    "dojo/on",
    "dojo/topic",
    "dojo/dom-class",
    "dojo/_base/lang",
    // dijit
    "dijit/focus",
    "dijit/ProgressBar",
    // epi
    "epi/Url",
    "epi/routes",
    "epi/dependency",
    "epi/shell/dnd/Target",
    "epi/shell/DialogService",
    // epi-cms
    "epi-cms/core/ContentReference",
    "epi-cms/widget/MediaSelector",
    "epi-cms/widget/ThumbnailSelector",
    "epi-cms/contentediting/_ContextualContentContextMixin",
    "epi-cms/command/UploadContent",
    "epi-cms/widget/UploadUtil",
    "epi-cms/widget/viewmodel/MultipleFileUploadViewModel",
    "epi-cms/widget/LocalFolderUploader",
    "dojo/text!./templates/AprimoAssetSelector.html",
    "epi/i18n!epi/cms/nls/episerver.cms.widget.thumbnailselector",
    "epi-cms/widget/FilesUploadDropZone"
], function (
    // dojo
    declare,
    when,
    on,
    topic,
    domClass,
    lang,
    // dijit
    focusManager,
    ProgressBar,
    // epi
    Url,
    routes,
    dependency,
    Target,
    dialogService,
    ContentReference,
    MediaSelector,
    ThumbnailSelector,
    _ContextualContentContextMixin,
    UploadContent,
    UploadUtil,
    MultipleFileUploadViewModel,
    LocalFolderUploader,
    template,
    resources
) {
    var defaultImageUrl = require.toUrl("epi-cms/themes/sleek/images/default-image.png");

    return declare([MediaSelector, ThumbnailSelector], {
        resources: resources,
        templateString: template,
        tenantUrl: null,
        aprimoSelectContentUrl: null,
        aprimoiconpath: null,
        aprimoTitle: null,
        aprimoDescription: null,
        aprimoLabelButton: null,
        aprimoDialogMode: null,
        aprimomodulepath: null,
        dialogListenerBound: false,
        allowedExtensions: [],
        _mappingStore: null,
        _aprimoContentStore: null,
        _onAprimoButtonClick: function () {
            tenantUrl = this.aprimoSelectContentUrl;
            const selectorOptions = {
                title: this.aprimoTitle,
                description: this.aprimoDescription,
                accept: this.aprimoLabelButton,
                dialogMode: this.aprimoDialogMode,
                select: 'singlerendition'
            };
            const encodedOptions = window.btoa(JSON.stringify(selectorOptions));
            const aprimoContentSelectorUrl = tenantUrl + '/dam/selectcontent#options=' + encodedOptions;

            const openSelector = () => {
                window.open(aprimoContentSelectorUrl, 'selector', 'resizable,scrollbars,status')
            };

            openSelector();

            if (!this.dialogListenerBound) {
                this._listener = this._listener.bind(this);
                window.addEventListener("message", this._listener, true);
                this.dialogListenerBound = true;
            }
        },

        _listener: function (event) {
            if (event.origin !== tenantUrl) {
                window.removeEventListener("message", this._listener, true);
                this.dialogListenerBound = false;
                return;
            }
            if (event.data.result === 'accept') {
                const masterId = event.data.selection.map((x) => x.id);
                const renditionId = event.data.selection.map((x) => x.rendition.id);
                const url = event.data.selection.map((x) => x.rendition.publicuri);
                const title = event.data.selection.map((x) => x.title);

                var self = this;
                this._mappingStore.query({ id: masterId, renditionId, publicUrl: url, title: title, ownerContentItemId: this._currentContext.id })
                    .then(function (data) {
                        if (data !== null) {
                            self._setValueAndFireOnChange(data.contentLink);
                        }
                    });

                window.removeEventListener("message", this._listener, true);
                this.dialogListenerBound = false;
            }
        },

        _setPreviewUrlAttr: function (value) {
            this.inherited(arguments);
            domClass.toggle(this.displayNode, "dijitHidden", !value);
            domClass.toggle(this.actionsContainer, "dijitHidden", value);
        },

        _updateDisplayNode: function (content) {
            this.inherited(arguments);
            if (content) {
                this.thumbnail.src = this._getThumbnailUrl(content);
            }

            this.stateNode.title = content ? content.name : "";
        },

        postCreate: function () {
            this.inherited(arguments);
            var registry = dependency.resolve("epi.storeregistry");
            this.query = { query: "getchildren", allLanguages: true };
            this._mappingStore = registry.get("aprimoassetmappingstore");
            this._aprimoContentStore = registry.get("aprimoassetstore");
        },

        _onButtonClick: function (event) {
            if (this.readOnly) {
                event.preventDefault();
                event.stopPropagation();
                return;
            }

            this.inherited(arguments);
        },

        _setReadOnlyAttr: function (value) {
            this.inherited(arguments);
            this.button.domNode.style.display = "";
            this.button.set("readOnly", value);
        }
    });
});