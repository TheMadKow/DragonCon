function ToggleSideMenu() {
    var $navbar = $(".admin-navbar");   
    var $sidebarToggle = $("#sidebar-toggle");
    var $container = $(".container-admin");

    if ($navbar.hasClass("d-none")) {
        $navbar.removeClass("d-none");
        $navbar.removeClass("d-md-block");
    } else {
        $navbar.addClass("d-none");
        $navbar.addClass("d-md-block");
    }
}

/* General */
function RemoveRow(selector) {
    $(selector).closest(".row").find(".update-deleted").attr('value','true');
    $(selector).closest(".row").hide();
}

/* Tickets */
function ToggleLimitedTicket(rowNumber) {
    var number = $(".update-events-number-" + rowNumber);
    var isOn = number.prop('disabled');
    if (isOn === true) {
        number.prop('disabled', false);
        number.trigger('chosen:updated');
    } else {
        number.prop('disabled', true);
        number.trigger('chosen:updated');
    }
}

function RemoveTicket(selector) {
    $(selector).closest(".ticket-container").find(".update-deleted").attr('value','true');
    $(selector).closest(".ticket-container").hide();
}


function AddNewTicket() {
    var $lastTicketTemplate = $("#last-ticket-template");
    var $lastTicketCount = parseInt($lastTicketTemplate.data("counter")) + 1;
    $lastTicketTemplate.data("counter", $lastTicketCount.toString());

    var clone = $lastTicketTemplate.clone();
    clone.attr("id", "");
    clone.prop("hidden", "");

    clone.find(".update-deleted").attr("name", `Tickets[${$lastTicketCount}].IsDeleted`);
 
    clone.find(".update-name").attr("name", `Tickets[${$lastTicketCount}].Name`);
    clone.find(".update-price").attr("name", `Tickets[${$lastTicketCount}].Price`);
    clone.find(".update-transaction").attr("name", `Tickets[${$lastTicketCount}].TransactionCode`);
    clone.find(".update-ticket-type").attr("name", `Tickets[${$lastTicketCount}].TicketType`);
    clone.find(".update-ticket-type").addClass("chosen-select");

    clone.find(".update-unlimited").attr("name", `Tickets[${$lastTicketCount}].IsUnlimited`);
    clone.find(".update-unlimited").attr("onchange", `ToggleLimitedTicket(${+$lastTicketCount})`);
    clone.find(".update-unlimited").addClass("bootstrap-toggle");
    clone.find(".update-unlimited").data("toggle", "toggle");

    clone.find(".update-num-of-activities").attr("name", `Tickets[${$lastTicketCount}].NumOfActivities`);
    clone.find(".update-num-of-activities").addClass("update-events-number-" + $lastTicketCount);
    clone.find(".update-num-of-activities").addClass("chosen-select");


    clone.find(".update-days").attr("name", `Tickets[${$lastTicketCount}].Days`);
    clone.find(".update-days").addClass("chosen-select");

    clone.find(".ticket-removal").click(function() { RemoveTicket(this); });

    $(".last-ticket-row").last().after(clone);
    setupToggles();
    setupChosen();
}


/* Days */
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
    clone.find("a").click(function () { RemoveRow(this); });

    $(".last-day-row").last().after(clone);
    setupPickers();
}


/* Halls */
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
    clone.find("a").click(function () { RemoveRow(this); });

    $(".last-hall-row").last().after(clone);
}