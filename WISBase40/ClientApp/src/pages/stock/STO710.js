import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { STO710ModificarLpnAtributo } from './STO710ModificarLpnAtributo'
import { STO710ModificarLpnDetAtributo } from './STO710ModificarLpnDetAtributo'
import { Modal, Container, Row, Col } from 'react-bootstrap';
export default function STO710(props) {

    const { t } = useTranslation();
    const [showModalUpdateAtributo, setshowModalUpdateAtributo] = useState(false);
    const [showPopupUpdateAtributo, setshowPopupUpdateAtributo] = useState(false);
    const [showModalUpdateAtributoDetalle, setshowModalUpdateAtributoDetalle] = useState(false);
    const [showPopupUpdateAtributoDetalle, setshowPopupUpdateAtributoDetalle] = useState(false);

    const [_nuLpn, setNuLpn] = useState(false);
    const [_tpLpn, setTpLpn] = useState(false);
    const [_idAtributo, setIdAtributo] = useState(false);
    const [_vlLpnAtributo, setVlLpnAtributo] = useState(false);

    const [_cdProduto, setCdProduto] = useState(false);
    const [_cdFaixa, setCdFaixa] = useState(false);
    const [_cdEmpresa, setCdEmpresa] = useState(false);
    const [_nuIdentificador, setNuIdentificador] = useState(false);
    const [_idLpnDet, setIdLpnDet] = useState(false);

    const [_nexus, setnexus] = useState(null);
    const [_detalle, setDetalle] = useState(null);
    const [detalleClassName, setDetalleClassName] = useState("hidden");
    const [cabezalClassName, setCabezalClassName] = useState("hidden");

    const [isInfoCabezalLpnDisplayed, setIsInfoCabezalLpnDisplayed] = useState(false);

    const [infoCabezalLpn, setInfoCabezalLpn] = useState({
        numeroLpn: "", tipoLpn: "", idLpnExterno: ""
    });

    const [isInfoDetalleLpnDisplayed, setIsInfoDetalleLpnDisplayed] = useState(false);

    const [infoDetalleLpn, setInfoDetalleLpn] = useState({
        numeroLpn: "", tipoLpn: "", idLpnExterno: "", idLpnDet: "", idLineaSistemaExterno:""
    });


    const onBeforeInitialize = (context, data, nexus) => {
        setnexus(nexus);
    }

    const onBeforeInitializeLogs = (context, data, nexus) => {
        context.parameters = [
            { id: "detalle", value: _detalle },
        ];
    }

    const closeFormDialogUpdateAtributo = (datos) => {
        setshowModalUpdateAtributo(false);
        setshowPopupUpdateAtributo(false);
        _nexus.getGrid("STO710_grid_1").refresh();
    }
    const closeFormDialogUpdateAtributoDetalle = (datos) => {
        setshowModalUpdateAtributoDetalle(false);
        setshowPopupUpdateAtributoDetalle(false);
        _nexus.getGrid("STO710_grid_1").refresh();
    }

    const onAfterButtonAction = (context, nexus) => {

        if (context.buttonId === "btnEditar" && context.parameters.find(f => f.id === "error") === undefined) {
            setNuLpn(context.parameters.find(f => f.id === "NU_LPN").value)
            setTpLpn(context.parameters.find(f => f.id === "TP_LPN_TIPO").value)
            setIdAtributo(context.parameters.find(f => f.id === "ID_ATRIBUTO").value)
            setVlLpnAtributo(context.parameters.find(f => f.id === "VL_LPN_ATRIBUTO").value)

            if (context.parameters.find(f => f.id === "CD_PRODUTO").value === "") {
                setshowModalUpdateAtributo(true);
                setshowPopupUpdateAtributo(true);
            } else {
                setIdLpnDet(context.parameters.find(f => f.id === "ID_LPN_DET").value)
                setCdProduto(context.parameters.find(f => f.id === "CD_PRODUTO").value)
                setCdFaixa(context.parameters.find(f => f.id === "CD_FAIXA").value)
                setCdEmpresa(context.parameters.find(f => f.id === "CD_EMPRESA").value)
                setNuIdentificador(context.parameters.find(f => f.id === "NU_IDENTIFICADOR").value)
                setshowModalUpdateAtributoDetalle(true);
                setshowPopupUpdateAtributoDetalle(true);
            }
        }
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(f => f.id === "detalle") !== undefined) {

            switch (parameters.find(p => p.id === "detalle").value) {
                case 'true':
                    setDetalleClassName("");
                    setCabezalClassName("hidden");

                    if (parameters.find(d => d.id === "STO710_NU_LPN") != null) {

                        setInfoDetalleLpn({
                        numeroLpn: parameters.find(d => d.id === "STO710_NU_LPN").value,
                        tipoLpn: parameters.find(d => d.id === "STO710_TP_LPN_TIPO").value,
                        idLpnExterno: parameters.find(d => d.id === "STO710_ID_LPN_EXTERNO").value,
                        idLpnDet: parameters.find(d => d.id === "STO710_ID_LPN_DET").value,
                        idLineaSistemaExterno: parameters.find(d => d.id === "STO710_ID_LINEA_SISTEMA_EXTERNO").value,
                        });

                        setIsInfoCabezalLpnDisplayed(false);
                        setIsInfoDetalleLpnDisplayed(true);
                    }
                    
                    break;

                case 'false':
                    setDetalleClassName("hidden");
                    setCabezalClassName("");

                    if (parameters.find(d => d.id === "STO710_NU_LPN") != null) {

                        setInfoCabezalLpn({
                        numeroLpn: parameters.find(d => d.id === "STO710_NU_LPN").value,
                        tipoLpn: parameters.find(d => d.id === "STO710_TP_LPN_TIPO").value,
                        idLpnExterno: parameters.find(d => d.id === "STO710_ID_LPN_EXTERNO").value,
                        });

                        setIsInfoCabezalLpnDisplayed(true);
                        setIsInfoDetalleLpnDisplayed(false);
                    }

                    break;
            }
        }
    };

    const isDetalle = detalleClassName != "hidden";
    const isCabezal = cabezalClassName != "hidden";
    const gridLogCabezalClassName = `row ${cabezalClassName}`;
    const gridLogDetalleClassName = `row ${detalleClassName}`;
    const gridAtributosClassName = 'form-title' + (isCabezal || isDetalle ? '' : ' hidden');
    const gridAtributosTitle = isDetalle ? 'STO710_grid1_title_AtributosDetalle' : (isCabezal ? 'STO710_grid1_title_AtributosCabezal' : '');

    return (
        <Page
            title={t("STO710_Sec0_pageTitle_Titulo")}
            {...props}
        >

         <Container fluid style={{ display: isInfoCabezalLpnDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("STO710_Sec0_Info_NumeroLPN")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalLpn.numeroLpn}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("STO710_Sec0_Info_TipoLPN")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalLpn.tipoLpn}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("STO710_Sec0_Info_IdExterno")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoCabezalLpn.idLpnExterno}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>

            <Container fluid style={{ display: isInfoDetalleLpnDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("STO710_Sec0_Info_NumeroLPN")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalleLpn.numeroLpn}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("STO710_Sec0_Info_TipoLPN")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalleLpn.tipoLpn}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("STO710_Sec0_Info_IdExterno")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoDetalleLpn.idLpnExterno}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("STO710_Sec0_Info_IdDetalle")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalleLpn.idLpnDet}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("STO710_Sec0_Info_IdSistemaLinea")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalleLpn.idLineaSistemaExterno}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>

           <hr style={{ display: isInfoCabezalLpnDisplayed || isInfoDetalleLpnDisplayed ? 'block' : 'none' }}></hr>

            <div className="row mb-4">
                <div className="col-12">
                    <h4 className={gridAtributosClassName}>{t(gridAtributosTitle)}</h4>
                    <Grid
                        id="STO710_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport={true}
                        onBeforeInitialize={onBeforeInitialize}
                        onAfterButtonAction={onAfterButtonAction}
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>

            <div className={gridLogDetalleClassName}>
                <div className="col-12">
                    <h4 className='form-title'>{t("STO700_Sec0_logs_TituloDetalle")}</h4>
                    <Grid
                        id="STO710_grid_logsDetalle"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport={true}
                        onBeforeInitialize={onBeforeInitializeLogs}
                    />
                </div>
            </div>

            <div className={gridLogCabezalClassName}>
                <div className="col-12">
                    <h4 className='form-title'>{t("STO700_Sec0_logs_TituloCabezal")}</h4>
                    <Grid
                        id="STO710_grid_logsCabezal"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport={true}
                        onBeforeInitialize={onBeforeInitializeLogs}
                    />
                </div>
            </div>

            <Modal show={showModalUpdateAtributo} onHide={closeFormDialogUpdateAtributo} dialogClassName="modal-50w" backdrop="static" >
                <STO710ModificarLpnAtributo show={showPopupUpdateAtributo} onHide={closeFormDialogUpdateAtributo} NuLpn={_nuLpn} LpnTipo={_tpLpn} IdAtributo={_idAtributo} VlLpnAtributo={_vlLpnAtributo} />
            </Modal>
            <Modal show={showModalUpdateAtributoDetalle} onHide={closeFormDialogUpdateAtributoDetalle} dialogClassName="modal-50w" backdrop="static" >

                <STO710ModificarLpnDetAtributo show={showPopupUpdateAtributoDetalle} onHide={closeFormDialogUpdateAtributoDetalle}
                    NuLpn={_nuLpn} LpnTipo={_tpLpn} IdAtributo={_idAtributo} VlLpnAtributo={_vlLpnAtributo} CdProduto={_cdProduto} CdEmpresa={_cdEmpresa}
                    NuIdentificador={_nuIdentificador} IdLpnDet={_idLpnDet} cdFaixa={_cdFaixa}
                />
            </Modal>
        </Page>
    );
}