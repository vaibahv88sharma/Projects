var web;  
var hostweburl;  
var appweburl;  
  
var pollSP;  
var availableTags;
  
var dataResult ;
var i;
var optionsArr = [];
  
NWF.FormFiller.Events.RegisterAfterReady(function () {  
    pollSP = setInterval(checkSPLoad, 500);  
});  
  
function checkSPLoad() {  
    if (clientContext) {  
        window.clearInterval(pollSP);  
        sharePointReady();  
    }  
}
  
function sharePointReady() {  
    NWF$().ready(function () {  
  
        hostweburl = decodeURIComponent(Utils.getQueryStringParameter('SPHostUrl'));  
        appweburl = decodeURIComponent(Utils.getQueryStringParameter('SPAppWebUrl'));  
		
        Data.getSomethingWithREST();  


	 
  NWF$("input[id^='ListForm_formFiller_FormView_ctl46_']").autocomplete({	  
      source: optionsArr
    });
  
    });  
}  
  
var Data = {  
    getSomethingWithREST: function () {  
        var context;  
        var factory;  
        var appContextSite;  
        var listName = "l_SupplierDemo";//"l_Supplier";  
  
        context = new SP.ClientContext(appweburl);  
        factory = new SP.ProxyWebRequestExecutorFactory(appweburl);  
        context.set_webRequestExecutorFactory(factory);  
        appContextSite = new SP.AppContextSite(context, hostweburl);  
        var executor = new SP.RequestExecutor(appweburl);  
        executor.executeAsync({  
            url: appweburl + "/_api/SP.AppContextSite(@target)/web/lists/getbytitle('" + listName + "')/items?$select=SupplierName&$top=5000&$orderby=SupplierName asc&@target='" + encodeURIComponent(hostweburl) + "'",  

            method: "GET",  
            headers: {  
                "Accept": "application/json; odata=verbose"
            },  
            success: function (data) {  
                var obj = JSON.parse(data.body);  
                console.log(obj.d.results); 
				dataResult = null;
				dataResult = obj.d.results;
                console.log("Count:- "+dataResult.length);
				//alert("Count:- "+dataResult.length);
 				for(i=0;i<dataResult.length;i++)
				{		
					optionsArr.push(dataResult[i].SupplierName);
				}
				console.log("success:-  " + JSON.stringify(data));
				//alert("success:-  " + JSON.stringify(data));				
            },  
            error: function (err) {  
                alert("error:-  " + JSON.stringify(err));  
            },  
        });  
    },  
};  
  
var Utils = {  
    getQueryStringParameter: function (param) {  
        var params = document.URL.split('?')[1].split('&');  
        var strParams = '';  
        for (var i = 0; i < params.length; i = i + 1) {  
            var singleParam = params[i].split('=');  
            if (singleParam[0] == param) {  
                return singleParam[1];  
            }  
        }  
    },  
};  