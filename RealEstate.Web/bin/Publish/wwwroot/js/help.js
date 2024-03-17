$(document).ready(function () {

    //if (localStorage.isHelpVisible == 'true') {
    //    Close();
    //}
    //else {
    //    Open();
    //}
    $(window).scroll(function () {
        $("#right-sidebar").css("margin-top", Math.max(0, $(this).scrollTop() - 112));
    });

    $(".help-sidebar").click(function () {
        var event = $(this);
        var title = $(this).attr("data-title");
        var description = $(this).attr("data-description");
        HelpSideBar(title, description);
    });

    //Function that fill help right side bar
    function HelpSideBar(title, description) {
        //debugger;
        $("#title").text(title);
        $("#description").text(description);
    }

    $("#close").click(function () {
        if ($("#right-sidebar").is(':visible')) {
            Open();
        }
        else {
            Close();
        }
    });

    function Open() {

        var option = { direction: 'right' };
        $("#right-sidebar").hide();
        $("#left-side-bar").removeClass("col-md-9");
        $("#left-side-bar").addClass("col-md-12");

        $("#icon-expand").removeClass("fa-chevron-right");
        $("#icon-expand").addClass("fa-chevron-left");
        $("#expand").html("@SharedLocalizer["OpenHelp"]");

        localStorage.isHelpVisible = false;
    }
    function Close() {
        //debugger;
        $("#left-side-bar").removeClass("col-md-12");
        $("#left-side-bar").addClass("col-md-9");
        var option = { direction: 'right' };
        $("#right-sidebar").show();
        $("#icon-expand").removeClass("fa-chevron-left");
        $("#icon-expand").addClass("fa-chevron-right");
        $("#expand").html("@SharedLocalizer["CloseHelp"]");

        localStorage.isHelpVisible = true;
    }
});