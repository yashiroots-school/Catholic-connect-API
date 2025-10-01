
document.onkeypress = function (event) {
    event = (event || window.event);
    if (event.keyCode == 123) {
        return false;
    }
}
document.onmousedown = function (event) {
    event = (event || window.event);
    if (event.keyCode == 123) {
        return false;
    }
}
document.onkeydown = function (event) {
    event = (event || window.event);
    if (event.keyCode == 123) {
        return false;
    }
}
document.onkeydown = function (e) {
    if (e.ctrlKey && (e.keyCode === 67 || e.keyCode === 86 || e.keyCode === 85 || e.keyCode === 117)) {
        return false;
    } else {
        return true;
    }
};
//Copy Paste Desable
//window.onload = function () {
//document.addEventListener("contextmenu", function (e) {
//e.preventDefault();
//}, false);

document.addEventListener("keydown", function (e) {
    //document.onkeydown = function(e) {
    // "I" key
    if (e.ctrlKey && e.shiftKey && e.keyCode == 73) {
        disabledEvent(e);
    }
    // "J" key
    if (e.ctrlKey && e.shiftKey && e.keyCode == 74) {
        disabledEvent(e);
    }
    // "S" key + macOS
    if (e.keyCode == 83 && (navigator.platform.match("Mac") ? e.metaKey : e.ctrlKey)) {
        disabledEvent(e);
    }
    // "U" key
    if (e.ctrlKey && e.keyCode == 85) {
        disabledEvent(e);
    }
    // "F12" key
    if (event.keyCode == 123) {
        disabledEvent(e);
    }
}, false);
function disabledEvent(e) {
    if (e.stopPropagation) {
        e.stopPropagation();
    } else if (window.event) {
        window.event.cancelBubble = true;
    }
    e.preventDefault();
    return false;
}