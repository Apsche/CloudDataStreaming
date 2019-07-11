import { Typography, makeStyles } from "@material-ui/core";

import React from "react";

const useStyles = makeStyles({
  title: {
    paddingTop: "30px"
  }
});

function LandingHeader() {
  const classes = useStyles();

  return (
    <Typography className={classes.title} variant="h2">
      Data Streaming
    </Typography>
  );
}

export default LandingHeader;
