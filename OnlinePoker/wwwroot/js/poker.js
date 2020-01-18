const MAX_PLAYERS = 8;
const CNT_RETRY_CONN = 3;
const THROW_TIME_ANIM = 700;
var crds = {};
COMB_NAMES = [
    "Высшая карта",
    "Пара",
    "Две пары",
    "Сет",
    "Стрит",
    "Флэш",
    "Фул-Хаус",
    "Карэ",
    "Стрит-Флэш",
    "Роял-Флэш"];
const GUI = {
    bank: $("#bank-count"),
    gameId: $("#game-id").val(),
    tableCenter: $("#table-center"),
    combination: $("#combination"),
    plActions: $("#player-actions")
}

//Настройка и пожключение к хабу
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/PokerHub")
    .configureLogging(signalR.LogLevel.Warning)
    .build();
hubConnection.serverTimeoutInMilliseconds = 1000 * 180 * 50;
//Объявление клиентских методов
hubConnection.on("WaitWindow", waitWindow);
hubConnection.on("CloseWaitWindow", closeWaitWindow);
hubConnection.on("AddPlayer", (nickNamesArr) => {
    for (var i in nickNamesArr) {
        addPlayer(nickNamesArr[i], parseInt(Number(i) + Number(1)))
    }
});
hubConnection.on("CardDist", (plCardsArr) => cardDist(plCardsArr));
hubConnection.on("QuickCardDist", (plCardsArr) => quickCardDist(plCardsArr));
hubConnection.on("AddCombName", (combNum) => { console.log("AddCombName"); addCombName(combNum); });
//Подключение к хабу
hubConnection.start(setTimeout(connectToGame));//.then(connectToGame());

//Подключение к хабу
function connectToGame(cntRetry = 0, isSending = false) {
    if (++cntRetry <= CNT_RETRY_CONN) {
        setTimeout(() => {
            if (hubConnection.state === "Connected" && !isSending) {
                hubConnection.send("ConnectToGame", GUI.gameId).then(connectToGame(cntRetry, true));
            }
            else if (hubConnection.state === "Disconnected") {
                hubConnection.start().then(connectToGame(cntRetry, false));
            }
        }, 500 * cntRetry);
    }
}
//Открытие анимации ожидания
function waitWindow() {
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
function closeWaitWindow() { $("#wait-window").remove(); }
//Добавление поля названия комбинации
function addCombName(combNum) {
    GUI.combination.empty();
    GUI.combination.append(`<div class="combination">${COMB_NAMES[combNum]}</div>`);
}
//Раздача карт с анимацией
function cardDist(plCardsArr, plNum) {
    crds = plCardsArr;
    if (plNum === undefined) {
        addDeck();
        plNum = Number(plCardsArr.length) + Number(1);
    }

    if (plNum > 1) {
        var plIndex = Number(--plNum) - Number(1);
        var cards = plCardsArr[plIndex];

        setTimeout(() => {
            var cardIndex = 0;
            throwCard(plNum);

            var cardTimerId = setInterval(() => {
                var imgName = cards[cardIndex] + ".png";
                if (cardIndex < cards.length - 1) {
                    throwCard(plNum);
                    cardIndex++;
                }
                else {
                    clearInterval(cardTimerId);
                    cardIndex = 0;
                }
                addCard(imgName, plNum);

            }, THROW_TIME_ANIM);

            if (plNum > 0) {
                setTimeout(() => {
                    cardDist(plCardsArr, plNum)
                }, THROW_TIME_ANIM * cards.length);
            }
        }); 
    }
    else { addChips(); }
}
//Раздача карт без анимации
function quickCardDist(plCardsArr) {
    crds = plCardsArr;
    setTimeout(addDeck, 50);

    setTimeout(() => {
        for (var i in plCardsArr) {
            var plNum = parseInt(Number(i) + Number(1));
            var cards = cards[i];

            for (var j in cards) { addCard(cards[j] + ".png", plNum); }
        }
    }, 100); 

    setTimeout(addChips, 150);
}
//Добавление имени игрока на стол
function addPlayer(nickName, plNum) {
    //if (nickName !== undefined && plNum >= 1 && plNum <= 8) {
        var el = $(`#player${plNum} .player-title`);
        el.empty();
        el.append(nickName).hide();
        el.fadeIn(2500);
    //}
}
//Анимация броска карты
function throwCard(plNum) {
    //if (plNum > 0 && plNum <= MAX_PLAYERS) {
        //console.log(plNum);
        var cardPoint = $("#table-center .game-card").offset();
        var playerPoint = $(`#player${plNum}`).offset();
        var top = Math.round(playerPoint.top - cardPoint.top);
        var left = Math.round(playerPoint.left - cardPoint.left + 40);

        $("#table-center .game-card")[0].lastChild.animate([
            {
                transform: "rotate(0deg)",
                top: "0px",
                left: "0px"
            },
            {
                transform: "rotate(180deg)",
                top: `${top}px`,
                left: `${left}px`
            }],
            { duration: THROW_TIME_ANIM });
    //}
}
//Добавление карты на стол
function addCard(imgName, plNum) {
    //if (imgName != undefined && plNum >= 1 && plNum <= 8) {
        var playerEl = $(`#player${plNum} .player-cards`);
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
        img.mouseenter((e) => { setTimeout(() => {
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
        }, 100);});
        img.mouseleave((e) => { setTimeout(() => {
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
        }, 100);});
    //}
}
//Выкладывание колоды на стол
function addDeck() {
    var div = $("<div class='game-card'></div>");
    var card1 = $("<img src='/images/cards/v1/0.png' alt='...' style='position: absolute'>");
    var card2 = $("<img src='/images/cards/v1/0.png' alt='...' style='z-index: 7'>");

    GUI.tableCenter.empty();
    div.append(card1); div.append(card2);
    GUI.tableCenter.prepend(div);
}
//Добавление картинки фишек и счётчка ставок
function addChips() {
    GUI.tableCenter.empty();
    GUI.tableCenter.append("<img src='/images/poker-chips.png' alt='...' height=80>");
    GUI.tableCenter.append("<span id='bank-count'>0</span>");
}