"use strict";
var tinymce = tinymce || {};

tinymce.PluginManager.add('aprimo', function (editor, url) {
    const aprimoIcon = url + '/aprimo-icon.png';

    const selectorOptions = {
        title: editor.settings.aprimoTitle,
        description: editor.settings.aprimoDescription,
        accept: editor.settings.aprimoLabelButton,
        dialogMode: editor.settings.aprimoDialogMode,
        select: editor.settings.aprimoSelectMode
    };
    const tenantUrl = editor.settings.aprimoSelectContentUrl;

    var dialogListenerBound = false;

    /* Add button to tinymce editor for episerver to retrive assets from aprimo*/
    editor.addButton('aprimo-insert-media', {
        title: 'Insert',
        image: aprimoIcon,
        tooltip: 'Insert Aprimo media',
        onclick: function () {
            const encodedOptions = window.btoa(JSON.stringify(selectorOptions));
            const aprimoContentSelectorUrl = tenantUrl + '/dam/selectcontent#options=' + encodedOptions;

            const openSelector = () => {
                window.open(aprimoContentSelectorUrl, 'selector', 'resizable,scrollbars,status')
            };

            openSelector();
            if (!dialogListenerBound) {
                window.addEventListener("message", insertMedia, false);
            }

            //verifyConfig()
            //    .then(function () {
            //        // Focus tiny (to trigger save and publish events)
            //        editor.focus();

            //        // Open Frontify
            //        openFrontifyWindow('insert');
            //    })
            //    .then(waitForWindowToClose)
            //    .then(function (media) { addMediaToEditor(media); })
            //    .catch(function (error) { handleError(error); });
        }
    });
    function onContentChanged() {
        editor.undoManager.add();
    }

    function insertContent(content) {
        editor.execCommand('mceInsertContent', false, content);
        onContentChanged();
    }
    function insertMedia(event) {
        // Ensure only messages from the Aprimo Content Selector are handled.
        if (event.origin !== tenantUrl) {
            window.removeEventListener("message", insertMedia, true);
            dialogListenerBound = false;
            return
        }
        if (event.data.result === 'cancel') {
            alert('You have canceled the selector')
        }
        if (event.data.result === 'accept') {
            const url = event.data.selection.map((selection) => selection.rendition.publicuri)
            insertContent('<img src="' + url + '"/>');
        }

        window.removeEventListener("message", insertMedia, true);
        dialogListenerBound = false;
    }

    return {
        getMetadata: function () {
            return {
                name: "Aprimo",
                url: "http://www.aprimo.com"
            };
        }
    };
});