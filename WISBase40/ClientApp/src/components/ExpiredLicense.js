import React from 'react';
import { Alert } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

export default function ExpiredLicense(props) {
    const { t } = useTranslation();

    return (
        <Alert variant="danger" className="error-page-message">
            <div className="error-icon">
                <span className="fas fa-exclamation-triangle"></span>
            </div>
            <div className="error-content">
                <strong>{t("Master_Sec0_lbl_ExpiredLicense")}</strong><br />
                {t("Master_Sec0_lbl_ExpiredLicenseDetail")}
            </div>
        </Alert>
    );
}