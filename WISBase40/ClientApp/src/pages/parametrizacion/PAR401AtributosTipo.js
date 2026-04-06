import React, { useState } from 'react';
import { Modal, Col, Row } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { StatusMessage, Field, FormButton, Form } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';
import { PAR401CrearAtributoTipoLpn } from './PAR401CrearAtributoTipoLpn'
import { PAR401CrearAtributoTipoLpnDet } from './PAR401CrearAtributoTipoLpnDet'
import { PAR401ModificarAtributoTipoLpn } from './PAR401ModificarAtributoTipoLpn'
import { PAR401ModificarAtributoTipoLpnDet } from './PAR401ModificarAtributoTipoLpnDet'

export default function PAR401AtributosTipo(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [TpLPNTipo, setTpLPNTipo] = useState("");
    const [NmLpnTipo, setNmLpnTipo] = useState("");
    const [showCreateModal, setshowModal] = useState(false);

    const [showPopupAddDet, setShowPopupAddDet] = useState(false);
    const [showCreateModalDet, setshowModalDet] = useState(false);

    const [showUpdateModal, setshowUpdateModal] = useState(false);
    const [showPopupUpdate, setshowPopupUpdate] = useState(false);
    const [IdAtributo, setIdAtributo] = useState("");

    const [_nexus, setnexus] = useState(null);

    const [showUpdateModalDet, setshowUpdateModalDet] = useState(false);
    const [showPopupUpdateDet, setshowPopupUpdateDet] = useState(false);

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        if (query.parameters.find(x => x.id === "LpnTipo") != null) {
            setTpLPNTipo(query.parameters.find(x => x.id === "LpnTipo").value);
            setNmLpnTipo(query.parameters.find(x => x.id === "Nombre").value);
        }
    };

    const handleOnBeforeFetch = (context, data, nexus) => {

        data.parameters = [
            { id: "LpnTipo", value: TpLPNTipo },
        ];
    };

    const handleOnBeforeFetchStats = (context, data, nexus) => {
        data.parameters = [
            { id: "LpnTipo", value: TpLPNTipo },

        ];
    };

    const closeFormDialogDet = (datos) => {
        setshowModalDet(false);
        setShowPopupAddDet(false);
        _nexus.getGrid("PAR401Atributos_grid_1").refresh();
        _nexus.getGrid("PAR401AtributosDet_grid_2").refresh();

    }

    const closeFormDialog = (datos) => {
        setshowModal(false);
        setShowPopupAdd(false);
        _nexus.getGrid("PAR401Atributos_grid_1").refresh();
        _nexus.getGrid("PAR401AtributosDet_grid_2").refresh();

    }

    const closeFormDialogUpdate = (datos) => {
        setshowUpdateModal(false);
        setshowPopupUpdate(false);
        _nexus.getGrid("PAR401Atributos_grid_1").refresh();
        _nexus.getGrid("PAR401AtributosDet_grid_2").refresh();
    }

    const closeFormDialogUpdateDet = (datos) => {
        setshowUpdateModalDet(false);
        setshowPopupUpdateDet(false);
        _nexus.getGrid("PAR401Atributos_grid_1").refresh();
        _nexus.getGrid("PAR401AtributosDet_grid_2").refresh();

    }

    const afterButtonAction = (context, nexus) => {
        if (context.buttonId === "btnDown" || context.buttonId === "btnUp") {
            nexus.getGrid("PAR401Atributos_grid_1").refresh();
            nexus.getGrid("PAR401AtributosDet_grid_2").refresh();
        } else if (context.buttonId === "btnEditarLpnTipoAtributo") {
            setIdAtributo(context.parameters.find(f => f.id === "ID_ATRIBUTO").value)
            setshowUpdateModal(true);
            setshowPopupUpdate(true);
        } else {
            setIdAtributo(context.parameters.find(f => f.id === "ID_ATRIBUTO").value)
            setshowUpdateModalDet(true);
            setshowPopupUpdateDet(true);
        }
    }

    const formOnBeforeButtonAction = (context, form, query, nexus) => {

        context.abortServerCall = true;
        if (query.buttonId === "btnLpnAtributo") {
            setshowModal(true);
            setShowPopupAdd(true);
        } else {
            setshowModalDet(true);
            setShowPopupAddDet(true);
        }
    };

    const onBeforeInitialize = (context, data, nexus) => {
        setnexus(nexus);
    }

    return (
        <Page
            title={t("PAR401AtributosTipo_Sec0_modalTitle_Titulo")}
            application="PAR401AtributosTipo"
            {...props}
        >

            <Form
                id="PAR401_form_CreatePedido"
                application="PAR401AtributosTipo"
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeButtonAction={formOnBeforeButtonAction}

            >
                <Row>
                    <Col>
                        <Row>
                            <Col>
                                <div className="form-group">
                                    <label htmlFor="TP_LPN_TIPO">{t("PAR401_frm1_lbl_TP_LPN_TIPO")}</label>
                                    <Field name="TP_LPN_TIPO" value={TpLPNTipo} readOnly />
                                    <StatusMessage for="TP_LPN_TIPO" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group">
                                    <label htmlFor="NM_LPN_TIPO">{t("PAR401_frm1_lbl_NM_LPN_TIPO")}</label>
                                    <Field name="NM_LPN_TIPO" value={NmLpnTipo} readOnly />
                                    <StatusMessage for="NM_LPN_TIPO" />
                                </div>
                            </Col>
                        </Row>
                        <br />
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <h4 className='form-title'>{t("PAR401_Sec0_Grid1Title_Titulo")}</h4>
                    </Col>
                </Row>
                <div className="form-group" style={{ textAlign: "center" }}>
                    <FormButton id="btnLpnAtributo" className="btn btn-primary" label="PAR401_Sec0_btn_AgregarAtributoLpn" />
                </div>
                <div className="row mb-4">
                    <div className="col-12">
                        <Grid id="PAR401Atributos_grid_1"
                            application="PAR401AtributosTipo"
                            rowsToFetch={30} rowsToDisplay={15}
                            onAfterButtonAction={afterButtonAction}
                            onBeforeFetch={handleOnBeforeFetch}
                            onBeforeFetchStats={handleOnBeforeFetchStats}
                            onBeforeInitialize={onBeforeInitialize}
                            enableExcelExport enableExcelImport={false}
                        />
                    </div>
                </div>
                <Row>
                    <Col>
                        <h4 className='form-title'>{t("PAR401_Sec0_Grid2Title_Titulo")}</h4>
                    </Col>
                </Row>
                <div className="form-group" style={{ textAlign: "center" }}>
                    <FormButton id="btnLpnAtributoDet" className="btn btn-primary" label="PAR401_Sec0_btn_AgregarAtributoLpnDet" />
                </div>
                <div className="row mb-4">
                    <div className="col-12">
                        <Grid id="PAR401AtributosDet_grid_2"
                            application="PAR401AtributosTipo"
                            rowsToFetch={30} rowsToDisplay={15}
                            onBeforeFetch={handleOnBeforeFetch}
                            onAfterButtonAction={afterButtonAction}
                            onBeforeFetchStats={handleOnBeforeFetchStats}
                            onBeforeInitialize={onBeforeInitialize}
                            enableExcelExport enableExcelImport={false}
                        />
                    </div>
                </div>
            </Form>
            <Modal show={showCreateModal} onHide={closeFormDialog} dialogClassName="modal-50w" backdrop="static" >
                <PAR401CrearAtributoTipoLpn show={showPopupAdd} onHide={closeFormDialog} LpnTipo={TpLPNTipo} />
            </Modal>
            <Modal show={showCreateModalDet} onHide={closeFormDialogDet} dialogClassName="modal-50w" backdrop="static" >
                <PAR401CrearAtributoTipoLpnDet show={showPopupAddDet} onHide={closeFormDialogDet} LpnTipo={TpLPNTipo} />
            </Modal>
            <Modal show={showUpdateModal} onHide={closeFormDialogUpdate} dialogClassName="modal-50w" backdrop="static" >
                <PAR401ModificarAtributoTipoLpn show={showPopupUpdate} onHide={closeFormDialogUpdate} LpnTipo={TpLPNTipo} IdAtributo={IdAtributo} />
            </Modal>

            <Modal show={showUpdateModalDet} onHide={closeFormDialogUpdateDet} dialogClassName="modal-50w" backdrop="static" >
                <PAR401ModificarAtributoTipoLpnDet show={showPopupUpdateDet} onHide={closeFormDialogUpdateDet} LpnTipo={TpLPNTipo} IdAtributo={IdAtributo} />
            </Modal>

        </Page>
    );
}