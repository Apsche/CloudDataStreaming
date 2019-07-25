import React, { Component } from "react";

import { Grid } from "@material-ui/core";
import LandingContent from "./components/LandingContent";
import LandingHeader from "./components/LandingHeader";

export class Landing extends Component {
  constructor() {
    super();

    this.state = {
      data: {
        windDeg: 0,
        temp: 0,
        weatherDescription: "",
        confidence: 0,
        name: "",
        humidity: 0,
        clouds: 0,
        currentSpeed: 0,
        windSpeed: 0,
        Timestamp: 0
      }
    };
  }

  componentDidMount() {
    this.interval = setInterval(() => this.fetchData(), 20000);
  }

  componentWillUnmount() {
    clearInterval(this.interval);
  }

  // Makes a request to the api to get the latest consumed message
  fetchData = () => {
    fetch("https://localhost:5001/api/values")
      .then(response => response.json())
      .then(data => this.setState({ data }));
  };

  render() {
    return (
      <div>
        <Grid container justify="center" spacing={3}>
          <Grid item xs={12} md={10} lg={8}>
            <LandingHeader />
            <LandingContent data={this.state.data} />
          </Grid>
        </Grid>
      </div>
    );
  }
}

export default Landing;
