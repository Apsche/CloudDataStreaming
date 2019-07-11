import { orange, teal } from "@material-ui/core/colors";

import { createMuiTheme } from "@material-ui/core/styles";

const theme = createMuiTheme({
  palette: {
    primary: {
      light: teal[300],
      main: teal[500],
      dark: teal[700]
    },
    secondary: {
      light: orange[300],
      main: orange[500],
      dark: orange[700]
    },
    type: "dark"
  }
});

export default theme;
