"use strict";

(function () {
    const dateElements = document.getElementsByClassName("js-convert-date");
    for (let element of dateElements) {
        const date = new Date(parseInt(element.dataset.timestamp));
        element.innerText = date.toLocaleDateString();
        element.classList.remove("js-convert-date");
    }

    const datetimeElements = document.getElementsByClassName("js-convert-datetime");
    for (let element of datetimeElements) {
        const date = new Date(parseInt(element.dataset.timestamp));
        element.innerText = date.toLocaleString();
        element.classList.remove("js-convert-datetime");
    }
})();
