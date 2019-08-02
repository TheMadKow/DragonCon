function ChangedEventDay(caller) {
    var selectedDay = caller;
    var value = selectedDay.options[selectedDay.selectedIndex].value;
    var selectList = document.getElementById("template-starttime-" + value);
    var clone = $(selectList).clone();
    
    clone.addClass("chosen-select");
    
    var startTimeSelect = $("#event-start-time");
    startTimeSelect.html(clone);
    
    setupChosen();
}

function ChangedEventStartTime(dayId, caller) {
    var selectTime = caller;
    var value = selectTime.options[selectTime.selectedIndex].value;
    var selectList = document.getElementById("template-duration-" + dayId + "-" + value);
    var clone = $(selectList).clone();
    
    clone.addClass("chosen-select");
    
    var durationSelect = $("#event-duration");
    durationSelect.html(clone);
    
    setupChosen();

}