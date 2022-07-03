$(function () {
    $('#loginForm').submit(function (e) {
        e.preventDefault();
        var a = $('#loginForm').serializeArray();
        var d = JSON.stringify(objectifyForm(a, 0, a.length - 1));
        console.log(d);
        $.ajax({
            url: '/api/actions/login',
            method: 'post',
            contentType: 'application/json',
            data: d,
            success: function (res) {
                var obj = JSON.parse(res);
                if (obj.Success) {
                    var returnUrl = getParamValue('ReturnUrl');
                    returnUrl = returnUrl.replace(/%2F/g, '/')
                    if (returnUrl.startsWith("/User/")) {
                        returnUrl = "/";
                    }
                    window.location.replace(returnUrl)
                }
                else {
                    toastr.error(obj.Message);
                }
            },
            error: function (res) {
                console.log(res);
                toastr.error("Coś poszło nie tak. Sprobuj ponownie za kilka minut")
            }
        });
    });
});