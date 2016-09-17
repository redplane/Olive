$(function() {
    
    // Calculate the beginning of date of the given time.
    // Then convert it to unix.
    $.findBeginningTimeOfDay = function(dateTime) {
        try {
            dateTime.setHours(0);
            dateTime.setMinutes(0);
            dateTime.setSeconds(0);
            dateTime.setMilliseconds(0);

            return dateTime.getTime();
        } catch (exception) {
            console.log(exception);
            return null;
        }
    }

    // Calculate the ending of date of the given time.
    // Then convert it to unix.
    $.findEndingTimeOfDay = function(dateTime) {
        try {
            dateTime.setHours(23);
        } catch (exception) {
            console.log(exception);
            return null;
        }
    }

    // Calculate the beginning time of the month
    // Then convert it to unix.
    $.findBeginningTimeOfMonth = function(dateTime) {
        try {
            dateTime.setHours(0);
            dateTime.setMinutes(0);
            dateTime.setSeconds(0);
            dateTime.setMilliseconds(0);
            dateTime.setDate(1);

            return dateTime.getTime();
        } catch (exception) {
            console.log(exception);
            return 0;
        }
    }
})