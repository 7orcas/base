window.recaptchaFunctions = {

    renderRecaptcha: function (elementId, siteKey) {

        return grecaptcha.render(elementId, {
            'sitekey': siteKey
        });
    },

    getResponse: function (widgetId) {
        return grecaptcha.getResponse(widgetId);
    },

    reset: function (widgetId) {
        grecaptcha.reset(widgetId);
    }
};