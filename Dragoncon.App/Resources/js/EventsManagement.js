function AddNewSubActivity() {
    var $lastSystemTemplate = $("#last-system-template");
    var $lastSystemCounter = parseInt($lastSystemTemplate.data("counter")) +1;
    $lastSystemTemplate.data("counter", $lastSystemCounter.toString());

    var clone = $lastSystemTemplate.clone();
    clone.attr("id", "");
    clone.prop("hidden", "");

    clone.find(".update-system").attr("name", `SubActivities[${$lastSystemCounter}].Name`);
    clone.find("a").click(function () { RemoveRow(this); });

    $(".last-system-row").last().after(clone);
}


function ToggleEventSpecialPrice() {
    var toggle = $("#is-special-price");
    var isOn = toggle.prop('disabled');
    if (isOn === true) {
        toggle.prop('disabled', false);
    } else {
        toggle.prop('disabled', true);
        toggle.val(null);
    }
}


function QuickEventUpdate(jsonData) {
    var data = JSON.parse(jsonData);
    var $modal = $("#quick-event-update-modal");
    $("#quick-event-update-id").val(data.Id);
    $("#quick-event-update-name").html(data.Name);

    $("#quick-event-update-status").val(data.Status);
    $('#quick-event-update-status').change();
    $("#quick-event-update-status").trigger("chosen:updated");

    $("#quick-event-update-location").val(data.Location);
    $('#quick-event-update-location').change();
    $("#quick-event-update-location").trigger("chosen:updated");

    $modal.modal();
}


function ViewEventHistory(eventId) {
    AjaxModal('#event-history-ajax', `/Management/Events/ViewEventHistory?eventId=${eventId}`);
}

