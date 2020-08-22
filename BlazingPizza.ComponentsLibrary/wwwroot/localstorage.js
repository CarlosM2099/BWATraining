
    blazorLocalStorage = {
        set: (key, value) => {
            localStorage.setItem(key, JSON.stringify(value));
        },
        get: key => {
            return (key in localStorage ? JSON.parse(localStorage.getItem(key)) : null)
        },
        delete: key => {
            localStorage.removeItem(key);
        },
        contains: key => {
            return !(localStorage[key] === undefined);
        }
    }
