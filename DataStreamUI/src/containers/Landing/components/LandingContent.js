import { Card, CardContent, Grid, Typography } from "@material-ui/core";

import React from "react";
import { makeStyles } from "@material-ui/styles";

const useStyles = makeStyles({
  card: {
    marginTop: "30px"
  }
});

function LandingContent() {
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
                You know... when we have it...
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </div>
  );
}

export default LandingContent;
