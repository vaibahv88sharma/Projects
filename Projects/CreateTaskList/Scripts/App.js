'use strict';

(function () {

    // This code runs when the DOM is ready and creates a context object which is 
    // needed to use the SharePoint object model
    $(document).ready(function () {
		
        getUserName();
		getJobItems();
		getSupervisor();
		getTaskCategoryLookupData();		
        $("#btn-new").on('click', function () {
            createlist();
        });										
		$("#btn-new-items").on('click', function () {
			//insertTaskItemsData();
			getUserID(document.getElementById("mySupervisorSelect").value);
        });				
    });


//Public Variables: Begin
	var context = SP.ClientContext.get_current();
	var user = context.get_web().get_currentUser();
	var hostWebUrl  = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
	var appWebUrl   = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));

    var currentcontext = new SP.ClientContext.get_current();
    var hostcontext = new SP.AppContextSite(currentcontext, hostWebUrl);
    var hostweb = hostcontext.get_web();	
//Public Variables: End

//Get Username: Begin
    function getUserName() {
        context.load(user);
        context.executeQueryAsync(onGetUserNameSuccess, onGetUserNameFail);
    }
    function onGetUserNameSuccess() {
        $('#message').text('Hello ' + user.get_title() + ',');
    }
    function onGetUserNameFail(sender, args) {
        alert('Failed to get user name. Error:' + args.get_message());
    }
//Get Username: End

//Create Auto Populate Dropdown for displaying Jobs Name: Begin
function getJobItems() {
	    $.ajax({
			url : appWebUrl + "/_api/SP.AppContextSite(@target)/web/lists/getbytitle('JobDetailsLookup')/items?" +
				    "@target='" + hostWebUrl + "'",  
	        method: "GET",  
	        headers: { "Accept": "application/json; odata=verbose","content-type" : "application/json;odata=verbose" },  
	        success: function (data) {onSucceedGetJobItems(data);},  
	        //error: function(jqXHR, textStatus, errorThrown){alert(textStatus);alert(errorThrown);alert(jqXHR.responseText);}
			error: function (data) {onFailureGetJobItems(data);}    
	    }); 
}
function onFailureGetJobItems(data) {
	alert("Failure: " + JSON.stringify(data));
}
function onSucceedGetJobItems(data){	
	var dataResult = data.d.results;
	var i;
	var optionsArr = ["<option></option>"];
	var inputElement = $('#mySelect');		
	for(i=0;i<dataResult.length;i++)
	{		
		optionsArr.push("<option value='" + dataResult[i].JobAddress + "'>" + dataResult[i].JobAddress + "</option>");
	}
    inputElement.html(optionsArr.join(""));
    inputElement.on('change', function() {
		alert($(this).val());
		document.getElementById("text2").value  = document.getElementById("mySelect").value;		
    });		
}
//Create Auto Populate Dropdown for displaying Jobs Name: End

//Create Auto Populate Dropdown for displaying Site Supervisor Name: Begin
function getSupervisor() {
	    $.ajax({
			url : appWebUrl + "/_api/SP.AppContextSite(@target)/web/lists/getbytitle('SiteSupervisor')/items?" +
					"$select=SiteSupervisor/Title,SiteSupervisor/EMail,SiteSupervisor/FirstName,SiteSupervisor/LastName,SiteSupervisor/EMail&$expand=SiteSupervisor/Id&"+
					//"&$filter=SiteSupervisor/Title eq '"+"office user" +"'&"+
							//&$filter=SiteSupervisor/Title eq 'office user'
				    "@target='" + hostWebUrl + "'",  
	        method: "GET",  
	        headers: { "Accept": "application/json; odata=verbose","content-type" : "application/json;odata=verbose" },  
	        success: function (data) {onSucceedSupervisor(data);},  
	        //error: function(jqXHR, textStatus, errorThrown){alert(textStatus);alert(errorThrown);alert(jqXHR.responseText);}
			error: function (data) {onFailureSupervisor(data);}    
	    }); 
}
function onFailureSupervisor(data) {
	alert("Failure: " + JSON.stringify(data));
}
function onSucceedSupervisor(data){	
	var dataResult = data.d.results;
	var i;
	var optionsArr = ["<option></option>"];
	var inputElement = $('#mySupervisorSelect');
	for(i=0;i<dataResult.length;i++)
	{
		optionsArr.push("<option value='" + dataResult[i].SiteSupervisor.Title + "'>" + dataResult[i].SiteSupervisor.Title + "</option>");
	}
    inputElement.html(optionsArr.join(""));
    inputElement.on('change', function() {
		alert($(this).val());
		//document.getElementById("text2").value  = document.getElementById("mySelect").value;		
    });		
}
//Create Auto Populate Dropdown fo displaying Site Supervisor Name: End

