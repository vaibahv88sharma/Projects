'use strict';


//region GlobalVar
var errScope;
//endregion GlobalVar

//region Exception Handling

function exceptionHandlingDemo() {
    var listName = $("#CategoryIdofNewList").val();
    var template = SP.ListTemplateType.genericList;
    var desc = "Created Prog to demonstrate JSOM error handling";

    var ctx = new SP.ClientContext(appWebUrl);
    var appCtxSite = new SP.AppContextSite(ctx, hostWebUrl);

    var web = appCtxSite.get_web();

    errScope = new SP.ExceptionHandlingScope(ctx);
    //errScope = new SP.ExceptionHandlingScope(appCtxSite);
    var scopeStart = errScope.startScope();

    var tryBlock = errScope.startTry();
    var theList = web.get_lists().getByTitle(listName);
    //var theList = appCtxSite.get_web().get_lists().getByTitle(listName);
    tryBlock.dispose();

    var catchBlock = errScope.startCatch();
    var listCI = new SP.ListCreationInformation();
    listCI.set_title(listName);
    listCI.set_templateType(template);
    listCI.set_description(desc);
    theList = web.get_lists().add(listCI);
    //theList = appCtxSite.get_web().get_lists().add(listCI);
    catchBlock.dispose();

    var finallyBlock = errScope.startFinally();
    ctx.load(theList);
    finallyBlock.dispose();

    scopeStart.dispose();

    ctx.executeQueryAsync(
        Function.createDelegate(this, success),
        Function.createDelegate(this, fail)
        );
}
//endregion Exception Handling