var currCntrlName = null;

$(document).ready(() => {
    bindToAccessRightsOnClick();
});


function bindToAccessRightsOnClick() {
    console.log("Method 'bindToAccessRightsOnClick' sucessfully ran");
    var cntrlSlct = document.getElementById("Controller_Name");
    currCntrlName = cntrlSlct.selectedOptions[0].value;
    cntrlSlct.onchange = function ()
    {
        var slctCntrlName = cntrlSlct.selectedOptions[0].value;
        var currActDiv = document.getElementById(`#${currCntrlName}`);
        var slctActDiv = document.getElementById(`#${slctCntrlName}`);

        currActDiv.style.display = "none";
        slctActDiv.style.display = "block";

        currCntrlName = slctCntrlName;
    }
    console.log("Method 'bindToAccessRightsOnClick' sucessfully made");
}