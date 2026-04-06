import React, { useState } from 'react';
import { Modal, Button } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

export function GridSaveFilterModal(props) {
    const [name, setName] = useState(null);
    const [description, setDescription] = useState(null);
    const [isGlobal, setIsGlobal] = useState(false);
    const [isDefault, setIsDefault] = useState(false);
    const [shouldIncludeSort, setShouldIncludeSort] = useState(true);
    const [isDisabled, setDisabled] = useState(false);
    const { t } = useTranslation("translation", { useSuspense: false });

    const handleClose = () => {
        props.closeSaveFilterModal();
    };

    const handleSave = () => {
        setDisabled(true);

        props.saveFilter({
            name: name,
            description: description,
            isGlobal: isGlobal,
            isDefault: isDefault,
            shouldIncludeSort: shouldIncludeSort
        }).then(d => {
            setDisabled(false);
            props.closeSaveFilterModal();
        });        
    };

    const handleChangeName = (evt) => {
        setName(evt.target.value);
    };
    const handleChangeDescription = (evt) => {
        setDescription(evt.target.value);
    };
    const handleChangeIsGlobal = () => {
        setIsGlobal(!isGlobal);
    };
    const handleChangeIsDefault = () => {
        setIsDefault(!isDefault);
    };
    const handleChangeShouldIncludeSort = () => {
        setShouldIncludeSort(!shouldIncludeSort);
    };

    return (
        <Modal show={props.isSaveFilterModalOpen} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>{t("General_Sec0_lbl_SAVE_FILTER_TITLE")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <div className="row">
                    <div className="col">
                        <div className="form-group">
                            <label htmlFor="saveFilterName">{t("General_Sec0_lbl_FILTER_NAME")}</label>
                            <input id="saveFilterName" className="form-control" onChange={handleChangeName} maxLength="20"/>
                        </div>
                        <div className="form-group">
                            <label htmlFor="saveFilterDescription">{t("General_Sec0_lbl_FILTER_DESCRIPTION")}</label>
                            <input id="saveFilterDescription" className="form-control" onChange={handleChangeDescription} maxLength="200"/>
                        </div>
                        <div className="form-check">
                            <input
                                type="checkbox"
                                id="saveFilterGlobal"
                                className="form-check-input"
                                checked={isGlobal}
                                onChange={handleChangeIsGlobal}
                            />
                            <label className="form-check-label" htmlFor="saveFilterGlobal">{t("General_Sec0_lbl_FILTER_GLOBAL")}</label>
                        </div>
                        <div className="form-check">
                            <input
                                type="checkbox"
                                id="saveFilterDefault"
                                className="form-check-input"
                                checked={isDefault}
                                onChange={handleChangeIsDefault}
                            />
                            <label className="form-check-label" htmlFor="saveFilterDefault">{t("General_Sec0_lbl_FILTER_INITIAL")}</label>
                        </div>
                        <div className="form-check">
                            <input
                                type="checkbox"
                                id="saveFilterIncludeSort"
                                className="form-check-input"
                                checked={shouldIncludeSort}
                                onChange={handleChangeShouldIncludeSort}
                            />
                            <label className="form-check-label" htmlFor="saveFilterIncludeSort">{t("General_Sec0_lbl_SAVE_FILTER_CHK_SORT")}</label>
                        </div>
                    </div>
                </div>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="danger" onClick={handleClose}>
                    {t("General_Sec0_btn_SAVE_FILTER_CANCEL")}
                </Button>
                <Button variant="primary" onClick={handleSave} disabled={isDisabled}>
                    {t("General_Sec0_btn_SAVE_FILTER_SAVE")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}