import React, { useState, useEffect, Suspense } from 'react';
import { Route, Routes } from 'react-router';
import { Layout } from './components/Layout';
import { Loading } from './components/Loading';
import { ScrollContextProvider } from './components/GridComponents/ScrollContextProvider';
import { setLocale } from 'yup';
import MenuTablet from './components/MenuTablet';
import i18n from './i18n';

const Home = React.lazy(() => import('./components/Home'));

const UserWithoutLocations = React.lazy(() => import('./components/UserWithoutLocations'));

const TooManySessions = React.lazy(() => import('./components/TooManySessions'));
const ExpiredLicense = React.lazy(() => import('./components/ExpiredLicense'));
const InvalidLicense = React.lazy(() => import('./components/InvalidLicense'));

const COF010 = React.lazy(() => import('./pages/configuracion/COF010'));
const COF020 = React.lazy(() => import('./pages/configuracion/COF020'));
const COF025 = React.lazy(() => import('./pages/configuracion/COF025'));
const COF030 = React.lazy(() => import('./pages/configuracion/COF030'));
const COF040 = React.lazy(() => import('./pages/configuracion/COF040'));
const COF050 = React.lazy(() => import('./pages/configuracion/COF050'));
const COF060 = React.lazy(() => import('./pages/configuracion/COF060'));
const COF070 = React.lazy(() => import('./pages/configuracion/COF070'));
const COF100 = React.lazy(() => import('./pages/configuracion/COF100'));
const COF110 = React.lazy(() => import('./pages/configuracion/COF110'));
const CON010 = React.lazy(() => import('./pages/configuracion/CON010'));

const AUT100 = React.lazy(() => import('./pages/automatizacion/AUT100'));
const AUT101 = React.lazy(() => import('./pages/automatizacion/AUT101'));

const IMP010 = React.lazy(() => import('./pages/impresion/IMP010'));

const REG009 = React.lazy(() => import('./pages/registro/REG009'));
const REG015 = React.lazy(() => import('./pages/registro/REG015'));
const REG010 = React.lazy(() => import('./pages/registro/REG010'));
const REG020 = React.lazy(() => import('./pages/registro/REG020'));
const REG035 = React.lazy(() => import('./pages/registro/REG035'));
const REG036 = React.lazy(() => import('./pages/registro/REG036'));
const REG050 = React.lazy(() => import('./pages/registro/REG050'));
const REG040 = React.lazy(() => import('./pages/registro/REG040'));
const REG060 = React.lazy(() => import('./pages/registro/REG060'));
const REG070 = React.lazy(() => import('./pages/registro/REG070'));
const REG080 = React.lazy(() => import('./pages/registro/REG080'));
const REG090 = React.lazy(() => import('./pages/registro/REG090'));
const REG100 = React.lazy(() => import('./pages/registro/REG100'));
const REG104 = React.lazy(() => import('./pages/registro/REG104'));
const REG130 = React.lazy(() => import('./pages/registro/REG130'));
const REG140 = React.lazy(() => import('./pages/registro/REG140'));
const REG220 = React.lazy(() => import('./pages/registro/REG220'));
const REG221 = React.lazy(() => import('./pages/registro/REG221'));
const REG240 = React.lazy(() => import('./pages/registro/REG240'));
const REG250 = React.lazy(() => import('./pages/registro/REG250'));
const REG300 = React.lazy(() => import('./pages/registro/REG300'));
const REG310 = React.lazy(() => import('./pages/registro/REG310'));
const REG410 = React.lazy(() => import('./pages/registro/REG410'));
const REG601 = React.lazy(() => import('./pages/registro/REG601'));
const REG602 = React.lazy(() => import('./pages/registro/REG602'));
const REG603 = React.lazy(() => import('./pages/registro/REG603'));
const REG605 = React.lazy(() => import('./pages/registro/REG605'));
const REG700 = React.lazy(() => import('./pages/registro/REG700'));
const REG701 = React.lazy(() => import('./pages/registro/REG701'));
const REG702 = React.lazy(() => import('./pages/registro/REG702'));
const REG910 = React.lazy(() => import('./pages/registro/REG910'));

const PRE052 = React.lazy(() => import('./pages/preparacion/PRE052'));
const PRE060 = React.lazy(() => import('./pages/preparacion/PRE060'));
const PRE061 = React.lazy(() => import('./pages/preparacion/PRE061'));
const PRE080 = React.lazy(() => import('./pages/preparacion/PRE080'));
const PRE100 = React.lazy(() => import('./pages/preparacion/PRE100'));
const PRE110 = React.lazy(() => import('./pages/preparacion/PRE110'));
const PRE110AnulacionDetPedidoAtri = React.lazy(() => import('./pages/preparacion/PRE110AnulacionDetPedidoAtri'));
const PRE110AnulacionDetPedidoLpn = React.lazy(() => import('./pages/preparacion/PRE110AnulacionDetPedidoLpn'));
const PRE120 = React.lazy(() => import('./pages/preparacion/PRE120'));
const PRE130 = React.lazy(() => import('./pages/preparacion/PRE130'));
const PRE150 = React.lazy(() => import('./pages/preparacion/PRE150'));
const PRE152 = React.lazy(() => import('./pages/preparacion/PRE152'));
const PRE153 = React.lazy(() => import('./pages/preparacion/PRE153'));
const PRE154 = React.lazy(() => import('./pages/preparacion/PRE154'));
const PRE155 = React.lazy(() => import('./pages/preparacion/PRE155'));
const PRE156 = React.lazy(() => import('./pages/preparacion/PRE156'));
const PRE160 = React.lazy(() => import('./pages/preparacion/PRE160'));
const PRE161 = React.lazy(() => import('./pages/preparacion/PRE161'));
const PRE162 = React.lazy(() => import('./pages/preparacion/PRE162'));
const PRE170 = React.lazy(() => import('./pages/preparacion/PRE170'));
const PRE220 = React.lazy(() => import('./pages/preparacion/PRE220'));
const PRE221 = React.lazy(() => import('./pages/preparacion/PRE221'));
const PRE250 = React.lazy(() => import('./pages/preparacion/PRE250'));
const PRE350 = React.lazy(() => import('./pages/preparacion/PRE350'));
const PRE351 = React.lazy(() => import('./pages/preparacion/PRE351'));
const PRE360 = React.lazy(() => import('./pages/preparacion/PRE360'));
const PRE640 = React.lazy(() => import('./pages/preparacion/PRE640'));
const PRE660 = React.lazy(() => import('./pages/preparacion/PRE660'));
const PRE450 = React.lazy(() => import('./pages/preparacion/PRE450'));
const PRE460 = React.lazy(() => import('./pages/preparacion/PRE460'));
/*const PRE670 = React.lazy(() => import('./pages/preparacion/PRE670'));*/
const PRE680 = React.lazy(() => import('./pages/preparacion/PRE680'));
const PRE810 = React.lazy(() => import('./pages/preparacion/PRE810'));
const PRE811 = React.lazy(() => import('./pages/preparacion/PRE811'));
const PRE812 = React.lazy(() => import('./pages/preparacion/PRE812'));

