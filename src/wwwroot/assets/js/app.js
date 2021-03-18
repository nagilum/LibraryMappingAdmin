"use strict";

/**
 * Display an error.
 * @param {String} err The error to show.
 * @returns null.
 */
const error = (err) => {
    console.error(err);
    alert(err);
    return null;
};

/**
 * Wrapper for fetch requests.
 * @param {String} url URL to fetch.
 * @param {String} method HTTP method to use. Defaults to 'GET'.
 * @param {Object} payload Payload to transmit.
 * @param {Object} headers Additional headers to transmit.
 */
const fw = (url, method, payload, headers) => {
    if (!method) {
        method = 'GET';
    }

    if (!headers) {
        headers = {};
    }

    headers['Accept'] = 'application/json';

    if (payload) {
        headers['Content-Type'] = 'application/json';
    }

    const options = {
        method: method,
        headers: headers
    };

    if (payload) {
        options.body = JSON.stringify(payload);
    }

    return fetch(url, options)
        .then(res => {
            switch (res.status) {
                case 200:
                case 400:
                case 401:
                case 404:
                    return res.json();

                default:
                    throw new Error('Invalid API response!');
            }
        })
        .catch(err => {
            return error(err);
        });
};

/**
 * Check if the user is already logged in and bypass the login form.
 */
const checkForLoggedInUser = () => {
    const token = localStorage.getItem('token');

    if (token) {
        window.token = token;
        document.querySelector('interface').classList.add('active');
    }
    else {
        document.querySelector('login').classList.add('active');
    }
};

/**
 * Get stats for each of the tab links.
 */
const getPanelLinkStats = () => {
    return fw('/api/stats')
        .then(obj => {
            if (!obj) {
                return;
            }

            const lfe = document.querySelector('a#LinkFileEntries'),
                lp = document.querySelector('a#LinkPackages'),
                lbp = document.querySelector('a#LinkBadPackages');

            const html1 = obj.fileEntriesWithoutPackage > 0
                ? `${lfe.getAttribute('data-title')} <span class="count">${obj.fileEntries}/<span class="red">${obj.fileEntriesWithoutPackage}</span></span>`
                : `${lfe.getAttribute('data-title')} <span class="count">${obj.fileEntries}</span>`;

            const html2 = `${lp.getAttribute('data-title')} <span class="count">${obj.packages}</span>`,
                html3 = `${lbp.getAttribute('data-title')} <span class="count ${(obj.badPackages > 0 ? 'red' : 'green')}">${obj.badPackages}</span>`;

            const title1 = obj.fileEntriesWithoutPackage > 0
                ? `Total entries: ${obj.fileEntries} - Entries without packages: ${obj.fileEntriesWithoutPackage}`
                : `Total entries: ${obj.fileEntries}`;

            const title2 = `Total packages: ${obj.packages}`,
                title3 = `Total bad packages: ${obj.badPackages}`;

            lfe.setAttribute('title', title1);
            lfe.innerHTML = html1;

            lp.setAttribute('title', title2);
            lp.innerHTML = html2;

            lbp.setAttribute('title', title3);
            lbp.innerHTML = html3;

            if (obj.fileEntriesWithoutPackage > 0) {
                lfe.classList.add('warning');
            }

            if (obj.badPackages > 0) {
                lbp.classList.add('warning');
            }
        });
};

/**
 * Load and display the File Entries table.
 */
const loadFileEntries = () => {
};

/**
 * Attempt to log the user in.
 * @param {Event} e Button click event.
 */
const login = (e) => {
    e.preventDefault();

    const username = document.querySelector('input#LoginUsername').value,
        password = document.querySelector('input#LoginPassword').value;

    if (!username || !password) {
        return error('Both username and password are required!');
    }

    e.target.classList.add('loading');

    return fw(
            '/api/user/login',
            'POST',
            {
                username: username,
                password: password
            })
        .then(obj => {
            e.target.classList.remove('loading');

            if (!obj.token) {
                return error(obj.message);
            }

            localStorage.setItem('token', obj.token);

            window.token = obj.token;

            document.querySelector('login').classList.remove('active');
            document.querySelector('interface').classList.add('active');

            return null;
        });
};

/**
 * Switch the active panel.
 * @param {Event} e Link click event.
 */
const switchPanel = (e) => {
    e.preventDefault();

    const id = e.target.getAttribute('data-panel-id');

    if (!id) {
        return;
    }

    // Switch the active link.
    document.querySelectorAll('a.panel-link')
        .forEach(a => {
            if (a.getAttribute('data-panel-id') === id) {
                a.classList.add('active');
            }
            else {
                a.classList.remove('active');
            }
        });

    // Switch the active panel.
    document.querySelectorAll('panel')
        .forEach(panel => {
            if (panel.getAttribute('id') === id) {
                panel.innerHTML = '';
                panel.classList.add('active');
                panel.classList.add('loading');
            }
            else {
                panel.classList.remove('active');
            }
        });

    // Run function, if present.
    const ftr = e.target.getAttribute('data-function');

    if (window[ftr]) {
        window[ftr]();
    }
};

/**
 * Init all the things..
 */
(() => {
    // Add single click events.
    document.querySelector('input#Login').addEventListener('click', login);

    // Add multi-link click events.
    document.querySelectorAll('a.panel-link')
        .forEach(a => {
            a.addEventListener('click', switchPanel);
        });

    // Set focus to the username input box.
    document.querySelector('input#LoginUsername').focus();

    // Check for logged in user.
    checkForLoggedInUser();

    // Query for stats.
    getPanelLinkStats();

    // Setup link to global functions.
    window.loadFileEntries = loadFileEntries;
})();