//Create Task List : Begin
function createlist() {
    var listCreationInfo = new SP.ListCreationInformation();
	var listTitle = document.getElementById("mySelect").value;
    listCreationInfo.set_title(listTitle);
	listCreationInfo.set_templateType(SP.ListTemplateType.tasksWithTimelineAndHierarchy);
	var lists = hostweb.get_lists();
    var newList = lists.add(listCreationInfo);
    currentcontext.load(newList);
    currentcontext.executeQueryAsync(onListCreationSuccess, onListCreationFail);
}
function onListCreationSuccess() {
    alert('List created successfully!');
	createlistView();
}
function onListCreationFail(sender, args) {
    alert('Failed to create the list. ' + args.get_message());
}

//Create Task List : End

//Create Task View : Begin
function createlistView() {
	var listTitle = document.getElementById("mySelect").value;
	var oList = hostweb.get_lists().getByTitle(listTitle);
	var viewCollection = oList.get_views();  
	currentcontext.load(viewCollection);		
	var createView = new SP.ViewCreationInformation();  
	createView.set_title("TasksCreated");
	//var viewFields = ["PercentComplete","Title","DueDate","AssignedTo","Body","Priority","Author"];
	var viewFields = ["Checkmark","PercentComplete","Title","DueDate","AssignedTo","Body","Priority","Author"];	
	createView.set_viewFields(viewFields);
	createView.set_rowLimit(30);
	createView.set_viewTypeKind(1);  //0, 1, 2048, 524288, 8193, 131072, 67108864  
	createView.set_setAsDefaultView(true);
	viewCollection.add(createView);   
	currentcontext.load(viewCollection);    
    currentcontext.executeQueryAsync(onViewCreationSuccess, onViewCreationFail);
}
function onViewCreationSuccess() {
    alert('View created successfully!');
}
function onViewCreationFail(sender, args) {
    alert('Failed to create the View of List. ' + args.get_message());
}
//Create Task View : End

//Get Task Category Lookup: Begin
function getTaskCategoryLookupData() {
	    $.ajax({
			url : appWebUrl + "/_api/SP.AppContextSite(@target)/web/lists/getbytitle('TaskCategoryLookup')/items?" +
	    "@target='" + hostWebUrl + "'",  
	        method: "GET",  
	        headers: { "Accept": "application/json; odata=verbose","content-type" : "application/json;odata=verbose" },  
	        success: function (data) {onTaskCategoryLookupDataSucceed(data);},  
	        //error: function(jqXHR, textStatus, errorThrown){alert(textStatus);alert(errorThrown);alert(jqXHR.responseText);}
			error: function (data) {onTaskCategoryLookupDataFailure(data);}    
	    }); 
}
function onTaskCategoryLookupDataFailure(data) {
	alert("Failure: " + JSON.stringify(data));
}
function onTaskCategoryLookupDataSucceed(data){
	var dataResult = data.d.results;
	var i;
	var optionsCategoryArr = ["<option></option>"];
	var inputElement = $('#myCategorySelect');		
	for(i=0;i<dataResult.length;i++)
	{		
		optionsCategoryArr.push("<option value='" + dataResult[i].TaskCategoryLookup + "'>" + dataResult[i].TaskCategoryLookup + "</option>");
	}
    inputElement.html(optionsCategoryArr.join(""));
    inputElement.on('change', function() {
		alert($(this).val());	
    });	
}
//Get Task Category Lookup : End

//Get User ID from SiteSupervisor List : Begin
function getUserID(userName) {
	    $.ajax({
			url : appWebUrl + "/_api/SP.AppContextSite(@target)/web/lists/getbytitle('SiteSupervisor')/items?"+
						"$select=SiteSupervisor/Id,SiteSupervisor/Name,SiteSupervisor/Title&"+
						"&$expand=SiteSupervisor/Id"+
						"&$filter=SiteSupervisor/Title eq '"+ userName +"'&"+
						"@target='" + hostWebUrl + "'", 
	        method: "GET",  
	        headers: { "Accept": "application/json; odata=verbose","content-type" : "application/json;odata=verbose" },  
	        success: function (data) {ongetUserIDSucceed(data);},  
	        //error: function(jqXHR, textStatus, errorThrown){alert(textStatus);alert(errorThrown);alert(jqXHR.responseText);}
			error: function (data) {ongetUserIDFailure(data);}    
	    }); 
}
function ongetUserIDFailure(data) {
	alert("Failure: " + JSON.stringify(data));
}
function ongetUserIDSucceed(data){
	if (data.d.results[0].SiteSupervisor.Id===""){
		alert("Please select a valid user");
	}else{
		insertTaskItemsData(data.d.results[0].SiteSupervisor.Id);
	}
}
//Get User ID from SiteSupervisor List : End

