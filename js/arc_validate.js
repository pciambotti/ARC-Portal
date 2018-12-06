var errorClass = "error";
var validClass = "validClass";


function PageValid() {
}
function DonEnd() {
}

function ValidateRequired(source, arguments) {
    arguments.IsValid = false;

    var vlu = document.getElementById(source.controltovalidate).value;
    var elmnt = document.getElementById(source.controltovalidate);
    var wtrmrk = "";
    if ($("#" + source.controltovalidate).attr("WatermarkText")) { wtrmrk = $("#" + source.controltovalidate).attr("WatermarkText"); }

    if (required(vlu, elmnt)) { if (!(vlu == wtrmrk)) { arguments.IsValid = true; DonEnd(); } }
    if (arguments.IsValid) { unhighlight(document.getElementById(source.controltovalidate), errorClass, validClass); }
    else { highlight(document.getElementById(source.controltovalidate), errorClass, validClass); document.getElementById(source.controltovalidate).focus(); }
}
function ValidateRequired_DropDown(source, arguments) {
    arguments.IsValid = false;

    var vlu = document.getElementById(source.controltovalidate).value;
    var elmnt = document.getElementById(source.controltovalidate);
    var wtrmrk = "";
    if ($("#" + source.controltovalidate).attr("WatermarkText")) { wtrmrk = $("#" + source.controltovalidate).attr("WatermarkText"); }

    if (required(vlu, elmnt)) { if (!(vlu == wtrmrk)) { arguments.IsValid = true; DonEnd(); } }
    if (arguments.IsValid) { unhighlight(document.getElementById(source.controltovalidate), errorClass, validClass); }
    else { highlight(document.getElementById(source.controltovalidate), errorClass, validClass); }
}

function hide(element) { if (element) { element.style.visibility = "hidden" }; if (element) { element.style.display = "none" }; }
function show(element) { element.style.visibility = "visible"; element.style.display = ""; }
function digits(value) { return /^\d+$/.test(value); }
function digits_phone(value) { return /^(\(?\+?[0-9]*\)?)?[0-9_\- .\(\)]*$/.test(value); }
function min(value, param) { return value >= param; }
function required(value, element) { return $.trim(value).length > 0; }
function highlight(element) {
    $(element).parent().addClass(errorClass).removeClass(validClass);
    //$(element).parent().css({ 'background-color': '#ffe57f' });
    PageInvalid();
}
function unhighlight(element) {
    $(element).parent().removeClass(errorClass).addClass(validClass);
    $(element).parent().removeAttr('style');
}