function objectifyForm(formArray, first, last) {
    //serialize data function
    var returnArray = {};
    for (var i = first; i < last; i++) {
        returnArray[formArray[i]['name']] = formArray[i]['value'];
    }
    return returnArray;
}

$(function () {
    $('#registerForm').submit(function (e) {
        e.preventDefault();
        var a = $('#registerForm').serializeArray();
        console.log(a.length - 1);
        var d = JSON.stringify(objectifyForm(a, 0, a.length - 1));
        console.log(d);
        console.log(typeof (d));
        $.ajax({
            url: 'https://localhost:5001/api/actions/register',
            method: 'post',
            contentType: 'application/json',
            data: d,
            success: function (res) {
                alert(res);
            },
            error: function (res) {
                alert(res);
            }
        });
    });
});