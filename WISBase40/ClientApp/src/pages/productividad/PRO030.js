import React, { useState, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';
import './PRO030.css';

export default function PRO030(props) {

    const { t } = useTranslation();
    const [endpoint, setEndpoint] = useState(null);

    const onAfterPageLoad = (data) => {

        if (data.parameters.length > 0) {
            setEndpoint(data.parameters.find(x => x.id === "Endpoint").value)
        }
    }

    return (
        <Page
            title={t("PRO030_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterPageLoad}
            {...props}
        >
            {(endpoint) ? (
                <div id="embed-container-wis" class="embed-container">
                    <iframe src={endpoint} id="wis-frame"  ></iframe>
                </div>

            )
                : null}

        </Page>
    );
}