const INV030 = React.lazy(() => import('./pages/inventario/INV030'));
const INV060 = React.lazy(() => import('./pages/inventario/INV060'));
const INV050 = React.lazy(() => import('./pages/inventario/INV050'));
const INV100 = React.lazy(() => import('./pages/inventario/INV100'));
const INV410 = React.lazy(() => import('./pages/inventario/INV410'));
const INV411 = React.lazy(() => import('./pages/inventario/INV411'));
const INV412 = React.lazy(() => import('./pages/inventario/INV412'));
const INV413 = React.lazy(() => import('./pages/inventario/INV413'));
const INV414 = React.lazy(() => import('./pages/inventario/INV414'));
const INV415 = React.lazy(() => import('./pages/inventario/INV415'));
const INV416 = React.lazy(() => import('./pages/inventario/INV416'));
const INV417 = React.lazy(() => import('./pages/inventario/INV417'));

const INT050 = React.lazy(() => import('./pages/interfaz/INT050'));
const INT100 = React.lazy(() => import('./pages/interfaz/INT100'));
const INT101 = React.lazy(() => import('./pages/interfaz/INT101'));
const INT102 = React.lazy(() => import('./pages/interfaz/INT102'));
const INT103 = React.lazy(() => import('./pages/interfaz/INT103'));
const INT104 = React.lazy(() => import('./pages/interfaz/INT104'));
const INT105 = React.lazy(() => import('./pages/interfaz/INT105'));
const INT106 = React.lazy(() => import('./pages/interfaz/INT106'));
const INT107 = React.lazy(() => import('./pages/interfaz/INT107'));
const INT108 = React.lazy(() => import('./pages/interfaz/INT108'));
const INT109 = React.lazy(() => import('./pages/interfaz/INT109'));

const REC010 = React.lazy(() => import('./pages/recepcion/REC010'));
const REC011 = React.lazy(() => import('./pages/recepcion/REC011'));
const REC150 = React.lazy(() => import('./pages/recepcion/REC150'));
const REC180 = React.lazy(() => import('./pages/recepcion/REC180'));
const REC171 = React.lazy(() => import('./pages/recepcion/REC171'));
const REC170 = React.lazy(() => import('./pages/recepcion/REC170'));
const REC200 = React.lazy(() => import('./pages/recepcion/REC200'));
const REC210 = React.lazy(() => import('./pages/recepcion/REC210'));
const REC250 = React.lazy(() => import('./pages/recepcion/REC250'));
const REC141 = React.lazy(() => import('./pages/recepcion/REC141'));
const REC300 = React.lazy(() => import('./pages/recepcion/REC300'));
const REC220 = React.lazy(() => import('./pages/recepcion/REC220'));
const REC270 = React.lazy(() => import('./pages/recepcion/REC270'));
const REC275 = React.lazy(() => import('./pages/recepcion/REC275'));
const REC280 = React.lazy(() => import('./pages/recepcion/REC280'));
const REC400 = React.lazy(() => import('./pages/recepcion/REC400'));
const REC410 = React.lazy(() => import('./pages/recepcion/REC410'));
const REC500GrillaDetalleFactura = React.lazy(() => import('./pages/recepcion/REC500GrillaDetalleFactura'));
const REC500 = React.lazy(() => import('./pages/recepcion/REC500'));
const SEG020 = React.lazy(() => import('./pages/seguridad/SEG020'));
const SEG030 = React.lazy(() => import('./pages/seguridad/SEG030'));
const SEG070 = React.lazy(() => import('./pages/seguridad/SEG070'));
const SEG210 = React.lazy(() => import('./pages/seguridad/SEG210'));

const STO005 = React.lazy(() => import('./pages/stock/STO005'));
const STO030 = React.lazy(() => import('./pages/stock/STO030'));
const STO060 = React.lazy(() => import('./pages/stock/STO060'));
const STO110 = React.lazy(() => import('./pages/stock/STO110'));
const STO150 = React.lazy(() => import('./pages/stock/STO150'));
const STO151 = React.lazy(() => import('./pages/stock/STO151'));
const STO152 = React.lazy(() => import('./pages/stock/STO152'));
const STO153 = React.lazy(() => import('./pages/stock/STO153'));
const STO395 = React.lazy(() => import('./pages/stock/STO395'));
const STO498 = React.lazy(() => import('./pages/stock/STO498'));
const STO210 = React.lazy(() => import('./pages/stock/STO210'));
const STO211 = React.lazy(() => import('./pages/stock/STO211'));
const STO310 = React.lazy(() => import('./pages/stock/STO310'));
const STO500 = React.lazy(() => import('./pages/stock/STO500'));
/*const STO640 = React.lazy(() => import('./pages/stock/STO640'));*/
const STO700 = React.lazy(() => import('./pages/stock/STO700'));
const STO710 = React.lazy(() => import('./pages/stock/STO710'));
const STO720 = React.lazy(() => import('./pages/stock/STO720'));
const STO721 = React.lazy(() => import('./pages/stock/STO721'));
const STO722 = React.lazy(() => import('./pages/stock/STO722'));
const STO730 = React.lazy(() => import('./pages/stock/STO730'));
const STO740 = React.lazy(() => import('./pages/stock/STO740'));
const STO750 = React.lazy(() => import('./pages/stock/STO750'));
const STO800 = React.lazy(() => import('./pages/stock/STO800'));
const STO810 = React.lazy(() => import('./pages/stock/STO810'));
const STO820 = React.lazy(() => import('./pages/stock/STO820'));

