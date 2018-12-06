function GH_Button() {
    $(document).ready(function() {
        // Turn all Submit Buttons to jQuery themed buttons
        $('input[type=submit]').each(function(i, obj) {
            $(this).button();
        });

        $('input[type=button]').each(function(i, obj) {
            $(this).button();
        });
    });
}
function GH_Select() {
    $(document).ready(function() {
        // Turn all Drop Down Lists to jQuery themed lists
        $('select').each(function(i, obj) {
            var w = $(this).width() + 25;
            if ($(this).attr("multiple") != "multiple") {
                if ($(this).attr("Width2")) {
                    w = $(this).attr("Width2");
                }
                $(this).selectmenu({
                    menuWidth: w,
                    wrapperElement: "<div class='wrap' />"
                });
            };
        });
    });
}
function GH_SelectMultiple() {
    $(document).ready(function() {
        // Turn all Drop Down Lists to jQuery themed multi select lists
        $('.multiselect').each(function(i, obj) {
            var w = $(this).width() + 100;
            if ($(this).attr("Width2")) {
                w = $(this).attr("Width2");
            }
            $(this).multiselect({
                minWidth: $(this).width()
                , height: w
                , classes: "multieselect"
                , header: true
                , noneSelectedText: 'Select'
            });

        });
    });
}

