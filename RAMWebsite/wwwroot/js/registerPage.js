$('#username-input').focusout(function () {
    var username = $('#username-input').val();
    alert('test');
    $.ajax({
        type: 'GET',
        url: '/api/actions/duplicate/' + username,
        success: function (response) {
            alert(response);
            //duplikat
            if (response == true) {
                alert('dobrze');
            }
            else {
                alert('zle');
            }
        }
    });
});