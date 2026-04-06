import React from 'react';

export default function withPageDataProvider(WrappedComponent) {
    return function WithPageDataProvider(props) {
        const performLoad = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    application: application,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Page/Load", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    if (response.status === 403) {
                        throw new Error(response.status);
                    }

                    return response;
                })
                .then(response => response ? response.json() : response);
        };
        const performUnload = (data, pageToken) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    application: application,
                    pageToken: pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Page/Unload", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then(response => response ? response.json() : response)
                .catch(d => {
                    if (window.location.pathname !== "/api/Security/Logout")
                        window.location = "/api/Security/Logout";
                });
        };

        return (
            <WrappedComponent
                performLoad={performLoad}
                performUnload={performUnload}
                {...props}
            />
        );
    };
}