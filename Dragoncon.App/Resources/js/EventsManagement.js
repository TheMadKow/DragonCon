function AddNewSystem() {
    var $lastSystemTemplate = $("#last-system-template");
    var $lastSystemCounter = parseInt($lastSystemTemplate.data("counter")) +1;
    $lastSystemTemplate.data("counter", $lastSystemCounter.toString());

    var clone = $lastSystemTemplate.clone();
    clone.attr("id", "");
    clone.prop("hidden", "");

    clone.find(".update-system").attr("name", `Systems[${$lastSystemCounter}].Name`);
    clone.find("a").click(function () { RemoveRow(this); });

    $(".last-system-row").last().after(clone);
}