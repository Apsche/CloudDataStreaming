import React, { Component } from "react";

import { Grid } from "@material-ui/core";
import LandingContent from "./components/LandingContent";
import LandingHeader from "./components/LandingHeader";

export class Landing extends Component {
  render() {
    return (
      <div>
        <Grid container justify="center" spacing={3}>
          <Grid item xs={12} md={10} lg={8}>
            <LandingHeader />
            <LandingContent />
          </Grid>
        </Grid>
      </div>
    );
  }
}

export default Landing;
