import React from 'react';
import { AccessDeniedPage } from './ErrorPages/AccessDeniedPage';
import { UnexpectedErrorPage } from './ErrorPages/UnexpectedErrorPage';

export function ErrorPage(props) {
    console.log(props.errorCode);

    switch (props.errorCode) {
        case "403": return (<AccessDeniedPage />);
    }

    return (<UnexpectedErrorPage />);
}