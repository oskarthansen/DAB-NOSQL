// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



$('#myInput').change(function () {
    var url = "/User/GetUsers";
    var name = $('#leve').val();
    $.get(url, { parameter: name }, function (data) {
        alert(data);
    });


});