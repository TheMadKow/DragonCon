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