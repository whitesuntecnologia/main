function version() {
    console.log('20221105 15:50');
}

function changeColor(value) {
    
    jQuery.App.layout.leftSidebar.changeColor(value)
    jQuery.App.layout.changeMode(value);
    return false;
};

function changeWidth(value) {
    jQuery.App.layout.changeLayoutWidth(value);
    return false;
};

function changeLeftSidebarColor(value) {

    jQuery.App.layout.leftSidebar.changeColor(value)
    return false;
};

function initApp(jsonData) {

    $("body").attr("data-layout", '{"mode": "light", "width": "fluid", "menuPosition": "fixed", "sidebar": { "color": "light", "size": "default", "showuser": false}, "topbar": {"color": "dark"}, "showRightSidebarOnPageLoad": true}');
    jQuery.App.init();
    return false;
}

function saveAsFile(filename, bytesBase64) {
    const link = document.createElement("a");
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

/*------------------------------------------------------------------------------
 * DOWNLOAD FILE
 * ----------------------------------------------------------------------------*/
// Use it for .NET 6+
function BlazorDownloadFile(filename, contentType, content) {
    const file = new File([content], filename, { type: contentType });
    const exportUrl = URL.createObjectURL(file);

    const a = document.createElement("a");
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a); // Limpieza

    // Liberar memoria después de un pequeño retardo (mejor para Safari)
    setTimeout(() => URL.revokeObjectURL(exportUrl), 100);
}

function getBlobObject(b64Data) {
    var blob = base64ToBlob(b64Data, 'application/pdf');
    var url = URL.createObjectURL(blob);
    return url;
}

function base64ToBlob(base64, type = "application/octet-stream") {
    const binStr = atob(base64);
    const len = binStr.length;
    const arr = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        arr[i] = binStr.charCodeAt(i);
    }
    return new Blob([arr], { type: type });
}

class ClipboardEventsHelpers {
    static dotNetHelper;

    static setDotNetHelper(value) {
        ClipboardEventsHelpers.dotNetHelper = value;
    }

    static async handlePasteEvent(event, handlerMethodName) {
        const pasted = event.clipboardData.getData('Text');
        event.target.value = await ClipboardEventsHelpers.dotNetHelper.invokeMethodAsync(handlerMethodName, pasted);
    }
}

(function () {
    const target = document.body;

    const observer = new MutationObserver(function () {
        if (document.getElementById('wrapper')) {
            document.body.style.backgroundImage = "none";
            observer.disconnect();
        }
    });

    const config = { childList: true };
    observer.observe(target, config);
})()

window.ClipboardEventsHelpers = ClipboardEventsHelpers;

function getIsSecureContext() {
    return window.isSecureContext;
}
