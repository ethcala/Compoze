import logo from './logo.svg';
import './App.css';
import React, { useEffect, useRef } from 'react';
import { render } from 'react-dom';

class App extends React.Component {
  constructor() {
    super();
    this.state = { pageDisplayed: "About" };
  }

  render() {
      return (
        <div className="App">
          <header>
            <h1><strong>Compoze</strong></h1>
            <a className="no-link" onClick={() => this.setState({pageDisplayed: "About"})}>About</a>
            <a className="no-link" onClick={() => this.setState({pageDisplayed: "Downloads"})}>Downloads</a>
          </header>
          <PageContent pageDisplay={this.state.pageDisplayed} />
        </div>
      );
  }
}


function downloadFile(url, btnRef) {
  btnRef.current.disabled = true;
  var req = new XMLHttpRequest();
  req.open("GET", url, true);
  req.responseType = "blob";
  req.onload = function (event) {
      var blob = req.response;
      var fileName = req.getResponseHeader("fileName")
      var link=document.createElement('a');
      link.href=window.URL.createObjectURL(blob);
      link.download=fileName;
      link.click();
  };

  req.send();
}


function PageContent(props) {
  var btnRef = useRef(null);
  useEffect(() => {
    console.log(btnRef.current);
  }, []);

  switch(props.pageDisplay) {
    case "About":
      return (
        <div className="page">
          <h1>About Compoze</h1>
          <p>Our mission is to create a completely customizable, cross-platform writing app for all kinds of writers. Compoze is a lightweight, easy to use app anyone can download.</p>
        </div>
      );
      case "Downloads":
        return (
          <div className="page">
            <h1>Downloads</h1>
            <p>How to install: Unzip the downloaded archive. Run the setup .exe contained. Compoze will be installed and can be run by searching 'Compoze' in Windows.</p>
            <div class="row">
              <div className="download-square">
                <h3>Windows</h3>
                <button ref={btnRef} onClick={el => {btnRef.current = el; downloadFile("/Compoze-win64-0-1.zip", btnRef);}} className="download-btn">Win x64</button>
              </div>
            </div>
          </div>
        );
        default:
          return (
            <div className="page">
              <h1>Error 404</h1>
              <p>Could not find the correct page.</p>
            </div>
          )
  }
}

export default App;