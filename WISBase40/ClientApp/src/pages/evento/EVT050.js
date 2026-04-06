import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { EVT050VerAdjuntos } from './EVT050VerAdjuntos';
import { EVT050VerMensaje } from './EVT050VerMensaje';

export default function EVT050(props) {
    const { t } = useTranslation();

    const [notificacion, setNotificacion] = useState("");
    const [showMensajePopup, setShowMensajePopup] = useState(false);
    const [showAdjuntosPopup, setShowAdjuntosPopup] = useState(false);

    const closePopup = () => {
        setShowMensajePopup(false);
        setShowAdjuntosPopup(false);
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        var notificacion = data.row.cells.find(w => w.column == "NU_EVENTO_NOTIFICACION").value;

        if (data.buttonId === "btnVerMensaje") {
            context.abortServerCall = true;
            setNotificacion(notificacion);
            setShowMensajePopup(true);
        } else if (data.buttonId === "btnVerAdjuntos") {
            context.abortServerCall = true;
            setNotificacion(notificacion);
            setShowAdjuntosPopup(true);
        } else if (data.buttonId === "btnReenviar") {
            data.parameters = [
                { id: "numeroNotificacion", value: data.row.cells.find(d => d.column === "NU_EVENTO_NOTIFICACION").value},
            ];
        }
    }

    const onAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnReenviar") {
            nexus.getGrid("EVT050_grid_1").refresh();
        }
    };

    return (

        <Page
            title={t("EVT050_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="EVT050_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeButtonAction={onBeforeButtonAction}
                        onAfterButtonAction={onAfterButtonAction}
                    /> 
                </div>
            </div>

            <EVT050VerAdjuntos
                show={showAdjuntosPopup}
                onHide={closePopup}
                notificacion={notificacion}
            />

            <EVT050VerMensaje
                show={showMensajePopup}
                onHide={closePopup}
                notificacion={notificacion}
            />
        </Page>
    );
}