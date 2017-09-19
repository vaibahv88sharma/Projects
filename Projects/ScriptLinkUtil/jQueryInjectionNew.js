var collListItem;

$( document ).ready(function() {
//window.onload = function () {


    var hostweburl = _spPageContextInfo.webAbsoluteUrl;
    var scriptbase = hostweburl + "/_catalogs/masterpage/Watersun/";
    $.getScript(scriptbase + "jquery-2.2.4.js",
        execOperation
    );


//}
});


function execOperation() {
    //alert("reached execOperation");	 

    if ($("[name='CalledBest']").length) {

        //$("[id='scriptWPQ1']").css(
        //                       //     "border", "4px solid black"
        //                            );
        $("[id='scriptWPQ1']").addClass('callForward1');

        $("td:nth-child(5)").css("color", "red");
        $("td:nth-child(7)").addClass('columns');
        $(".ms-cellstyle.ms-vb2").addClass('columnsBorder');
        //$( 'span.ms-noWrap' )
    }

    if ($("[name='SupName']").length) {
        if ($("[name='Duration']").length) {
            if ($("[name='CC']").length) {
                //_spPageContextInfo.webAbsoluteUrl + "/lists/" + $("a[title*='_Data']").attr("title")
                //$("a[title*='_Data']").attr("title").split("_")[0]

                var fullUrl = "/_api/web/Lists/GetByTitle('Call Forwards')/Items?$select=JobAddr&$filter=Title eq " + $("a[title*='_Data']").attr("title").split("_")[0];

                $.ajax({
                    url: _spPageContextInfo.webAbsoluteUrl + fullUrl,//"/lists/" + $("a[title*='_Data']").attr("title"),
                    type: "GET",
                    headers: {
                        "accept": "application/json;odata=verbose",
                    },
                    success: function (data) {
                        console.log(data.d.results);
                        console.log(data.d.results[0].JobAddr);
                        //$("a[title*='_Data']").text(data.d.results[0].JobAddr);
                        ////////////////$("a[title*='5921_Data']").val("Replacement text");


                    },
                    error: function (error) {
                        alert(JSON.stringify(error));
                    }
                });
            }
        }
    }


}