const EXP040 = React.lazy(() => import('./pages/expedicion/EXP040'));
const EXP041 = React.lazy(() => import('./pages/expedicion/EXP041'));
const EXP043 = React.lazy(() => import('./pages/expedicion/EXP043'));
const EXP045 = React.lazy(() => import('./pages/expedicion/EXP045'));
const EXP050 = React.lazy(() => import('./pages/expedicion/EXP050'));
const EXP051 = React.lazy(() => import('./pages/expedicion/EXP051'));
const EXP052 = React.lazy(() => import('./pages/expedicion/EXP052'));
const EXP090 = React.lazy(() => import('./pages/expedicion/EXP090'));
const EXP110 = React.lazy(() => import('./pages/expedicion/EXP110'));
const EXP150 = React.lazy(() => import('./pages/expedicion/EXP150'));
const EXP320 = React.lazy(() => import('./pages/expedicion/EXP320'));
const EXP330 = React.lazy(() => import('./pages/expedicion/EXP330'));
const EXP340 = React.lazy(() => import('./pages/expedicion/EXP340'));
const EXP400 = React.lazy(() => import('./pages/expedicion/EXP400'));

//const EVT000 = React.lazy(() => import('./pages/evento/EVT000'));
//const EVT001 = React.lazy(() => import('./pages/evento/EVT001'));
//const EVT002 = React.lazy(() => import('./pages/evento/EVT002'));
const EVT010 = React.lazy(() => import('./pages/evento/EVT010'));
const EVT020 = React.lazy(() => import('./pages/evento/EVT020'));
const EVT030 = React.lazy(() => import('./pages/evento/EVT030'));
const EVT040 = React.lazy(() => import('./pages/evento/EVT040'));
const EVT050 = React.lazy(() => import('./pages/evento/EVT050'));
const EVT060 = React.lazy(() => import('./pages/evento/EVT060'));

const PRO030 = React.lazy(() => import('./pages/productividad/PRO030'));

//const POR010 = React.lazy(() => import('./pages/porteria/POR010'));
//const POR020 = React.lazy(() => import('./pages/porteria/POR020'));
//const POR030 = React.lazy(() => import('./pages/porteria/POR030'));
//const POR060 = React.lazy(() => import('./pages/porteria/POR060'));
//const POR070 = React.lazy(() => import('./pages/porteria/POR070'));
//const POR040 = React.lazy(() => import('./pages/porteria/POR040'));
//const POR050 = React.lazy(() => import('./pages/porteria/POR050'));
//const POR080 = React.lazy(() => import('./pages/porteria/POR080'));

const PRD100 = React.lazy(() => import('./pages/produccion/PRD100'));
const PRD110 = React.lazy(() => import('./pages/produccion/PRD110'));
const PRD111 = React.lazy(() => import('./pages/produccion/PRD111'));
const PRD112 = React.lazy(() => import('./pages/produccion/PRD112'));
const PRD113 = React.lazy(() => import('./pages/produccion/PRD113'));
//const PRD120 = React.lazy(() => import('./pages/produccion/PRD120'));
//const PRD130 = React.lazy(() => import('./pages/produccion/PRD130'));
const PRD150 = React.lazy(() => import('./pages/produccion/PRD150'));
const PRD151 = React.lazy(() => import('./pages/produccion/PRD151'));
const PRD170 = React.lazy(() => import('./pages/produccion/PRD170'));
const PRD171 = React.lazy(() => import('./pages/produccion/PRD171'));
const PRD175 = React.lazy(() => import('./pages/produccion/PRD175'));
//const PRD180 = React.lazy(() => import('./pages/produccion/PRD180'));
//const PRD190 = React.lazy(() => import('./pages/produccion/PRD190'));
//const PRD191 = React.lazy(() => import('./pages/produccion/PRD191'));
//const PRD200 = React.lazy(() => import('./pages/produccion/PRD200'));
//const PRD210 = React.lazy(() => import('./pages/produccion/PRD210'));
//const PRD220 = React.lazy(() => import('./pages/produccion/PRD220'));
//const PRD230 = React.lazy(() => import('./pages/produccion/PRD230'));
//const PRD240 = React.lazy(() => import('./pages/produccion/PRD240'));
//const PRD250 = React.lazy(() => import('./pages/produccion/PRD250'));
//const PRD260 = React.lazy(() => import('./pages/produccion/PRD260'));
//const PRD400 = React.lazy(() => import('./pages/produccion/PRD400'));

const PAR110 = React.lazy(() => import('./pages/parametrizacion/PAR110'));
const PAR050 = React.lazy(() => import('./pages/parametrizacion/PAR050'));

const PAR401 = React.lazy(() => import('./pages/parametrizacion/PAR401'));
const PAR401AtributosTipo = React.lazy(() => import('./pages/parametrizacion/PAR401AtributosTipo'));

const PAR400 = React.lazy(() => import('./pages/parametrizacion/PAR400'));
const PAR604 = React.lazy(() => import('./pages/parametrizacion/PAR604'));

