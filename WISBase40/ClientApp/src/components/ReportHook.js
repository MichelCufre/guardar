import React from "react";
import { toast } from 'react-toastify';
import { useCustomTranslation } from './TranslationHook';
import { ToastMessage } from './ToastMessage';
import { notificationType } from './Enums';
import { useToaster } from "./ToasterHook";

export function useReportDownloader() {
    const toaster = useToaster();

    const downloadReport = (reportId) => {
        const path = window.location.pathname;

        const application = path.substring(path.lastIndexOf('/') + 1);

        const request = {
            method: "POST",
            cache: "no-cache",
            headers: {
                "Content-Type": "application/json",
                "X-Requested-With": "XMLHttpRequest"
            },
            body: JSON.stringify({
                application: application,
                reportId: reportId
            })
        };

        return fetch("api/Report/Fetch", request)
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
            .then(response => response ? response.json() : response)
            .then(response => {
                if (response.Status === "ERROR")
                    throw new Error(response.Message);

                window.location = "/api/Report/Download";
            })
            .catch(err => {
                if (err && err.message) {
                    toaster.toastException(err);
                    return;
                }

                toaster.toastError(err);
            });
    };

    return {
        downloadReport: downloadReport
    };
}