$(".toggle-content-btn").click(function () {
    var $this = $(this);
    var sectionId = "#" + $this.data("section");
    var $section = $(sectionId);

    $(".toggle-content:not(" + sectionId + ")").hide(300);

    $section.toggle(300);
});
