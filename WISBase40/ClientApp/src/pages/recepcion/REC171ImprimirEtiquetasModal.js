import React, { useRef, useState } from 'react';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import {
	FieldSelect,
	FieldTextArea,
	Form,
	StatusMessage,
	SubmitButton,
} from "../../components/FormComponents/Form";
import * as Yup from "yup";
import { useCustomTranslation } from "../../components/TranslationHook";

function REC171ImprimirEtiquetasModal (props) {
	const {t} = useCustomTranslation ();

	const infoCantidadImpresiones = useRef ({});
	const refCantidadImpresiones = useRef ({});

	const [
		isCantidadImpresionesDisplayed,
		setCantidadImpresionesDisplayed] = useState (false);

	const initialValues = {
		impresora: "",
		lenguaje: "",
		desc_lenguaje: "",
		estilo: "",
		tipo_barras: "",
		modalidad: "",
	};

	const validationSchema = {
		impresora: Yup.string (),
		lenguaje: Yup.string (),
		desc_lenguaje: Yup.string (),
		estilo: Yup.string (),
		tipo_barras: Yup.string (),
		modalidad: Yup.string (),
	};

	const addParameters = (context, data, query) => {
		query.parameters.push ({
			id: "SELECTED_KEYS",
			value: props.selectedKeys,
		});
	};

	const handleOnBeforeInitialize = (context, data, query) => {
		refCantidadImpresiones.current = 0;
		setCantidadImpresionesDisplayed(false);
	};
	
	const handleOnAfterInitialize = (context, data, query, nexus) => {
		addParameters(context, data, query);
		let { options: arrayModalidad } = data.fields.find(x => x.id === "modalidad");

		for (let i = 0; i < arrayModalidad.length; i++) {
			arrayModalidad[i].label = t (arrayModalidad[i].label);
		}
	}

	const handleFormBeforeSubmit = (context, form, query, nexus) => {
		query.parameters.push ({
			id: "SELECTED_KEYS",
			value: props.selectedKeys,
		},
		{
			id: "CANT_PRINT",
			value: refCantidadImpresiones.current,
		});
	};

	const handleFormAfterSubmit = (context, form, query, nexus) => {
		if (context.responseStatus === "OK") {
			nexus.getGrid ("REC171_grid_1").refresh ();
			props.onHide ();
			refCantidadImpresiones.current = 0;
			setCantidadImpresionesDisplayed (false);
		}
	};

	const handleFormAfterValidateField = (context, form, query, nexus) => {
		if (query.fieldId === "modalidad") {
			refCantidadImpresiones.current = 0;
			setCantidadImpresionesDisplayed (false);

			const infoCantidad = query.parameters.find (x => x.id === "MENSAJE_IMPRESION");
			const cantidadImpresion = query.parameters.find (x => x.id === "CANTIDAD_IMPRESION");

			if (cantidadImpresion) {
				refCantidadImpresiones.current = cantidadImpresion.value;
				infoCantidadImpresiones.current = t (infoCantidad.value, [cantidadImpresion.value]);
				setCantidadImpresionesDisplayed (true);
			}
		}
	};

	const handleClose = () => props.onHide ();

	return (
		<Modal
			show={ props.show }
			onHide={ handleClose }
			dialogClassName="modal-50w"
		>
			<Form
				id="REC171Imprimir_form_1"
				application="REC171Imprimir"
				initialValues={ initialValues }
				validationSchema={validationSchema}
				onBeforeInitialize={ handleOnBeforeInitialize }
				onAfterInitialize={handleOnAfterInitialize}
				onBeforeSubmit={ handleFormBeforeSubmit }
				onAfterSubmit={ handleFormAfterSubmit }
				onAfterValidateField={ handleFormAfterValidateField }
				onBeforeValidateField={ addParameters }
			>
				<Modal.Header closeButton>
					<Modal.Title>
						{ t ("REC171_frm1_title_ImprimirEtiqueta") }
					</Modal.Title>
				</Modal.Header>
				<Modal.Body>
					<Row>
						<Col>
							<div className="form-group">
								<label htmlFor="impresora">
									{ t ("REC171_frm1_lbl_impresora") }
								</label>
								<FieldSelect name="impresora"/>
								<StatusMessage for="impresora"/>
							</div>
						</Col>
						<Col>
							<div className="form-group">
								<label htmlFor="lenguaje">
									{ t ("REC171_frm1_lbl_lenguaje") }
								</label>
								<FieldTextArea name="lenguaje" readOnly/>
								<StatusMessage for="lenguaje"/>
							</div>
						</Col>
						<Col>
							<div className="form-group">
								<label htmlFor="desc_lenguaje">
									{ t ("REC171_frm1_lbl_desc_lenguaje") }
								</label>
								<FieldTextArea name="desc_lenguaje"
											   readOnly/>
								<StatusMessage for="desc_lenguaje"/>
							</div>
						</Col>
					</Row>
					<Row>
						<div className="form-group">
							<label htmlFor="estilo">
								{ t ("REC171_frm1_lbl_estilo") }
							</label>
							<FieldSelect name="estilo"/>
							<StatusMessage for="estilo"/>
						</div>
					</Row>
					<Row>
						<div className="form-group">
							<label htmlFor="tipo_barras">
								{ t ("REC171_frm1_lbl_tipoBarras") }
							</label>
							<FieldSelect name="tipo_barras"/>
							<StatusMessage for="tipo_barras"/>
						</div>
					</Row>
					<Row>
						<div className="form-group">
							<label htmlFor="modalidad">
								{ t ("REC171_frm1_lbl_modalidad") }
							</label>
							<FieldSelect name="modalidad"/>
							<StatusMessage for="modalidad"/>
						</div>
					</Row>
					<Col style={ {
						textAlign: "right",
						justifyContent: 'center'
					} }>

						<div style={
							{
								display:
									isCantidadImpresionesDisplayed
										? 'block'
										: 'none'
							}
						}>
							<span className="text-info">
								{ `${ infoCantidadImpresiones.current }` }
							</span>
						</div>
					</Col>
				</Modal.Body>
				<Modal.Footer>
					<Button variant="secondary" onClick={ handleClose }>
						{ t ("REC171_frm1_btn_Cancelar") }
					</Button>
					<SubmitButton
						id="btnSubmit"
						variant="primary"
						label="REC171_frm1_btn_Imprimir"/>
				</Modal.Footer>
			</Form>
		</Modal>
	);
}

export default REC171ImprimirEtiquetasModal;
