const GUI = {
    bank: $("#bank-count"),
    gameId: $("#game-id").val()
}
var playerPlacesCount = 1;

const hubConnection = new signalR.HubConnectionBuilder().withUrl("/PokerHub").build();
hubConnection.serverTimeoutInMilliseconds = 1000 * 180 * 10;
hubConnection.start();

//Клиентсике методы для запуска на стороне хаба
hubConnection.on("Wait", () => { wait(); });
hubConnection.on("CloseWait", () => { closeWait(); });
hubConnection.on("AddPlayer", (nickNames) => {
    if (!nickNames) {
        if (Array.isArray(nickNames)) {
            for (var item = nickNames) {
                item = nickNames[item];
                addPlayer(item);
            }
        }
        else { addPlayer(nickName); }
    }
});

setTimeout(function () { 
    //Подключение к игре
    hubConnection.invoke("ConnectToGame", GUI.gameId);
}, 1000);

//poker.client.addPlayer = function (playerNumber, playerName) {
//    AddPlayer(playerNumber, playerName);
//};
//poker.client.addCard = function (playerNumber, cardImg) {
//    AddCard(playerNumber, cardImg);
//};
//$.connection.hub.start().done(function () {
//    poker.server.connect(GUI.gameId);
//    openWaiting();
//});

//poker.client.addPlayers = function (players) {
//    console.log(players);

//    if (Array.isArray(players)) {
//        for (var item in players) {
//            item = players[item];
//            addPlayer(item);
//        }
//    }
//};
//poker.client.addCards = function (cards) {
//    console.log(cards);

//    if (Array.isArray(cards)) {
//        for (var item in cards) {
//            item = cards[item];
//            var imgName = `${item.rank}_${item.suit}.png`

//            addCard(imgName);
//        }
//    }
//    else {
//        for (var i = 0; i <= 5; i++) {
//            addCard("0.png");
//        }
//    }
//};

//Открытие анимации ожидания
function wait() {
    let title = "Ожидание подключения соперников...";
    let img = "<img src='/images/loading.svg' alt='...' class='card-img'>";
    let waitWindow = document.createElement("div");
    waitWindow.setAttribute("id", "wait-window");
    waitWindow.setAttribute("class", "wait-window");
    $("body").prepend(waitWindow);
    $("#wait-window").insertAfter(".navbar.navbar-inverse.navbar-fixed-top");

    $("#wait-window").append(`
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title">${img}</h5>
      </div>
      <div class="modal-body">
        <p>${title}</p>
      </div>
    </div>`);
}

//Закрытие анимации ожидания
function closeWait() { $("#wait-window").remove(); }

//Метод добавления имени игрока на стол
function addPlayer(nickName) {
    if (playerPlacesCount > 0 && playerPlacesCount < 8) {
        $(`#player${playerPlacesCount++}`).append(`<div class="player">
                <div class="player-title">${nickName}</div>
                <div class="player-cards"></div>
            <div>`);
    }
}

//Метод добавления карты на стол
function addCard(imgName) {
    var playerEl = $(`#player1 .player .player-cards`);
    var cardNumber = playerEl[0].childElementCount + 1;
    var offset = (cardNumber == 1) ? '0' : ``;

    var style = `style="
        left: ${offset}px;
        z-index: ${cardNumber};
        background-image: url('/Content/images/cards/${imgName}');"` 

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