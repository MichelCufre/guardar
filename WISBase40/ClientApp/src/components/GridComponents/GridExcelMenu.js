import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { exportExcelType } from '../Enums';

export function GridExcelMenu(props) {
    const { t } = useTranslation();
    const [show, setShow] = useState(false);

    const handleMenuShow = (evt) => {
        evt.preventDefault();

        setShow(current => !current);
    };

    const handleMenuLeave = () => {
        setShow(false);
    };

    const handleClickExportExcel = (evt) => {
        evt.preventDefault();

        if (props.enableExcelExport)
            props.exportExcel("excel", exportExcelType.filtered);
    };

    const handleClickImportExcel = (evt) => {
        evt.preventDefault();
        props.openImportExcelModal();
    };

    const style = {
        display: show ? "block" : "none"
    };

    const iconStyle = {
        marginLeft: "auto",
        marginTop: "auto",
        marginBottom: "auto"
    };

    const exportExcelClass = props.enableExcelExport ? "dropdown-item" : "dropdown-item disabled";
    //const importExcelClass = (props.enableExcelImport || (props.isEditingEnabled && props.isCommitEnabled && props.isAddEnabled)) ? "dropdown-item" : "dropdown-item disabled";
    const importExcelClass = props.enableExcelImport ? "dropdown-item" : "dropdown-item disabled";
    
    return (
        <React.Fragment>
            <div className="dropdown">
                <button key="btnExcel" className="gr-toolbar-btn excel" title={t("General_Sec0_lbl_Excel")} onClick={handleMenuShow}>
                    <i className="fas fa-file-excel" />
                </button>
                <div className="dropdown-menu" style={style} onMouseLeave={handleMenuLeave}>
                    <button className={exportExcelClass} style={{ display: "flex" }} onClick={handleClickExportExcel}>
                        <span>{t("General_Sec0_btn_ToolTip_exportExcel")}</span>
                    </button>
                    <button className={importExcelClass} style={{ display: "flex" }} onClick={handleClickImportExcel}>
                        <span>{t("General_Sec0_btn_ToolTip_importExcel")}</span>&nbsp;<i className="fas fa-external-link-alt" style={iconStyle} />
                    </button>
                </div>
            </div>
        </React.Fragment>
    );
}