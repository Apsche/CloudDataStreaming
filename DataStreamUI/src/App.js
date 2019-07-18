import React, { Component } from "react";

import Header from "./containers/Header/Header";
import Landing from "./containers/Landing/Landing";

class App extends Component {
  constructor() {
    super();
    this.state = {
      response: false,
      connected: false,
      endpoint: "ws://127.0.0.1:8080"
    };
  }

  componentDidMount() {
    const { endpoint } = this.state;
    const ws = new WebSocket(endpoint);
    ws.binaryType = "arraybuffer";
    ws.onerror = error => {
      console.error(`There was an error`);
      console.error(error);
    };
    ws.onclose = event => {
      console.warn("websocket closed");
      console.warn(event.code);
      console.warn(event.reason);
    };

    ws.onopen = () => {
      alert("Connection established");
      this.setState({ connected: true });
      ws.send("Hello socket!");
    };

    ws.onmessage = msg => {
      console.warn(`message received`);
      console.warn(msg.data);
    };
  }

  render() {
    return (
      <>
        <Header />
        <Landing response={this.state.response} />
      </>
    );
  }
}

export default App;
