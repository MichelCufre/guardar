import React, { Component } from 'react';

export default function withReportDataProvider(WrappedComponent) {
    return class WithReportDataProvider extends Component {
        generate(name, parameters) {
            const data = {
                parameters: parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    application: name,
                    data: JSON.stringify(data)
                })
            };

            return fetch("api/Report/GenerateReport", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";
                    }

                    return response;
                })
                .then((response) => response.json());
        }

        render() {
            return (
                <WrappedComponent
                    generateReport={this.generate}
                    {...this.props}
                />
            );
        }
    };
}