function Button_Custom() {
    $(document).ready(function() {
        // Turn all Submit Buttons to jQuery themed buttons
        $('input[type=submit]').each(function(i, obj) {
            $(this).button();
        });

        $('input[type=button]').each(function(i, obj) {
            $(this).button();
        });

        $('select').each(function(i, obj) {
            var w = $(this).width() + 25;
            if ($(this).attr("Width2")) {
                w = $(this).attr("Width2");
            }
            $(this).selectmenu({
                menuWidth: w,
                wrapperElement: "<div class='wrap' />"
            });
        });
    });
}
function GH_Buttons() {
    $(document).ready(function() {
        // Turn all Submit Buttons to jQuery themed buttons
        $('input[type=submit]').each(function(i, obj) {
            $(this).button();
        });
        $('input[type=button]').each(function(i, obj) {
            $(this).button();
        });
    });
}
function GH_DropDown() {
    $(document).ready(function() {
        // Turn all Drop Down Lists to jQuery themed lists
        $('select').each(function(i, obj) {
            var w = $(this).width() + 25;
            if ($(this).attr("Width2")) {
                w = $(this).attr("Width2");
            }
            $(this).selectmenu({
                menuWidth: w,
                wrapperElement: "<div class='wrap' />"
            });
        });
    });
}
function GH_DatePicker() {
    $(document).ready(function() {
        // Date Picker
        $('.ghDatePicker').each(function(i, obj) {
            $(this).datepicker();
            if ($(this).val() == "") {
                $(this).datepicker('setDate', '-1d');
            }
        });
        $('.ghDatePickerStart').each(function(i, obj) {
            $(this).datepicker();
            if ($(this).val() == "") {
                $(this).datepicker('setDate', '-1d');
            }
        });
        $('.ghDatePickerEnd').each(function(i, obj) {
            $(this).datepicker();
            if ($(this).val() == "") {
                $(this).datepicker('setDate', '-1d');
            }
        });
        // Time Picker
        $('.ghTimePicker').each(function(i, obj) {
            $(this).timepicker({});
            if ($(this).val() == "") {
                $(this).val("00:00");
            }
        });
        $('.ghTimePickerStart').each(function(i, obj) {
            $(this).timepicker({});
            if ($(this).val() == "") {
                $(this).val("00:00");
            }
        });
        $('.ghTimePickerEnd').each(function(i, obj) {
            $(this).timepicker({});
            if ($(this).val() == "") {
                $(this).val("23:59");
            }
        });
    });
}
function GH_DatePicker_OneDate() {
    $(document).ready(function() {
        // Turn all Date Pickers to jQuery themed date pickers
        $('.ghDatePicker').each(function(i, obj) {
            $(this).datepicker();
            if ($(this).val() == "") {
                $(this).datepicker('setDate', '-1d');
            }
        });
        $('.ghTimePicker').each(function(i, obj) {
            $(this).timepicker({});
            if ($(this).val() == "") {
                $(this).val("00:00");
            }
        });
        $('.ghTimePickerEnd').each(function(i, obj) {
            $(this).timepicker({});
            if ($(this).val() == "") {
                $(this).val("23:59");
            }
        });

    });
}
function SelectDate(type) {
    $(document).ready(function() {
        // Today
        var d1 = new Date();
        var d2 = new Date();

        if (type == "Yesterday") {
            // Yesterday
            d1.setDate(d1.getDate() - 1);
            d2.setDate(d2.getDate() - 1);
        }
        else if (type == "LastWeek") {
            // Last Week
            d1.setDate(d1.getDate() - 7);
            d1 = getMonday(d1);
            d2.setDate(d1.getDate() + 6);
        }
        else if (type == "ThisWeek") {
            // This week
            d1 = getMonday(d1);
            d2.setDate(d1.getDate() + 6);
        }
        else if (type == "LastMonth") {
            // Last Month
            d1 = new Date(d1.getFullYear(), d1.getMonth() - 1, 1);
            d2 = new Date(d2.getFullYear(), d2.getMonth(), 1);
            d2.setDate(d2.getDate() - 1);
        }
        else if (type == "ThisMonth") {
            // This Month
            d1 = new Date(d1.getFullYear(), d1.getMonth(), 1);
            d2 = new Date(d2.getFullYear(), d2.getMonth() + 1, 1);
            d2.setDate(d2.getDate() - 1);
        }
        else if (type == "YearToDate") {
            // Year to Date
            d1 = new Date(d1.getFullYear(), 0, 1);
        }
        var dA = d1.getMonth() + 1 + "/" + d1.getDate() + "/" + d1.getFullYear();
        var dB = d2.getMonth() + 1 + "/" + d2.getDate() + "/" + d2.getFullYear();

        //$("#dateResult").val(type + "\n\r" + dA + "\n\r" + dB);
        //alert(type + "\n\r" + dA + "\n\r" + dB);
        $('.ghDatePickerStart').datepicker('setDate', dA);
        $('.ghDatePickerEnd').datepicker('setDate', dB);
    });
}
function getMonday(d) {
    d = new Date(d);
    var day = d.getDay();
    var diff = d.getDate() - day + (day == 0 ? -6 : 1);
    // adjust when day is sunday
    return new Date(d.setDate(diff));
}
function Filter_Toggle(btn, field, nme) {
    $(document).ready(function() {
        type = $(btn).val();
        if (type.substring(0, 4) == "Hide") {
            $("#" + field).hide();
            $(btn).val("Show " + nme);
        } else {
            $("#" + field).show();
            $(btn).val("Hide " + nme);
        }
    });
}
function Filter_Hide(btn, field, nme) {
    $(document).ready(function() {
        $("#" + field).hide();
        $(btn).val("Show " + nme);
    });
}
function Priority_Stars() {
    $(document).ready(function() {
        $('.priority').each(function(i, obj) {
            Priority_Stars_Load(this, $(this).attr("priority"));
        });
    });
}
function Priority_Stars_Load(obj, strs) {
    $(document).ready(function() {
        var onTxt = '<div class="star_on"></div>';
        var offTxt = '<div class="star_off"></div>';
        var addHtml = "";
        addHtml += strs <= 0 ? offTxt : onTxt;
        addHtml += strs <= 1 ? offTxt : onTxt;
        addHtml += strs <= 2 ? offTxt : onTxt;
        addHtml += strs <= 3 ? offTxt : onTxt;
        addHtml += strs <= 4 ? offTxt : onTxt;
        $(obj).html(addHtml);
    });
}
function CountDownStart() {
    $(document).ready(function() {
        var newDate = new Date();
        $('#sinceCountdown').countdown({ since: newDate, compact: true,
            layout: '{desc}: <b>{hnn}{sep}{mnn}{sep}{snn}</b>',
            description: 'Elapsed Time'
        });
    });
}
function CountDownStop() {
    $(document).ready(function() {
        $('#sinceCountdown').countdown('destroy');
    });
}