const FAC001 = React.lazy(() => import('./pages/facturacion/FAC001'));
const FAC002 = React.lazy(() => import('./pages/facturacion/FAC002'));
const FAC003 = React.lazy(() => import('./pages/facturacion/FAC003'));
const FAC004 = React.lazy(() => import('./pages/facturacion/FAC004'));
const FAC005 = React.lazy(() => import('./pages/facturacion/FAC005'));
const FAC006 = React.lazy(() => import('./pages/facturacion/FAC006'));
const FAC007 = React.lazy(() => import('./pages/facturacion/FAC007'));
const FAC008 = React.lazy(() => import('./pages/facturacion/FAC008'));
const FAC009 = React.lazy(() => import('./pages/facturacion/FAC009'));
const FAC010 = React.lazy(() => import('./pages/facturacion/FAC010'));
const FAC011 = React.lazy(() => import('./pages/facturacion/FAC011'));
const FAC012 = React.lazy(() => import('./pages/facturacion/FAC012'));
//const FAC200 = React.lazy(() => import('./pages/facturacion/FAC200'));
//const FAC230 = React.lazy(() => import('./pages/facturacion/FAC230'));
const FAC255 = React.lazy(() => import('./pages/facturacion/FAC255'));
const FAC256 = React.lazy(() => import('./pages/facturacion/FAC256'));
const FAC405 = React.lazy(() => import('./pages/facturacion/FAC405'));
const FAC249 = React.lazy(() => import('./pages/facturacion/FAC249'));
const FAC250 = React.lazy(() => import('./pages/facturacion/FAC250'));
const FAC251 = React.lazy(() => import('./pages/facturacion/FAC251'));

const ORT010 = React.lazy(() => import('./pages/ordenTarea/ORT010'));
const ORT020 = React.lazy(() => import('./pages/ordenTarea/ORT020'));
const ORT030 = React.lazy(() => import('./pages/ordenTarea/ORT030'));
const ORT040 = React.lazy(() => import('./pages/ordenTarea/ORT040'));
const ORT060 = React.lazy(() => import('./pages/ordenTarea/ORT060'));
const ORT070 = React.lazy(() => import('./pages/ordenTarea/ORT070'));
const ORT080 = React.lazy(() => import('./pages/ordenTarea/ORT080'));
const ORT090 = React.lazy(() => import('./pages/ordenTarea/ORT090'));
const ORT120 = React.lazy(() => import('./pages/ordenTarea/ORT120'));

const DOC020 = React.lazy(() => import('./pages/documento/DOC020'));
const DOC021 = React.lazy(() => import('./pages/documento/DOC021'));
const DOC035 = React.lazy(() => import('./pages/documento/DOC035'));
const DOC036 = React.lazy(() => import('./pages/documento/DOC036'));
const DOC080 = React.lazy(() => import('./pages/documento/DOC080'));
const DOC081 = React.lazy(() => import('./pages/documento/DOC081'));
const DOC082 = React.lazy(() => import('./pages/documento/DOC082'));
const DOC085 = React.lazy(() => import('./pages/documento/DOC085'));
const DOC095 = React.lazy(() => import('./pages/documento/DOC095'));
const DOC096 = React.lazy(() => import('./pages/documento/DOC096'));
const DOC260 = React.lazy(() => import('./pages/documento/DOC260'));
const DOC290 = React.lazy(() => import('./pages/documento/DOC290'));
const DOC300 = React.lazy(() => import('./pages/documento/DOC300'));
const DOC310 = React.lazy(() => import('./pages/documento/DOC310'));
//const DOC320 = React.lazy(() => import('./pages/documento/DOC320'));
const DOC330 = React.lazy(() => import('./pages/documento/DOC330'));
const DOC340 = React.lazy(() => import('./pages/documento/DOC340'));
const DOC350 = React.lazy(() => import('./pages/documento/DOC350'));
const DOC360 = React.lazy(() => import('./pages/documento/DOC360'));
const DOC361 = React.lazy(() => import('./pages/documento/DOC361'));
const DOC362 = React.lazy(() => import('./pages/documento/DOC362'));
const DOC363 = React.lazy(() => import('./pages/documento/DOC363'));
//const DOC400 = React.lazy(() => import('./pages/documento/DOC400'));
//const DOC401 = React.lazy(() => import('./pages/documento/DOC401'));
//const DOC402 = React.lazy(() => import('./pages/documento/DOC402'));
//const DOC410 = React.lazy(() => import('./pages/documento/DOC410'));
const DOC500 = React.lazy(() => import('./pages/documento/DOC500'));
//const DOC501 = React.lazy(() => import('./pages/documento/DOC501'));
const DOC100 = React.lazy(() => import('./pages/documento/DOC100'));

const PTL010 = React.lazy(() => import('./pages/ptl/PTL010PickToLight'));

const FUC001 = React.lazy(() => import('./components/FileUploadComponentModal'));

setLocale({
    mixed: {
        required: i18n.t("General_Sec0_Error_Error25")
    },
    string: {
        required: i18n.t("General_Sec0_Error_Error25"),
        max: i18n.t("General_Sec0_Error_LargoMaxExcedido")
    },
    number: {
        required: i18n.t("General_Sec0_Error_Error25")
    },
    date: {
        required: i18n.t("General_Sec0_Error_Error25")
    }
});

