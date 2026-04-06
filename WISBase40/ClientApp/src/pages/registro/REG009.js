import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col } from 'react-bootstrap';
import REG009Create from './REG009Create';
import REG009Update from './REG009Update';
import REG009Grupo from './REG009Grupo';
import { IMP060Modal } from '../impresion/IMP060';

export default function REG009(props) {

    const { t } = useTranslation();

    const [infoCompleta, setInfoCompleta] = useState({
        NM_EMPRESA: "", CD_EMPRESA: "", CD_PRODUCTO: "", DS_PRODUCTO: ""
    });

    const [infoEmpresa, setInfoEmpresa] = useState({
        NM_EMPRESA: "", CD_EMPRESA: ""
    });

    const [productoSeleccionado, setProductoSeleccionado] = useState(null);

    const [isInfoCompletaDisplayed, setInfoCompletaDisplayed] = useState(false);

    const [isInfoEmpresaOnly, setIsInfoEmpresaOnly] = useState(false);

    const [showPopupCreate, setshowPopupCreate] = useState(false);

    const [showPopupUpdate, setshowPopupUpdate] = useState(false);

    const [showPopupGrupo, setShowPopupGrupo] = useState(false);

    const [showPopupImprimir, setShowPopupImprimir] = useState(false);

    const [rowSeleccionadasImprimir, setRowSeleccionadasImprimir] = useState(null);

    const [nexxus, setNexxus] = useState(null);

    const openModalCreate = () => {

        setshowPopupCreate(true);
    }

    const closeModalCreate = (nexus) => {
        setshowPopupCreate(false);
        if (nexus)
            nexus.getGrid("REG009_grid_1").refresh()
    }

    const openModalUpdate = () => {

        setshowPopupUpdate(true);
    }

    const closeModalUpdate = (nexus) => {
        setshowPopupUpdate(false);
        if (nexus)
            nexus.getGrid("REG009_grid_1").refresh()
    }

    const openModalGrupo = () => {
        setShowPopupGrupo(true);
    }

    const closeModalGrupo = (nexus) => {
        setShowPopupGrupo(false);
    }

    const openImprimirDialog = () => {
        setShowPopupImprimir(true);
    }

    const closeImprimirDialog = () => {
        setShowPopupImprimir(false);
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {


        if ((parameters.find(d => d.id === "REG009_CD_EMPRESA") != null) && (parameters.find(d => d.id === "REG009_CD_PRODUCTO") != null)) {

            setInfoCompleta({
                CD_EMPRESA: parameters.find(d => d.id === "REG009_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REG009_NM_EMPRESA").value,
                CD_PRODUCTO: parameters.find(d => d.id === "REG009_CD_PRODUCTO").value,
                DS_PRODUCTO: parameters.find(d => d.id === "REG009_DS_PRODUCTO").value,
            });

            setInfoCompletaDisplayed(true);

        } else if (parameters.find(d => d.id === "REG009_CD_EMPRESA") != null) {
            setInfoEmpresa({
                CD_EMPRESA: parameters.find(d => d.id === "REG009_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REG009_NM_EMPRESA").value,
            });

            setIsInfoEmpresaOnly(true);
            setInfoCompletaDisplayed(false);

        } else {

            setInfoCompletaDisplayed(false);
            setIsInfoEmpresaOnly(false);
        }

        setNexxus(nexus);
    };

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;

            setProductoSeleccionado({
                Producto: data.row.cells.find(d => d.column === "CD_PRODUTO").value,
                Empresa: data.row.cells.find(d => d.column === "CD_EMPRESA").value
            });

            openModalUpdate();
        } else if (data.buttonId === "btnVerGrupo") {
            context.abortServerCall = true;

            setProductoSeleccionado({
                Producto: data.row.cells.find(d => d.column === "CD_PRODUTO").value,
                Empresa: data.row.cells.find(d => d.column === "CD_EMPRESA").value
            });

            openModalGrupo();
        }
        else if (data.buttonId === "btnVerGrupo") {
            context.abortServerCall = true;

            setProductoSeleccionado({
                Producto: data.row.cells.find(d => d.column === "CD_PRODUTO").value,
                Empresa: data.row.cells.find(d => d.column === "CD_EMPRESA").value
            });

            openModalGrupo();
        }
    }
    const GridOnBeforeMenuItemAction = (context, data, nexus) => {
        if (data.parameters.find(x => x.id === "producto") != null)
            data.parameters = [
                { id: "empresa", value: data.parameters.find(x => x.id === "empresa").value },
                { id: "producto", value: data.parameters.find(x => x.id === "producto").value }
            ];
    };

    const GridOnAfterMenuItemAction = (context, data, nexus) => {

        let jsonAdded = data.parameters.find(w => w.id === "ListaFilasSeleccionadas").value;

        setRowSeleccionadasImprimir(jsonAdded);

        openImprimirDialog();
    }

    return (
        <Page
            title={t("REG009_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoCompletaDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REG009_Sec0_Info_Cabezal_Empresa")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCompleta.CD_EMPRESA}`} - {`${infoCompleta.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REG009_Sec0_Info_Cabezal_Producto")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoCompleta.CD_PRODUCTO}`} - {`${infoCompleta.DS_PRODUCTO}`} </span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                    </Col>

                </Row>
            </Container>
            <Container fluid style={{ display: isInfoEmpresaOnly ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("REG009_Sec0_Info_Cabezal_Empresa")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoEmpresa.CD_EMPRESA}`} - {`${infoEmpresa.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col sm={8}>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr style={{ display: isInfoEmpresaOnly || isInfoCompletaDisplayed ? 'block' : 'none' }}></hr>
            <div style={{ textAlign: "center" }}>
                <button id="btnNuevoProducto" style={{ display: 'none' }} className="btn btn-primary" onClick={openModalCreate}>{t("REG009_Sec0_btn_NuevoProducto")}</button>
            </div>

            <Grid
                id="REG009_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                enableSelection
                onAfterInitialize={onAfterInitialize}
                onBeforeButtonAction={handleBeforeButtonAction}
                onBeforeMenuItemAction={GridOnBeforeMenuItemAction}
                onAfterMenuItemAction={GridOnAfterMenuItemAction}
            />
            <REG009Create show={showPopupCreate} onHide={closeModalCreate} nexxus={nexxus} hideLoadError={true} opener={"btnNuevoProducto"} />
            <REG009Update show={showPopupUpdate} onHide={closeModalUpdate} producto={productoSeleccionado} nexxus={nexxus} hideLoadError={true} />
            <REG009Grupo show={showPopupGrupo} onHide={closeModalGrupo} producto={productoSeleccionado} nexxus={nexxus} hideLoadError={true} />
            <IMP060Modal show={showPopupImprimir} onHide={closeImprimirDialog} rowSeleccionadas={rowSeleccionadasImprimir} hideLoadError={true} />
        </Page>
    );
}