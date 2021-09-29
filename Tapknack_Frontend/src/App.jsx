import React, { useEffect, useState } from "react";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import { Button } from "@material-ui/core";
import "./App.css";
import Layout from "./Layout";
import NotFound from "./Views/NotFound";
import Landing from "./Views/Landing";
import Signin from "./Views/Signin";
import axios from "axios";

const App = () => {
  const paths = [
    { path: "/", View: Landing },
    { path: "/signin", View: Signin },
  ];

  const [testingEnabled, setTestingEnabled] = useState(false);

  const exitTimeout = 500;

  useEffect(() => {
    const {
      location: { href: locationHref },
    } = window;
    const { length: locationHrefLength } = locationHref;
    const hrefNoHash = locationHref.substring(
      0,
      locationHref.includes("/#")
        ? locationHref.indexOf("/#")
        : locationHrefLength
    );

    // TODO: How do we handle changing the prev page when we do a force load on another page?
    // .. Requirements: Need to set prevPage to the current page somehow without making it loop back on itself..
    // e.g. If I'm on the landing page and I goto "Profile", prevPage needs to = "landingPage" so I return there
    // when I click the back button
    if (localStorage.prevPage === hrefNoHash)
      window.location = `${hrefNoHash}/#return`;
  }, []);

  // ANIMATIONS

  const GetHrefNoHash = () => {
    const {
      location: { href: locationHref },
    } = window;

    return locationHref.substring(0, locationHref.indexOf("/#"));
  };

  const GotoUrl = (url) => {
    const hrefNoHash = GetHrefNoHash();

    localStorage.prevPage = hrefNoHash;

    window.location = `${hrefNoHash}/#exit`;
    setTimeout(() => (window.location = `${url}/#enter`), exitTimeout);
  };

  const ReturnToPrevUrl = () => {
    const hrefNoHash = GetHrefNoHash();

    window.location = `${localStorage.prevPage}/#return`;
    localStorage.prevPage = hrefNoHash;
  };

  // END ANIMATIONS

  // AXIOS INTERCEPTS

  const requestHandler = (request) => {
    const { token } = localStorage;
    const {
      location: { pathname },
    } = window;
    const { headers } = request;
    const auth = `Bearer ${token}`;

    /* console.log(
      "request=",
      pathname === "/signin" || pathname === "/"
        ? request
        : {
            ...request,
            headers: { ...headers, Authorization: auth },
          }
    ); */

    return pathname === "/signin" || pathname === "/"
      ? request
      : {
          ...request,
          headers: { ...headers, Authorization: auth },
        };
  };

  const responseHandler = (response) => {
    if (response.status === "401") window.location = "/signin";

    return response;
  };

  const errorHandler = (err) => Promise.reject(err);

  axios.interceptors.request.use(requestHandler, errorHandler);
  axios.interceptors.response.use(responseHandler, errorHandler);

  // END AXIOS INTERCEPTS

  return (
    <>
      <Button
        color="primary"
        variant="contained"
        style={{ position: "absolute", margin: 50, zIndex: 200 }}
        onClick={() => setTestingEnabled(!testingEnabled)}
      >
        {!testingEnabled ? "Enable Testing" : "Disable Testing"}
      </Button>
      <BrowserRouter>
        <Layout exitTimeout={exitTimeout}>
          <Switch>
            {paths.map(({ path, View }) => (
              <Route
                key={path}
                exact
                path={path}
                render={() => (
                  <View gotoUrl={GotoUrl} testingEnabled={testingEnabled} />
                )}
              />
            ))}
            <Route
              key={window.location.pathname}
              exact
              path={window.location.pathname}
              render={() => <NotFound returnToPrevUrl={ReturnToPrevUrl} />}
            />
          </Switch>
        </Layout>
      </BrowserRouter>
    </>
  );
};

export default App;
