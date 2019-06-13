function ToggleUnlimitedTicket(selector) {
    var number = $(selector).closest(".row").find(".update-events-number");
    var isOn = number.prop('disabled');
    if (isOn === true) {
        number.prop('disabled', false);
    } else {
        number.prop('disabled', true);
        number.val(null);
    }

}

function RemoveDay(selector) {
    $(selector).closest(".row").find(".update-deleted").attr('value','true');
    $(selector).closest(".row").hide();
}

function AddNewDay() {
    var $lastDayTemplate = $("#last-day-template");
    var $lastDayCounter = parseInt($lastDayTemplate.data("counter")) +1;
    $lastDayTemplate.data("counter", $lastDayCounter.toString());

    var clone = $lastDayTemplate.clone();
    clone.attr("id", "");
    clone.prop("hidden", "");

    clone.find(".update-date").attr("name", `Days[${$lastDayCounter}].Date`);
    clone.find(".update-from").attr("name", `Days[${$lastDayCounter}].From`);
    clone.find(".update-to").attr("name", `Days[${$lastDayCounter}].To`);
    clone.find(".update-deleted").attr("name", `Days[${$lastDayCounter}].IsDeleted`);
    clone.find("a").click(function () { RemoveDay(this); });

    $(".last-day-row").last().after(clone);
    setupPickers();
}


function RemoveHall(selector) {
    $(selector).closest(".row").find(".update-deleted").attr('value','true');
    $(selector).closest(".row").hide();
}

function AddNewHall() {
    var lastHallTemplate = $("#last-hall-template");
    var $lastHallCounter = parseInt(lastHallTemplate.data("counter")) +1;
    lastHallTemplate.data("counter", $lastHallCounter.toString());

    var clone = lastHallTemplate.clone();
    clone.attr("id", "");
    clone.prop("hidden", "");

    clone.find(".update-name").attr("name", `Halls[${$lastHallCounter}].Name`);
    clone.find(".update-desc").attr("name", `Halls[${$lastHallCounter}].Description`);
    clone.find(".update-first").attr("name", `Halls[${$lastHallCounter}].FirstTable`);
    clone.find(".update-last").attr("name", `Halls[${$lastHallCounter}].LastTable`);
    clone.find(".update-deleted").attr("name", `Halls[${$lastHallCounter}].IsDeleted`);
    clone.find("a").click(function () { RemoveDay(this); });

    $(".last-hall-row").last().after(clone);
}