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