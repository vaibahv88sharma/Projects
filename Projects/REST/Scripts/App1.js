//Create List using REST

'use strict';  
var hostweburl;   
var appweburl;

$(document).ready(function(){
	hostweburl = decodeURIComponent(getQueryStringParameter("SPHostUrl"));
	appweburl = decodeURIComponent(getQueryStringParameter("SPAppWebUrl"));
	$("#message1").text("hostweburl : " + hostweburl);
	$("#message1").append("<br><br>");
	$("#message1").append("appweburl :  " + appweburl);
	$("#message1").append("<br><br>");
	$("#message1").append("Page Context: " + _spPageContextInfo.siteAbsoluteUrl);
	createSPList();
});

function createSPList(){
	
	$.ajax(
		{
			url : appweburl + "/_api/SP.AppContextSite(@target)/web/lists?@target='"+ hostweburl + "/sites/apps'",
			//hostweburl + "/sites/apps'",
			type: "POST",
			data: JSON.stringify({
			'__metadata' : {'type':'SP.List'},
			'AllowContentTypes'	: true,
			'BaseTemplate':100,
			'ContentTypesEnabled':true,
			'Description': 'My TestCustomList description',
			'Title': 'TestCustomList'
			}),
			headers:{
				"accept" : "application/json;odata=verbose",
				"content-type" : "application/json;odata=verbose",
				"X-RequestDigest" : $("#__REQUESTDIGEST").val()
			},
			success: successHandler,
			error: errorHandler
		});
}

function successHandler(){
	$("#message2").text("Success");
}

function errorHandler(data,errorCode,errorMessage){
	$("#message2").text("Failure , Message: " + errorMessage);
}


function getQueryStringParameter(paramToRetrieve){   
   var params =   
   document.URL.split("?")[1].split("&");   
   var strParams = "";   
   for (var i = 0; i < params.length; i = i + 1) {   
   var singleParam = params[i].split("=");   
   if (singleParam[0] == paramToRetrieve)   
      return singleParam[1];   
   }
}