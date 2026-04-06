import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, FieldSelect, FieldSelectAsync, FieldTextArea, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Modal, Row, Col, FormGroup, Button, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { REC010CreateReferenciaModal } from './REC010CreateReferenciaModal';
import { REC010UpdateReferenciaModal } from './REC010UpdateReferenciaModal';

export default function REC010(props) {

    const { t } = useTranslation();

    const [showModal, setshowModal] = useState(false);

    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [keyReferencia, setkeyReferencia] = useState(null);
    const [showBotonCreate, setShowBotonCreate] = useState(true);

    const PageOnAfterLoad = (data) => {

        let PermiteDigitacion = data.parameters.find(p => p.id === "PermiteDigitacion");

        if (PermiteDigitacion) {

            if (PermiteDigitacion.value == "true") {
                setShowBotonCreate(false);
            }
        }
    }

    const openFormDialog = () => {
        setShowPopupAdd(true);

        setshowModal(true);
    }

    const closeFormDialog = (referencia) => {

        setShowPopupAdd(false);

        if (referencia) {

            setkeyReferencia(referencia);

            openFormUpdateDialog();

        } else {
            setshowModal(false);
        }
    }

    const openFormUpdateDialog = () => {
        setShowPopupUpdate(true);
        setshowModal(true);
    }

    const closeFormUpdateDialog = () => {
        setShowPopupUpdate(false);
        setshowModal(false);

    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setkeyReferencia(data.row.cells.find(w => w.column === "NU_RECEPCION_REFERENCIA").value);

            openFormUpdateDialog();
        }
    };

    const showFormCreate = () => { return (<REC010CreateReferenciaModal show={showPopupAdd} onHide={closeFormDialog} />); }
    const showFormUpdate = () => { return (<REC010UpdateReferenciaModal show={showPopupUpdate} onHide={closeFormUpdateDialog} referencia={keyReferencia} />); }

    return (

        <Page
            onAfterLoad={PageOnAfterLoad}
            title={t("Master_Menu_lbl_WRECEPCION_WREC010")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="AgregarReferencia" className="btn btn-primary" disabled={showBotonCreate} onClick={openFormDialog}>{t("REC010_Sec0_btn_AgregarLinea")}</button>
            </div>

            <Grid
                id="REC010_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={GridOnBeforeButtonAction}
            />

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-90w" backdrop="static" >

                {showPopupAdd ? showFormCreate() : null}
                {showPopupUpdate ? showFormUpdate() : null}

            </Modal>

        </Page>
    );
}
