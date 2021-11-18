"use strict";

/* upload functions */

(function () {
    const fileInput = document.getElementById("files");
    const dropContainer = document.getElementById("file-drop-container");
    const submitButton = document.getElementById("files-submit");

    function update() {
        if (fileInput.files.length == 0) {
            dropContainer.innerText = "Dateien zum Hochladen hierhin ziehen oder klicken"
            submitButton.parentElement.classList.add("d-none");
        } else {
            dropContainer.innerText = fileInput.files.length + " Dateien ausgewählt"
            submitButton.parentElement.classList.remove("d-none");
        }
    }

    fileInput.onchange = update;

    dropContainer.onclick = function (ev) {
        fileInput.click();
    }
    dropContainer.ondragenter = function (ev) {
        this.classList.add("border-primary");
        return false;
    };
    dropContainer.ondragover = function (ev) {
        return false;
    }
    dropContainer.ondragleave = function (ev) {
        this.classList.remove("border-primary");
        return false;
    };
    dropContainer.ondrop = function (ev) {
        this.classList.remove("border-primary");
        fileInput.files = ev.dataTransfer.files;
        update();
        ev.preventDefault();
    };
})();

/* gallery functions */

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
    const html = "[![](" + src + "?size=1600)](" + src + ")";
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
