import { Button, Card, CardContent, Grid, Typography } from "@material-ui/core";

import React from "react";
import { makeStyles } from "@material-ui/styles";

const useStyles = makeStyles({
  card: {
    marginTop: "30px"
  }
});

function LandingContent(props) {
  const classes = useStyles();
  return (
    <div>
      <Grid container>
        <Grid item xs={10} md={8} lg={6}>
          <Card className={classes.card}>
            <CardContent>
              <Typography variant="h4">
                This is where we'll display our sweet info
              </Typography>
            </CardContent>
          </Card>
          {/* This is the data coming from our consumer */}
          <p>Wind Deg: {props.data.windDeg}</p>
          <p>Temp: {props.data.temp}</p>
          <p>Weather desc: {props.data.weatherDescription}</p>
          <p>Confidence: {props.data.confidence}</p>
          <p>Name: {props.data.name}</p>
          <p>Humidity: {props.data.humidity}</p>
          <p>Clouds: {props.data.clouds}</p>
          <p>Current Speed: {props.data.currentSpeed}</p>
          <p>Wind Speed: {props.data.windSpeed}</p>
          <p>Timestamp: {props.data.Timestamp}</p>
        </Grid>
      </Grid>
    </div>
  );
}

export default LandingContent;
