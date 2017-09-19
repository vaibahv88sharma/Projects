<!DOCTYPE html>
<html>
<head>
<title>Title of the document</title>
</head>

<body>

<select id="mySelect">
    <option></option>
</select>

</body>

</html>


<script type="text/javascript" src="/sites/365Build/Watersun/SiteAssets/jquery-1.12.3.js"></script>


<script type="text/javascript">



/*    function getUserName() {
		var context = SP.ClientContext.get_current();
		var user = context.get_web().get_currentUser();
        context.load(user);
        context.executeQueryAsync(onGetUserNameSuccess, onGetUserNameFail);
    }

    // This function is executed if the above call is successful
    // It replaces the contents of the 'message' element with the user name
    function onGetUserNameSuccess() {
        $('#message').text('Hello ' + user.get_title());
    }*/

$( document ).ready(function() {
    //alert("Before retrieveListItems");
    SP.SOD.executeFunc('sp.js', 'SP.ClientContext', retrieveListItems);
    //retrieveListItems();
    //alert("After retrieveListItems");
});


var siteUrl = '/sites/365Build/Watersun/';
var collListItem;

var optionsArr = ["<option></option>"];
var inputElement = $('#mySelect');

function retrieveListItems() {
    //alert("Before retrieveListItems 1");  
    var clientContext = new SP.ClientContext(siteUrl);
    //console.log(clientContext);
    //alert(clientContext+ " Before retrieveListItems 2");  
    var oList = clientContext.get_web().get_lists().getByTitle('l_JobsData');
    //alert(clientContext.get_web());
    //alert(oList);
    //console.log(oList);
    var camlQuery = new SP.CamlQuery();
/*    camlQuery.set_viewXml('<View><Query><Where><Geq><FieldRef Name=\'ID\'/>' + 
        '<Value Type=\'Number\'>1</Value></Geq></Where></Query><RowLimit>10</RowLimit></View>');*/
        camlQuery.set_viewXml('<View><Query><Where><Eq><FieldRef Name=\'SupFilter\' /><Value Type=\'User\'>Tim Norris</Value></Eq></Where><OrderBy><FieldRef Name=\'Supervisor_x0020_Job\' Ascending=\'True\' /></OrderBy></Query></View>');
    //alert(" Before retrieveListItems 4");      
    //this.collListItem = oList.getItems(camlQuery);
    collListItem = oList.getItems(camlQuery);
    //alert(" Before retrieveListItems 5");  
    console.log(collListItem);    
    clientContext.load(collListItem);
    //alert(" Before retrieveListItems 6");    
    //clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));        
        clientContext.executeQueryAsync(
        function(sender, args) {
            //alert("Success 1");            
            var listItemInfo = '';

            var listItemEnumerator = collListItem.getEnumerator();            
            var optionsArr = ["<option></option>"];
            var inputElement = $('#mySelect');

            while (listItemEnumerator.moveNext()) {
                var oListItem = listItemEnumerator.get_current();
                //alert("Success while 1");  
                listItemInfo += '\nID: ' + oListItem.get_id() +  '\nTitle: ' + oListItem.get_item('Supervisor_x0020_Job') +  '\nBody: ' + oListItem.get_item('Title');
                //alert("while");
                console.log(oListItem.get_item('Title'));
                optionsArr.push("<option value='" + oListItem.get_item('Supervisor_x0020_Job') + "'>" + oListItem.get_item('Supervisor_x0020_Job') + "</option>");
            }
            inputElement.html(optionsArr.join(""));
            inputElement.on('change', function() {
                alert($(this).val());
            });
            alert(listItemInfo.toString());            
        },
        function(sender, args) {
            alert("fail");
        }
    );         
}

function onQuerySucceeded(sender, args) {
    alert("successful");
    var listItemInfo = '';

    var listItemEnumerator = collListItem.getEnumerator();
        
    while (listItemEnumerator.moveNext()) {
        var oListItem = listItemEnumerator.get_current();
        listItemInfo += '\nID: ' + oListItem.get_id() + 
            '\nTitle: ' + oListItem.get_item('Title') + 
            '\nBody: ' + oListItem.get_item('Body');
    }

    alert(listItemInfo.toString());
}

function onQueryFailed(sender, args) {
    alert("failed badly");
    alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
}


</script>