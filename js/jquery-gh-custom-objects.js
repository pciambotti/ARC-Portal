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
                    wrapperElement: "<div class='wrap' />",
                    // This is the hack I had to do because jQuery is disabling my autopost back which creates an onchange event
                    change: function (event, ui) { if ($(this).attr("onchange")) { eval($(this).attr("onchange")); } }
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
                wrapperElement: "<div class='wrap' />",
                // This is the hack I had to do because jQuery is disabling my autopost back which creates an onchange event
                change: function (event, ui) { if ($(this).attr("onchange")) { eval($(this).attr("onchange")); } }
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
                wrapperElement: "<div class='wrap' />",
                // This is the hack I had to do because jQuery is disabling my autopost back which creates an onchange event
                change: function (event, ui) { if ($(this).attr("onchange")) { eval($(this).attr("onchange")); } }
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
                if ($(this).is('.clearme')) {
                    // Do Nothing
                } else {
                    $(this).datepicker('setDate', '-1d');
                }
            }
        });
        $('.ghDatePickerEnd').each(function(i, obj) {
            $(this).datepicker();
            if ($(this).val() == "") {
                if ($(this).is('.clearme')) {
                    // Do Nothing
                } else {
                    $(this).datepicker('setDate', '-1d');
                }
            }
        });
        // Time Picker
        $('.ghTimePicker').each(function (i, obj) {
            $(this).timepicker({ timeFormat: 'hh:mm:00' });
            if ($(this).val() == "") {
                $(this).val("00:00");
            }
        });
        $('.ghTimePickerStart').each(function (i, obj) {
            $(this).timepicker({ timeFormat: 'hh:mm:00' });
            if ($(this).val() == "") {
                $(this).val("00:00:00");
            }
        });
        $('.ghTimePickerEnd').each(function (i, obj) {
            $(this).timepicker({ timeFormat: 'hh:mm:59' });
            if ($(this).val() == "") {
                $(this).val("23:59:59");
            }
        });
        /// http://stackoverflow.com/questions/389743/how-to-limit-days-available-in-jquery-ui-date-picker
        $('.ghDatePickerSustainerStart').each(function (i, obj) {
            $(this).datepicker({ beforeShowDay: sustainerDays });
            //if ($(this).val() == "") {
            //    if ($(this).is('.clearme')) {
            //        // Do Nothing
            //    } else {
            //        $(this).datepicker('setDate', '-1d');
            //    }
            //}
        });

    });
}
function GH_DatePickerToday() {
    $(document).ready(function () {
        // Date Picker
        // http://trentrichardson.com/examples/timepicker/
        $('.ghDatePicker').each(function (i, obj) {
            $(this).datepicker();
            if ($(this).val() == "") {
                $(this).datepicker('setDate', 'd');
            }
        });
        $('.ghDatePickerStart').each(function (i, obj) {
            $(this).datepicker({ changeYear: true, yearRange: "-10:+0", numberOfMonths: 2, showCurrentAtPos: 1 });
            if ($(this).val() == "") {
                $(this).datepicker('setDate', 'd');
            }
        });
        $('.ghDatePickerEnd').each(function (i, obj) {
            $(this).datepicker({ changeYear: true, yearRange: "-10:+0", numberOfMonths: 2, showCurrentAtPos: 1 });
            if ($(this).val() == "") {
                $(this).datepicker('setDate', 'd');
            }
        });
        // Time Picker
        $('.ghTimePicker').each(function (i, obj) {
            $(this).timepicker({ timeFormat: 'hh:mm:00' });
            if ($(this).val() == "") {
                $(this).val("00:00");
            }
        });
        $('.ghTimePickerStart').each(function (i, obj) {
            $(this).timepicker({ timeFormat: 'hh:mm:00' });
            if ($(this).val() == "") {
                $(this).val("00:00:00");
            }
        });
        $('.ghTimePickerEnd').each(function (i, obj) {
            $(this).timepicker({ timeFormat: 'hh:mm:59' });
            if ($(this).val() == "") {
                $(this).val("23:59:59");
            }
        });

        $('.ghDatePickerSustainerStart').each(function (i, obj) {
            $(this).datepicker({ beforeShowDay: sustainerDays });
            //if ($(this).val() == "") {
            //    if ($(this).is('.clearme')) {
            //        // Do Nothing
            //    } else {
            //        $(this).datepicker('setDate', '-1d');
            //    }
            //}
        });
    });
}
natDays = [
  [1, 1, 'au'], [1, 15, 'nz'], [3, 17, 'ie'],
  [4, 27, 'za'], [5, 25, 'ar'], [6, 6, 'se'],
  [7, 4, 'us'], [8, 17, 'id'], [9, 7, 'br'],
  [10, 1, 'cn'], [11, 22, 'lb'], [12, 12, 'ke']
];

function sustainerDays(date) {
    if (date.getDate() == 1 || date.getDate() == 15) {
        // return [true, date.getMonth(), date.getDay(), 'us_day'];
        return [true, ''];
    }
    return [false, ''];
}

function nationalDays(date) {
    for (i = 0; i < natDays.length; i++) {
        if (date.getMonth() == natDays[i][0] - 1
            && date.getDate() == natDays[i][1]) {
            return [true, natDays[i][2] + '_day'];
        }
    }
    return [false, ''];
}

