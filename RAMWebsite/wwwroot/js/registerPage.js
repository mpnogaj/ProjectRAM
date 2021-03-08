$(function () {
    $('#registerForm').submit(function (e) {
        e.preventDefault();
        var a = $('#registerForm').serializeArray();
        var d = JSON.stringify(objectifyForm(a, 0, a.length - 1));
        $.ajax({
            url: '/api/actions/register',
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