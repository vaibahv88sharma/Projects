//Get List data using REST

'use strict';    
var hostweburl;   
var appweburl;

//var hostWebUrl = decodeURIComponent(getQueryStringParameter("SPHostUrl"));
//var	appWebUrl = decodeURIComponent(getQueryStringParameter("SPAppWebUrl"));
var hostWebUrl;
var	appWebUrl;
	

$(document).ready(function () {
	
});
  
hostWebUrl  = decodeURIComponent(manageQueryStringParameter('SPHostUrl'));
appWebUrl   = decodeURIComponent(manageQueryStringParameter('SPAppWebUrl'));
	
//start update ListItem
function Update() {
    var listName = "TestCustomList";
    var url = _spPageContextInfo.siteAbsoluteUrl;//webAbsoluteUrl is for App Context;
    var itemId = "1"; // Update Item Id here
    var currentdate = new Date();
    var datetime =  currentdate.getDate() ;
    var title = "New Title Updated  at: " + datetime;
    updateListItem(itemId, listName, url, title, function () {
        alert("Item updated, refreshing available items");
    }, function () {
        alert("Ooops, an error occured. Please try again" + data);
    });
}
function updateListItem(itemId, listName, siteUrl, title, success, failure) {
    var itemType = GetItemTypeForListName(listName);

    var item = {
        "__metadata": { "type": itemType },
        "Title": title
    };

    getListItemWithId(itemId, listName, siteUrl, function (data) {
        $.ajax({
            url: data.__metadata.uri,
            type: "POST",
            contentType: "application/json;odata=verbose",
            data: JSON.stringify(item),
            headers: {
                "Accept": "application/json;odata=verbose",
                "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                "X-HTTP-Method": "MERGE",
                "If-Match": data.__metadata.etag
            },
            success: function (data) {
                success(data);
            },
            error: function (data) {
                failure(data);
            }
        });
    }, function (data) {
        failure(data);
    });
}
function getListItemWithId(itemId, listName, siteurl, success, failure) {
    var url = siteurl + "/_api/web/lists/getbytitle('" + listName + "')/items?$filter=Id eq " + itemId;
    $.ajax({
        url: url,
        method: "GET",
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            if (data.d.results.length == 1) {
                success(data.d.results[0]);
            }
            else {
                failure("Multiple results obtained for the specified Id value");
            }
        },
        error: function (data) {
            failure(data);
        }
    });
}
function GetItemTypeForListName(name) {
    return "SP.Data." + name.charAt(0).toUpperCase() + name.slice(1) + "ListItem";
}
//end update ListItem


//Begin SP.RequestExecutor.js
function getListUsingSPRequestExecutorJS() {
    $.getScript(hostweburl + "/_layouts/15/SP.RequestExecutor.js", runCrossDomainRequest);
}
function runCrossDomainRequest() {
    var executor = new SP.RequestExecutor(appweburl); 
    executor.executeAsync({
        url: appWebUrl + "/_api/SP.AppContextSite(@target)/web/lists?@target='" + hostWebUrl + "'",
        method: "GET", 
        headers: { "Accept": "application/json; odata=verbose" }, 
        success: function (data) { alert("Success: "+JSON.stringify(data)) },
        error: function (data) { alert("Failure: "+JSON.stringify(data)) }
    });
}
//End  SP.RequestExecutor.js


//Begin Add ListItem
function addListItem() {
    var listname = "TestCustomList";
    var currentdate = new Date();
    var datetime = currentdate.getDate();
    var title = "New Title Created  at: " + datetime;
    var item = {
        "__metadata": { "type": GetItemTypeForListName(listname) }, 
        "Title": title
    };

    // Executing our add
    $.ajax({
        url: appWebUrl + "/_api/SP.AppContextSite(@target)/web/lists/getbytitle('" +
                                                listname + "')/items?@target='" + hostWebUrl + "'",
        type: "POST",
        contentType: "application/json;odata=verbose",
        data: JSON.stringify(item),
        headers: {
            "Accept": "application/json;odata=verbose",
            "X-RequestDigest": $("#__REQUESTDIGEST").val()
        },
        success: function (data) {
             alert("Success: "+JSON.stringify(data)) 
        },
        error: function (data) {
            alert("Failure: " + JSON.stringify(data))
        }
    });
}
//End Add ListItem


function getListTitle() {  
var scriptbase = hostweburl ;  
    $.ajax({  
		 url : appWebUrl + "/_api/SP.AppContextSite(@target)/web/lists/getbytitle('Tasks1')/items?" +
    "@target='" + hostWebUrl + "'",  
        method: "GET",  
        headers: { "Accept": "application/json; odata=verbose","content-type" : "application/json;odata=verbose" },  
        success: function (data) {  
            // Returning the results  
            onSucceed(data);  
        },  
        error: function (data) {  
            onFailure(data);  
        }  
    });  
}


function getLookUpList() {
	var urlForAllItems = appWebUrl + "/_api/SP.AppContextSite(@target)/web/lists/getbytitle('State')/items?"  						
						+"$select=Title,Population,ID,Country/Name,PopularRiver/Title&"
						+"$expand=Country,PopularRiver&" 
						+"$filter=Mantry eq 50 and Country/Name eq 'India' and PopularRiver/Title eq 'ganga'&"
						+"@target='" + hostWebUrl + "'";
 $.ajax({  
		url: urlForAllItems, 
        method: "GET",  
        headers: { "Accept": "application/json; odata=verbose","content-type" : "application/json;odata=verbose" },  
        success: function (data) {  
            // Returning the results  
           // onSucceed(JSON.stringify(data));  
		   alert("Success: "+JSON.stringify(data));
        },  
        error: function (error,errorCode,errorMessage) {  
            alert("Error is: " + JSON.stringify(error) + " ..... "+errorCode+" ..... "+errorMessage);  
        }  
    });  
} 


function getLists1() {  
    $.ajax({  
		url: _spPageContextInfo.siteAbsoluteUrl + "/_api/web/lists/getbytitle('TestCustomList')/items",  
        method: "GET",  
        headers: { "Accept": "application/json; odata=verbose","content-type" : "application/json;odata=verbose" },  
        success: function (data) {  
            // Returning the results  
            onSucceed(data);  
        },  
        error: function (data) {  
            onFailure(data);  
        }  
    });  
} 


function getLists() {
var siteurl = _spPageContextInfo.webAbsoluteUrl;  
    $.ajax({  
		url: siteurl + "/_api/web/lists",  
        method: "GET",  
        headers: { "Accept": "application/json; odata=verbose","content-type" : "application/json;odata=verbose" },  
        success: function (data) {  
            // Returning the results  
            onSucceed(data);  
        },  
        error: function (data) {  
            onFailure(data);  
        }  
    });  
} 
 

function onFailure(data) {
	 $("#message3").text("Failure ");
	 $("#message3").append("<br>");
	 $("#message3").append(data);	   	 	 
}
function onSucceed(data){
	$("#message3").text("Success: ");
	$("#message3").append("<br>");
	 var dataResult = data.d.results;
	 var i;
	 for(i=0;i<dataResult.length;i++)
	 {
		 $("#message3").append(dataResult[i].Title);
	 }	
}


function getQueryStringParameter(paramToRetrieve) {     
    var params =  document.URL.split("?")[1].split("&");       
    for (var i = 0; i < params.length; i = i + 1) {     
        var singleParam = params[i].split("=");     
        if (singleParam[0] == paramToRetrieve)     
        return singleParam[1];     
        }     
}          
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