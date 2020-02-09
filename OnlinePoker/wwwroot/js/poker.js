const MAX_PLAYERS = 8;
const CNT_RETRY_CONN = 3;
const THROW_TIME_ANIM = 700;
const COMB_NAMES_ARR = [
    "Старшая карта",
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
    BANK: $("#bank-count"),
    GAME_ID: $("#game-id").val(),
    BANK: $("#bank"),
    DECK: $("#deck"),
    COMB_NAME: $("#combination"),
    ACTIONS: $("#player-actions"),
    ALERTS: $("#alerts")
}
var currCombNum = null;
var coinsInBank = null;
var crds = {};

//Настройка и пожключение к хабу
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/PokerHub")
    .configureLogging(signalR.LogLevel.Warning)
    .build();

hubConnection.serverTimeoutInMilliseconds = 7200000; //2 часа
////Объявление клиентских методов
hubConnection.on("WaitWindow", waitWindow);
hubConnection.on("CloseWaitWindow", closeWaitWindow);
hubConnection.on("AddPlayer", (nickNamesArr) => {
    for (var i in nickNamesArr) {
        addPlayer(nickNamesArr[i], parseInt(Number(i) + Number(1)))
    }
});
hubConnection.on("CardDist", (plCardsArr) => cardDist(plCardsArr));
hubConnection.on("QuickCardDist", (plCardsArr) => quickCardDist(plCardsArr));
hubConnection.on("AddCombName", (combNum) => { currCombNum = combNum; });
hubConnection.on("AddCoinsToBank", (coinsAmount) => { coinsInBank = coinsAmount });
//Подключение к хабу
hubConnection.start(setTimeout(connectToGame));

////Объвление серверных методов
//Кнопка сделать ставку
function btnBet() {
    const amountBet = $("#amount-bet").val();
    hubConnection.send("Bet", amountBet, GUI.GAME_ID).then(console.log("btnBet run error"));
}
//Кнопка завершить игру
function btnGameOver() {
    hubConnection.send("GameOver", GUI.GAME_ID).then(console.log("btnGameOver run error"));
}

//Подключение к хабу
function connectToGame(cntRetry = 0, isSending = false) {
    if (++cntRetry <= CNT_RETRY_CONN) {
        setTimeout(() => {
            if (hubConnection.state === "Connected" && !isSending) {
                hubConnection.send("ConnectToGame", GUI.GAME_ID).then(connectToGame(cntRetry, true));
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
    GUI.COMB_NAME.empty();
    GUI.COMB_NAME.append(COMB_NAMES_ARR[Number(combNum)]);
}
//Раздача карт с анимацией и показ названия комбинации
function cardDist(plCardsArr, plNum) {
    crds = plCardsArr;
    if (plNum === undefined) {
        GUI.DECK.show();
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
    else {
        GUI.DECK.hide();
        GUI.COMB_NAME.show();
        GUI.BANK.show();
        GUI.ACTIONS.show();
        addCombName(Number(currCombNum));
        addCoinsToBank(Number(coinsInBank));
    }
}
//Раздача карт без анимации и показ названия комбинации
function quickCardDist(plCardsArr) {
    crds = plCardsArr;
    GUI.DECK.show();

    for (var i in plCardsArr) {
            var plNum = parseInt(Number(i) + Number(1));
            var cards = plCardsArr[i];

            for (var j in cards) { addCard(cards[j] + ".png", plNum); }
    }

    GUI.DECK.hide();
    GUI.COMB_NAME.show();
    GUI.BANK.show();
    setTimeout(addCombName(Number(currCombNum)), 1000);
    addCoinsToBank(Number(coinsInBank));
    GUI.ACTIONS.show();
}
//Добавление имени игрока на стол
function addPlayer(nickName, plNum) {
    var el = $(`#player${plNum} .player-title`);
    el.empty();
    el.append(nickName).hide();
    el.fadeIn(2500);
}
//Анимация броска карты
function throwCard(plNum) {
    var cardPoint = GUI.DECK.offset();
    var playerPoint = $(`#player${plNum}`).offset();
    var top = Math.round(playerPoint.top - cardPoint.top);
    var left = Math.round(playerPoint.left - cardPoint.left + 40);

    GUI.DECK[0].lastElementChild.animate([
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
}
//Добавление карты на стол
function addCard(imgName, plNum) {
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
}
//Добавление монет в банк
function addCoinsToBank(coins) {
    const BANK = $("#bank-count");
    BANK.empty();
    BANK.append(Number(coins) + ' ' + "монет");
}
//Добавление уведомления
function addAlert(title, text, color) {
    var alert = $(`<div class="alert alert-${color} alert-dismissible fade show" role="alert">
        <strong>${title}</strong>&emsp;${text}
  <button type="button" class="close" data-dismiss="alert" aria-label="Close">
            <span aria-hidden="true">&times;</span>
        </button>
    </div>`);
    GUI.ALERTS.append(alert.hide().fadeIn());
}