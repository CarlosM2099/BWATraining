utils = {
    setFocus: (selector) => {
        var element = document.querySelector(selector);

        if (element) {
            element.focus();
        }
    }
}