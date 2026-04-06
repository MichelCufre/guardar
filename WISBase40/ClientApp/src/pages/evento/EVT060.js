import React, { useState } from 'react';
import { Col, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { EVT060CreatePlantillaModal } from './EVT060CreatePlantillaModal';
import { EVT060UpdatePlantillaModal } from './EVT060UpdatePlantillaModal';

export default function EVT060(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [showModalCreate, setShowModalCreate] = useState(false);
    const [showModalUpdate, setShowModalUpdate] = useState(false);
    const [plantillaEditar, setPlantillaEditar] = useState({ nuEvento: '', tpNotificacion: '', cdPlantilla: 0 });
    const [isHtml, setIsHtml] = useState(false);

    const openModalCreate = () => {
        setShowModalCreate(true);
    }

    const closeModalCreate = () => {
        setShowModalCreate(false);
    }

    const closeModalUpdate = () => {
        setShowModalUpdate(false);
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        const nuEvento = data.row.cells.find(d => d.column === "NU_EVENTO").value;
        const tpNotificacion = data.row.cells.find(d => d.column === "TP_NOTIFICACION").value;
        const cdPlantilla = data.row.cells.find(d => d.column === "CD_LABEL_ESTILO").value;
        const flHtml = data.row.cells.find(d => d.column === "FL_HTML").value;

        if (flHtml === "S") {
            setIsHtml(true);
        } else {
            setIsHtml(false);
        }

        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;

            setPlantillaEditar({ nuEvento, tpNotificacion, cdPlantilla });

            setShowModalUpdate(true);

        }
        else {
            data.parameters = [
                { id: "nuEvento", value: nuEvento },
                { id: "tpNotificacion", value: tpNotificacion },
                { id: "cdPlantilla", value: cdPlantilla }
            ];

        }
    }

    return (
        <Page
            title={t("EVT060_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Row>
                <Col sm={5}>
                </Col>
                <Col sm={7} className="d-flex justify-content-left">
                    <button className="btn btn-primary" style={{ marginLeft: '7%' }} onClick={openModalCreate}>{t("EVT060_Sec0_btn_CrearPlantilla")}</button>
                </Col>
            </Row>
            <br>
            </br>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="EVT060_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeButtonAction={handleBeforeButtonAction}
                        enableExcelExport
                    />
                </div>
            </div>

            <EVT060CreatePlantillaModal show={showModalCreate} onHide={closeModalCreate} />
            <EVT060UpdatePlantillaModal show={showModalUpdate} onHide={closeModalUpdate} plantilla={plantillaEditar} isHtml={isHtml} />

        </Page>
    );
}