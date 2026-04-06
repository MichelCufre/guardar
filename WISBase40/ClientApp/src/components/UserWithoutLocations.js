import React from 'react';
import { Alert } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

export default function UserWithoutLocations(props) {
    const { t } = useTranslation();

    return (
        <Alert variant="danger" className="error-page-message">
            <div className="error-icon">
                <span className="fas fa-exclamation-triangle"></span>
            </div>
            <div className="error-content">
                <strong>{t("Master_Sec0_lbl_UserWithoutLocations")}</strong><br />
                {t("Master_Sec0_lbl_UserWithoutLocationsDetail")}
            </div>
        </Alert>
    );
}
