import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';


export default function FAC256(props) {

    const { t } = useTranslation();

    const [infoListaPrecio, setInfoListaPrecio] = useState({
        CD_LISTA_PRECIO: "", DS_LISTA_PRECIO: ""
    });

    const [infoComponente, setInfoComponente] = useState({
        NU_COMPONENTE: "", CD_FACTURACION: "", DS_SIGNIFICADO: ""
    });

    const [showInfoListaPedido, setShowInfoListaPedido] = useState(false);
    const [showInfoComponente, setShowInfoComponente] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        if (parameters.find(x => x.id === "idListaPrecio") != null)
        {
            setShowInfoListaPedido(true);
            setShowInfoComponente(false);

            setInfoListaPrecio({
                CD_LISTA_PRECIO: parameters.find(x => x.id === "idListaPrecio").value,
                DS_LISTA_PRECIO: parameters.find(x => x.id === "descripcionListaPrecio").value,
            });
        }

        if (parameters.find(x => x.id === "nuComponente") != null)
        {
            setShowInfoComponente(true);
            setShowInfoListaPedido(false);

            setInfoComponente({
                NU_COMPONENTE: parameters.find(x => x.id === "nuComponente").value,
                CD_FACTURACION: parameters.find(x => x.id === "cdFacturacion").value,
                DS_SIGNIFICADO: parameters.find(x => x.id === "descripcionListaPrecio").value
            });
        }
    };

    return (
        <Page
            title={t("FAC256_Sec0_pageTitle_Titulo")}
            {...props}
        >

            
                <div style={{ display: showInfoListaPedido ? 'block' : 'none' }}>
                    <Row style={{ marginBottom: '6vh' }}>
                        <Col sm={2}>
                            <span style={{ fontWeight: "bold" }}>{t("FAC256_frm1_lbl_IdListaPrecio")}: </span>
                        </Col>
                        <Col className='p-0'>
                            <span>{`${infoListaPrecio.CD_LISTA_PRECIO ? infoListaPrecio.CD_LISTA_PRECIO : "-"}`}</span>
                        </Col>
                        <Col sm={2}>
                            <span style={{ fontWeight: "bold" }}>{t("FAC256_frm1_lbl_DescripcionLista")}: </span>
                        </Col>
                        <Col className='p-0'>
                            <span>{`${infoListaPrecio.DS_LISTA_PRECIO ? infoListaPrecio.DS_LISTA_PRECIO : "-"}`}</span>
                        </Col>
                    </Row>
                </div>

            <div style={{ display: showInfoComponente ? 'block' : 'none' }}>
                <Row style={{ marginBottom: '6vh' }}>
                    <Col sm={2}>
                        <span style={{ fontWeight: "bold" }}>{t("FAC256_frm1_lbl_NumeroComponte")}: </span>
                    </Col>
                    <Col className='p-0'>
                        <span>{`${infoComponente.NU_COMPONENTE ? infoComponente.NU_COMPONENTE : "-"}`}</span>
                    </Col>
                    <Col sm={2}>
                        <span style={{ fontWeight: "bold" }}>{t("FAC256_frm1_lbl_CodigoFacturacion")}: </span>
                    </Col>
                    <Col className='p-0'>
                        <span>{`${infoComponente.CD_FACTURACION ? infoComponente.CD_FACTURACION : "-"}`}</span>
                    </Col>
                    <Col sm={2}>
                        <span style={{ fontWeight: "bold" }}>{t("FAC256_frm1_lbl_Significado")}: </span>
                    </Col>
                    <Col className='p-0'>
                        <span>{`${infoComponente.DS_SIGNIFICADO ? infoComponente.DS_SIGNIFICADO : "-"}`}</span>
                    </Col>
                </Row>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="FAC256_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>

        </Page>
    );
}