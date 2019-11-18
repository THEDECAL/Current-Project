const poker = $.connection.gameHub;
$.connection.hub.logging = true;
$.connection.hub.start();

const GUI = {
    bank: $("#bank-count"),
    gameId: $("#game-id").val()
}

poker.client.AddPlayer = function (playerNumber, playerName) {
    AddPlayer(playerNumber, playerName);
};
poker.client.AddCard = function (playerNumber, cardImg) {
    AddCard(playerNumber, cardImg);
};

$.connection.hub.start().done(function () {
    poker.server.connect(GUI.gameId);

    OpenWaiting();
});

function OpenWaiting() {
    let title = "Ожидание подключения соперников...";
    let text = "<img src='/Content/images/loading.svg' alt='...'>";
    let waitWindow = document.createElement("div");
    waitWindow.setAttribute("id", "wait-window");
    waitWindow.setAttribute("class", "wait-window");
    $("body").prepend(waitWindow);
    $("#wait-window").insertAfter(".navbar.navbar-inverse.navbar-fixed-top");

    $("#wait-window").append(`<div class='panel panel-primary' style='opacity: 1;'> 
        <div class='panel-heading'>
            <h4 class="panel-title">${title}</h4>
        </div>
        <div class='panel-body'><center>${text}</center></div>
    </div`);
}
function CloseWaiting() { $("#wait-window").remove(); }
function AddPlayer(playerNumber, playerName) {
    $(`#player${playerNumber}`).append(`<div class="player">
            <div class="player-title">${playerName}</div>
            <div class="player-cards"></div>
        <div>`);
}
function AddCard(playerNumber, imgName) {
    var playerEl = $(`#player${playerNumber} .player .player-cards`);
    var cardNumber = playerEl[0].childElementCount + 1;
    var offset = (cardNumber == 1) ? '0' : ``;

    var style = `style="
        left: ${offset}px;
        z-index: ${cardNumber};
        background-image: url('/Content/images/cards/${imgName}.png');"` 

    playerEl.append(`<div class="card" ${style}></div>`);
    //$(`#player${playerNumber}`).append(`
    //            <div class="player-cards">
    //                <div class="card" style="z-index: 1;"></div>
    //                <div class="card" style="left: -35px; z-index: 2;"></div>
    //                <div class="card" style="left: -70px; z-index: 3;"></div>
    //                <div class="card" style="left: -105px; z-index: 4;"></div>
    //                <div class="card" style="left: -140px; z-index: 5;"></div>
    //            </div>`);
}