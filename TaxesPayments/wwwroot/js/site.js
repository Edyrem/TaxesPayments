// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Check page scripts

var errorBorder = "1px solid red";
var normalBorder = "1px solid #ddd";

function selectChange(nextSelect, currentSelect) {
    code = document.getElementById(currentSelect).value;
    console.log("currentSelect: " + currentSelect + " nextSelect: " + nextSelect + " code: " + code);
    
    setTimeout(function () {
        GetDropdownList(nextSelect, code);
        if (currentSelect == "States") {            
            setTimeout(function () {
                selectChange("Villages", nextSelect);
            }, 400)
        }
    }, 400);   
}

function addOptionsToSelect(selectId, options) {
    selector = document.getElementById(selectId);    
    oldOptions = document.querySelectorAll('#'+ selectId + ' option');
    oldOptions.forEach(o => o.remove());
    console.log(options);
    if (options.reponce != "false") {
        document.getElementById('mrHide').style.display = 'block';
        for (var property in options) {
            var singleOption = document.createElement('option');
            singleOption.value = property;
            singleOption.innerHTML = options[property];
            selector.appendChild(singleOption);
            //console.log(`${property}: ${options[property]}`);
        }
    }
    else {
        document.getElementById('mrHide').style.display = 'none';
    }
}

function GetDropdownList(locationName, locationCode) {
    var ajaxResult = null;
    $.ajax({
        type: "POST",
        url: "/Taxes/GetLocations",
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        data: { "locationCode": locationCode, "locationName": locationName },
        success: function (result) {
            //console.log(result);
            ajaxResult = JSON.parse(result);
            addOptionsToSelect(locationName, ajaxResult);
            //ajaxResult = result;
        }
    });
}

function calculateSum(value) {

    var comission = parseInt(document.getElementById('comission').innerHTML);
    var realSum = document.getElementById('realSumm');
    var fullSum = document.getElementById('fullSumm');

    var sum = (value - comission) > 0 ? (value - comission) : 0;
    realSum.innerHTML = sum;
    fullSum.innerHTML = value == "" ? 0 : value;
    //console.log(value + " " + realSum + " " + fullSum);
}

function checkInputs(event) {
    
    var accountInput = document.getElementById('persacc');
    var sumInput = document.getElementById('sum');
    var caNumberInput = document.getElementById('carNumber');
    
    if (accountInput != null && accountInput.value == "") {
        event.preventDefault();
        accountInput.style.border = errorBorder;
    }

    if (sumInput != null && sumInput.value == "") {
        event.preventDefault();
        sumInput.style.border = errorBorder;
    }

    if (caNumberInput != null && caNumberInput.value == "") {
        console.log("carnumber");
        event.preventDefault();
        caNumberInput.style.border = errorBorder;
    }
}

function changeBorder(id) {
    document.getElementById(id).style.border = normalBorder;
}

// Check scripts end

// Pay scripts

document.addEventListener("DOMContentLoaded", () => {
    var sum = parseInt(document.getElementById('insertedSum').innerHTML);
    calculateSum(sum);
});

//function GetDropdownList(locationName, locationCode) {
//    var xhr = new XMLHttpRequest();
//    xhr.open("POST", "/Taxes/GetLocations");
//    xhr.setRequestHeader("RequestVerificationToken", $('input[name="__RequestVerificationToken"]').val());
//    console.log(JSON.stringify({ "LocationCode": locationCode, "locationName": locationName }));
//    var jsonData = JSON.stringify({ "LocationCode": "01", "locationName": "Districts" });
//    var jsonData = JSON.stringify({ "LocationCode": "01", "locationName": "Districts" });
    
//    // тело ответа 
//    xhr.onload = function () {
//        if (this.status >= 200 && this.status < 400) {
//            // If successful
//            console.log(this.response);
//        } else {
//            // If fail
//            console.log(this.response);
//        }
//        var responseObj = JSON.parse(xhr.response);
//        //console.log(responseObj);
//        return responseObj
//    }
//    var location = new Object();
//    location.locationName = "Districts";
//    location.LocationCode = "01";
//    xhr.send(JSON.stringify(location));
//}