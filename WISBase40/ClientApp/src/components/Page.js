import React, { Component, useState, useRef, useEffect } from 'react';
import withPageDataProvider from './WithPageDataProvider';
import withReportDataProvider from './WithReportDataProvider';
import { PageContextProvider } from './WithPageContext';
import { componentType, notificationType } from './Enums';
import { withTranslation } from 'react-i18next';
import { toast } from 'react-toastify';
import { ConfirmationBox } from './ConfirmationBox';
import { PageBottomFiller } from './PageBottomFiller';
import { ToastMessage } from './ToastMessage';
import { ErrorPage } from './ErrorPage';
import { ComponentError } from './ComponentError';
import { useNavigate } from 'react-router-dom';

function InternalPage(props) {
    const navigate = useNavigate();

    const [isLoadComplete, setLoadComplete] = useState(false);
    const [tooManySessions, setTooManySessions] = useState(false);
    const [isConfirmationOpen, setConfirmationOpen] = useState(false);
    const [requestError, setRequestError] = useState("");
    const [confirmationMessage, setConfirmationMessage] = useState(null);
    const [confirmationAcceptLabel, setConfirmationAcceptLabel] = useState(null);
    const [confirmationArgMessage, setConfirmationArgMessage] = useState(null);
    const [confirmationCancelLabel, setConfirmationCancelLabel] = useState(null);
    const [confirmationAcceptVariant, setConfirmationAcceptVariant] = useState(null);
    const [confirmationCancelVariant, setConfirmationCancelVariant] = useState(null);
    const [pageBottomFillerHeight, setPageBottomFillerHeight] = useState(null);
    //const [pageToken, setPageToken] = useState(null);
    const pageToken = useRef(null);

    const bottomFillerRef = useRef(null);
    const components = useRef([]);
    const confirmationAcceptAction = useRef(null);
    const confirmationCancelAction = useRef(null);

    useEffect(() => {
        const { t } = props;
        const nameTitle = t(props.title);

        if (props.title != null)
            document.title = nameTitle;

        if (nameTitle != null && nameTitle != "") {
            document.title = nameTitle;
            document.getElementById("page-title").innerHTML = nameTitle;
        }

        onLoad();

        return () => {
            onUnload();
        };
    }, []);
    useEffect(() => {
        window.addEventListener("unload", handleAppAbandon);

        return () => {
            window.removeEventListener("unload", handleAppAbandon);
        };
    }, [pageToken]);

    const handleAppAbandon = () => {
        var application = props.application;

        if (!application) {
            application = window.location.pathname.split('/').pop();
        }

        const body = {
            token: pageToken.current,
            application: application
        };

        const headers = {
            type: 'application/json'
        };

        const blob = new Blob([JSON.stringify(body)], headers);

        navigator.sendBeacon("api/Page/Abandon", blob);
    }

    const onLoad = () => {
        let parameters = getQueryParameters() || []

        const data = {
            parameters: parameters,
            application: props.application
        };

        beforeLoad(data);

        props.performLoad(data)
            .then(response => {
                try {
                    if (!response)
                        return false;

                    if (props.hideLoadError) {
                        if (response.Status === "OK") {
                            if (props.opener)
                                document.getElementById(props.opener).style.display = "";
                        } else {
                            console.error(response.Message);
                            return false;
                        }
                    } else if (response.Status === "ERROR") {
                        throw new ComponentError(response.MessageArguments, response.Message);
                    }

                    let data = JSON.parse(response.Data);

                    afterLoad(data);

                    pageToken.current = response.PageToken;

                    setTooManySessions(response.TooManySessions);
                    setLoadComplete(true);
                }
                catch (ex) {
                    toastException(ex);
                }
            })
            .catch((error) => {
                setRequestError(error.message);
            });
    }
    const onUnload = () => {
        let parameters = getQueryParameters() || []

        beforeUnload(parameters);

        const data = {
            parameters: parameters,
            application: props.application
        };

        props.performUnload(data, pageToken.current)
            .then(response => {
                try {
                    if (!response)
                        return false;

                    if (response.Status === "ERROR") {
                        if (props.hideLoadError) {
                            console.error(response.Message);
                            return false;
                        } else {
                            throw new ComponentError(response.MessageArguments, response.Message);
                        }
                    }

                    let data = JSON.parse(response.Data);

                    afterUnload(data);

                    setTooManySessions(response.TooManySessions);
                    setLoadComplete(true);
                }
                catch (ex) {
                    toastException(ex);
                }
            });
    }

    const registerComponent = (componentId, type, api) => {
        if (components.current.some(c => c.id === componentId))
            throw "Duplicate component ID found: " + componentId + ". Cannot register component";

        components.current = [
            ...components.current,
            {
                id: componentId,
                type: type,
                api: api
            }
        ];
    };
    const unregisterComponent = (componentId) => {
        const index = components.current.findIndex(c => c.id === componentId);

        components.current = [
            ...components.current.slice(0, index),
            ...components.current.slice(index + 1)
        ];
    };
    const getComponents = () => {
        return components.current;
    };
    const getGrid = (gridId) => {
        const res = getComponent(gridId);

        if (!res)
            return res;

        if (res.type !== componentType.grid)
            throw "Component " + gridId + " is not a grid";

        return res.api;
    };
    const getForm = (formId) => {
        const res = getComponent(formId);

        if (!res)
            return res;

        if (res.type !== componentType.form)
            throw "Component " + formId + " is not a form";

        return res.api;
    };
    const getComponent = (componentId) => {
        return components.current.find(d => d.id === componentId);
    };

    const beforeLoad = (data) => {
        if (props.onBeforeLoad)
            props.onBeforeLoad(data);
    }
    const afterLoad = (data) => {
        if (props.onAfterLoad)
            props.onAfterLoad(data);
    }
    const beforeUnload = (data) => {
        if (props.onBeforeUnload)
            props.onBeforeUnload(data);
    }
    const afterUnload = (data) => {
        if (props.onAfterUnload)
            props.onAfterUnload(data);
    }

    const redirect = (url, openInNewWindow, parameters) => {
        if (!url)
            return false;

        let queryString = "";
        let queryList = [];

        for (let param of parameters) {
            queryList.push(`${param.id}=${encodeURI(param.value)}`);
        }

        if (queryList.length > 0) {
            queryString += `?${queryList.join("&")}`;
        }

        if (openInNewWindow) {
            window.open(url.trim("/") + queryString, "_blank");
        } else {
            navigate(url.trim("/") + queryString);      
        }
    }

    const toastMessage = (type, message, options, args) => {
        let translatedMessage = props.t(message);

        if (args && args.length > 0) {
            args.forEach((arg, index) => {
                translatedMessage = translatedMessage.replace(new RegExp("\\{" + index + "\\}"), arg);
            });
        }

        switch (type) {
            case notificationType.error:
                toast.error(<ToastMessage message={translatedMessage} type={notificationType.error} />, options);
                break;
            case notificationType.info:
                toast.info(<ToastMessage message={translatedMessage} type={notificationType.info} />, options);
                break;
            case notificationType.warning:
                toast.warn(<ToastMessage message={translatedMessage} type={notificationType.warn} />, options);
                break;
            case notificationType.success:
                toast.success(<ToastMessage message={translatedMessage} type={notificationType.success} />, options);
                break;
            default:
                toast(<ToastMessage message={translatedMessage} />, options);
        }
    }
    const toastException = (exception) => {
        const translatedMessage = props.t(exception.message);

        toastMessage(notificationType.error, exception.name + ": " + translatedMessage);
    }
    const toastNotifications = (notifications) => {
        if (!notifications)
            return;

        notifications.map(d => {
            return toastMessage(d.type, d.message, null, d.arguments);
        });
    }

    const generateReport = (name, parameters) => {
        return props.generateReport(name, parameters)
            .then(response => {
                try {
                    if (response.Status === "ERROR")
                        throw new ComponentError(response.MessageArguments, response.Message);

                    window.location = "/api/Report/DownloadReport";
                }
                catch (ex) {
                    toastException(ex);
                }
            });
    }

    const setBottomFillerHeight = (height) => {
        setPageBottomFillerHeight(height);
    }
    const getBottomFillerViewportPosition = () => {
        if (bottomFillerRef.current) {
            return bottomFillerRef.current.getBoundingClientRect().top;
        }

        return 0;
    }

    const showConfirmation = ({ message, onAccept, onCancel, acceptLabel, cancelLabel, acceptVariant, cancelVariant, argsMessage }) => {
        setConfirmationOpen(true);
        setConfirmationMessage(message);
        setConfirmationAcceptLabel(acceptLabel);
        setConfirmationCancelLabel(cancelLabel);
        setConfirmationAcceptVariant(acceptVariant);
        setConfirmationCancelVariant(cancelVariant);
        setConfirmationArgMessage(argsMessage);
        confirmationAcceptAction.current = onAccept;
        confirmationCancelAction.current = onCancel;
    }
    const closeConfirmation = () => {
        setConfirmationOpen(false);
    }

    const getPageToken = () => {
        return pageToken.current;
    }

    const getQueryParameters = () => {
        const entries = new URLSearchParams(window.location.search).entries();

        let result = [];

        for (let entry of entries) {
            result.push({
                id: entry[0],
                value: decodeURI(entry[1])
            });
        }

        return result;
    }

    const getNexus = () => {
        return {
            registerComponent: registerComponent,
            unregisterComponent: unregisterComponent,
            getComponents: getComponents,
            getComponent: getComponent,
            getGrid: getGrid,
            getForm: getForm,
            redirect: redirect,
            toast: toastMessage,
            toastException: toastException,
            toastNotifications: toastNotifications,
            showConfirmation: showConfirmation,
            setBottomFillerHeight: setBottomFillerHeight,
            getBottomFillerViewportPosition: getBottomFillerViewportPosition,
            generateReport: generateReport,
            getPageToken: getPageToken,
            getQueryParameters: getQueryParameters
        };
    }

    if (requestError) {
        return (
            <ErrorPage errorCode={requestError} />
        );
    }

    if (isLoadComplete) {
        const { t } = props;

        const style = { fontSize: "28px" };

        const title = props.title ? (
            <div className="row">
                <div className="col-md-12">
                    <h2>
                        <span className={props.icon} style={style} /> {t(props.title)}
                    </h2>
                    <hr />
                </div>
            </div>
        ) : null;

        const renderTooManySessions = () => {
            if (tooManySessions) {
                return (
                    <div style={{ display: props.show !== undefined && !props.show ? 'none' : 'block' }} class="text-center alert alert-danger" role="alert">
                        {t("Master_Sec0_lbl_TooManySessions")}
                    </div>);
            } else {
                return null;
            }
        }

        return (
            <div className="base-page">
                {/*{title}*/}
                {renderTooManySessions()}
                <PageContextProvider value={getNexus()}>
                    {props.children}
                </PageContextProvider>
                <PageBottomFiller
                    ref={bottomFillerRef}
                    height={pageBottomFillerHeight}
                />
                <ConfirmationBox
                    isOpen={isConfirmationOpen}
                    message={confirmationMessage}
                    argsMessage={confirmationArgMessage}
                    onAccept={confirmationAcceptAction}
                    onCancel={confirmationCancelAction}
                    acceptLabel={confirmationAcceptLabel}
                    cancelLabel={confirmationCancelLabel}
                    acceptVariant={confirmationAcceptVariant}
                    cancelVariant={confirmationCancelVariant}
                    close={closeConfirmation}
                />
            </div>
        );
    }

    return null;
}

export const Page = withTranslation()(withReportDataProvider(withPageDataProvider(InternalPage)));