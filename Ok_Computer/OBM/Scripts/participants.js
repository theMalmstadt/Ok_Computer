$(document).ready(function () {
    $("select").change(function () {
        $(this).find("option:selected").each(function () {
            var optionvalue = $(this).attr("value");
            if (optionvalue) {
                $(".add").not("." + optionvalue).hide();
                $("." + optionvalue).show();
            }
            else {
                $(".add").hide();
            }
        });
    }).change();
})