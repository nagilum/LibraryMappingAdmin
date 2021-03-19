"use strict";

/**
 * Show a popup that allows the user to add a new package.
 * @param {Event} e Button click event.
 */
const addNewPackage = (e) => {
    e.preventDefault();

    showPopup('PopupNewPackage');

    const popup = document.querySelector('popup#PopupNewPackage'),
        filename = e.target.getAttribute('data-filename');

    popup.setAttribute('data-filename', filename);
    popup.classList.remove('loading');
}

/**
 * Create the new package and assign the file entry.
 * @param {Event} e Click event.
 */
const addNewPackageSave = (e) => {
    e.preventDefault();

    const popup = document.querySelector('popup#PopupNewPackage'),
        name = document.querySelector('input#NewPackageName').value,
        nuGetUrl = document.querySelector('input#NewPackageNuGetUrl').value,
        infoUrl = document.querySelector('input#NewPackageInfoUrl').value,
        repoUrl = document.querySelector('input#NewPackageRepoUrl').value;

    e.target.classList.add('loading');

    const io = {
        name: name,
        nuGetUrl: nuGetUrl,
        infoUrl: infoUrl,
        repoUrl: repoUrl,
        files: [popup.getAttribute('data-filename')]
    };

    return fw(
            '/api/package',
            'POST',
            io)
        .then(obj => {
            if (obj.message) {
                return error(obj.message);
            }

            e.target.classList.remove('loading');

            // Refresh the stats.
            return getPanelLinkStats()
                .then(() => {
                    // Load and display the File Entries table.
                    return loadFileEntries()
                        .then(() => {
                            // Close the popup.
                            closePopup();
                        });
                });
        });
};

/**
 * Show a popup that allows the user to assign a package to a filename.
 * @param {Event} e Link click event.
 */
const attachPackage = (e) => {
    e.preventDefault();

    showPopup('PopupAttachPackage');

    return fw('/api/package')
        .then(packages => {
            if (!packages) {
                return;
            }

            const popup = document.querySelector('popup#PopupAttachPackage'),
                select = document.querySelector('select#SelectAttachPackage'),
                save = document.querySelector('input#ButtonAttachPackage'),
                filename = e.target.getAttribute('data-filename');

            save.setAttribute('data-filename', filename);

            select.innerHTML = '';

            packages.forEach(pkg => {
                const option = ce('option');

                option.value = pkg.id;
                option.innerText = pkg.name;

                select.appendChild(option);
            });

            popup.classList.remove('loading');
        });
};

/**
 * Attach the specified package to a given filename.
 * @param {Event} e Button click event.
 */
const attachPackageSave = (e) => {
    e.preventDefault();

    const select = document.querySelector('select#SelectAttachPackage');

    e.target.classList.add('loading');

    return fw(
            `/api/package/${select.value}/attach`,
            'POST',
            {
                fileName: e.target.getAttribute('data-filename')
            })
        .then(obj => {
            if (obj.message) {
                return error(obj.message);
            }

            e.target.classList.remove('loading');

            // Refresh the stats.
            return getPanelLinkStats()
                .then(() => {
                    // Load and display the File Entries table.
                    return loadFileEntries()
                        .then(() => {
                            // Close the popup.
                            closePopup();
                        });
                });
        });
};

/**
 * Shorthand for document.createElement.
 * @param {String} tagName Name of the tag.
 * @returns {Element}
 */