//Get Task List Items from TaskLookup: Begin
function insertTaskItemsData(siteSupervisorId) {
//https://enterpriseuser.sharepoint.com/sites/worksite/_api/web/lists/GetByTitle('TaskLookup')/items?
//$select=Title,TaskCategoryLookup/TaskCategoryLookup&$expand=TaskCategoryLookup&
//$filter=TaskCategoryLookup/TaskCategoryLookup eq 'House Type 1'
	    $.ajax({
			url : appWebUrl + "/_api/SP.AppContextSite(@target)/web/lists/getbytitle('TaskLookup')/items?"+
						"$select=Description,StartDate,EndDate,DetailsOfTask,TaskCategoryLookup/TaskCategoryLookup&"+
						"$expand=TaskCategoryLookup&"+
						//"$filter=TaskCategoryLookup/TaskCategoryLookup eq '"+document.getElementById("myCategorySelect").value+"' and Active eq 1&"+
						"$filter=TaskCategoryLookup/TaskCategoryLookup eq '"+document.getElementById("myCategorySelect").value+"'&"+						
						"@target='" + hostWebUrl + "'", 
	        method: "GET",  
	        headers: { "Accept": "application/json; odata=verbose","content-type" : "application/json;odata=verbose" },  
	        success: function (data) {oninsertTaskItemsDataSucceed(siteSupervisorId, data);},  
	        //error: function(jqXHR, textStatus, errorThrown){alert(textStatus);alert(errorThrown);alert(jqXHR.responseText);}
			error: function (data) {oninsertTaskItemsDataFailure(data);}    
	    }); 
}
function oninsertTaskItemsDataFailure(data) {
	alert("Failure: " + JSON.stringify(data));
}
function oninsertTaskItemsDataSucceed(siteSupervisorId, data){
	var dataResult = data.d.results;
	var i;
	for(i=0;i<dataResult.length;i++)
	{						
		createlistItems(dataResult[i].Description, dataResult[i].DetailsOfTask, dataResult[i].EndDate, siteSupervisorId);	 
	}	
}
//Get Task List Items from TaskLookup: End

//Create Task items : Begin
function createlistItems(title, description, date, siteSupervisorId) {
	var listTitle = document.getElementById("mySelect").value;
	
	var oList = hostweb.get_lists().getByTitle(listTitle);
	var itemCreateInfo = new SP.ListItemCreationInformation();
    var oListItem = oList.addItem(itemCreateInfo);

    oListItem.set_item('Title', title);
	oListItem.set_item('Body', description);
	oListItem.set_item('DueDate', new Date(date.substring(0, 10)));
	
	//User Field : Begin
	var assignedToVal = new SP.FieldUserValue();
	assignedToVal.set_lookupId(siteSupervisorId);
	//assignedToVal.set_lookupId(10);
	oListItem.set_item('AssignedTo', assignedToVal);
	//User Field : Begin
		
    oListItem.update();
    currentcontext.load(oListItem);
    currentcontext.executeQueryAsync(onListItemsCreationSuccess, onLisItemstCreationFail);
}
function onListItemsCreationSuccess() {
    alert('Tasks created successfully!');		
	
	//var mySelectIndex = document.getElementById("mySelect").selectedIndex;
	//var x = document.getElementById("mySelect");
	//x.remove(mySelectIndex);
	
	//Reseting both select elements and text box : Begin
	var mySelect = document.getElementById("mySelect");
	mySelect.options[0].selected = true;
	
	var myCategorySelect = document.getElementById("myCategorySelect");
	myCategorySelect.options[0].selected = true;
	
	var mySupervisorSelect = document.getElementById("mySupervisorSelect");
	mySupervisorSelect.options[0].selected = true;	
	
	document.getElementById("text2").value = document.getElementById("mySelect").value;	
	//Reseting both select elements and text box : End
}
function onLisItemstCreationFail(sender, args) {
    alert('Failed to create the tasks. ' + args.get_message());
}
//Create Task items : End

//Get App URL : Begin
function getQueryStringParameter(paramToRetrieve) {     
    var params =  document.URL.split("?")[1].split("&");       
    for (var i = 0; i < params.length; i = i + 1) {     
        var singleParam = params[i].split("=");     
        if (singleParam[0] == paramToRetrieve)     
        return singleParam[1];     
        }     
} 
//Get App URL : End

})();