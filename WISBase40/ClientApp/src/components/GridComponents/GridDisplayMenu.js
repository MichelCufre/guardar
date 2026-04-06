import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { GridShowHideColumnsModal } from './GridShowHideColumnsModal';

export function GridDisplayMenu(props) {
    const { t } = useTranslation();
    const [show, setShow] = useState(false);
    const [showModal, setShowModal] = useState(false);

    const handleShowModal = (evt) => {
        evt.preventDefault();

        setShow(prevShow => !prevShow);
    };

    const handleCloseModal = () => {
        setShowModal(false);
    };

    const handleMenuLeave = () => {
        setShow(false);
    };

    const onClickShowHideColumns = (evt) => {
        evt.preventDefault();

        setShowModal(true);
    };

    const onClickChangesOnly = (evt) => {
        evt.preventDefault();

        props.toggleDisplayModifiedOnly();
    };

    const onClickSelectionOnly = (evt) => {
        evt.preventDefault();

        props.toggleDisplaySelectionOnly();
    };

    const style = {
        display: show ? "block" : "none"
    };

    const iconStyle = {
        marginLeft: "auto",
        marginTop: "auto",
        marginBottom: "auto"
    };

    const disabledIconStyle = {
        ...iconStyle,
        color: "#eaeaea"
    };

    const selectionOnlyIcon = props.displaySelectionOnly ? <i className="fas fa-check-square" style={iconStyle} /> : <i className="fas fa-square" style={disabledIconStyle} />;
    const modifiedOnlyIcon = props.displayModifiedOnly ? <i className="fas fa-check-square" style={iconStyle} /> : <i className="fas fa-square" style={disabledIconStyle} />;

    const selectionOnlyButtonClass = `dropdown-item ${props.enableSelection ? "" : "hidden"}`;

    return (
        <React.Fragment>
            <div className="dropdown">
                <button key="showHideColumns" className="gr-toolbar-btn" onClick={handleShowModal} title={t("General_Sec0_lbl_Visualizar")}>
                    <i className="fas fa-eye" />
                </button>
                <div className="dropdown-menu" style={style} onMouseLeave={handleMenuLeave}>
                    <h6 className="dropdown-header">{t("General_Sec0_lbl_Visualizar")}</h6>
                    <button className="dropdown-item" style={{ display: "flex" }} onClick={onClickShowHideColumns}>
                        <span>{t("General_Sec0_lbl_Columnas")}</span><i className="fas fa-external-link-alt" style={iconStyle} />
                    </button>
                    <button className="dropdown-item" style={{ display: "flex" }} onClick={onClickChangesOnly}>
                        <span>{t("General_Sec0_lbl_SoloCambios")}</span>{modifiedOnlyIcon}
                    </button>
                    <button className={selectionOnlyButtonClass} style={{ display: "flex" }} onClick={onClickSelectionOnly}>
                        <span>{t("General_Sec0_lbl_SoloSeleccion")}</span>{selectionOnlyIcon}
                    </button>
                </div>
            </div>
            <GridShowHideColumnsModal
                columns={props.columns}
                show={showModal}
                onClose={handleCloseModal}
                showColumn={props.showColumn}
                hideColumn={props.hideColumn}
            />
        </React.Fragment>
    );
}