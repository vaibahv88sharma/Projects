window.onload = function() {
	
document.getElementById('r1c1').innerHTML="new r1c1";
document.getElementById('r1c2').innerHTML="new r1c2";
document.getElementById('r1c3').innerHTML="new r1c3";
document.getElementById('r1c4').innerHTML="new r1c4";
document.getElementById('r2c1').innerHTML="new r2c1";
document.getElementById('r2c2').innerHTML="new r2c2";
document.getElementById('r2c3').innerHTML="new r2c3";
document.getElementById('r2c4').innerHTML="new r2c4";
document.getElementById('r3c1').innerHTML="new r3c1";
document.getElementById('r3c2').innerHTML="new r3c2";
document.getElementById('r3c3').innerHTML="new r3c3";
document.getElementById('r3c4').innerHTML="new r3c4";	
	
	//call function
	//alert(_spPageContextInfo.webAbsoluteUrl);
	//alert(_spPageContextInfo.webAbsoluteUrl.substring(_spPageContextInfo.webAbsoluteUrl.lastIndexOf("/") + 1, _spPageContextInfo.webAbsoluteUrl.length));
	
	//alert("1");
/*         hostweburl =
            decodeURIComponent(
                getQueryStringParameter("SPHostUrl")
        ); */
		//alert("1");
		var hostweburl = _spPageContextInfo.webAbsoluteUrl;
        var scriptbase = hostweburl + "/_layouts/15/";
        $.getScript(scriptbase + "SP.Runtime.js",
            function () {
                $.getScript(scriptbase + "SP.js", execOperation);
            }
        );	
	//alert("end");
	/* var mycol = document.getElementById('r1c1');
	mycol.innerHTML="new r1c1";	 */
	
}

function execOperation(){
	alert("reached execOperation");
    var clientContext = new SP.ClientContext(_spPageContextInfo.webAbsoluteUrl);
	var webSiteName = _spPageContextInfo.webAbsoluteUrl.substring(_spPageContextInfo.webAbsoluteUrl.lastIndexOf("/") + 1, _spPageContextInfo.webAbsoluteUrl.length);
 	var oList = clientContext.get_web().get_lists().getByTitle(webSiteName);
			
 	var camlQuery = new SP.CamlQuery();
// 	camlQuery.set_viewXml(
	//	'<View><Query><Where><Geq><FieldRef Name=\'ID\'/>' + 
	//	'<Value Type=\'Number\'>1</Value></Geq></Where></Query>' + 
	//	'<RowLimit>10</RowLimit></View>'
	//); 
	camlQuery.set_viewXml(
		'<View></View>'
	);	
	
	this.collListItem = oList.getItems(camlQuery); 
	clientContext.executeQueryAsync(
		function(data){
			debugger;
			alert("Success");
		}, 
		function(data){
			alert("fail");
		}
	); 	 	
}
/* 
	
    function getQueryStringParameter(paramToRetrieve) {
        var params =
            document.URL.split("?")[1].split("&");
        var strParams = "";
        for (var i = 0; i < params.length; i = i + 1) {
            var singleParam = params[i].split("=");
            if (singleParam[0] == paramToRetrieve)
                return singleParam[1];
        }
    }	 */