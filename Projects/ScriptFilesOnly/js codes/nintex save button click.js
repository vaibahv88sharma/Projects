// Nintex Button: Add this code in the Settings of the Save button under Advanced section in Client Click

NWF$(document).ready(function(){
		//alert("Begin");		
		alrt();
	});
	

NWF$(document).ready(function(){
		ExecuteOrDelayUntilScriptLoaded(alrt, "sp.js"); 
	});
	
	
// Nintex Form: Form Setting >  Custom Javascript
	
function alrt()
{
	alert("Hi there, buddy");
}

////////////////////
    function alrt() {
		alert("Started");

		//Public Variables: Begin
			var context = SP.ClientContext.get_current();
			var user = context.get_web().get_currentUser();
			var hostWebUrl  = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
			var appWebUrl   = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));

			var currentcontext = new SP.ClientContext.get_current();
			var hostcontext = new SP.AppContextSite(currentcontext, hostWebUrl);
			var hostweb = hostcontext.get_web();	
		//Public Variables: End

		
        // Get the liste instance
		var oList = hostweb.get_lists().getByTitle('counterList');
        //var oList = clientContext.get_web().get_lists().getByTitle('counterList');

        var camlQuery = new SP.CamlQuery();

        // Get only the last element
        camlQuery.set_viewXml('<Query><OrderBy><FieldRef Name=\'count\' Ascending=\'False\' /></OrderBy></Query><RowLimit>1</RowLimit></View>');
        this.collListItem = oList.getItems(camlQuery);

        context.load(collListItem);
		alert("retrieveListItems before execute");
        //context.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));         
		context.executeQueryAsync(
				function(data){
					alert("success");
				},
				function(xhr){
					alert("fail");
					alert('Error : (' + xhr.status + ') ' + xhr.statusText + ' Message: ' + xhr.responseJSON.error.message.value);
				}
			);
    }
	

/////////////////////////
