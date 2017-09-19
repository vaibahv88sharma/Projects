'use strict';

var context = SP.ClientContext.get_current();
var user = context.get_web().get_currentUser();

(function () {

    // This code runs when the DOM is ready and creates a context object which is 
    // needed to use the SharePoint object model
    $(document).ready(function () {        
        getUserName();

        listAllCategories();

        $("#btn-new").on('click', function () {
            $(".c1").val('');
        });

        $("#btn-add").on('click', function () {
            createCategory();
            listAllCategories();
        });

        $("#btn-update").on('click', function () {
            updateItem();
            listAllCategories();
        });

        $("#btn-find").on('click', function () {
            findListItem();
            listAllCategories();
        });


        $("#btn-delete").on('click', function () {
            deleteListItem();
            listAllCategories();
        });

        $("#btn-findNewList").on('click', function () {
            exceptionHandlingDemo();
        })
    });

    // This function prepares, loads, and then executes a SharePoint query to get 
    // the current users information
    function getUserName() {
        context.load(user);
        context.executeQueryAsync(onGetUserNameSuccess, onGetUserNameFail);
    }

    // This function is executed if the above call is successful
    // It replaces the contents of the 'message' element with the user name
    function onGetUserNameSuccess() {
        $('#message').text('Hello ' + user.get_title());
    }

    // This function is executed if the above call fails
    function onGetUserNameFail(sender, args) {
        alert('Failed to get user name. Error:' + args.get_message());
    }

    //region Exception Handling
    
    //function exceptionHandlingDemo() {
    //    var listName = $("#CategoryIdofNewList").val();
    //    var template = SP.ListTemplateType.genericList;
    //    var desc = "Created Prog to demonstrate JSOM error handling";

    //    var ctx = new SP.ClientContext(appWebUrl);
    //    var appCtxSite = new SP.AppContextSite(ctx, hostWebUrl);

    //    var web = appCtxSite.get_web();

    //    errScope = new SP.ExceptionHandlingScope(appCtxSite);
    //    var scopeStart = errScope.startScope();

    //    var tryBlock = errScope.startTry();
    //    var theList = web.get_lists().getByTitle(listName);
    //    //var theList = appCtxSite.get_web().get_lists().getByTitle(listName);
    //    tryBlock.dispose();

    //    var catchBlock = errScope.startCatch();
    //    var listCI = new SP.ListCreationInformation();
    //    listCI.set_title(listName);
    //    listCI.set_templateType(template);
    //    listCI.set_description(desc);
    //    theList = web.get_lists().add(listCI);
    //    //theList = appCtxSite.get_web().get_lists().add(listCI);
    //    catchBlock.dispose();

    //    var finallyBlock = errScope.startFinally();
    //    ctx.load(theList);
    //    finallyBlock.dispose();

    //    scopeStart.dispose();

    //    ctx.executeQueryAsync(
    //        Function.createDelegate(this, success),
    //        Function.createDelegate(this, fail)
    //        );
    //}
    //endregion Exception Handling
})();