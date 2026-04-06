import { useToaster } from "./ToasterHook";

export function useFileDownloader() {
    const toaster = useToaster();

    const downloadFile = (fileId) => {
        const path = window.location.pathname;

        const application = path.substring(path.lastIndexOf('/') + 1);

        const downloadUrl = "api/File/Download?application=" + application + "&fileId=" + fileId;

        return fetch(downloadUrl)
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
            .then(response => response ? response.blob() : response)
            .then(response => {
                if (response.Status === "ERROR")
                    throw new Error(response.Message);

                window.location = downloadUrl;
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
        downloadFile: downloadFile
    };
}