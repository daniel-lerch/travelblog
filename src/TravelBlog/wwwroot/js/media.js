"use strict";

$("figure").click(function (e) {
    const buttons = e.currentTarget.getElementsByTagName("button");
    for (let button of buttons) {
        if (button.classList.contains("d-none")) {
            button.classList.replace("d-none", "d-inline");
        } else if (button.classList.contains("d-inline")) {
            button.classList.replace("d-inline", "d-none");
        }
    }
});

$(".clipboard").click(function (e) {
    const src = e.currentTarget.parentElement.dataset.src;
    if (!navigator.clipboard) {
        alert("Dein Browser unterstützt die aktuelle Clipboard-API nicht 😟");
    }
    const html = "<a href=\"" + src + "\"><img src=\"" + src + "?size=1600\" alt=\"\" class=\"w-100\"></a>";
    navigator.clipboard.writeText(html);
});

$(".delete").click(function (e) {
    const src = e.currentTarget.parentElement.dataset.src;
    if (confirm("Möchtest du die Datei " + src + " wirklich löschen?")) {
        $.ajax({
            method: "DELETE",
            url: src
        }).done(function (data, textStatus, jqXHR) {
            e.currentTarget.parentElement.remove();
        }).fail(function (jqXHR, textStatus, errorThrown) {
            alert("Die Datei " + src + " konnte nicht gelöscht werden: " + textStatus);
        });
    } else {
        e.originalEvent.stopPropagation();
    }
});
