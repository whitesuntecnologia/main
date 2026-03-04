function RedirectToJobs(url, token) {
    var form = document.createElement("form");
    form.setAttribute("method", "POST");
    form.setAttribute("action", url);

    form.setAttribute("target", "_blank");

    var hiddenField = document.createElement("input");
    hiddenField.setAttribute("type", "hidden");
    hiddenField.setAttribute("name", "access_token");
    hiddenField.setAttribute("value", token);
    form.appendChild(hiddenField);
    document.body.appendChild(form);

    form.submit();
}