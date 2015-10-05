window.setInterval(function () {
    game.getData();
}, 17);

function changeSelection(index) {
    jQuery(".box.selected").attr("class", "box");
    jQuery("#inv-" + index).addClass("selected");
}

function setValue(name, text) {
    document.getElementById(name).innerText = text;
}