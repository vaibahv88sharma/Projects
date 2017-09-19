'use strict';


//region List CRUD Operations
function listAllCategories() {

    var ctx = new SP.ClientContext(appWebUrl);
    var appCtxSite = new SP.AppContextSite(ctx, hostWebUrl);

    var web = appCtxSite.get_web(); //Get the Web 

    var list = web.get_lists().getByTitle("CategoryList"); //Get the List

    var query = new SP.CamlQuery(); //The Query object. This is used to query for data in the List

    query.set_viewXml('<View><RowLimit></RowLimit>10</View>');

    var items = list.getItems(query);

    ctx.load(list); //Retrieves the properties of a client object from the server.
    ctx.load(items);

    var table = $("#tblcategories");
    var innerHtml = "<tr><td>ID</td><td>Category Id</td><td>Category Name</td></tr>";

    //Execute the Query Asynchronously
    ctx.executeQueryAsync(
        Function.createDelegate(this, function () {
            var itemInfo = '';
            var enumerator = items.getEnumerator();
            while (enumerator.moveNext()) {
                var currentListItem = enumerator.get_current();
                innerHtml += "<tr><td>" + currentListItem.get_item('ID') + "</td><td>" + currentListItem.get_item('Title') + "</td><td>" + currentListItem.get_item('CategoryName') + "</td></tr>";
            }
            table.html(innerHtml);
        }),
        Function.createDelegate(this, fail)
        );
}

function createCategory() {
    var ctx = new SP.ClientContext(appWebUrl);//Get the SharePoint Context object based upon the URL
    var appCtxSite = new SP.AppContextSite(ctx, hostWebUrl);

    var web = appCtxSite.get_web(); //Get the Site 

    var list = web.get_lists().getByTitle("CategoryList"); //Get the List based upon the Title
    var listCreationInformation = new SP.ListItemCreationInformation(); //Object for creating Item in the List
    var listItem = list.addItem(listCreationInformation);

    listItem.set_item("Title", $("#CategoryId").val());
    listItem.set_item("CategoryName", $("#CategoryName").val());
    listItem.update(); //Update the List Item

    ctx.load(listItem);
    //Execute the batch Asynchronously
    ctx.executeQueryAsync(
        Function.createDelegate(this, success),
        Function.createDelegate(this, fail)
       );
}

function findListItem() {

    listItemId = prompt("Enter the Id to be Searched ");
    var ctx = new SP.ClientContext(appWebUrl);
    var appCtxSite = new SP.AppContextSite(ctx, hostWebUrl);

    var web = appCtxSite.get_web();

    var list = web.get_lists().getByTitle("CategoryList");

    ctx.load(list);

    listItemToUpdate = list.getItemById(listItemId);

    ctx.load(listItemToUpdate);

    ctx.executeQueryAsync(
        Function.createDelegate(this, function () {
            //Display the Data into the TextBoxes
            $("#CategoryId").val(listItemToUpdate.get_item('Title'));
            $("#CategoryName").val(listItemToUpdate.get_item('CategoryName'));
        }),
        Function.createDelegate(this, fail)
        );


}

function updateItem() {
    listItemId = prompt("Enter the Id to be Searched ");

    var ctx = new SP.ClientContext(appWebUrl);
    var appCtxSite = new SP.AppContextSite(ctx, hostWebUrl);

    var web = appCtxSite.get_web();

    var list = web.get_lists().getByTitle("CategoryList");
    ctx.load(list);

    listItemToUpdate = list.getItemById(listItemId);

    ctx.load(listItemToUpdate);

    listItemToUpdate.set_item('CategoryName', $("#CategoryName").val());
    listItemToUpdate.update();

    ctx.executeQueryAsync(
        Function.createDelegate(this, success),
        Function.createDelegate(this, fail)
        );

}

function deleteListItem() {
    listItemId = prompt("Enter the Id to be Searched ");

    var ctx = new SP.ClientContext(appWebUrl);
    var appCtxSite = new SP.AppContextSite(ctx, hostWebUrl);

    var web = appCtxSite.get_web();

    var list = web.get_lists().getByTitle("CategoryList");
    ctx.load(list);

    listItemToUpdate = list.getItemById(listItemId);

    ctx.load(listItemToUpdate);

    listItemToUpdate.deleteObject();

    ctx.executeQueryAsync(
        Function.createDelegate(this, success),
        Function.createDelegate(this, fail)
        );
}


//endregion List CRUD Operations