export default function App(props) {
    const [doneLoading, setDoneLoading] = useState(false);
    const [userName, setUserName] = useState("");
    const [userLanguage, setUserLanguage] = useState("");

    useEffect(() => {
        const isTooManySessions = window.location.pathname === "/TooManySessions";
        const isExpiredLicense = window.location.pathname === "/ExpiredLicense";
        const isInvalidLicense = window.location.pathname === "/InvalidLicense";
        const isUserWithoutLocations = window.location.pathname === "/UserWithoutLocations";

        const request = {
            method: "POST",
            cache: "no-cache",
            headers: {
                "Content-Type": "application/json",
                "X-Requested-With": "XMLHttpRequest",
                "PreventSessionUpdate": (isTooManySessions || isExpiredLicense || isInvalidLicense || isUserWithoutLocations),
            }
        };

        fetch("/api/Security/GetUserData", request)
            .then(d => {
                if (d.status === 401)
                    throw "Authorization Error";

                return d.json();
            })
            .then(d => {
                setUserName(d.UserName);
                setUserLanguage(d.Language);
                setDoneLoading(true);
            })
            .catch(d => {
                if (window.location.pathname !== "/api/Security/Logout")
                    window.location = "/api/Security/Logout";
            });

        fetch("/api/Image/GetFaviconName")
            .then(response => response.text())
            .then(d => {                
                const faviconElement = document.getElementById('favicon');

                if (faviconElement) {                    
                    const publicUrl = faviconElement.href.split('/favicon.ico')[0];
                    faviconElement.href = `${publicUrl}/${d}`;
                }
            })
            .catch(error => {
            });
    }, []);

    const changeLanguage = (language) => {
        setUserLanguage(language);

        const request = {
            method: "POST",
            cache: "no-cache",
            body: JSON.stringify({
                Language: language
            }),
            headers: {
                "Content-Type": "application/json",
                "X-Requested-With": "XMLHttpRequest"
            }
        };

        fetch("/api/Security/UpdateUserLanguage", request)
            .then(d => window.location.reload());
    };

    i18n.changeLanguage(userLanguage);

    document.title = i18n.t("Master_Sec0_metaTitle_MasterTitle");

    if (doneLoading) {
        return (
            <Layout
                userName={userName}
                userLanguage={userLanguage}
                changeLanguage={changeLanguage}
            >
                <ScrollContextProvider>
                    <Suspense fallback={<Loading />}>
                        <Routes>
                            <Route exact path='/Default' element={<Home {...props} />} />

                            <Route exact path='/UserWithoutLocations' element={<UserWithoutLocations {...props} />} />

                            <Route exact path='/TooManySessions' element={<TooManySessions {...props} />} />
                            <Route exact path='/ExpiredLicense' element={<ExpiredLicense {...props} />} />
                            <Route exact path='/InvalidLicense' element={<InvalidLicense {...props} />} />

                            <Route path='/Menu' render={() => <MenuTablet {...props} />} />

                            <Route path='/configuracion/COF010' element={<COF010 {...props} />} />
                            <Route path='/configuracion/COF020' element={<COF020 {...props} />} />
                            <Route path='/configuracion/COF025' element={<COF025 {...props} />} />
                            <Route path='/configuracion/COF030' element={<COF030 {...props} />} />
                            <Route path='/configuracion/COF040' element={<COF040 {...props} />} />
                            <Route path='/configuracion/COF050' element={<COF050 {...props} />} />
                            <Route path='/configuracion/COF060' element={<COF060 {...props} />} />
                            <Route path='/configuracion/COF070' element={<COF070 {...props} />} />
                            <Route path='/configuracion/COF100' element={<COF100 {...props} />} />
                            <Route path='/configuracion/COF110' element={<COF110 {...props} />} />
                            <Route path='/configuracion/CON010' element={<CON010 {...props} />} />

                            <Route path='/automatizacion/AUT100' element={<AUT100 {...props} />} />
                            <Route path='/automatizacion/AUT101' element={<AUT101 {...props} />} />

                            <Route path='/impresion/IMP010' element={<IMP010 {...props} />} />

                            <Route path='/registro/REG009' element={<REG009 {...props} />} />
                            <Route path='/registro/REG010' element={<REG010 {...props} />} />
                            <Route path='/registro/REG015' element={<REG015 {...props} />} />
                            <Route path='/registro/REG020' element={<REG020 {...props} />} />
                            <Route path='/registro/REG035' element={<REG035 {...props} />} />
                            <Route path='/registro/REG036' element={<REG036 {...props} />} />
                            <Route path='/registro/REG040' element={<REG040 {...props} />} />
                            <Route path='/registro/REG050' element={<REG050 {...props} />} />
                            <Route path='/registro/REG060' element={<REG060 {...props} />} />
                            <Route path='/registro/REG070' element={<REG070 {...props} />} />
                            <Route path='/registro/REG080' element={<REG080 {...props} />} />
                            <Route path='/registro/REG090' element={<REG090 {...props} />} />
                            <Route path='/registro/REG100' element={<REG100 {...props} />} />
                            <Route path='/registro/REG104' element={<REG104 {...props} />} />
                            <Route path='/registro/REG130' element={<REG130 {...props} />} />
                            <Route path='/registro/REG140' element={<REG140 {...props} />} />
                            <Route path='/registro/REG220' element={<REG220 {...props} />} />
                            <Route path='/registro/REG221' element={<REG221 {...props} />} />
                            <Route path='/registro/REG240' element={<REG240 {...props} />} />
                            <Route path='/registro/REG250' element={<REG250 {...props} />} />
                            <Route path='/registro/REG300' element={<REG300 {...props} />} />
                            <Route path='/registro/REG310' element={<REG310 {...props} />} />
                            <Route path='/registro/REG410' element={<REG410 {...props} />} />
                            <Route path='/registro/REG601' element={<REG601 {...props} />} />
                            <Route path='/registro/REG602' element={<REG602 {...props} />} />
                            <Route path='/registro/REG603' element={<REG603 {...props} />} />
                            <Route path='/registro/REG605' element={<REG605 {...props} />} />
                            <Route path='/registro/REG700' element={<REG700 {...props} />} />
                            <Route path='/registro/REG701' element={<REG701 {...props} />} />
                            <Route path='/registro/REG702' element={<REG702 {...props} />} />
                            <Route path='/registro/REG910' element={<REG910 {...props} />} />

                            <Route path='/preparacion/PRE052' element={<PRE052 {...props} />} />
                            <Route path='/preparacion/PRE060' element={<PRE060 {...props} />} />
                            <Route path='/preparacion/PRE061' element={<PRE061 {...props} />} />
                            <Route path='/preparacion/PRE080' element={<PRE080 {...props} />} />
                            <Route path='/preparacion/PRE100' element={<PRE100 {...props} />} />
                            <Route path='/preparacion/PRE110' element={<PRE110 {...props} />} />
                            <Route path='/preparacion/PRE110AnulacionDetPedidoAtri' element={<PRE110AnulacionDetPedidoAtri {...props} />} />
                            <Route path='/preparacion/PRE110AnulacionDetPedidoLpn' element={<PRE110AnulacionDetPedidoLpn {...props} />} />
                            <Route path='/preparacion/PRE120' element={<PRE120 {...props} />} />
                            <Route path='/preparacion/PRE130' element={<PRE130 {...props} />} />
                            <Route path='/preparacion/PRE150' element={<PRE150 {...props} />} />
                            <Route path='/preparacion/PRE152' element={<PRE152 {...props} />} />
                            <Route path='/preparacion/PRE153' element={<PRE153 {...props} />} />
                            <Route path='/preparacion/PRE154' element={<PRE154 {...props} />} />
                            <Route path='/preparacion/PRE155' element={<PRE155 {...props} />} />
                            <Route path='/preparacion/PRE156' element={<PRE156 {...props} />} />
                            <Route path='/preparacion/PRE160' element={<PRE160 {...props} />} />
                            <Route path='/preparacion/PRE161' element={<PRE161 {...props} />} />
                            <Route path='/preparacion/PRE162' element={<PRE162 {...props} />} />
                            <Route path='/preparacion/PRE170' element={<PRE170 {...props} />} />
                            <Route path='/preparacion/PRE220' element={<PRE220 {...props} />} />
                            <Route path='/preparacion/PRE221' element={<PRE221 {...props} />} />
                            <Route path='/preparacion/PRE250' element={<PRE250 {...props} />} />
                            <Route path='/preparacion/PRE350' element={<PRE350 {...props} />} />
                            <Route path='/preparacion/PRE351' element={<PRE351 {...props} />} />
                            <Route path='/preparacion/PRE360' element={<PRE360 {...props} />} />
                            <Route path='/preparacion/PRE640' element={<PRE640 {...props} />} />
                            <Route path='/preparacion/PRE450' element={<PRE450 {...props} />} />
                            <Route path='/preparacion/PRE460' element={<PRE460 {...props} />} />
                            <Route path='/preparacion/PRE660' element={<PRE660 {...props} />} />
                            {/*<Route path='/preparacion/PRE670'  element={<PRE670 {...props} />} />*/}
                            <Route path='/preparacion/PRE680' element={<PRE680 {...props} />} />
                            <Route path='/preparacion/PRE810' element={<PRE810 {...props} />} />
                            <Route path='/preparacion/PRE811' element={<PRE811 {...props} />} />
                            <Route path='/preparacion/PRE812' element={<PRE812 {...props} />} />

                            <Route path='/inventario/INV030' element={<INV030 {...props} />} />
                            <Route path='/inventario/INV050' element={<INV050 {...props} />} />
                            <Route path='/inventario/INV060' element={<INV060 {...props} />} />
                            <Route path='/inventario/INV100' element={<INV100 {...props} />} />
                            <Route path='/inventario/INV410' element={<INV410 {...props} />} />
                            <Route path='/inventario/INV411' element={<INV411 {...props} />} />
                            <Route path='/inventario/INV412' element={<INV412 {...props} />} />
                            <Route path='/inventario/INV413' element={<INV413 {...props} />} />
                            <Route path='/inventario/INV414' element={<INV414 {...props} />} />
                            <Route path='/inventario/INV415' element={<INV415 {...props} />} />
                            <Route path='/inventario/INV416' element={<INV416 {...props} />} />
                            <Route path='/inventario/INV417' element={<INV417 {...props} />} />

                            <Route path='/interfaz/INT050' element={<INT050 {...props} />} />
                            <Route path='/interfaz/INT100' element={<INT100 {...props} />} />
                            <Route path='/interfaz/INT101' element={<INT101 {...props} />} />
                            <Route path='/interfaz/INT102' element={<INT102 {...props} />} />
                            <Route path='/interfaz/INT103' element={<INT103 {...props} />} />
                            <Route path='/interfaz/INT104' element={<INT104 {...props} />} />
                            <Route path='/interfaz/INT105' element={<INT105 {...props} />} />
                            <Route path='/interfaz/INT106' element={<INT106 {...props} />} />
                            <Route path='/interfaz/INT107' element={<INT107 {...props} />} />
                            <Route path='/interfaz/INT108' element={<INT108 {...props} />} />
                            <Route path='/interfaz/INT109' element={<INT109 {...props} />} />

                            <Route path='/recepcion/REC010' element={<REC010 {...props} />} />
                            <Route path='/recepcion/REC011' element={<REC011 {...props} />} />
                            <Route path='/recepcion/REC171' element={<REC171 {...props} />} />
                            <Route path='/recepcion/REC170' element={<REC170 {...props} />} />
                            <Route path='/recepcion/REC150' element={<REC150 {...props} />} />
                            <Route path='/recepcion/REC180' element={<REC180 {...props} />} />
                            <Route path='/recepcion/REC200' element={<REC200 {...props} />} />
                            <Route path='/recepcion/REC210' element={<REC210 {...props} />} />
                            <Route path='/recepcion/REC250' element={<REC250 {...props} />} />
                            <Route path='/recepcion/REC141' element={<REC141 {...props} />} />
                            <Route path='/recepcion/REC300' element={<REC300 {...props} />} />
                            <Route path='/recepcion/REC220' element={<REC220 {...props} />} />
                            <Route path='/recepcion/REC270' element={<REC270 {...props} />} />
                            <Route path='/recepcion/REC275' element={<REC275 {...props} />} />
                            <Route path='/recepcion/REC280' element={<REC280 {...props} />} />
                            <Route path='/recepcion/REC400' element={<REC400 {...props} />} />
                            <Route path='/recepcion/REC410' element={<REC410 {...props} />} />
                            <Route path='/recepcion/REC500' element={<REC500 {...props} />} />
                            <Route path='/recepcion/REC500GrillaDetalleFactura' element={<REC500GrillaDetalleFactura {...props} />} />
                            <Route path='/seguridad/SEG020' element={<SEG020 {...props} />} />
                            <Route path='/seguridad/SEG030' element={<SEG030 {...props} />} />
                            <Route path='/seguridad/SEG070' element={<SEG070 {...props} />} />
                            <Route path='/seguridad/SEG210' element={<SEG210 {...props} />} />

                            <Route path='/expedicion/EXP040' element={<EXP040 {...props} />} />
                            <Route path='/expedicion/EXP041' element={<EXP041 {...props} />} />
                            <Route path='/expedicion/EXP043' element={<EXP043 {...props} />} />
                            <Route path='/expedicion/EXP045' element={<EXP045 {...props} />} />
                            <Route path='/expedicion/EXP050' element={<EXP050 {...props} />} />
                            <Route path='/expedicion/EXP051' element={<EXP051 {...props} />} />
                            <Route path='/expedicion/EXP052' element={<EXP052 {...props} />} />
                            <Route path='/expedicion/EXP090' element={<EXP090 {...props} />} />
                            <Route path='/expedicion/EXP110' element={<EXP110 {...props} />} />
                            <Route path='/expedicion/EXP150' element={<EXP150 {...props} />} />
                            <Route path='/expedicion/EXP320' element={<EXP320 {...props} />} />
                            <Route path='/expedicion/EXP330' element={<EXP330 {...props} />} />
                            <Route path='/expedicion/EXP340' element={<EXP340 {...props} />} />
                            <Route path='/expedicion/EXP400' element={<EXP400 {...props} />} />

                            {/*<Route path='/porteria/POR010'  element={<POR010 {...props} />} />*/}
                            {/*<Route path='/porteria/POR020'  element={<POR020 {...props} />} />*/}
                            {/*<Route path='/porteria/POR030'  element={<POR030 {...props} />} />*/}
                            {/*<Route path='/porteria/POR060'  element={<POR060 {...props} />} />*/}
                            {/*<Route path='/porteria/POR070'  element={<POR070 {...props} />} />*/}
                            {/*<Route path='/porteria/POR040'  element={<POR040 {...props} />} />*/}
                            {/*<Route path='/porteria/POR050'  element={<POR050 {...props} />} />*/}
                            {/*<Route path='/porteria/POR080'  element={<POR080 {...props} />} />*/}

                            <Route path='/stock/STO005' element={<STO005 {...props} />} />
                            <Route path='/stock/STO030' element={<STO030 {...props} />} />
                            <Route path='/stock/STO060' element={<STO060 {...props} />} />
                            <Route path='/stock/STO110' element={<STO110 {...props} />} />
                            <Route path='/stock/STO150' element={<STO150 {...props} />} />
                            <Route path='/stock/STO151' element={<STO151 {...props} />} />
                            <Route path='/stock/STO152' element={<STO152 {...props} />} />
                            <Route path='/stock/STO153' element={<STO153 {...props} />} />
                            <Route path='/stock/STO210' element={<STO210 {...props} />} />
                            <Route path='/stock/STO211' element={<STO211 {...props} />} />
                            <Route path='/stock/STO310' element={<STO310 {...props} />} />
                            <Route path='/stock/STO500' element={<STO500 {...props} />} />
                            {/*<Route path='/stock/STO640'  element={<STO640 {...props} />} />*/}

                            <Route path='/stock/STO395' element={<STO395 {...props} />} />
                            <Route path='/stock/STO498' element={<STO498 {...props} />} />
                            <Route path='/stock/STO700' element={<STO700 {...props} />} />
                            <Route path='/stock/STO710' element={<STO710 {...props} />} />
                            <Route path='/stock/STO720' element={<STO720 {...props} />} />
                            <Route path='/stock/STO721' element={<STO721 {...props} />} />
                            <Route path='/stock/STO722' element={<STO722 {...props} />} />
                            <Route path='/stock/STO730' element={<STO730 {...props} />} />
                            <Route path='/stock/STO740' element={<STO740 {...props} />} />
                            <Route path='/stock/STO750' element={<STO750 {...props} />} />

                            <Route path='/stock/STO800' element={<STO800 {...props} />} />
                            <Route path='/stock/STO810' element={<STO810 {...props} />} />
                            <Route path='/stock/STO820' element={<STO820 {...props} />} />

                            {/*<Route path='/evento/EVT000'  element={<EVT000 {...props} />} />*/}
                            {/*<Route path='/evento/EVT001'  element={<EVT001 {...props} />} />*/}
                            {/*<Route path='/evento/EVT002'  element={<EVT002 {...props} />} />*/}
                            <Route path='/evento/EVT010' element={<EVT010 {...props} />} />
                            <Route path='/evento/EVT020' element={<EVT020 {...props} />} />
                            <Route path='/evento/EVT030' element={<EVT030 {...props} />} />
                            <Route path='/evento/EVT040' element={<EVT040 {...props} />} />
                            <Route path='/evento/EVT050' element={<EVT050 {...props} />} />
                            <Route path='/evento/EVT060' element={<EVT060 {...props} />} />

                            <Route path='/productividad/PRO030' element={<PRO030 {...props} />} />

                            <Route path='/produccion/PRD100' element={<PRD100 {...props} />} />
                            <Route path='/produccion/PRD110' element={<PRD110 {...props} />} />
                            <Route path='/produccion/PRD111' element={<PRD111 {...props} />} />
                            <Route path='/produccion/PRD112' element={<PRD112 {...props} />} />
                            <Route path='/produccion/PRD113' element={<PRD113 {...props} />} />
                            {/*<Route path='/produccion/PRD120'  element={<PRD120 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD130'  element={<PRD130 {...props} />} />*/}
                            <Route path='/produccion/PRD150' element={<PRD150 {...props} />} />
                            <Route path='/produccion/PRD151' element={<PRD151 {...props} />} />
                            <Route path='/produccion/PRD170' element={<PRD170 {...props} />} />
                            <Route path='/produccion/PRD171' element={<PRD171 {...props} />} />
                            <Route path='/produccion/PRD175' element={<PRD175 {...props} />} />
                            {/*<Route path='/produccion/PRD180'  element={<PRD180 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD190'  element={<PRD190 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD191'  element={<PRD191 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD200'  element={<PRD200 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD210'  element={<PRD210 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD220'  element={<PRD220 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD230'  element={<PRD230 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD240'  element={<PRD240 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD250'  element={<PRD250 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD260'  element={<PRD260 {...props} />} />*/}
                            {/*<Route path='/produccion/PRD400'  element={<PRD400 {...props} />} />*/}

                            <Route path='/parametrizacion/PAR110' element={<PAR110 {...props} />} />
                            <Route path='/parametrizacion/PAR050' element={<PAR050 {...props} />} />


                            <Route path='/parametrizacion/PAR401' element={<PAR401 {...props} />} />
                            <Route path='/parametrizacion/PAR401AtributosTipo' element={<PAR401AtributosTipo {...props} />} />
                            <Route path='/parametrizacion/PAR400' element={<PAR400 {...props} />} />
                            <Route path='/parametrizacion/PAR604' element={<PAR604 {...props} />} />

                            <Route path='/facturacion/FAC001' element={<FAC001 {...props} />} />
                            <Route path='/facturacion/FAC002' element={<FAC002 {...props} />} />
                            <Route path='/facturacion/FAC003' element={<FAC003 {...props} />} />
                            <Route path='/facturacion/FAC004' element={<FAC004 {...props} />} />
                            <Route path='/facturacion/FAC005' element={<FAC005 {...props} />} />
                            <Route path='/facturacion/FAC006' element={<FAC006 {...props} />} />
                            <Route path='/facturacion/FAC007' element={<FAC007 {...props} />} />
                            <Route path='/facturacion/FAC008' element={<FAC008 {...props} />} />
                            <Route path='/facturacion/FAC009' element={<FAC009 {...props} />} />
                            <Route path='/facturacion/FAC010' element={<FAC010 {...props} />} />
                            <Route path='/facturacion/FAC011' element={<FAC011 {...props} />} />
                            <Route path='/facturacion/FAC012' element={<FAC012 {...props} />} />
                            { /*<Route path='/facturacion/FAC200'  element={<FAC200 {...props} />} />*/}
                            { /*<Route path='/facturacion/FAC230'  element={<FAC230 {...props} />} />*/}
                            <Route path='/facturacion/FAC255' element={<FAC255 {...props} />} />
                            <Route path='/facturacion/FAC256' element={<FAC256 {...props} />} />
                            <Route path='/facturacion/FAC405' element={<FAC405 {...props} />} />
                            <Route path='/facturacion/FAC249' element={<FAC249 {...props} />} />
                            <Route path='/facturacion/FAC250' element={<FAC250 {...props} />} />
                            <Route path='/facturacion/FAC251' element={<FAC251 {...props} />} />

                            <Route path='/ordenTarea/ORT010' element={<ORT010 {...props} />} />
                            <Route path='/ordenTarea/ORT020' element={<ORT020 {...props} />} />
                            <Route path='/ordenTarea/ORT030' element={<ORT030 {...props} />} />
                            <Route path='/ordenTarea/ORT040' element={<ORT040 {...props} />} />
                            <Route path='/ordenTarea/ORT060' element={<ORT060 {...props} />} />
                            <Route path='/ordenTarea/ORT070' element={<ORT070 {...props} />} />
                            <Route path='/ordenTarea/ORT080' element={<ORT080 {...props} />} />
                            <Route path='/ordenTarea/ORT090' element={<ORT090 {...props} />} />
                            <Route path='/ordenTarea/ORT120' element={<ORT120 {...props} />} />

                            <Route path='/documento/DOC020' element={<DOC020 {...props} />} />
                            <Route path='/documento/DOC021' element={<DOC021 {...props} />} />
                            <Route path='/documento/DOC035' element={<DOC035 {...props} />} />
                            <Route path='/documento/DOC036' element={<DOC036 {...props} />} />
                            <Route path='/documento/DOC080' element={<DOC080 {...props} />} />
                            <Route path='/documento/DOC081' element={<DOC081 {...props} />} />
                            <Route path='/documento/DOC082' element={<DOC082 {...props} />} />
                            <Route path='/documento/DOC085' element={<DOC085 {...props} />} />
                            <Route path='/documento/DOC095' element={<DOC095 {...props} />} />
                            <Route path='/documento/DOC096' element={<DOC096 {...props} />} />
                            <Route path='/documento/DOC260' element={<DOC260 {...props} />} />
                            <Route path='/documento/DOC290' element={<DOC290 {...props} />} />
                            <Route path='/documento/DOC300' element={<DOC300 {...props} />} />
                            <Route path='/documento/DOC310' element={<DOC310 {...props} />} />
                            {/*<Route path='/documento/DOC320'  element={<DOC320 {...props} />} />*/}
                            <Route path='/documento/DOC330' element={<DOC330 {...props} />} />
                            <Route path='/documento/DOC340' element={<DOC340 {...props} />} />
                            <Route path='/documento/DOC350' element={<DOC350 {...props} />} />
                            <Route path='/documento/DOC360' element={<DOC360 {...props} />} />
                            <Route path='/documento/DOC361' element={<DOC361 {...props} />} />
                            <Route path='/documento/DOC362' element={<DOC362 {...props} />} />
                            <Route path='/documento/DOC363' element={<DOC363 {...props} />} />
                            {/*<Route path='/documento/DOC400'  element={<DOC400 {...props} />} />*/}
                            {/*<Route path='/documento/DOC401'  element={<DOC401 {...props} />} />*/}
                            {/*<Route path='/documento/DOC402'  element={<DOC402 {...props} />} />*/}
                            {/*<Route path='/documento/DOC410'  element={<DOC410 {...props} />} />*/}
                            <Route path='/documento/DOC500' element={<DOC500 {...props} />} />
                            {/*<Route path='/documento/DOC501'  element={<DOC501 {...props} />} />*/}
                            <Route path='/documento/DOC100' element={<DOC100 {...props} />} />

                            <Route path='/ptl/PTL010' element={<PTL010 {...props} />} />

                            <Route path='../components/FileUploadComponentModal' element={<FUC001 {...props} />} />
                        </Routes>
                    </Suspense>
                </ScrollContextProvider>
            </Layout>
        );
    }

    return (
        <Layout
            doneLoading={doneLoading}
            userName={userName}
            userLanguage={userLanguage}
            changeLanguage={changeLanguage}
        >
            <Loading />
        </Layout>
    );
}
