'use strict';


//region GlobalVar

var listItemId;
var listItemToUpdate;

var context = SP.ClientContext.get_current();

var hostWebUrl;
var appWebUrl;

//endregion GlobalVar


hostWebUrl = decodeURIComponent(manageQueryStringParameter('SPHostUrl'));
appWebUrl = decodeURIComponent(manageQueryStringParameter('SPAppWebUrl'));

function manageQueryStringParameter(paramToRetrieve) {
    var params =
    document.URL.split("?")[1].split("&");
    var strParams = "";
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split("=");
        if (singleParam[0] == paramToRetrieve) {
            return singleParam[1];
        }
    }
}

function success() {
    $("#dvMessage").text("Operation Completed Successfully");
}

function fail() {
    $("#dvMessage").text("Operation failed  " + arguments[1].get_message());
}
