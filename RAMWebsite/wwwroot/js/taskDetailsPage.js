function getData() {
    $.ajax({
        url: '/api/values/user_id',
        method: 'get',
        dataType: 'json',
        success: function (res) {
            let userId = res;
            let pathname = window.location.pathname;
            let taskId = pathname.split("/")[3]
            loadSubmissions(userId, taskId);
        },
        error: function (res) {
            console.log(res);
        }
    });
}

function loadSubmissions(userId, taskId) {
    $.ajax({
        url: '/api/values/reports/' + userId + '/' + taskId,
        method: 'get',
        dataType: 'json',
        success: function (res) {
            const template = $('#submissionRow').children();
            const table = $('#submissions');
            res.forEach(function (element) {
                let newRow = template.clone(true);
                let date = element.SubmitionDate.substring(0, 19).replace('T', ' ');
                console.log(date);
                console.log(element);
                newRow.children().eq(0).html(date);
                if (element.Passed === true) {
                    newRow.children().eq(1).html("Zaliczony");
                }
                else {
                    newRow.children().eq(1).html("Niezaliczony");
                }
                const downloadFile = "/Download/DownloadItem?fileName=" + element.Id;
                newRow.children().eq(2).children().eq(0).attr("href", downloadFile + "&open=true");
                newRow.children().eq(2).children().eq(1).attr("href", downloadFile);
                newRow.children().eq(3).children().attr("href", "/Tasks/Report/" + element.Id);
                table.append(newRow);
            });
        }
    });
}

getData();