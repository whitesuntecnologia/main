function DownloadFile(filename, contentType, data) {

    // Create the URL
    let exportUrl;

    if (contentType == "application/pdf") {
        var data64 = base64ToArrayBuffer(data);
        var blob = new Blob([data64], { type: contentType });
        exportUrl = URL.createObjectURL(blob);
    } else {
        const file = new File([data], filename, { type: contentType });
        exportUrl = URL.createObjectURL(file);
    }
    // Create the <a> element and click on it
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    a.click();

    URL.revokeObjectURL(exportUrl);
}

function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}