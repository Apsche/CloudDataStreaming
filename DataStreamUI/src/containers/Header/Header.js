import React, { Component } from "react";

import AppBar from "@material-ui/core/AppBar";
import IconButton from "@material-ui/core/IconButton";
import MenuIcon from "@material-ui/icons/Menu";
import Toolbar from "@material-ui/core/Toolbar";
import Typography from "@material-ui/core/Typography";

export class Header extends Component {
  render() {
    return (
      <div>
        <AppBar position="static">
          <Toolbar>
            <IconButton edge="start" color="inherit" aria-label="Menu">
              <MenuIcon />
            </IconButton>
            <Typography variant="h6">Data Streaming Project</Typography>
          </Toolbar>
        </AppBar>
      </div>
    );
  }
}

export default Header;
