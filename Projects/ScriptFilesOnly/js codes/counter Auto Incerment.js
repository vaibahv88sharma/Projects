// refere this script in content editor Webpart
<script src="/sites/365Build/Watersun/SiteAssets/jquery-1.11.3.js"></script>
<script src="/sites/365Build/Watersun/SiteAssets/sputility.min.js"></script>
<script>

    // Get the current Site
    var siteUrl = '/sites/365Build/Watersun/';

    function retrieveListItems() {

        var clientContext = new SP.ClientContext(siteUrl);
        // Get the liste instance
        var oList = clientContext.get_web().get_lists().getByTitle('counterList');

        var camlQuery = new SP.CamlQuery();

        // Get only the last element
        camlQuery.set_viewXml('<Query><OrderBy><FieldRef Name=\'ID\' Ascending=\'False\' /></OrderBy></Query><RowLimit>1</RowLimit></View>');
        this.collListItem = oList.getItems(camlQuery);

        clientContext.load(collListItem);
		alert("Before fetching ID");
        clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));        

    }

    function onQuerySucceeded(sender, args) {

        var listItemInfo = '';
		alert("onQuerySucceeded entered");
        var listItemEnumerator = collListItem.getEnumerator();

        while (listItemEnumerator.moveNext()) {
            var oListItem = listItemEnumerator.get_current();

            var listItemInfovalue = oListItem.get_item('count');


            // Based in you request the id is : 14-00001

            // Split the first
			////////////////////////////////////////////////////////
            /* var res = listItemInfovalue.split("-");

            console.log(res[1]);

            // increment the index
            var newId = parseInt(res[1])+1;

            // create the new id
            SPUtility.GetSPField('Occurrence #').SetValue(res[0] + '-' + pad(newId, 5) ); */
			///////////////////////////////////////////////////////
			//var res = listItemInfovalue.split("-");

            //console.log(res[1]);

            // increment the index
            var newId = listItemInfovalue+1;

            // create the new id
            SPUtility.GetSPField('count').SetValue(newId);
        }

        console.log(listItemInfo.toString());
    }

    function onQueryFailed(sender, args) {
		alert("failed");
        alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace()); 
    }   


    // Create new id with fixed size : 
    // exp : 00001, 00001

    // num : is the number
    // size : is the number size
    function pad(num, size) {
        var s = num+"";
        while (s.length < size) s = "0" + s;
        return s;
    }


$(document).ready(function(){
    ExecuteOrDelayUntilScriptLoaded(retrieveListItems, "sp.js"); 
    });

</script>