//Internet Explorer support
if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, position) {
        position = position || 0;
        return this.indexOf(searchString, position) === position;
    };
}

function objectifyForm(formArray, first, last) {
    //serialize data function
    var returnArray = {};
    for (var i = first; i < last; i++) {
        returnArray[formArray[i]['name']] = formArray[i]['value'];
    }
    return returnArray;
}

function getParamValue(param) {
    var pageUrl = window.location.search.substring(1);
    var variables = pageUrl.split('&');
    for (var i = 0; i < variables.length; i++){
        var parameter = variables[i].split('=');
        if (parameter[0] == param)
        {
            return parameter[1];
        }
    }
}

$(function () {
    $("#logoutBtn").click(function () {
        $.ajax({
            url: '/api/actions/logout',
            method: 'post',
            contentType: 'application/json',
            success: function (res) {
                var obj = JSON.parse(res);
                if (obj.Success) {
                    window.location.replace("/");
                }
                else {
                    toastr.error(obj.Message);
                }
            },
            error: function (res) {
                toastr.error("Coś poszło nie tak. Sprobuj ponownie za kilka minut")
            }
        });
    })
})


