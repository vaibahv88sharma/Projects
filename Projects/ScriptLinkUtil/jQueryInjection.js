var collListItem;
window.onload = function() {
	
/* document.getElementById('r1c1').innerHTML="new r1c1";
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
document.getElementById('r3c4').innerHTML="new r3c4";	 */
	
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
	//alert("reached execOperation");
    var clientContext = new SP.ClientContext(_spPageContextInfo.webAbsoluteUrl);
	//var webSiteName = _spPageContextInfo.webAbsoluteUrl.substring(_spPageContextInfo.webAbsoluteUrl.lastIndexOf("/") + 1, _spPageContextInfo.webAbsoluteUrl.length);
	var url = _spPageContextInfo.webAbsoluteUrl+"/_api/Web/Lists/GetByTitle('JobStatus')/Items";	
    $.ajax({
        url: url,//_spPageContextInfo.webAbsoluteUrl + url,
        type: "GET",
        headers: {
            "accept": "application/json;odata=verbose",
        },
        success: function (data) {
            //console.log(data.d.results);
			$.each(data.d.results, function(index, item){
                //alert(item.Value);
				}
			);
			var newRows = "<table style=\"width:300%\">";
			debugger;
			for (var i = 0; i < data.d.results.length; i++) {
			   newRows += "<tr><td>" + data.d.results[i].Title + 
						  "</td><td>" + data.d.results[i].Value + "</td></tr>";
			}
			newRows += "</table>";
			//alert(newRows);
			document.getElementById('pageStatusBar').innerHTML=newRows; //div1
			
        },
        error: function (error) {
            alert(JSON.stringify(error));
        }
    }); 	 	
}


/* function execOperation(){
	//alert("reached execOperation");
    var clientContext = new SP.ClientContext(_spPageContextInfo.webAbsoluteUrl);
	//var webSiteName = _spPageContextInfo.webAbsoluteUrl.substring(_spPageContextInfo.webAbsoluteUrl.lastIndexOf("/") + 1, _spPageContextInfo.webAbsoluteUrl.length);
 	var oList = clientContext.get_web().get_lists().getByTitle("JobStatus");
			
 	var camlQuery = new SP.CamlQuery();
 	camlQuery.set_viewXml(
		'<View><Query><Where><Geq><FieldRef Name=\'ID\'/>' + 
		'<Value Type=\'Number\'>1</Value></Geq></Where></Query>' + 
		//'<RowLimit>10</RowLimit></View>'
		'</View>'
	); 

	var collListItem = oList.getItems(camlQuery); 
	//this.collListItem = oList.getItems(camlQuery); 
	clientContext.executeQueryAsync(
		function(sender, args){
			debugger;
			alert("Success");	
			var listItemInfo = '';
			var listItemEnumerator = collListItem.getEnumerator();
			alert("Success1");	
			while (listItemEnumerator.moveNext()) {
				alert("Success2");
				var oListItem = listItemEnumerator.get_current();
				listItemInfo += '\nID: ' + oListItem.get_id() + 
					'\nTitle: ' + oListItem.get_item('Title') + 
					'\nBody: ' + oListItem.get_item('Value');
			}

			alert(listItemInfo.toString());			
		}, 
		function(sender, args){
			alert('Failed to get user name. Error:' + args.get_message());
		}
	); 	 	
} */

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