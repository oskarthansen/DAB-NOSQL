﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).click(function (event) {
    deleteItems();
});

var toggleMenu = false;


$('#myInput').click(function (e) {
    e.stopPropagation();
    if (!toggleMenu) {
        $("#myUL").show();
        var url = "/User/GetUsersJson";

        $.get(url,
            function(users) {
                users.forEach(function(username) {
                    var li = document.createElement("LI");
                    var a = document.createElement('a');
                    a.href = "/User/ShowWall/" + username.id;
                    var linkText = document.createTextNode(username.name);
                    a.appendChild(linkText);
                    li.append(a);
                    document.getElementById("myUL").appendChild(li);
                });
            });
        toggleMenu = true;
    } else {
        deleteItems();
    }
});

function myFunction() {
    // Declare variables
    var input, filter, ul, li, a, i, txtValue;
    input = document.getElementById('myInput');
    filter = input.value.toUpperCase();
    ul = document.getElementById("myUL");
    li = ul.getElementsByTagName('li');

    // Loop through all list items, and hide those who don't match the search query
    for (i = 0; i < li.length; i++) {
        a = li[i].getElementsByTagName("a")[0];
        txtValue = a.textContent || a.innerText;
        if (txtValue.toUpperCase().indexOf(filter) > -1) {
            li[i].style.display = "";
        } else {
            li[i].style.display = "none";
        }
    }
}

function deleteItems() {
    $("#myUL").hide();
    $("#myUL").empty();
    toggleMenu = false;
}


function deleteNode(e) {
    console.log(e.parentNode.parentNode);
    e.parentNode.parentNode.removeChild(e.parentNode);
}


document.getElementById('add').addEventListener("click", addInput);

function addInput() {
    var demo = document.getElementById('inputElements');
    var demoChildren = demo.children;
    var id = parseInt(demoChildren[demoChildren.length - 1].children[0].id) + 1;

    var input1 = '<div><input name="Users[' +
        id +  
        ']" id="' + id +
        '" class="form-control inputForm" value type="text" /><p style="margin-bottom: 15px !important; cursor: pointer; color: #007bff; " onclick="deleteNode(this)">Remove</p></div>';

    $(demo).append(input1);

}


