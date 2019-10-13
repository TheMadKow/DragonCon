function ChangedEventDay(caller) {
    var selectedDay = caller;
    var value = selectedDay.options[selectedDay.selectedIndex].value;
    var selectList = document.getElementById("template-duration-" + value);
    var clone = $(selectList).clone();
    
    clone.addClass("chosen-select");
    
    var startTimeSelect = $("#event-duration");
    startTimeSelect.html(clone);
    
    setupChosen();
}

function SetupEventDay() {
    var value = $("#event-start-time").val();
    var selectList = document.getElementById("template-duration-" + value);
    var clone = $(selectList).clone();
    
    clone.addClass("chosen-select");
    
    var startTimeSelect = $("#event-duration");
    startTimeSelect.html(clone);
    
    setupChosen();
}


function ViewEventHistory(eventId) {
    AjaxModal('#event-history-ajax', `/Management/Events/ViewEventHistory?eventId=${eventId}`);
}