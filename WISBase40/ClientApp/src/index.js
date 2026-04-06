import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import "react-datepicker/dist/react-datepicker.css";
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import 'react-toastify/dist/ReactToastify.css';
import App from './App';
import './autosuggest.css';
import "./i18n";
import './index.css';
import './lib/font-awesome/css/fontawesome.css';
import './lib/font-awesome/css/regular.css';
import './lib/font-awesome/css/solid.css';
import './toaster.css';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

ReactDOM.render(
  <BrowserRouter basename={baseUrl}>
    <App />
  </BrowserRouter>,
  rootElement);

//registerServiceWorker();
