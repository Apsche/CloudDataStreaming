import { Card, CardContent, Grid, Typography } from "@material-ui/core";

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
              <Typography variant="body2">
                {props.response ? (
                  <span>We have it! {props.response}</span>
                ) : (
                  <span>We don't have it :(</span>
                )}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </div>
  );
}

export default LandingContent;
