import React from 'react';
import { useTranslation } from 'react-i18next';
import { Loading } from '../Loading';

export function GridImportExcelButton(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    if (!props.isUploading) {                           
        const buttonClassName = props.isDraggingOver ? "excel-import-btn is-dragging" : "excel-import-btn";
        const buttonLabel = props.isDraggingOver ? t('IExcel_Modal_lbl_DropOver') : t('IExcel_Modal_lbl_DropZone');

        return (
            <React.Fragment>
                <input
                    id="excelImportField"
                    className="excel-import-input"
                    type="file"
                    accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    onChange={props.onChange}
                />
                <label
                    className={buttonClassName}
                    onDragOver={props.onDragOver}
                    onDragLeave={props.onDragLeave}
                    onDragEnter={props.onDragEnter}
                    onDragEnd={props.onDragEnd}
                    onDrop={props.onDrop}
                    htmlFor="excelImportField"
                >
                    <div className="excel-import-btn-content">
                        <i className="fas fa-upload"></i>
                        <span>{buttonLabel}</span>
                    </div>
                </label>
            </React.Fragment>
        )
    }
    else {
        const uploadMessage = t("IExcel_Modal_lbl_Uploading");

        return (
            <div className="excel-loading-container">
                <span>{uploadMessage}</span>
                <Loading />
            </div>
        )
    }
}