function GH_DatePicker_FullMonth() {
    $(document).ready(function () {
        // Start Date
        var d = new Date();
        $('.ghDatePickerStart').each(function (i, obj) {
            $(this).datepicker();
            if ($(this).val() == "") {
                if ($(this).is('.clearme')) {
                    // Do Nothing
                } else {
                    $(this).datepicker('setDate', new Date(d.getFullYear(), d.getMonth(), 1));
                }
            }
        });
        // End Date
        $('.ghDatePickerEnd').each(function (i, obj) {
            $(this).datepicker();
            if ($(this).val() == "") {
                if ($(this).is('.clearme')) {
                    // Do Nothing
                } else {
                    $(this).datepicker('setDate', d);
                }
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
function GH_TimePicker() {
    $(document).ready(function () {
        // Time Picker
        $('.ghTimePicker').each(function (i, obj) {
            $(this).timepicker({});
            if ($(this).val() == "") {
                $(this).val("00:00");
            }
        });
        $('.ghTimePickerStart').each(function (i, obj) {
            $(this).timepicker({});
            if ($(this).val() == "") {
                $(this).val("00:00");
            }
        });
        $('.ghTimePickerEnd').each(function (i, obj) {
            $(this).timepicker({});
            if ($(this).val() == "") {
                $(this).val("23:59");
            }
        });
    });
}
function SelectDate(type) {
    $(document).ready(function () {
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
            d1 = getMonday(d1);
            d2.setDate(d1.getDate() - 1);
            d1.setDate(d1.getDate() - 7);
        }
        else if (type == "ThisWeek") {
            // This week
            d1 = getMonday(d1);
            //d2.setDate(d1.getDate() + 6);
        }
        else if (type == "LastMonth") {
            // Last Month
            d1 = new Date(d1.getFullYear(), d1.getMonth() - 1, 1);
            d2 = new Date(d2.getFullYear(), d2.getMonth(), 1);
            d2.setDate(d2.getDate() - 1);
        }
        else if (type == "ThisMonthYesterday") {
            // This Month
            d1 = new Date(d1.getFullYear(), d1.getMonth(), 1);
            d2.setDate(d2.getDate() - 1);
            //d2 = new Date(d2.getFullYear(), d2.getMonth() + 1, 1);
            //d2.setDate(d2.getDate() - 1);
        }
        else if (type == "ThisMonth") {
            // This Month
            d1 = new Date(d1.getFullYear(), d1.getMonth(), 1);
            //d2 = new Date(d2.getFullYear(), d2.getMonth() + 1, 1);
            //d2.setDate(d2.getDate() - 1);
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

        $('.ghDatePickerStart').each(function (i, obj) {
            getMyDay(this);
        });
        $('.ghDatePickerEnd').each(function (i, obj) {
            getMyDay(this);
        });
    });
}
function SelectDateVariable(type, dtStart, dtEnd) {
    $(document).ready(function () {
        // Today
        var d1 = new Date();
        var d2 = new Date();
        //alert(dtStart + "|" + dtEnd);

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
        else if (type == "DayPrevious") {
            d1 = new Date($('#' + dtStart).val());
            d2 = new Date($('#' + dtEnd).val());
            d1.setDate(d1.getDate() - 1);
            d2.setDate(d2.getDate() - 1);
            // alert($('#' + dtStart).val() + "|" + $('#' + dtEnd).val() + "\n\r" + d1 + "\n\r" + d2);
        }
        else if (type == "DayNext") {
            d1 = new Date($('#' + dtStart).val());
            d2 = new Date($('#' + dtEnd).val());
            d1.setDate(d1.getDate() + 1);
            d2.setDate(d2.getDate() + 1);
            // alert($('#' + dtStart).val() + "|" + $('#' + dtEnd).val() + "\n\r" + d1 + "\n\r" + d2);
        }

        var dA = d1.getMonth() + 1 + "/" + d1.getDate() + "/" + d1.getFullYear();
        var dB = d2.getMonth() + 1 + "/" + d2.getDate() + "/" + d2.getFullYear();

        //$("#dateResult").val(type + "\n\r" + dA + "\n\r" + dB);
        //alert(type + "\n\r" + dA + "\n\r" + dB);
        $('#' + dtStart).datepicker('setDate', dA);
        $('#' + dtEnd).datepicker('setDate', dB);

        

        $('#' + dtStart).each(function (i, obj) {
            getMyDay(this);

        });
        $('#' + dtEnd).each(function (i, obj) {
            getMyDay(this);
        });
    });
}
function getMyDay(obj) {
    $(document).ready(function () {
        // Turn all Submit Buttons to jQuery themed buttons
        var dayDate = $(obj).val();
        var objID = obj.id;
        objID = objID.replace("_dt", "_lbl");

        if ($("#" + objID).length) {
            var d = new Date(dayDate);

            var weekday = new Array(7);
            //weekday[0] = "Sunday";
            //weekday[1] = "Monday";
            //weekday[2] = "Tuesday";
            //weekday[3] = "Wednesday";
            //weekday[4] = "Thursday";
            //weekday[5] = "Friday";
            //weekday[6] = "Saturday";
            weekday[0] = "Sun";
            weekday[1] = "Mon";
            weekday[2] = "Tue";
            weekday[3] = "Wed";
            weekday[4] = "Thu";
            weekday[5] = "Fri";
            weekday[6] = "Sat";

            var dayName = weekday[d.getDay()];

            $("#" + objID).text(dayName);
        }
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
