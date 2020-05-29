"use strict";

$(".clipboard").click(function (e) {
    const token = e.currentTarget.dataset.token;
    if (!navigator.clipboard) {
        alert("Dein Browser unterstützt die aktuelle Clipboard-API nicht 😟");
    }
    navigator.clipboard.writeText(token);
});
