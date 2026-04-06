import React from 'react';

export default function withFormDataProvider(WrappedComponent) {
    return function WithFormDataProvider(props) {
        const initialize = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                form: data.form,
                context: data.context
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.form.id,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Form/Initialize", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);

        };
        const validateField = (data) => {
            const path = window.location.pathname;
            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                form: data.form,
                context: data.context
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.form.id,
                    pageToken: data.pageToken,
                    application: application,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Form/ValidateField", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);

        };
        const performButtonAction = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                form: data.form,
                context: data.context
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.form.id,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Form/ButtonAction", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);

        };
        const submit = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                form: data.form,
                context: data.context
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.form.id,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Form/Submit", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);

        };
        const selectSearch = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                form: data.form,
                context: data.context
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.form.id,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Form/SelectSearch", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);

        };

        return (
            <WrappedComponent
                formInitialize={initialize}
                formValidateField={validateField}
                formPerformButtonAction={performButtonAction}
                formSubmit={submit}
                formSelectSearch={selectSearch}
                {...props}
            />
        );
    };
}