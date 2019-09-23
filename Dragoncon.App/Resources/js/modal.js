function AjaxModal(modalId, url) {
    $("#modal-placeholder").show(150);
    $.ajax({
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        url: url,
        success: function (data) {
            $(modalId).html(data);
            $(modalId).modal({ backdrop: 'static', keyboard: false });
            $("#modal-placeholder").hide();
        },
        error: function (xhr, textStatus, error) {
            Swal.fire(
                'תקלה',
                'כשלון בשליפת המידע',
                'warning'
            );
            $("#modal-placeholder").hide();
        }
    });
}