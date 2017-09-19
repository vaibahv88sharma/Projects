'use strict';

(function() {

	// This code runs when the DOM is ready and creates a context object which is 
	// needed to use the SharePoint object model
	

	
	$(document).ready(function() {

		getUserName();
		getJobItems();
		getSupervisor();
		getTaskCategoryLookupData();
		$("#btn-new").on('click', function() {
			createlist();
		});
		$("#btn-new-items").on('click', function() {
			//insertTaskItemsData();			
			//getUserID(document.getElementById("mySupervisorSelect").value);
			insertTaskItemsData(document.getElementById("mySupervisorSelect").value)
		});
	});


	//Public Variables: Begin
	var context = SP.ClientContext.get_current();
	var user = context.get_web().get_currentUser();
	var hostWebUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
	var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));

	var currentcontext = new SP.ClientContext.get_current();
	var hostcontext = new SP.AppContextSite(currentcontext, hostWebUrl);
	var hostweb = hostcontext.get_web();
	var workflowDefinitionId = '12642fda-acd1-4c06-938b-4494419bcf0d';
    var listGuid = 'PUT-YOUR-GUID-HERE';	
	var historyList = '86E90715-0B4C-4DB6-8AD1-7A062C81ECF3';
	var tasksList   = '003EF4D6-957E-4B85-8C89-FD0A0E4390C2';
	//Public Variables: End
	
	//Create Task List : Begin
	var siteCxt;
	var rr;
	var flag = 0;
	var item;
	var en;
	function createlist() {
		var siteCxt = hostcontext.get_site();
		currentcontext.load(siteCxt);
		//var tpl = siteCxt.getCustomListTemplates(hostweb);
		//var result = siteCxt.getCustomListTemplates(hostweb);
		//currentcontext.load(result);
		currentcontext.executeQueryAsync(
			function (sender, args) {
				alert("success 1");	
				currentcontext.load(hostweb);	
					currentcontext.executeQueryAsync(
					function (sender, args) {
						alert("success 2");	
						rr = siteCxt.getCustomListTemplates(hostweb) ;
						currentcontext.load(rr);	
							currentcontext.executeQueryAsync(
							function (data) {
								alert("success 3");
								    en = rr.getEnumerator();
									//var flag = 0;
									//var item;
								    while (en.moveNext())
								    {
								        item = en.get_current();
										//alert(item.get_name());
								        if (item.get_name() === 'DemoTaskTemplate')
								        {
											debugger;
											alert("found");
											flag  = 1;
											createTaskList();
								        }				
										else{ flag  = 0;}						
								    }
					        },
							function (sender, args) 
							{
								alert("Failed to load 3");
							}	
						);
			        },
					function (sender, args) 
					{
						alert("Failed to load 2");
					}	
				);	
	        },
			function (sender, args) 
			{
				alert("Failed to load 1");
			}	
		);			
	}	
	function createTaskList() {
										//if(flag === 1){
										debugger;
											var listCreationInfo = new SP.ListCreationInformation();
											listCreationInfo.set_title("New List1");
											listCreationInfo.set_templateType(171);//item
											listCreationInfo.set_templateFeatureId('F9CE21F8-F437-4F7E-8BC6-946378C850F0');
											var list = hostweb.get_lists().add(listCreationInfo);
											currentcontext.load(list);																			
											currentcontext.executeQueryAsync(function (sender, args){alert("success");},function (sender, args){alert(args.get_message());});										
									//}
	}

	function getQueryStringParameter(paramToRetrieve) {
		var params = document.URL.split("?")[1].split("&");
		for (var i = 0; i < params.length; i = i + 1) {
			var singleParam = params[i].split("=");
			if (singleParam[0] == paramToRetrieve) return singleParam[1];
		}
	}
	//Get App URL : End
})();