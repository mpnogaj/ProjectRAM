$(function () {
    $('#loginForm').submit(function (e) {
        e.preventDefault();
        var a = $('#loginForm').serializeArray();
        var d = JSON.stringify(objectifyForm(a, 0, a.length - 1));
        $.ajax({
            url: '/api/actions/login',
            method: 'post',
            contentType: 'application/json',
            data: d,
            success: function (res) {
                var obj = JSON.parse(res);
                if (obj.Success) {
                    var returnUrl = getParamValue('ReturnUrl');
                    returnUrl = returnUrl.replaceAll('%2F', '/')
                    toastr.options.onHidden = function () { window.location.replace(returnUrl) }
                    toastr.success(obj.Message);
                }
                else {
                    toastr.error(obj.Message);
                }
            },
            error: function (res) {
                toastr.error("Coś poszło nie tak. Sprobuj ponownie za kilka minut")
            }
        });
    });
});