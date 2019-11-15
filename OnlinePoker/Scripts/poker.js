const poker = $.connection.pokerHub;
//$.connection.hub.start();
$(function () {
    poker.connection.start();
})

function OpenWaiting(){
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
function AddPlayer(playerNumber, name) {
    $(`#player${playerNumber}`).empty();
    $(`#player${playerNumber}`).append(`<div class="player">
                    <div class="player-title">${name}</div>
                    <div class="player-cards">
                        <div class="card" style="z-index: 1;"></div>
                        <div class="card" style="left: -35px; z-index: 2;"></div>
                        <div class="card" style="left: -70px; z-index: 3;"></div>
                        <div class="card" style="left: -105px; z-index: 4;"></div>
                        <div class="card" style="left: -140px; z-index: 5;"></div>
                    </div>
                </div>`);
}