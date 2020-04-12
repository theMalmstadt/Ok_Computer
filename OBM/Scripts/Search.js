console.log("hihihihihi");

var search = $("#searchterm").val();
var myTable; 
drawEvents()
function drawEvents()
{   
    var result = $.ajax({
        type: 'GET',
        dataType: "json",
        contentType:"application/json",
        url: '/Events/PublicEvents',
        error: errorOnAjax,
        success: function (response) {
            drawTable(response);
        }
    });
    
}
function drawTable(data) {
    console.log(data);
    
myTable=$('#searchResults').dataTable({
   "data": data,
   "columns": [
       { "data": "EventName" },
       { "data": "Location" },
      { 
         "data": "url",
         "render": function(data, type, row, meta){
            if(type === 'display'){
                data = '<a href="' + data + '">' + data + '</a>';
            }
            
            return data;
         }
      } 
   ]
});
    myTable.fnFilter(search);

    
}


function errorOnAjax()
{
    console.log("error on search load ajax");
}