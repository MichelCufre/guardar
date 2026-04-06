import React from 'react';
import { Button, Col, Modal, Row, Table } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { FormWarningMessage } from '../../components/FormComponents/FormWarningMessage';

export function GridGuideFilterModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const handleClose = () => {
        props.closeGuideFilterModal();
    };

    const filters = [
        { command: "IN", description: "General_Sec0_lbl_Filter_IN" },
        { command: "NOTIN", description: "General_Sec0_lbl_Filter_NOTIN" },
        { command: "BETWEEN", description: "General_Sec0_lbl_Filter_BETWEEN" },
        { command: "today", description: "General_Sec0_lbl_Filter_today" },
        { command: "yesterday", description: "General_Sec0_lbl_Filter_yesterday" },
        { command: "tomorrow", description: "General_Sec0_lbl_Filter_tomorrow" },
        { command: "daybeforeyesterday", description: "General_Sec0_lbl_Filter_daybeforeyesterday" },
        { command: "dayaftertomorrow", description: "General_Sec0_lbl_Filter_dayaftertomorrow" },
        { command: "startweek", description: "General_Sec0_lbl_Filter_startweek" },
        { command: "endweek", description: "General_Sec0_lbl_Filter_endweek" },
        { command: "startmonth", description: "General_Sec0_lbl_Filter_startmonth" },
        { command: "midmonth", description: "General_Sec0_lbl_Filter_midmonth" },
        { command: "endmonth", description: "General_Sec0_lbl_Filter_endmonth" },
        { command: "startyear", description: "General_Sec0_lbl_Filter_startyear" },
        { command: "midyear", description: "General_Sec0_lbl_Filter_midyear" },
        { command: "endyear", description: "General_Sec0_lbl_Filter_endyear" },
        { command: "day", description: "General_Sec0_lbl_Filter_day" },
    ];
    return (
        <Modal show={props.isGuideFilterModalOpen} dialogClassName="modal-40w" onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title style={{ overflowX: "auto" }}>{t("General_Sec0_btn_Tooltip_FilterTypes")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col style={{ overflowX: "auto" }}>
                        <Table className="table table-bordered ">
                            <thead>
                                <tr>
                                    <th style={{ width: "200px" }}>
                                        <h6 className="mb-0">{t("Filtro")}</h6>
                                    </th>
                                    <th><h6>{t("Descripción")}</h6></th>
                                </tr>
                            </thead>
                            <tbody>
                                {filters.map((f, i) => (
                                    <tr key={i} >
                                        <td className="px-2 py-1">{f.command}</td>
                                        <td className="px-2 py-1">{t(f.description)}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </Table>

                    </Col>
                </Row>
                <Row>
                    <FormWarningMessage message={t("General_Sec0_lbl_Filter_Warning")} show={true} />
                </Row>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}