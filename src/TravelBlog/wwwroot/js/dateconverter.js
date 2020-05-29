"use strict";

(function () {
    const dateElements = document.getElementsByClassName("js-convert-date");
    for (let element of dateElements) {
        const date = new Date(parseInt(element.dataset.timestamp));
        element.innerText = date.toLocaleDateString();
    }

    const datetimeElements = document.getElementsByClassName("js-convert-datetime");
    for (let element of datetimeElements) {
        const date = new Date(parseInt(element.dataset.timestamp));
        element.innerText = date.toLocaleString();
    }
})();
