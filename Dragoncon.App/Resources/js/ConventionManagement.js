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
}