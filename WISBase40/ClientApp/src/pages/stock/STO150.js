import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col, Modal, Button } from 'react-bootstrap';

export default function STO150(props) {
    const { t } = useTranslation();

    const [isTotalizador, setIsTotalizador] = useState(false);

    const [totalizador, setTotalizador] = useState({
        qtStock: "", qtReserva: "", qtTransito: ""
    })

    const [infoStock, setInfoStock] = useState({
        EMPRESA: "", PRODUCTO: "", EXCLUIR: "",
    });

    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);

    const onBeforeApplyFilter = (context, data, nexus) => {

        setTotalizador({
            qtStock: "",
            qtReserva: "",
            qtTransito: ""
        });
        setIsTotalizador(false);

    };



    const onAfterMenuItemAction = (context, data, nexus) => {
        if (data) {
            if (data.parameters.find(d => d.id === "QT_ESTOQUE") != null) {
                setTotalizador({
                    qtStock: data.parameters.find(d => d.id === "QT_ESTOQUE").value,
                    qtReserva: data.parameters.find(d => d.id === "QT_RESERVA_SAIDA").value,
                    qtTransito: data.parameters.find(d => d.id === "QT_TRANSITO_ENTRADA").value
                });

                setIsTotalizador(true);
            }
        } else {
            setTotalizador({
                qtStock: "",
                qtReserva: "",
                qtTransito: ""
            });
            setIsTotalizador(false);
        }

    };

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "STO150_EMPRESA") != null) {

            setInfoStock({
                EMPRESA: parameters.find(d => d.id === "STO150_EMPRESA").value,
                PRODUCTO: parameters.find(d => d.id === "STO150_PRODUCTO").value,
                EXCLUIR: parameters.find(d => d.id === "STO150_EXCLUIR") == true ? parameters.find(d => d.id === "STO150_EXCLUIR").value == "true" ? "STO150_frm1_lbl_ExluirSi" : "STO150_frm1_lbl_ExluirNo" : "STO150_frm1_lbl_ExluirNo"
            });

            setIsInfoDisplayed(true);
        }
    }


    return (
        <Page
            icon="fas fa-cubes"
            title={t("STO150_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }}>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("STO150_frm1_lbl_Empresa")}</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoStock.EMPRESA}`}</span>
                            </Col>

                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("STO150_frm1_lbl_Producto")}</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoStock.PRODUCTO}`}</span>
                            </Col>

                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={6}>
                                <span style={{ fontWeight: "bold" }}>{t("STO150_frm1_lbl_ExcluirAreaTran")}</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {t(`${infoStock.EXCLUIR}`)}</span>
                            </Col>

                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }}></hr>
            <div className="row mb-4">
                <div className="col">
                    <Grid
                        id="STO150_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeApplyFilter={onBeforeApplyFilter}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>

            <Modal
                show={isTotalizador}
                dialogClassName={"modal-30w"} backdrop="static"
            >
                <Modal.Header>
                    <Modal.Title>{t("STO150_mdl_title_Totales")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col sm={4}>
                            <span style={{ fontWeight: "bold" }}>{t("STO150_frm1_lbl_QT_ESTOQUE")}: </span>
                        </Col>
                        <Col className='p-0'>
                            <span> {t(`${totalizador.qtStock}`)}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col sm={4}>
                            <span style={{ fontWeight: "bold" }}>{t("STO150_frm1_lbl_QT_RESERVA_SAIDA")}: </span>
                        </Col>
                        <Col className='p-0'>
                            <span> {t(`${totalizador.qtReserva}`)}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col sm={4}>
                            <span style={{ fontWeight: "bold" }}>{t("STO150_frm1_lbl_QT_TRANSITO_ENTRADA")}: </span>
                        </Col>
                        <Col className='p-0'>
                            <span> {t(`${totalizador.qtTransito}`)}</span>
                        </Col>
                    </Row>
                    
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={onAfterMenuItemAction}>{t("General_Sec0_btn_Cerrar")}</Button>
                </Modal.Footer>
            </Modal>
        </Page>
    );
}