const ce = (tagName) => {
    return document.createElement(tagName);
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
 * Close all active popups.
 */
const closePopup = (e) => {
    if (e) {
        e.preventDefault();
    }

    document.querySelectorAll('popup.active')
        .forEach(m => {
            m.classList.remove('active');
        });

    document.querySelectorAll('popups')
        .forEach(m => {
            m.classList.remove('active');
        });
}

/**
 * Close all active popups.
 * @param {Event} e 'popups' click event.
 */
const closePopups = (e) => {
    if (e.target.tagName.toLowerCase() !== 'popups') {
        return;
    }

    closePopup();
}

/**
 * Display an error.
 * @param {String} err The error to show.
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
 * @returns {Promise}
 */
const fw = (url, method, payload, headers) => {
    if (!method) {
        method = 'GET';
    }

    if (!headers) {
        headers = {};
    }

    headers['Accept'] = 'application/json';

    if (window.token) {
        headers['Authorization'] = `Bearer ${window.token}`;
    }

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
    const panel = document.querySelector('panel#PanelFileEntries');

    panel.classList.add('loading');
    panel.innerHTML = '';

    return fw('/api/fileentry')
        .then(obj => {
            return fw('/api/package')
                .then(list => {
                    return {
                        servers: obj.servers,
                        packages: list
                    };
                });
        })
        .then(wr => {
            if (!wr) {
                return;
            }

            const servers = wr.servers,
                packages = wr.packages;

            panel.classList.remove('loading');

            const srvtitle = ce('h1'),
                srvlist = ce('ul');

            srvtitle.innerText = 'Servers';

            panel.appendChild(srvtitle);
            panel.appendChild(srvlist);

            servers.forEach(server => {
                const sn = ce('h1'),
                    ips = ce('span'),
                    table = ce('table'),
                    thead = ce('thead'),
                    tbody = ce('tbody'),
                    srvitem = ce('li'),
                    srvlink = ce('a');

                srvlink.setAttribute('data-server-name', server.serverName);
                srvlink.classList.add('scroll-to-server');
                srvlink.innerText = server.serverName;
                srvlink.addEventListener('click', scrollToServer);

                srvitem.appendChild(srvlink);
                srvlist.appendChild(srvitem);

                sn.setAttribute('data-server-name', server.serverName);
                sn.classList.add('server-entry');
                sn.innerHTML = `Server: <span>${server.serverName}</span>`;

                ips.innerText = `${server.serverIps.join(', ')}`;

                thead.innerHTML =
                    '<tr>' +
                    '  <th>Filename</th>' +
                    '  <th>File Version</th>' +
                    '  <th>Product Version</th>' +
                    '  <th>Package</th>' +
                    '  <th>Last Scanned</th>' +
                    '</tr>';

                table.appendChild(thead);
                table.appendChild(tbody);

                panel.appendChild(sn);
                panel.appendChild(ips);
                panel.appendChild(table);

                server.fileEntries.forEach(fe => {
                    const tr = ce('tr'),
                        tdFilename = ce('td'),
                        tdFileVersion = ce('td'),
                        tdProductVersion = ce('td'),
                        tdPackage = ce('td'),
                        tdLastScanned = ce('td');

                    // Filename
                    tdFilename.innerText = fe.fileName;

                    // File Version
                    tdFileVersion.innerText = fe.fileVersion;

                    // Product Version
                    tdProductVersion.innerText = fe.productVersion;

                    // Package
                    if (fe.packageId) {
                        let pf = false;

                        packages.forEach(pkg => {
                            if (pkg.id === fe.packageId) {
                                tdPackage.innerText = pkg.name;
                                pf = true;
                            }
                        });

                        if (!pf) {
                            tdPackage.classList.add('warning');
                            tdPackage.innerText = `Package Not Found: #${fe.packageId}`;
                        }
                    }
                    else {
                        const aep = ce('a'),
                            anp = ce('a');

                        aep.setAttribute('data-filename', fe.fileName);
                        aep.setAttribute('data-fe-id', fe.id);
                        aep.classList.add('warning');
                        aep.innerText = 'Attach Existing Package';
                        aep.addEventListener('click', attachPackage);

                        anp.setAttribute('data-filename', fe.fileName);
                        anp.setAttribute('data-fe-id', fe.id);
                        anp.classList.add('warning');
                        anp.innerText = 'Create New Package';
                        anp.addEventListener('click', addNewPackage);

                        tdPackage.appendChild(aep);
                        tdPackage.appendChild(anp);
                    }

                    // Last Scanned
                    tdLastScanned.innerText = fe.lastScan;

                    // Done.
                    tr.appendChild(tdFilename);
                    tr.appendChild(tdFileVersion);
                    tr.appendChild(tdProductVersion);
                    tr.appendChild(tdPackage);
                    tr.appendChild(tdLastScanned);

                    tbody.appendChild(tr);
                });
            });
        });
};

/**
 * Load and display the Packages table.
 */
const loadPackages = () => {
    const panel = document.querySelector('panel#PanelPackages');

    panel.classList.add('loading');
    panel.innerHTML = '';

    return fw('/api/package')
        .then(list => {
            panel.classList.remove('loading');

            if (!list) {
                return;
            }

            const table = ce('table'),
                thead = ce('thead'),
                tbody = ce('tbody');

            thead.innerHTML =
                '<tr>' +
                '  <th>Name</th>' +
                '  <th>Files</th>' +
                '</tr>';

            table.appendChild(thead);
            table.appendChild(tbody);

            list.forEach(pkg => {
                const tr = ce('tr'),
                    tdName = ce('td'),
                    tdFiles = ce('td');

                const name = ce('div');
                name.innerText = pkg.name;
                tdName.appendChild(name);

                if (pkg.files) {
                    const fileList = JSON.parse(pkg.files),
                        flul = ce('ul');

                    fileList.forEach(file => {
                        const flli = ce('li');

                        flli.innerText = file;
                        flul.appendChild(flli);
                    });

                    tdFiles.appendChild(flul);
                }

                if (pkg.nuGetUrl) {
                    const a = ce('a');

                    a.setAttribute('href', pkg.nuGetUrl);
                    a.innerText = 'NuGet';

                    tdName.appendChild(a);
                }

                if (pkg.infoUrl) {
                    const a = ce('a');

                    a.setAttribute('href', pkg.infoUrl);
                    a.innerText = 'Info';

                    tdName.appendChild(a);
                }

                if (pkg.repoUrl) {
                    const a = ce('a');

                    a.setAttribute('href', pkg.repoUrl);
                    a.innerText = 'Repository';

                    tdName.appendChild(a);
                }

                // Add up.
                tr.appendChild(tdName);
                tr.appendChild(tdFiles);
                tbody.appendChild(tr);
            });

            panel.appendChild(table);
        });
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
 * Scroll the screen to a given server block.
 * @param {Event} e Click event.
 */
const scrollToServer = (e) => {
    const serverName = e.target.getAttribute('data-server-name');

    document.querySelectorAll('h1.server-entry')
        .forEach(el => {
            if (el.getAttribute('data-server-name') !== serverName) {
                return;
            }

            el.scrollIntoView({
                block: "end"
            });
        });
};

/**
 * Show a specific popup dialog.
 * @param {String} id ID of the element.
 */
const showPopup = (id) => {
    const popup = document.querySelector(`popup#${id}`),
        popups = popup.parentElement;

    popup.classList.add('active');
    popup.classList.add('loading');

    popups.classList.add('active');
}

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
    document.querySelector('input#ButtonAttachPackage').addEventListener('click', attachPackageSave);
    document.querySelector('input#ButtonNewPackage').addEventListener('click', addNewPackageSave);
    document.querySelector('popups').addEventListener('click', closePopups);

    // Add trigger for switching panels.
    document.querySelectorAll('a.panel-link')
        .forEach(a => {
            a.addEventListener('click', switchPanel);
        });

    // Add trigger for closing dialogs.
    document.querySelectorAll('button.auto-close-popup')
        .forEach(b => {
            b.addEventListener('click', closePopup);
        });

    // Set focus to the username input box.
    document.querySelector('input#LoginUsername').focus();

    // Check for logged in user.
    checkForLoggedInUser();

    // Query for stats.
    getPanelLinkStats();

    // Setup link to global functions.
    window.loadFileEntries = loadFileEntries;
    window.loadPackages = loadPackages;
})();