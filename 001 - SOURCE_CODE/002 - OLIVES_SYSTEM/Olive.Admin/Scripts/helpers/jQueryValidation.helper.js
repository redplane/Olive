/*
    This helper is for calling common functions of jquery.validate.unobtrusive
*/
$(function() {

    // Find validation result of a form which implemented jquery.validate.unobtrusive
    $.findValidationResult = function(form) {
        try {
            var errors = $(form).validate();
            return errors;
        } catch (exception) {
            console.log(exception);
            return null;
        }
    }

    // Find validation error messages of a form which implemented jquery.validate.unobtrusive
    $.findValidationMessages = function(form) {

        // Find validation messages of a form.
        var validationResult = $.findValidationResult(form);

        // Validation result is invalid.
        if (validationResult == null)
            return null;

        // Validation has no error.
        if (validationResult.errorList == null || validationResult.errorList.length < 1)
            return null;

        // Messages list construction.
        var messages = validationResult.errorList.map(function(x) {
            return x.message;
        });

        return messages;
    }

});
