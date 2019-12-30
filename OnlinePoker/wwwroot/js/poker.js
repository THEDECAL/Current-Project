const GUI = {
    bank: $("#bank-count"),
    gameId: $("#game-id").val(),
    tableCenter: $("#table-center")
}
const MAX_PLAYERS = 8;
//var cardSelectedzIndex = 0;
var isRunCardAnimation = false;

const hubConnection = new signalR.HubConnectionBuilder().withUrl("/PokerHub").build();
hubConnection.serverTimeoutInMilliseconds = 1000 * 180 * 10;
hubConnection.start();

//Клиентсике методы для запуска на стороне хаба
hubConnection.on("Wait", () => { wait(); });
hubConnection.on("CloseWait", () => { closeWait(); });
hubConnection.on("AddPlayer",  (nickName) => {
    if (Array.isArray(nickName)) {
        for (var i in nickName) {
            addPlayer(nickName[i], parseInt(Number(i) + Number(1)))
        }
    }
    else { addPlayer(nickName); }
});
hubConnection.on("CardDistribution", (cards) => {
    addDeck();

    if (Array.isArray(cards)) {
        for (var i in cards) {
            if (Array.isArray(cards[i])) {
                for (var j in cards[i]) {
                    //console.log("card: " + i); console.log(cards[i]);
                    addCard(cards[i][j] + ".png", parseInt(Number(i) + Number(1)));
                }
            }
        }
    }
})

setTimeout(function () { 
    //Подключение к игре
    hubConnection.invoke("ConnectToGame", GUI.gameId);
}, 1000);

//Открытие анимации ожидания
function wait() {
    let title = "Ожидание подключения соперников...";
    let img = "<img src='/images/loading.svg' alt='...' class='card-img'>";
    let waitWindow = document.createElement("div");
    waitWindow.setAttribute("id", "wait-window");
    waitWindow.setAttribute("class", "wait-window");
    $("main").prepend(waitWindow);

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

//Добавление имени игрока на стол
function addPlayer(nickName, playerNumber) {
    if (nickName != undefined && playerNumber >= 1 && playerNumber <= 8) {
        var el = $(`#player${playerNumber} .player-title`);
        el.empty(); el.append(nickName).hide();
        el.fadeIn(2500);
    }
}

//Анимация броска карты
function throwCard(playerNumber) {
    $("#table-center .game-card")[0].lastChild.animate([
        { transform: "rotate(0deg)", bottom: "0px" },
        { transform: "rotate(180deg)", bottom: "200px" }],
        { duration: 700 });
}

//Добавление карты на стол
function addCard(imgName, playerNumber) {
    if (imgName != undefined && playerNumber >= 1 && playerNumber <= 8) {
        var playerEl = $(`#player${playerNumber} .player-cards`);
        var childCount = playerEl[0].childElementCount;
        var cardNumber = parseInt(childCount + 1);
        var currOffset = (childCount === 0) ? 0 : playerEl[0].childNodes[parseInt(childCount - 1)].style.left;
        if (currOffset !== 0) { currOffset = parseInt(currOffset.substring(0, currOffset.length - 2)); }
        var offset = (cardNumber == 1) ? 0 : `${currOffset + (-35)}`;

        var style = `style="
            left: ${offset}px;
            z-index: ${cardNumber};"`;

        var el = $(`<div class="game-card" ${style}></div>`);
        var img = $(`<img src="/images/cards/${imgName}", alt="...">`);
        el.append(img);
        playerEl.append(el);
        img.mouseenter((e) => {
            if (!e.target.src.includes("0.png")) {
                var el = e.target;
                el.animate(
                    [{
                        bottom: "0px"
                    }, {
                        bottom: "35px"
                    }],
                    { duration: 500 });
                e.target.style.bottom = "35px";
            }
        });
        img.mouseleave((e) => {
            if (!e.target.src.includes("0.png")) {
                var el = e.target;
                el.animate(
                    [{
                        bottom: "35px"
                    }, {
                        bottom: "0px"
                    }],
                    { duration: 500 });
                e.target.style.bottom = "0px";
            }
        });
    }
}

//Выкладывание колоды на стол
function addDeck() {
    var div = $("<div class='game-card'></div>");
    var card1 = $("<img src='/images/cards/v1/0.png' alt='...' style='position: absolute'>");
    var card2 = $("<img src='/images/cards/v1/0.png' alt='...' style='z-index: 7'>");
    div.append(card1); div.append(card2);
    GUI.tableCenter.prepend(div);
}